using Microsoft.EntityFrameworkCore;

//これが最低限のコンテキストクラス
//dbcontext-efというスニペットでひな形作成
//自分のモデル名にEntityNameを書き換えただけ(今回はSmaple)
namespace SelfAspNet.Models {
    public class MyContext : DbContext {
        //
        public MyContext (DbContextOptions<MyContext> options) : base (options) {
            // DbContextが自動で初期化してくれるので不要になる(あとnew DbSet× Set〇だった)
            // Samples = Set<Sample>(); // 不要な初期化
            // SampleRelation1 = Set<SampleRelation1>(); // 不要な初期化
         }

        // SampleモデルをDbsetしてSmaplesメソッドで操作出来るようにしている
        // またnull!でnull許容型警告を回避
        // データを追加するためにSeed.csを作成していたら、以前書いていたrequiredではエラーになってだめった

        // 時間かかってまとめた資料↓
        // https://github.com/tm-qc/asp.net-core-practice/commit/ebd8feb01a41d2cb7009a1959d03d74e516a34a9#commitcomment-151034219
        public DbSet<Sample> Samples { get; set; } = null!;
        public DbSet<SampleRelation1> SampleRelation1 { get; set; } = null!;
    }
}