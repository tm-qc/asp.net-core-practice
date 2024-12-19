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
}
