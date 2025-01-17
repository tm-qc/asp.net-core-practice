using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoreIdentity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    // 認証ライブラリ ASP.NET CORE Identity をインストールし、
    // Authorize属性をつけるとログインページに誘導される
    // 
    // ちなみにユーザー登録は存在するメールアドレスで登録しないと、ログインできないっぽい
    // AspNetUsersテーブルのEmailConfirmedカラムが0になってたので、1にしてみたらログインできた
    // 
    // UPDATEのSQLサンプル
    // UPDATE [dbo].[AspNetUsers]
    // SET [EmailConfirmed] = 1
    // WHERE [Email] = 't-mine-admin@rustic.co.jp';
    // 
    // ロール機能テスト：ログイン済みでロールがAdminのユーザだけ閲覧可能にした
    [Authorize(Roles = "Admin")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
