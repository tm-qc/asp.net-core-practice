using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SelfAspNet.Models;
using SelfAspNet.Repository;
// using Microsoft.Extensions.Options;

// ページネーションのプラグイン使うために追加
using X.PagedList;//型IPagedListを使うため
using X.PagedList.EF;//メソッドToPagedListAsyncを使うため

// Filterサンプル(MyLogAttributeをつかうため)
using SelfAspNet.Filters;
using SelfAspNet.Lib;

// IOptions使うため
using Microsoft.Extensions.Options;

using SelfAspNet.Extensions;
using SelfAspNet.Record;

namespace SelfAspNet.Controllers
{
    // コントローラー単位でフィルター適用の場合
    [MyLog]
    public class SamplesController : Controller
    {
        private readonly MyContext _context;
        private readonly ISampleRepository _rep;
        private readonly IConfiguration _config;
        private readonly MyAppOptions _app = null!;//null警告が出た+nullはありえないので!で対処
        private readonly ILogger _logger;
        
        // ログで何の処理か判別するための任意の処理ID
        public const int IndexView = 1001;

        public SamplesController(
            MyContext context,
            ISampleRepository rep,
            IConfiguration config,
            IOptions<MyAppOptions> app,
            //<SamplesController>はログのカテゴリ名となる。クラス名をつけるのが一般的
            ILogger<SamplesController> logger
            //任意のログのカテゴリ名の設定をするときはfactoryとCreateLoggerを使う必要があり
            // ILoggerFactory factory 
        )
        {
            _context = context;
            _rep = rep;
            _config = config;
            _app = app.Value;
            _logger = logger;
            // _logger = factory.CreateLogger("任意のログのカテゴリ名");
        }

        // GET: Samples?page=
        // - <IActionResult>：非同期(async await)の戻り値の型
        // - awaitの処理が終わって次の処理に行く。非同期と違い同期処理にする
        //   (非同期処理って命名がわかりづらいが、要は同期処理)
        // - ToListAsync()はコレクションでデータを非同期的に取得する

        // 非同期処理とは？
        // 「非同期」の本質は、「待ち時間にスレッドを解放して他の作業を進められること」 です。
        // 一見すると同期的に見える動作（結果を待つ）をしつつ、内部的にはリソースを効率的に使っています。
        
        // 非同期に効率よく処理しつつ、最終的に同期されて順番に処理をするということ

        // Filter定義：これでMyLogAttributeが動く
        // Program.csでアプリ全体に設定もできる
        // 今回はメソッドでSamplesにアクセスしたときに動くようにした
        // [MyLog]

        // ResponseCache属性
        // キャッシュすることでパフォーマンスをあげる
        // ただし変更が表示に反映しない原因になるので、変更がすくないぺーじにしか使えない
        // [ResponseCache(Duration = 60)]//60秒キャッシュ
        // public async Task<IActionResult> Index(int page = 1)
        // {
        //     // 型指定の選定とやり方メモ

        //     // まずvarで記載して、デバッグを動かし、[]の中に書いてある型で指定するのがいいかも
        //     // コンパイル時の決定なので間違いない。

        //     // いろんな型があるが、特に狙いない限り、表示されてるものでＯＫ

        //     // 最初からvarでいいやんってなるかもしれないが、そこはPJの方針による
        //     // 個人的にはvarで型推論で書くより、最初から固定で書いて置く方が、後々予期せぬことが起きないと思うので型指定はしたい

        //     // LINQサンプル
        //     // クエリ構文
        //     // シンプルだがすべての問い合わせは表現できない。またコンパイル時にメソッド構文に置換され実行される。
        //     IQueryable<Sample> samplesLinqQuery = from s in _context.Samples where s.Id == 5 select s;

        //     // メソッド構文
        //     // 基本はこれを使う方がいい
        //     // やや冗長だが、LINQの機能をすべて使える
        //     IQueryable<Sample> samplesLinqMethod = _context.Samples.Where(s => s.Id == 3).Select(s => s);
        //     Console.WriteLine("ここでデバックとめたらLINQの結果が見れる");

        //     string sampleStr = "string";
        //     int sampleInt = 123;

        //     ViewBag.Mes = $"ViewBagにデータを入れるとRazorビューで参照できます{sampleStr}{sampleInt}";
 
        //     // ページャー表示
        //     // List<Sample> samplesList = await Pager(page).ToListAsync();
        //     // return View(samplesList);

