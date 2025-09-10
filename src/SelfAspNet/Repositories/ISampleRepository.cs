using System;

namespace SelfAspNet.Repositories;

// モデル
using SelfAspNet.Models;
// 型IPagedListを使うため
using X.PagedList;

// リポジトリ、リポジトリのインターフェースを実装中メモ
// 
// コントローラーのメソッドをリポジトリ、リポジトリのインターフェースと移植するだけだが、
// 型定義とかで難しくすんなりかけない
// 
// とりあえず今回の場合の作成ポイント
// 
// 1.まずコントローラーかリポジトリにメソッドを作成する
// 2.メソッドを作成してる場合は、型指定しつつメソッドが定義されてるはずなので、それをインターフェースに定義する
// 3.インターフェースをリポジトリに設定する
// 4.リポジトリをProgram.csに登録する
// 5.コントローラーでリポジトリのメソッドを参照する
public interface ISampleRepository
{
    Task<IPagedList<Sample>> GetAllPagerAsync(int page);
    Task CreateAsync(Sample sample);//Task<Sample>→戻り値不要ならTask
}
