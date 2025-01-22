using System;

namespace SelfAspNet.Middleware;

/// <summary>
/// ミドルウェアはUseXxxxというメソッドで登録するのが普通なので、
/// 拡張メソッドで登録したほうがいいらしい
/// 
/// このクラスがあるとProgram.cs上で登録するときに以下のようにできるようになる
/// 
/// 拡張メソッドなし
/// app.UseMiddleware<HeadersInfoMiddleware>();
/// ↓
/// 拡張メソッドあり
/// app.UseHeadersInfo
/// 
/// 個人的にリポジトリと同じで直感的じゃないし、冗長でややこしい印象だけど、
/// ミドルウェアに関しては公式で推奨されてるので拡張メソッドありきで使う。
/// </summary>
public static class HeadersInfoMiddlewareExtensions
{
    public static IApplicationBuilder UseHeadersInfo(
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<HeadersInfoMiddleware>();
    }
}
