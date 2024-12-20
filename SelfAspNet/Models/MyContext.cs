using Microsoft.EntityFrameworkCore;

//これが最低限のコンテキストクラス
//dbcontext-efというスニペットでひな形作成
//自分のモデル名にEntityNameを書き換えただけ(今回はSmaple)
namespace SelfAspNet.Models {
    public class MyContext : DbContext {
        public MyContext (DbContextOptions<MyContext> options) : base (options) { }
        public DbSet<Sample> Samples { get; set; }
    }
}