using System;
using Microsoft.AspNetCore.Mvc;
using SelfAspNet.Models;//モデルを利用するために追加

namespace SelfAspNet.Controllers;

//名前の末尾はControllerにする
//Controllerは継承する(この時usingが自動追加される→using Microsoft.AspNetCore.Mvc;)
public class HelloController : Controller
{

  private readonly MyContext _db;

  //DIするためのコンストラクタ
  //DIは整合性を保つために自動でインスタンスを生成してくれる仕組み(引数dbにMyContextのインスタンスが入る)
  public HelloController(MyContext db)
  {
    _db = db;
  }
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

    public IActionResult List()
    {
      //データベースからデータをList<T>のコレクションで取得
      // List<T>の中身は、Sample 型のオブジェクトが順序通りに格納されたリストになります。

      // var samples = _db.Samples.ToList();//varの場合
      // List<Sample> samples = _db.Samples.ToList();

      // これでもいける
      // var samples = _db.Samples;
      IEnumerable<SelfAspNet.Models.Sample> samples = _db.Samples;

      //型のメモ
      // 遅延実行を活用して効率良くデータ処理をしたい → IEnumerable<T>
      // データをメモリにロードして、柔軟に操作したい → List<T>

      // どっち使う？
      // ビューに渡す場合やデータを複数回使う場合:
      // → List<Sample> samples = _db.Samples.ToList();

      // クエリを遅延実行(必要な時だけ取得)してデータベース負荷を軽減したい場合:
      // → IEnumerable<SelfAspNet.Models.Sample> samples = _db.Samples;

      // List<T>の中身のイメージ
      // List<Sample> samples = new List<Sample>()
      // {
      //     new Sample { id = 1, title = "タイトルA", sub_title = "サブタイトルA" }, // 1つ目のSampleオブジェクト
      //     new Sample { id = 2, title = "タイトルB", sub_title = "サブタイトルB" }, // 2つ目のSampleオブジェクト
      //     new Sample { id = 3, title = "タイトルC", sub_title = "サブタイトルC" }  // 3つ目のSampleオブジェクト
      // };
    
      // samples オブジェクトを ViewData.Model または Model プロパティに格納してビューに渡す
      // ビューではsamplesでは参照せずにModelという名前で参照する
      return View(samples);
    }
}
