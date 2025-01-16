using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SelfAspNet.Models;

// ページネーションのプラグイン使うために追加
using X.PagedList;//型IPagedListを使うため
using X.PagedList.EF;//メソッドToPagedListAsyncを使うため

// Filterサンプル(MyLogAttributeをつかうため)
using SelfAspNet.Filters;

namespace SelfAspNet.Controllers
{
    public class SamplesController : Controller
    {
        private readonly MyContext _context;

        public SamplesController(MyContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            // 型指定の選定とやり方メモ

            // まずvarで記載して、デバッグを動かし、[]の中に書いてある型で指定するのがいいかも
            // コンパイル時の決定なので間違いない。

            // いろんな型があるが、特に狙いない限り、表示されてるものでＯＫ

            // 最初からvarでいいやんってなるかもしれないが、そこはPJの方針による
            // 個人的にはvarで型推論で書くより、最初から固定で書いて置く方が、後々予期せぬことが起きないと思うので型指定はしたい

            // LINQサンプル
            // クエリ構文
            // シンプルだがすべての問い合わせは表現できない。またコンパイル時にメソッド構文に置換され実行される。
            IQueryable<Sample> samplesLinqQuery = from s in _context.Samples where s.Id == 5 select s;

            // メソッド構文
            // 基本はこれを使う方がいい
            // やや冗長だが、LINQの機能をすべて使える
            IQueryable<Sample> samplesLinqMethod = _context.Samples.Where(s => s.Id == 3).Select(s => s);
            Console.WriteLine("ここでデバックとめたらLINQの結果が見れる");

            string sampleStr = "string";
            int sampleInt = 123;

            ViewBag.Mes = $"ViewBagにデータを入れるとRazorビューで参照できます{sampleStr}{sampleInt}";
 
            // ページャー表示
            // List<Sample> samplesList = await Pager(page).ToListAsync();
            // return View(samplesList);

            // ページャー(X.PagedList.Mvc.Coreを利用し作成)

            // Nugetのページネイション用ライブラリのメリット
            // Pager関数も不要
            // ビュー側も一行でHTML生成ですごく短く書ける
            int pageSize = 3;
            IPagedList<Sample> samplesNugetList = await _context.Samples.OrderBy(s => s.Id).ToPagedListAsync(page, pageSize);
            return View(samplesNugetList);

            // 全件表示(ページャーなし)
            // return View(await _context.Samples.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Title,SubTitle")] Sample sample)
        {
            // バリデーション問題なければ登録
            if (ModelState.IsValid)
            {
                _context.Add(sample);//コンテキストにsampleオブジェクトを追加
                await _context.SaveChangesAsync();//コンテキストの内容を非同期でDBに新規データを保存
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

        private bool SampleExists(int Id)
        {
            return _context.Samples.Any(e => e.Id == Id);
        }
    }
}
