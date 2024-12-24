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
}