        //     // ページャー(X.PagedList.Mvc.Coreを利用し作成)

        //     // Nugetのページネイション用ライブラリのメリット
        //     // Pager関数も不要
        //     // ビュー側も一行でHTML生成ですごく短く書ける
        //     int pageSize = 3;
        //     IPagedList<Sample> samplesNugetList = await _context.Samples.OrderBy(s => s.Id).ToPagedListAsync(page, pageSize);
        //     return View(samplesNugetList);

        //     // 全件表示(ページャーなし)
        //     // return View(await _context.Samples.ToListAsync());
        // }

        // リポジトリからアクション呼び出すバージョン(index一覧ページ)

        /// <summary>
        /// index一覧ページ
        /// </summary>
        /// <param name="page">現在のページ数</param>
        /// <returns>表示データ</returns>
        public async Task<IActionResult> Index(int page = 1)
        {
            // ロギングサンプル
            // 
            // ログはクラス単位でコンストラクタでILoggerを読込使うのが基本
            _logger.LogTrace("トレース：詳細な診断情報（通常は開発時のみ使用）");
            _logger.LogDebug("デバッグ：デバッグ情報");
            _logger.LogInformation("通常の動作情報");
            _logger.LogWarning("警告：エラーではないが、潜在的な問題で対応したほうが無難");

            // 第一引数：処理を識別するために任意で設定できるID IndexView
            // 第二引数：任意のメッセージを引数で指定するサンプル
            // 第三引数以降：記載順で任意のプレイスホルダーに適用される
            _logger.LogError(IndexView,"エラー：{Path}→{Current:yyyy年MM月dd日}",Request.Path,DateTime.Now);

            _logger.LogCritical("致命的な問題");
            // これは全体のログレベルの設定ではなく、個々に単発で設定できる書き方
            // _logger.Log(LogLevel.Critical, "致命的な問題");
            

            // 構成情報の取得(今回はSelfAspNet\appsettings.jsonから取得)

            // 構成情報の取得サンプル1
            // 一番シンプルだが型指定なし+文字列でしか取得できない
            Console.WriteLine($"構成情報取得1：{_config["MyAppOptions:Title"]}");
            Console.WriteLine($"構成情報取得1：{_config["MyAppOptions:Projects:0"]}");
            Console.WriteLine($"構成情報取得1：{_config["MyAppOptions:Published"]}");

            // 構成情報の取得サンプル2
            // シンプルだが型指定あり+文字列でしか取得できない
            // ※サンプルに日付追加
            // ※またここでも単一の値しかとれず、コレクションみたいな複数の値は参照できない
            Console.WriteLine($"構成情報取得2：{_config.GetValue<string>("MyAppOptions:Title")}");
            Console.WriteLine($"構成情報取得2：{_config.GetValue<DateTime>("MyAppOptions:Published")}");

            // 【推奨】構成情報の取得サンプル3
            // 構成情報と型を紐づける形。最初手間かかるけど、使う場合はこれが無難そう
            // 単一の値しかとれず、コレクションみたいな複数の値は参照できないが、取得時に型指定意識しなくていい
            Console.WriteLine($"構成情報取得3：{_app.Title}");
            Console.WriteLine($"構成情報取得3：{_app.Published}");
            Console.WriteLine($"構成情報取得3：{_app.Projects[0]}");

            // 元のコード
            // int pageSize = 3;
            // IPagedList<Sample> samplesNugetList = await _context.Samples.OrderBy(s => s.Id).ToPagedListAsync(page, pageSize);
            // return View(samplesNugetList);
            
            ViewBag.CreateResult = TempData["CreateResult"];
            // リポジトリから処理を呼び出してるコード
            return View(await _rep.GetAllPagerAsync(page));
        }


        /// <summary>
        /// ページャーの関数
        /// 現在のページに合わせたデータを取得
        /// 
        /// IQueryableはデータベース操作の型
        /// </summary>
        /// <param name="page">現在のぺージ数</param>
        /// <returns>表示データ</returns>
        public IQueryable<Sample> Pager(int page = 1){
            // 1ページあたりのデータの表示量
            int pageSize = 3;
            // 1:ユーザ的には1ページ目が最初
            // -1:DBのオフセット、Skipで参照するときは0が1なので、page - 1となる
            int pageNum = page - 1;
        
            // 全表示データ数をカウント
            int totalConut = _context.Samples.Count();
            // 現在のページに表示するデータを取得
            IQueryable<Sample> samplesList = _context.Samples.OrderBy(s => s.Id).Skip(pageSize * pageNum).Take(pageSize);
            
            // 現在のページ数をビューに渡す
            ViewBag.currentPage = page;
            // 総ページ数を計算
            // Math.Ceilingで小数点以下を切り上げ
            // 例えば10件データがある場合は10/3=3.3333333333333335となり、この場合4ページとなるため
            ViewBag.totalPage = (int)Math.Ceiling(totalConut / (double)pageSize);
            return samplesList;
        }

