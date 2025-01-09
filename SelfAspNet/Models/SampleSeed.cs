using Microsoft.EntityFrameworkCore;
using SelfAspNet.Models;

namespace SelfAspNet.Models;

// 起動時にデータを入れるためのSeedクラスだが、Program.csに設定したら動くので、クラス名自体に決まりはない
// わかりやすければＯＫ
public static class SampleSeed
{
    public static async Task Initialize(IServiceProvider provider)
    {
        using MyContext db = new MyContext(provider.GetRequiredService<DbContextOptions<MyContext>>());

        // ここで「テーブルデータの削除とIDのリセット」組み込むか、別に削除してから「データが存在したら終了」させるかは用途による
        // 起動の度に毎回のこのデーターにリセットしていいなら、「テーブルデータの削除とIDのリセット」いれてもOK

        // テーブルデータの削除とIDのリセット
        // DBCCはSQL Server専用のオートインクリメントリセット
        // await db.Database.ExecuteSqlRawAsync("DELETE FROM SampleRelation1;");
        // await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('SampleRelation1', RESEED, 0);");
        // await db.Database.ExecuteSqlRawAsync("DELETE FROM Samples;");
        // await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Samples', RESEED, 0);");

        // データが存在したら終了
        if(await db.Samples.AnyAsync()) return;

        // 親データ定義
        Sample sample1 = new Sample { Title = "タイトル1cs", SubTitle = "サブタイトル1cs" };
        Sample sample2 = new Sample { Title = "タイトル2cs", SubTitle = "サブタイトル2cs" };
        Sample sample3 = new Sample { Title = "タイトル3cs", SubTitle = "サブタイトル3cs" };

        // リレーションの時に親のidが呼べないので上記の変数でいれてる
        // Sample samples = new List<Sample>+foreachでも出きるっぽいけど、全然シンプルじゃないからさけたい
        // idを参照して子のデータにいれるもっと簡単で良い方法ありそうだけどないのかな・・データ登録するだけなのに不便すぎる

        // db.Samples.AddRange(
        //     new Sample { Title = "タイトル1", SubTitle = "サブタイトル1" },
        //     new Sample { Title = "タイトル2", SubTitle = "サブタイトル2" },
        //     new Sample { Title = "タイトル3", SubTitle = "サブタイトル3" }
        // );

        // 親データ追加
        db.Samples.AddRange(sample1,sample2,sample3);
        await db.SaveChangesAsync();

        // リレーションの子テーブルにデータを追加
        db.SampleRelation1.AddRange(
            new SampleRelation1 { SampleId = sample1.Id, RTitle = "リレーションタイトル1cs", },
            new SampleRelation1 { SampleId = sample2.Id, RTitle = "リレーションタイトル2cs", }
        );
        await db.SaveChangesAsync();
    }
}
