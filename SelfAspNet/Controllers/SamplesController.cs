using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SelfAspNet.Models;

namespace SelfAspNet.Controllers
{
    public class SamplesController : Controller
    {
        private readonly MyContext _context;

        public SamplesController(MyContext context)
        {
            _context = context;
        }

        // GET: Samples
        // - <IActionResult>：非同期(async await)の戻り値の型
        // - awaitの処理が終わって次の処理に行く。非同期と違い同期処理にする
        //   (非同期処理って命名がわかりづらいが、要は同期処理)
        // - ToListAsync()はコレクションでデータを非同期的に取得する

        // 非同期処理とは？
        // 「非同期」の本質は、「待ち時間にスレッドを解放して他の作業を進められること」 です。
        // 一見すると同期的に見える動作（結果を待つ）をしつつ、内部的にはリソースを効率的に使っています。
        
        // 非同期に効率よく処理しつつ、最終的に同期されて順番に処理をするということ


        public async Task<IActionResult> Index()
        {
            ViewBag.Mes = "ViewBagにデータを入れるとRazorビューで参照できます";
            return View(await _context.Samples.ToListAsync());
        }

        // GET: Samples/Details/5
        // リクエストデータと同じ名前の引数で自動で値を受け取れる(int? id)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // 404エラーを返す
                return NotFound();
            }

            // idと一致するデータを取得。なければnullを返す
            // FindAsync(id)でもいいらしい。こっちがシンプルで良さそう

            // FirstOrDefaultAsync：主キー以外で条件つけて検索できる
            // FindAsync：主キーで検索でき、キャッシュも残るのでエコ
            var sample = await _context.Samples
                // .FirstOrDefaultAsync(m => m.id == id);
                .FindAsync(id);
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

        // [Bind("id,title,sub_title")]：必要なデータしか受け取らないことで、セキュリティを高める(オーバーポストを防ぐ)
        // ただし、idは不要：自動インクリメントされるため、外部から渡す必要はない。むしろ危険なので不要
        // bindはモデルのプロパティで自動できまるので、毎回しっかり開発者が受け取るべきものだけに整理しないといけない

        // Modelが変更されたら、Bindも変更しないといけない
        // コントローラーからは例えば引数の Sample sample でModelが判断できる
        // modelからはvs codeで例えばpublic class Sampleモデルを右クリックですべての参照を検索でみつけれる
        public async Task<IActionResult> Create([Bind("title,sub_title")] Sample sample)
        {
            // バリデーション問題なければ登録
            if (ModelState.IsValid)
            {
                _context.Add(sample);//コンテキストにsampleオブジェクトを追加
                await _context.SaveChangesAsync();//コンテキストの内容を非同期でDBに新規データを保存
                return RedirectToAction(nameof(Index));//保存後にリダイレクトして、サーバー側でIndexメソッドを実行
            }
            return View(sample);//バリデーション失敗の時はsample/createを表示
        }

        // GET: Samples/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _context.Samples.FindAsync(id);
            if (sample == null)
            {
                return NotFound();
            }
            return View(sample);
        }

        // POST: Samples/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,sub_title")] Sample sample)
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

            // id=引数のint idでルートパラメータのidを受け取る
            // sample.id=Bindのid(リクエストデータだがルートパラメータも受けとってる)
            if (id != sample.id)
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
                    if (!SampleExists(sample.id)) //idを持ったデータがあるか確認
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _context.Samples
                .FirstOrDefaultAsync(m => m.id == id);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sample = await _context.Samples.FindAsync(id);
            if (sample != null)
            {
                _context.Samples.Remove(sample);

                // ↓今までeditとcreateではifの中でSaveChangesAsyncしていたが、ここではしてないのは謎です
                // コードの一貫性やデータなくてもSaveChangesAsync実行するのが、ぱっと見無駄に見えるので、個人的には好ましくない
                // await _context.SaveChangesAsync();
                // return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SampleExists(int id)
        {
            return _context.Samples.Any(e => e.id == id);
        }
    }
}
