using Microsoft.EntityFrameworkCore;//UseSqlServerにひつようだった
using SelfAspNet.Filters;
using SelfAspNet.Models;
using SelfAspNet.Repository;
using SelfAspNet.Middleware;
using SelfAspNet.Lib;
using Microsoft.Extensions.Logging;

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
Bind(builder.Configuration.GetSection(nameof(MyAppOptions))).
ValidateDataAnnotations();

// アプリから環境変数を追加する場合
// 
// なぜDictionary?
// DictionaryもListと同じコレクション
// 
// Dictionary：順不同のLaravelでいう連想配列
// List：順番のLaravelでいう配列
// 
// ちなみにnew Dictionary<string, string?>補足
// stringはキーの型
// string?：値の型でnull許容
builder.Configuration.AddInMemoryCollection(
    new Dictionary<string,string?>{
        ["Company"] = "会社名",
        ["Since"] = "2025-01-23",
    }
);

// ロギングファイル出力のためのプロバイダーを登録
builder.Logging.ClearProviders();
builder.Logging.AddFile(
    // プロジェクト配下のLogsフォルダにログを記録する
    // 事前にLogsフォルダを作成しておかないと起動時にエラーになる
    Path.Combine(builder.Environment.ContentRootPath, "Logs"));


// Swaggerで必要なサービスを登録

// コントローラーを使えるようにする設定
// APIのルーティング（コントローラーのエンドポイント）が有効になります
// builder.Services.AddControllers();
// エンドポイントのメタデータ（HTTPメソッド、ルート情報）を探索可能にする設定
// Swagger がルート情報を収集するために必要です
// builder.Services.AddEndpointsApiExplorer();
// Swagger の生成を有効にする設定
// Swagger UI や JSON ドキュメントを生成するために必要です
// builder.Services.AddSwaggerGen();


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
    // 試しにデフォルトで表示される物を変更
    // http://localhost:5103/ で内容は http://localhost:5103/Samples/Index が表示されるようになる

    // pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Samples}/{action=Index}/{page:int?}");

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

// ログをファイルに出力した場合、これがないとdotnet watchで自動でブラウザが起動しない
// アプリの起動はしてるので、ファイルログをみて自分でブラウザを開く必要がある
// 開発機でのみ明示的にブラウザを開く
if (app.Environment.IsDevelopment())
{
    // IConfigurationで設定ファイルの情報を取得
    IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();
    // SelfAspNet\Properties\launchSettings.jsonのapplicationUrlで指定しているURLを取得
    string? applicationUrl = configuration["ASPNETCORE_URLS"];

    // applicationUrlが取得できなかった場合の処理。基本ないと思うが、本当はエラーで止めるのが良いとは思う
    if (string.IsNullOrEmpty(applicationUrl)){
        Console.WriteLine("Warning : applicationUrlが取得できませんでした。Logsフォルダーのログ Now listening のURLを確認して手動でアクセスしてください");
    }else{
        // アプリケーションが起動したタイミングで実行する処理を登録
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            try
            {
                // ブラウザを開くプロセスを起動
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = applicationUrl,
                    UseShellExecute = true // OSのデフォルトブラウザで開く
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch browser: {ex.Message}");
            }
        });  
    }
}

// Swaggerの有効化
// ルーティングリストがないようなので以下のコマンドでインストールした
// dotnet add package Swashbuckle.AspNetCore
// 
// 【結果】
// APIの仕様を確認するための Swagger だった・・
// 通常のWEBアプリケーションの場合はProgram.csに手動でルーティングリスト出力するためのコードを書く必要がある・・。
// 
// URL
// http://localhost:5103/swagger
// 
// Swagger の JSON ドキュメントを提供するミドルウェア
// 開発者が API の仕様を確認できるようになります
// app.UseSwagger();
// Swagger UI を有効にするミドルウェア
// ブラウザで Swagger ドキュメントを視覚的に確認できるインターフェイスを提供します
// app.UseSwaggerUI();
// コントローラーのルーティングを有効化
// コントローラーに定義したエンドポイントをアプリに登録します
// app.MapControllers();

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
