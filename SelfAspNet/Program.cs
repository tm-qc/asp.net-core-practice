using Microsoft.EntityFrameworkCore;//UseSqlServerにひつようだった
using SelfAspNet.Filters;
using SelfAspNet.Models;
using SelfAspNet.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// アプリ全体にフィルターを適用する場合はここに設定(設定後はアプリ再起動が必要)
builder.Services.AddControllersWithViews(
    // options => options.Filters.Add<MyLogAttribute>()
);

// モデルコンテキストを登録
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyContext")
    )
);

// リポジトリを登録
builder.Services.AddTransient<
    ISampleRepository, 
    SampleRepository
>();

// リポジトリ登録応用
// IServiceCollection AddSampleRepositoryがある場合
// 以下のように短く書けるらしい
// builder.Services.AddSampleRepository();


WebApplication app = builder.Build();

// SampleSeed.csでデータを投入するために追加
// DIコンテナ(機能させるためのクラス)の注入準備でscope作成
using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    // ServiceProviderはこんなものを主に呼び出せ、起動時に設定などできる
    // 
    //  一般的によく使われるもの
    // 
    //  データベースコンテキスト (MyContext)
    //  ロガー (ILogger)
    //  認証関連 (UserManager)
    //  カスタムサービス (独自のビジネスロジック)
    //  設定情報 (IConfiguration)

    // scopeにServiceProvider(アプリ起動時に動く機能)を注入
    IServiceProvider provider = scope.ServiceProvider;

    // SampleSeed.csのInitializeにprovider(ServiceProvider)を渡してMyContextを呼び出せるようにする
    // SampleSeed.InitializeのSampleSeedは動かしたいクラス名になる
    // Initializeがasyncもってるのでawait追加が必要
    await SampleSeed.Initialize(provider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