        // GET: Samples/Details/5
        // リクエストデータと同じ名前の引数で自動で値を受け取れる(int? Id)
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                // 404エラーを返す
                return NotFound();
            }

            // idと一致するデータを取得。なければnullを返す
            // FindAsync(Id)でもいいらしい。こっちがシンプルで良さそう

            // FirstOrDefaultAsync：主キー以外で条件つけて検索できる
            // FindAsync：主キーで検索でき、キャッシュも残るのでエコ
            var sample = await _context.Samples
                // .FirstOrDefaultAsync(m => m.Id == Id);
                .FindAsync(Id);
            if (sample == null)
            {
                return NotFound();
            }

            return View(sample);
        }

        // GET: Samples/Create
        public IActionResult Create()
        {
            // View メソッドに引数を指定しない場合、デフォルトではアクションメソッド名と同じ名前のビューを探して表示
            // Views/<コントローラー名>/Create.cshtml または Views/Shared/Create.cshtml
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample">入力データ</param>
        /// <returns>成功時はindexへ、失敗時はsmapleオブジェクトをもって登録画面へ</returns>

        // POST: Samples/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // [HttpPost]：POSTに対応。無指定ならGETになる
        [HttpPost]
        // CSRF対策
        [ValidateAntiForgeryToken]

        // [Bind("Id,Title,SubTitle")]：必要なデータしか受け取らないことで、セキュリティを高める(オーバーポストを防ぐ)
        // ただし、idは不要：自動インクリメントされるため、外部から渡す必要はない。むしろ危険なので不要
        // bindはモデルのプロパティで自動できまるので、毎回しっかり開発者が受け取るべきものだけに整理しないといけない

        // Modelが変更されたら、Bindも変更しないといけない
        // コントローラーからは例えば引数の Sample sample でModelが判断できる
        // modelからはvs codeで例えばpublic class Sampleモデルを右クリックですべての参照を検索でみつけれる

        // public async Task<IActionResult> Create([Bind("Title,SubTitle")] Sample sample)
        // {
        //     // バリデーション問題なければ登録
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(sample);//コンテキストにsampleオブジェクトを追加
        //         await _context.SaveChangesAsync();//コンテキストの内容を非同期でDBに新規データを保存
        //         return RedirectToAction(nameof(Index));//保存後にリダイレクトして、サーバー側でIndexメソッドを実行
        //     }
        //     // バリデーション失敗の時はsample/createを表示
        //     // View()指定なしで処理元のページ戻る
        //     // sampleオブジェクトを引数に指定しているので、バリデーションエラーがある場合はそのエラーが表示されるし、元の入力値も保持される
        //     return View(sample);
        // }
        
        //リポジトリからアクション呼び出すバージョン(新規登録機能)
        public async Task<IActionResult> Create([Bind("Title,SubTitle")] Sample sample)
        {
            // バリデーション問題なければ登録
            if (ModelState.IsValid)
            {
                await _rep.CreateAsync(sample);
                TempData["CreateResult"] = $"「{sample.Title}」を新規登録しました";
                return RedirectToAction(nameof(Index));//保存後にリダイレクトして、サーバー側でIndexメソッドを実行
            }
            // バリデーション失敗の時はsample/createを表示
            // View()指定なしで処理元のページ戻る
            // sampleオブジェクトを引数に指定しているので、バリデーションエラーがある場合はそのエラーが表示されるし、元の入力値も保持される
            return View(sample);
        }

        // GET: Samples/Edit/5
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            // リレーションのデータ取得について

            // ↓これではリレーションのデータ取れない(データがないだけでエラーにはならない)
            // var sample = await _context.Samples.FindAsync(Id);   

            // なぜ取れないか
            // 即時読込か明示的な読込で明示して取得する必要がある
            // なお、基本使わないが遅延読込の設定がされていれば即時読込の記載がなくてもリレーションのデータはとれる
            
            // ↓以下のいずれかでリレーションのデータも取れる

            // 即時読込(推奨)
            // IncludeではFindAsyncはつかえないので、FirstOrDefaultAsyncをつかわないといけない
            var sample = await _context.Samples.Include(s => s.SampleRelation1).FirstOrDefaultAsync(s => s.Id == Id);

            // 明示的な読込(あまりつかわない)
            // var sample = await _context.Samples.FindAsync(Id);//FirstOrDefaultAsync(s => s.Id == Id)でもOK
            // await _context.Entry(sample).Collection(s => s.SampleRelation1).LoadAsync();//ちなみにここでnull警告の対処も必要でちょっとめんどくさい

            // 遅延読込(不要)
            // 使うまでにはインストールまたは、ILazyLoaderを使う必要があって、冗長で難しい
            // またメリットとがInclude省略しかなく、デメリットのN+1もあるし、遅延読込は使わなくていい印象
            
            if (sample == null)
            {
                return NotFound();
            }

            var sampleRelation = sample.SampleRelation1;
            Console.WriteLine("リレーションテスト：ここで止めるとデバッグで、sampleRelationでデータ取れてるか確認できる");
            return View(sample);
        }

        // POST: Samples/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, [Bind("Id,Title,SubTitle")] Sample sample)
        {
            // 【大問題】
            // 1.
            // idは基本URLのidとるが、今回hiddenでidを送信しているので、それが最終hiddenのidで上書きされてる(結果ルートパラメータのidが受け取れてない)
            // なので、ルートパラメータと同じhiddenは不要(自動生成なので削除しないといけない)

            // 2.
            // idのデータはフロントでURL、フォームともに自由に書き換え可能なので、セキュリティ的には危険
            // 例えばid 1のデータを見てても、F12とかURLでidを書き換えて、DBに存在するidなら書き換えられて別のデータが更新される

            // 【解決策】
            // - フロントからのユニークキーでDB更新させない
            // - ログイン情報と紐づけてそのデータで更新データを特定させる

            // Id=引数のint idでルートパラメータのidを受け取る
            // sample.Id=Bindのid(リクエストデータだがルートパラメータも受けとってる)
            if (Id != sample.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sample);//コンテキストにsampleオブジェクトを更新を追加(.Stateも同じ機能らしい)
                    await _context.SaveChangesAsync();//コンテキストの内容を非同期でDBに更新データを保存
                }
                // 同じデータを違う人が同時更新した場合のエラーを検知(多分よほどのことがない限り起きない)
                catch (DbUpdateConcurrencyException)
                {
                    if (!SampleExists(sample.Id)) //idを持ったデータがあるか確認
                    {
                        return NotFound();//データがない場合は404エラーを返す
                    }
                    else
                    {
                        // エラーの時にDbUpdateConcurrencyExceptionなどのスタックトレースを維持しつつ出力、また上位のCatchがあればそれも実行される
                        // とりあえずエラーの投げたいときはこの書き方でOK
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));//保存後にリダイレクトして、サーバー側でIndexメソッドを実行
            }
            return View(sample);//バリデーション失敗の時はsample/editを表示
        }

        // GET: Samples/Delete/5

        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var sample = await _context.Samples
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (sample == null)
            {
                return NotFound();
            }

            return View(sample);
        }

        // POST: Samples/Delete/5
        // DeleteConfirmedメソッドをDeleteメソッドで呼べるようにしてる
        // (別に POST: Samples/Deleteでアクセスすればいいだけなので、メソッド名をDeleteConfirmed→DeleteにそもそもしてもOK)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            // トランザクションサンプル
            // トランザクションは複数テーブルで整合性を保たないといけないときに使う
            // 全部成功すれば実行、一個でも失敗したらロールバックで戻すということになる
            // 
            // ここではDeleteしかないが、あくまでトランザクションの雛形と参考として記載しておく
            using(IDbContextTransaction tx = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // varは型推論＋Null許容なので?は不要
                    // 型指定すると?が警告回避で必要
                    // 
                    // 型指定するとSampleはクラスなのでNullのはずないよね？という警告が出てしまうが、
                    // 実際はデータ取得後の結果なので、?で回避してOK
                    Sample? sample = await _context.Samples.FindAsync(Id);
                    if (sample != null)
                    {
                        _context.Samples.Remove(sample);

                        // ↓今までeditとcreateではifの中でSaveChangesAsyncしていたが、ここではしてないのは謎です
                        // コードの一貫性やデータなくてもSaveChangesAsync実行するのが、ぱっと見無駄に見えるので、個人的には好ましくない
                        // await _context.SaveChangesAsync();
                        // return RedirectToAction(nameof(Index));
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();//トランザクションコミット：ここでSQLが実際に反映する
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception)
                {
                    await tx.RollbackAsync();//トランザクションロールバック：失敗なのでSQLを反映しないで終了
                    throw;// 元の例外情報を保持したまま例外をスロー
                }
            }
        }


        /// <summary>
        /// クッキーサンプル
        /// ビュー
        /// </summary>
        /// <returns></returns>
        public IActionResult Cookie()
        {
            ViewBag.cookieVal = Request.Cookies["cookieVal"];
            return View();
        }

        /// <summary>
        /// クッキーサンプル
        /// クッキーの値を設定する
        /// </summary>
        /// <param name="cookieVal"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Cookie(string cookieVal = "初期値リクエストのクッキーなし")
        {
            HttpContext.Response.Cookies.Append("cookieVal", cookieVal,
                new CookieOptions
                {
                    //有効期限。指定なくてもブラウザを閉じると消える
                    // ブラウザ閉じた後も残す場合はExpiresかMaxAgeを指定する(双方ある場合はMAXAgeが優先)
                    Expires = DateTime.Now.AddMinutes(5),
                    // HTTPクッキー
                    // ブラウザの JavaScript（document.cookie）でこのクッキーの値を取得することができなくなる。
                    // XSS 攻撃によるクッキーの窃取を防ぐために重要。
                    // ただし、サーバーサイドでこのクッキーにアクセスすることは可能
                    HttpOnly = true,
                    // クロスサイトリクエスト（CSRF 攻撃）を防ぐための設定
                    // Strict: 同一サイト内のリクエストのみクッキーを送信。
                    // Lax: 異なるサイトのリクエストではGETでのみクッキーを送信(既定)
                    // None: 全てのクロスサイトリクエストでクッキーを送信(Secure = trueと組み合あわせないと使えない)
                    // SameSite = SameSiteMode.None,
                    // HTTPS 接続時のみクッキーが送信されるように設定。これもセキュリティを強化するために重要
                    // Secure = true,
                }
            );
            return RedirectToAction(nameof(Cookie));
        }

        /// <summary>
        /// セッションサンプルのビュー
        /// </summary>
        /// <returns></returns>
        public IActionResult Session()
        {
            // セッションの値をビューに渡す
            ViewBag.sessionVal = HttpContext.Session.GetString("sessionVal");
            return View();
        }


        /// <summary>
        /// セッションサンプル
        /// </summary>
        /// <param name="sessionVal"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Session(string sessionVal = "初期値リクエストのセッション値なし")
        {
            // フォームから送られた値をセッションの値に保存
            HttpContext.Session.SetString("sessionVal", sessionVal);
            return RedirectToAction(nameof(Session));
        }


        /// <summary>
        /// セッションをシリアライズするサンプル
        /// samples/Jsonにアクセスすると、セッションにPersonオブジェクトをシリアライズし保存して、その値を復元し取得して表示する
        /// 
        /// 分散キャッシュを使う場合に、セッションオブジェクトを保存する場合は、そのオブジェクトをシリアライズして保存する必要がある
        /// ※Redis、Memcachedは保存先を分散する分散キャッシュというのが本番環境で使われることがある
        /// ※現状は分散キャッシュをつかっていないので、シリアライズはしなくてもセッションオブジェクトに保存できる
        ///   (今はInMemoryのbuilder.Services.AddDistributedMemoryCache()をつかってるので、そのまま保存できる)
        /// </summary>
        /// <returns></returns>
        public IActionResult Json()
        {
            var session = HttpContext.Session;
            // usrというキーでセッションにPersonオブジェクトがない場合
            if (session.Get<Person>("usr") == null)
            {
                // Personオブジェクトで指定してるNameとAgeの値をセッションに保存
                // usrというキーでセッションにPersonオブジェクトを保存
                session.Set("usr", new Person("Personオブジェクトで指定してるNameです", 18));
            }
            var usr = session.Get<Person>("usr");
            // Viewに文字で表示
            return Content($"デシリアライズ(復元)された値 => {usr?.Name}：{usr?.Age}歳");
        }


        private bool SampleExists(int Id)
        {
            return _context.Samples.Any(e => e.Id == Id);
        }
    }
}
