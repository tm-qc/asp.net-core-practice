using System;
using System.Text;

namespace SelfAspNet.Middleware;

public class HeadersInfoMiddleware
{

    // RequestDelegate：ミドルウェアを次に進めるミドルウェアのnext関数
    private readonly RequestDelegate _next;

    public HeadersInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// HttpContextでHTTPリクエストに関するすべてのHTTP固有の情報を取得し書き出す
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 文字連結を5個以上する場合はStringBuilderのほうが速い
        // +とStringBuilderで10万回試行すると3000倍速い
        StringBuilder str = new StringBuilder();
        
        // ===Request Headers Info===の後に改行追加
        str.AppendLine("===Request Headers Info===");
        // 目印
        str.Append("[Middleware自作テストログ]").AppendLine();

        // リクエストの情報ごとに改行を追加する
        foreach (var header in context.Request.Headers)
        {
            str.AppendLine($"{header.Key}:{header.Value}");
        }

        // 文字列をコンソールに簡易的に書き出し
        Console.WriteLine(str.ToString());

        // 次のミドルウェアへ進む
        await _next(context);
    }
}
