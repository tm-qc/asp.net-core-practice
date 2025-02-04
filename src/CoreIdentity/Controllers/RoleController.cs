using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreIdentity.Controllers;

public class RoleController : Controller
{
    // 認証ロールのプロパティを定義
    private RoleManager<IdentityRole> _role;
    private UserManager<IdentityUser> _usr;

    // role、userで認証ロールをコンストラクタで格納
    public RoleController(
      RoleManager<IdentityRole> role, UserManager<IdentityUser> usr)
    {
        _role = role;
        _usr = usr;
    }

    // 認証ロールサンプルコード
    // /Role/Createでアクセス
    // 
    // ロールをアカウントに付与するメソッド
    // 
    // 通常はロール付与画面などで上位権限のアカウントがロールを付与する。などが普通なので、
    // こういったアクセスURLで付与はしないと思う。あくまでロールのサンプル
    [Authorize]
    public async Task<IActionResult> Create()
    {
        // Adminロールがそもそも存在しない場合作成
        var roleName = "Admin";
        var exist = await _role.RoleExistsAsync(roleName);
        if (!exist)
        {
            // AspNetUsers：ユーザーテーブル
            // AspNetUserRoles：ユーザーIDとロールIDを紐づけるテーブル
            // AspNetRoles：ロール管理テーブル
            // 
            // この三つのテーブルでロールが管理される
            // ロールが出来るとログインユーザのIDをもとにAspNetUserRolesとAspNetRolesに
            await _role.CreateAsync(new IdentityRole("Admin"));
        }

        // ログインユーザーの情報を取得
        // 
        // GetUserAsyncについて
        // ClaimsPrincipal 型のオブジェクトのユーザデータ
        // これは、認証されたユーザーのクレーム（属性情報）を保持しています。
        // クレームには、ユーザーID、名前、メールアドレス、ロールなど、ユーザーに関する様々な情報が含まれます。
        var current = await _usr.GetUserAsync(User);
        // 今回はユーザーの情報がある場合で判定されてる
        if (current != null)
        {
            // 本当はユーザの情報有無だけじゃなくてロール判定したほうがいいかもね
            // Adminロールもってたらtrue
            bool isAdmin = await _usr.IsInRoleAsync(current, roleName);
            Console.WriteLine($"isAdmin:{isAdmin}");
            // ログインユーザーのロールリストを取得
            var userRoleList = await _usr.GetRolesAsync(current);
            Console.WriteLine($"userRoleList:{userRoleList}");
            
            // 今のログインユーザーにAdminロールを付与
            await _usr.AddToRoleAsync(current, roleName);
        }
        return Content("現在のユーザーをAdminロールに登録しました。");
    }
}
