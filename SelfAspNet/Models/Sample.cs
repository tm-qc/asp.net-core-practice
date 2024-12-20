using System;

namespace SelfAspNet.Models;

public class Sample
{
    public int id { get; set; }
    public string title { get; set; } = String.Empty;//初期値空文字
    public string sub_title { get; set; } = "";//初期値空文字はこれでもいいらしい
}
