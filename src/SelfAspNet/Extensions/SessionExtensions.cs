using System;
using System.Text.Json;
namespace SelfAspNet.Extensions;

/// <summary>
/// SessionExtensions クラスは、セッションオブジェクト (ISession) を拡張し、
/// 任意の型 (T) の値をセッションに格納・取得するための便利なメソッド (Set<T>、Get<T>) を提供する
/// 
/// 拡張メソッドとは？
/// 
/// ・拡張メソッドは、既存のクラスやインターフェースに新しいメソッドを追加する機能
/// ・拡張メソッドは、static静的クラスで定義する
/// ・拡張メソッドは、第一引数にthis修飾子を付ける
/// 　(実質既存の型に新しいメソッドを追加してる)
/// 　(今回はthis ISessionで拡張先のIsessionオブジェクトと連携してる)
/// ・クラス名は拡張する型名の後ろにExtensionsを付けるのが一般的だが、固定値ではないのでExtensionsという名前じゃなくても動く
/// </summary>
public static class SessionExtensions
{
  /// <summary>
  /// 渡されたオブジェクト (value) を JsonSerializer.Serialize() でJSON文字列に変換し、session.SetString() でセッションに文字列として格納
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="session"></param>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void Set<T>(this ISession session, string key, T value)
  {
    // コンソールで確認できるようにシリアライズされた値を表示
    Console.WriteLine("シリアライズされた値：" + JsonSerializer.Serialize(value));
    session.SetString(key, JsonSerializer.Serialize(value));
  }

  /// <summary>
  /// セッションから文字列として格納された値を取得し、
  /// JsonSerializer.Deserialize<T>() で指定された型 (T) のオブジェクトにデシリアライズ (復元) し返す
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="session"></param>
  /// <param name="key"></param>
  /// <returns></returns>
  public static T? Get<T>(this ISession session, string key)
  {
    string? value = session.GetString(key);
    return value != null ? JsonSerializer.Deserialize<T>(value) : default;
  }
}