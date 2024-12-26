using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SelfAspNet.Models;

public class Sample
{
    public int id { get; set; }
    // ビューの@Html.DisplayNameForで表示する文字をここでも設定できる
    [Display(Name = "タイトル")]
    public string title { get; set; } = String.Empty;//初期値空文字
    public string sub_title { get; set; } = "";//初期値空文字はこれでもいいらしい

    // modelで表示を整形する方法
     
    // 今回金額のカラムないのでサンプル
    // DataTypeで型を割り当てたら表示を自動で型に沿った表示にも出来る
    // 金額のカラムなので、[DataType(DataType.Currency)]するとこの値を自動で1000を￥1,000に変換して表示してくれる

    // 表示整形をがんばるくらいなら、この方法で統一して書くといいかも

    // [DataType(DataType.Currency)]
    // public string price { get; set; } = 0;
}
