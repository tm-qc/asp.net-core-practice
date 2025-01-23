using Microsoft.EntityFrameworkCore;//UseSqlServerにひつようだった
using SelfAspNet.Filters;
using SelfAspNet.Models;
using SelfAspNet.Repository;
using SelfAspNet.Middleware;
using SelfAspNet.Lib;

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


// 構成情報を取得する際に型をマッピングするためのクラスを登録し紐づける
// builder.Services.AddOptionsはbuilder.Services.ConfigureでもOK
builder.Services.AddOptions<MyAppOptions>().
Bind(builder.Configuration.GetSection(nameof(MyAppOptions)));


WebApplication app = builder.Build();

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

// ミドルウェア登録基本
// app.UseMiddleware<HeadersInfoMiddleware>();
app.UseHeadersInfo();

app.Run();


// ミドルウェアの推奨順序のメモ
// インストールした時点でこれは出来てた
// https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/middleware/?view=aspnetcore-9.0#middleware-order

// var app = builder.Build();

// // 開発機なら
// if (app.Environment.IsDevelopment())
// {
//     app.UseMigrationsEndPoint();//マイグレーション実行
// }
// else
// {
//     app.UseExceptionHandler("/Error");//エラーページの設定
//     app.UseHsts();//Strict-Transport-Security応答ヘッダーの設定
// }

// app.UseHttpsRedirection();//HTTPSリダイレクト設定
// app.UseStaticFiles();//静的ファイルを提供
// // app.UseCookiePolicy();//クッキーポリシーの設定

// app.UseRouting();//ルーティング設定
// // app.UseRateLimiter();//レート(呼び出し制限回数)
// // app.UseRequestLocalization();//ローカライズの設定
// // app.UseCors();//CORS(クロスオリジン)要求の設定

// app.UseAuthentication();//認証の設定
// app.UseAuthorization();//認可の設定
// // app.UseSession();//セッションの設定
// // app.UseResponseCompression();//応答の圧縮の設定
// // app.UseResponseCaching();//応答のキャッシュの設定
