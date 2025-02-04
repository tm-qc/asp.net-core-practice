using System;

namespace SelfAspNet.Repository;

// モデル
using SelfAspNet.Models;

// ページネーションのプラグイン使うために追加
using X.PagedList;//型IPagedListを使うため
using X.PagedList.EF;//メソッドToPagedListAsyncを使うため

// リポジトリインターフェース
using SelfAspNet.Repository;

public class SampleRepository : ISampleRepository
{
    // コントローラーからリポジトリに処理を移行する
    // とりあえずindexの全件取得メソッドでサンプルとして作成

    // DB準備
    private readonly MyContext _context;

    // コンストラクタ
    public SampleRepository(MyContext context)
    {
        _context = context;
    }

    /// <summary>
    /// インデックスページで使う全件取得メソッド
    /// ※Nugetのページャライブラリもサンプルで使ってたので、流れで使っています
    /// 
    /// Task：非同期async awaitで処理するための定義
    /// IPagedList：ページャのNugetライブラリを使う場合の型
    /// IQueryable や IEnumerable:Entity Frameworkの場合の型
    /// </summary>
    /// <param name="page">現在のページ数</param>
    /// <returns>表示データ</returns>
    public async Task<IPagedList<Sample>> GetAllPagerAsync(int page = 1){
        int pageSize = 3;
        IPagedList<Sample> samplesNugetList = await _context.Samples.OrderBy(s => s.Id).ToPagedListAsync(page, pageSize);
        return samplesNugetList;
    }

    /// <summary>
    /// 新規登録のメソッド
    /// 
    /// ※本当ならtry catchあったがいいけど時間ないので割愛
    /// </summary>
    /// <param name="sample">バリデーションを通過した入力値</param>
    /// <returns>なし</returns>
    public async Task CreateAsync(Sample sample)
    {
        _context.Add(sample);//コンテキストにsampleオブジェクトを追加
        await _context.SaveChangesAsync();//コンテキストの内容を非同期でDBに新規データを保存
    }
}
