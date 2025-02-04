using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SelfAspNet.Models;

public class Sample
{
    // カラム名(プロパティ)はC#の場合PascalCaseが主流
    // 子に記載するリレーションの親の外部キーもNameIdと書くと自動で認識してくれる
    public int Id { get; set; }
    // ビューの@Html.DisplayNameForで表示する文字をここでも設定できる
    [Display(Name = "タイトル")]

    // バリデーションの設定
    // これでクライアント、サーバー双方でバリデーション可能
    // ErrorMessage{0}はプロパティ名、{1}は検証条件などを表示できる
    // 
    // コントローラーで検証結果検知「if (ModelState.IsValid)」
    // ビューで
    [Required(ErrorMessage = "{0}は必須です")]
    public string Title { get; set; } = String.Empty;//文字列として設定
    
    // 課題
    // 入力値がない場合、入力必須にせずに、nullではなく空文字でDB登録したい場合どうする？

    // 結論
    // 入力値がない場合のデフォルトを空文字で登録はマイグレーションファイルだけではできない。
    // ビューで入力必須を回避するために、nullを許容し空文字のときの値はnullで登録される前提で作るしかない
    // 
    // 別途マイグレーションファイル手動追記か、DB側で設定しない限りnullで登録されるがこれは危険なのでしない方がいい
    // カラム設定がString.Emptyでも""でも、空文字ならなずnullで登録される

    // 理由
    // ・Null非許容(stringに?なし)：ASP.NET COREのデフォルトバリデーションで、入力値がないときにビューでエラーになり、入力が強制される
    // ・Null許容(stringに?あり)：入力値なしでもビューにエラーはでないが、nullで登録される
    // ・「[Required(AllowEmptyStrings = true)] // null は非許容だけど、空文字列は許容する」も入力値がないときにビューでエラーになり、入力が強制される

    // 補足
    // どうしても空文字で登録したい場合は、以下の方法しかないっぽいが、整合性あわなくなって「危険すぎる」のでしない方がいい
    // 
    // ・モデルからマイグレーションファイル作った後にdefault値を空文字に設定を手動追記
    // ・DBのSQLで別途初期値設定
    public string? SubTitle { get; set; } = "";//String.Emptyと同じ

    // modelで表示を整形する方法
     
    // 今回金額のカラムないのでサンプル
    // DataTypeで型を割り当てたら表示を自動で型に沿った表示にも出来る
    // 金額のカラムなので、[DataType(DataType.Currency)]するとこの値を自動で1000を￥1,000に変換して表示してくれる

    // 表示整形をがんばるくらいなら、この方法で統一して書くといいかも

    // [DataType(DataType.Currency)]
    // public string price { get; set; } = 0;

    // リレーションサンプル

    // 1対多のリレーション(親→子)
    // public ICollection<SampleRelation1> SampleRelation1 { get;} = new List<SampleRelation1>();
    // C#12からは[]の書き方もできる
    // 子を参照するだけなのでgetのみでOK
    public ICollection<SampleRelation1> SampleRelation1 { get; } = [];
}
