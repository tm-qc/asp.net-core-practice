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
    public string Title { get; set; } = String.Empty;//初期値空文字
    public string SubTitle { get; set; } = "";//初期値空文字はこれでもいいらしい

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
