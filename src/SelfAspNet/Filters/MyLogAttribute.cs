using Microsoft.AspNetCore.Mvc.Filters;

namespace SelfAspNet.Filters;

// このフィルターの利用制限を記載したアノテーション
// AttributeTargets.Class：クラスに対してだけ利用可能
// AttributeTargets.Method：メソッドに対してだけ利用可能
// AllowMultiple = false：複数の属性を設定は不可
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method
    , AllowMultiple = false)]

// IActionFilter：アクションで実行するためのインターフェースを継承
// Attribute：属性クラスとして使用するためのインターフェースを継承
public class MyLogAttribute : Attribute, IActionFilter
{

    // IActionFilterインターフェースのメソッドを実装
    // 色々メソッドあるが今回はIActionFilterを実装してるので以下を実装

    // アクションメソッドが実行される前(モデルバインドのあと)に呼び出されるメソッド
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // おためしなのでConsole.WriteLineで出力
        Console.WriteLine($"【Before】{context.ActionDescriptor.DisplayName}が実行されます。");
    }

    // アクションメソッドが実行された後(IActionResult実行前=ビューに返す前)に呼び出されるメソッド 
    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"【After】{context.ActionDescriptor.DisplayName}が実行されました。");
    }
    
}

// ActionFilterAttributeクラスで書き換えた場合
// public class MyLogAttribute : ActionFilterAttribute
// {
//     public override void OnActionExecuting(ActionExecutingContext context)
//     {
//         Console.WriteLine($"【Before】{context.ActionDescriptor.DisplayName}が実行されます。");
//     }

//     public override void OnActionExecuted(ActionExecutedContext context)
//     {
//         Console.WriteLine($"【After】{context.ActionDescriptor.DisplayName}が実行されました。");
//     }
// }