using System;

namespace SelfAspNet.Record;

// Recordクラスとは？
// 
// データだけを扱うためのクラスでC#の機能
// 
// 使いどころ
// 固定した構成のデータを定義し参照したほうが良いときに使う
// キーバリューの構成を固定し、ぶれないので安全になるし、ToStringやEqualsなども最初から使えるので読み書き、比較もしやすい
// 
// public class クラス名じゃなくpublic record クラス名で宣言する
// 
// 特徴
// 以下の機能が書かなくても自動で機能し使えるコンパクトなクラス
// 
// コンストラクタ: プロパティを初期化するための引数付きコンストラクタ。
// init専用プロパティ: 読み取り専用プロパティとして値の不変性を保証。
// Equals/GetHashCode: 値に基づく等価性を提供。
// ToString: 人間が読みやすい文字列表現を生成。
// Deconstruct: 値を分解してタプルのように扱える。
// EqualityContract: 型の等価性をチェックする仕組み。
// 
// その他
// ・フォルダ分けとかはPJによって違うけど、何をrecordで作るかはわからないのでどうわけるのがいい？
//   (個人的には現状Recordフォルダを作ってそこに入れてる)
// ・便利っぽいけどC#に慣れるまでは無理しなくてもよさそう。応用編の印象
// ・recordはC#9から追加された機能


/// <summary>
/// Personクラスは名前と年齢を持つ
/// </summary>
/// <param name="Name"></param>
/// <param name="Age"></param>
public record Person(string Name, int Age);