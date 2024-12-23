using Microsoft.EntityFrameworkCore;

//これが最低限のコンテキストクラス
//dbcontext-efというスニペットでひな形作成
//自分のモデル名にEntityNameを書き換えただけ(今回はSmaple)
namespace SelfAspNet.Models {
    public class MyContext : DbContext {
        //
        public MyContext (DbContextOptions<MyContext> options) : base (options) {
            // requiredは初期化を補償してnullじゃないことを示すので、一見コンストラクタで初期化+代入が必要に見える
            // が、DbContextが自動で初期化してくれるので不要になる
            // Samples = new DbSet<Sample>(); // 不要な初期化
         }

        // SampleモデルをDbsetしてSmaplesメソッドで操作出来るようにしている
        public required DbSet<Sample> Samples { get; set; }
    }
}