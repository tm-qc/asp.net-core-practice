using System;
using Microsoft.AspNetCore.Mvc;

namespace SelfAspNet.Controllers;

//名前の末尾はControllerにする
//Controllerは継承する(この時usingが自動追加される→using Microsoft.AspNetCore.Mvc;)
public class HelloController : Controller
{
    //mvc-と打つとサジェストが出てくる
    //mvc-core-actionを選択するとメソッドの雛形が出来る

    //IActionResultは他の部分に連携するためのメソッドをもったオブジェクト
    public IActionResult Index(){
       return Content("こんにちは"); 
    }

    public IActionResult Show()
    {
      //ビュー変数：ビューに渡したい変数

      //変数
      //C#の識別子に - が使えないのでViewBagでは名前に - は使えない
      //dynamicオブジェクトらしい
      ViewBag.Message = "こんちにわ。世界";

      //ディクショナリ(コレクションの連想配列)
      ViewData["Message"] = "こんばんわ。世界";

      //↑この二つ別物に見えて、同じもの
      //変数名とキーが同じなら、後で書いてるものに上書きされてる
      //これはやっかいなので書き方は統一する
      //またこの二つよりViewModelを使うのが一般的らしい

      //テンプレート呼び出し
      //引数なしの場合、アクションと一致するテンプレートを呼ぶ
      //Views/コントローラー/アクション名.cshtml
      return View();
    }
}
