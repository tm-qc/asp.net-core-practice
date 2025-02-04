using Microsoft.EntityFrameworkCore;//UseSqlServerにひつようだった
using SelfAspNet.Filters;
using SelfAspNet.Models;
using SelfAspNet.Repository;
using SelfAspNet.Middleware;
using SelfAspNet.Lib;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// アプリ全体にフィルターを適用する場合はここに設定(設定後はアプリ再起動が必要)
builder.Services.AddControllersWithViews(
    // options => options.Filters.Add<MyLogAttribute>()
)
// TempDataをセッションで使うための設定(Cookieなら不要で何も書かなくても使える)
.AddSessionStateTempDataProvider()
// 国際化対応の基本設定
.AddViewLocalization(
    LanguageViewLocationExpanderFormat.Suffix,
    options => options.ResourcesPath = "Resources"
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

// セッションを有効にする

// キャッシュについて
// 
// ・ASP.NET COREはキャッシュがデフォで動いていないとのこと(証拠ないけど・・)、AddDistributedMemoryCacheやIMemoryCacheを定義し、メモリ保存で実装。一番シンプルで簡単。
// ・キャッシュ保存先がメモリだったらアプリ再起動で消える
// ・大規模、キャッシュが消えてはいけないツールの場合の本番環境では、Redis、Memcachedなどを利用した分散キャッシュの実装を使用するのが一般的
// ・AddDistributedMemoryCacheやIMemoryCacheは分散キャッシュではなく使用領域をメモリで指定してキャッシュをONにする意味
// ・Redis、Memcachedは保存先を本当に分散できる分散キャッシュ。高負荷、大規模、キャッシュを消してはいけないツールの場合に使用する
// ・LaravelはデフォでCacheフォルダにキャッシュがファイル形式で保存されてるが、本番環境ではRedisやMemcachedを使うのが一般的

// キャッシュサービスで保存先としてメモリを使う
// 
// ※本や公式ではセッション有効化時にキャッシュを定義してたがなくても動いた
// 　正確にはなぜ必要かわからないが、エコのためかな？
// https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0#cache
builder.Services.AddDistributedMemoryCache();
// セッションサービスを登録
builder.Services.AddSession(
    // セッションの設定
    options =>
    {
        // セッションのタイムアウト時間を設定
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        // セッションのクッキーのHTTPOnly属性を設定
        options.Cookie.HttpOnly = true;
        // セッションのクッキーのEssential属性を設定
        // EU系の法律で必須属性になる
        // アプリがセッション情報を管理するためにクッキーを使用する場合、そのクッキーを必須として設定することで、
        // プライバシー同意がなくても利用可能にするため
        options.Cookie.IsEssential = true;        // セッションのクッキー名を設定

        // options.Cookie.Name = ".MyApp.Session";
        // // セッションのクッキーの有効期限を設定
        // options.Cookie.MaxAge = TimeSpan.FromDays(1);
        // // セッションのクッキーの有効なドメインを設定
        // options.Cookie.Domain = "localhost";
        // // セッションのクッキーの有効なパスを設定
        // options.Cookie.Path = "/";
        // // セッションのクッキーのセキュア属性を設定
        // options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
        // // セッションのクッキーのSameSite属性を設定
        // options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    }
);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
// 開発機じゃない場合はエラーページを設定
// 環境変数 ASPNETCORE_ENVIRONMENT が Development 以外の場合はエラーページを設定
// 開発機は ASPNETCORE_ENVIRONMENT が Development だったらエラーページが開発用に表示されるようにデフォでなってるので設定不要
if (!app.Environment.IsDevelopment())
{
    // ここで本番機の場合のエラーページを設定している
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // Web サイトへのアクセスを常に HTTPS に強制するセキュリティ機能
    app.UseHsts();
}

// ちなみに
// 開発環境では app.UseDeveloperExceptionPage() により 詳細なエラー情報 が表示できる機能もある（エラー時に画面が真っ白になるのを防げる）
if (app.Environment.IsDevelopment())
{
    // ここで開発機の場合のエラーページで詳細を出すように設定もできる
    // エラー特定できないときに使ってみるといいかも
    app.UseDeveloperExceptionPage();
}

// URLを/Samples/Error/{0}にしたうえでエラーページを表示する
// 例）/Samples/存在しないURL → /Samples/Error/404
app.UseStatusCodePagesWithRedirects("/Samples/Error/{0}");

// URLが変わらずにエラーページを表示する
// 例）/Samples/存在しないURL → /Samples/存在しないURLでエラーを表示
// 個人的にこっちの方が良さそう(ユーザに与える情報が少ない)
// app.UseStatusCodePagesWithReExecute("/Samples/Error/{0}");

app.UseHttpsRedirection();

// このメソッドでプロジェクト直下のwwwrootフォルダを静的ファイルのルートディレクトリとして認識される
// localの例) wwwroot\css\sample.cssにhttp://localhost:5103/css/sample.cssでアクセスできる
// 
// 補足
// 既定のファイル：URLでファイル名を省略した場合(フォルダ名だけでアクセス)に表示されるファイル
// 　　　　　　　　既定ではindex.html、index.htm、default.html、default.htmの順で検索される
// フォルダ参照：フォルダ名だけでアクセスした場合に、表示するファイルがない場合にフォルダ内のファイル一覧が表示される。既定はfalseで表示されない
// 
// この設定もUseFileServer()(UseStaticFilesの機能も備えてる)で細かく触れるが、基本的にはデフォルトで問題ないのでさわらなくていい
app.UseStaticFiles();

// 静的リソースUseStaticFilesの応用
// 静的リソースをおくフォルダを追加する
// 
// 例えば、MyStorageフォルダをPJ直下作成し静的リソースを置いて、http://localhost:5103/storage/sample.jpgでアクセスできるようにする
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//     Path.Combine(builder.Environment.ContentRootPath, "MyStorage")),
//     RequestPath = "/storage",
// });

// 静的リソースUseStaticFilesの応用
// プロジェクト直下のwwwrootフォルダに対してキャッシュの設定
// app.UseStaticFiles(new StaticFileOptions
// {
//     OnPrepareResponse = staticContext =>
//     {
//         // publicはキャッシュの指定値(Cache-Control のキャッシュ制御ディレクティブ)
//         // public：各ユーザーのブラウザのキャッシュやCDN やプロキシサーバーのキャッシュに保存できる。主にjs、css、画像など誰が見ていい静的リソースに使う
//         // private：各ユーザーのブラウザのキャッシュに保存できるが、CDN やプロキシサーバーには保存できない。主にセッション情報などユーザー固有の個人情報に使う
//         // 
//         // 60 * 60 * 24 * 3 = 259200 秒、すなわち 3日間 のキャッシュ有効期間
//         staticContext.Context.Response.Headers.Append(
//           "Cache-Control", $"public, max-age={60 * 60 * 24 * 3}");
//     }
// });

app.UseRouting();

app.UseAuthorization();

// セッションを有効にする
// 
// 記載する順番に注意
// ・app.UseRouting();の後
// ・MapDefaultControllerRouteの前
// ・MapRazorPagesの前
app.UseSession();

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

// HttpContext.Itemsでリクエスト、レスポンスにかかわる情報を渡せる
// 
// リクエスト、レスポンスにかかわる情報を管理できるオブジェクト
// リクエスト時に初期化、レスポンス時に削除される
// 
// 使いどころ
// ミドルウェア間で情報を共有するだけなので、ミドルウェア→コントローラーなどリクエスト情報などを情報を渡したいときに使える
// ※無理やりこれを使う必要はない。例えばフォームリクエストの値などはここは使わなくて、通常のやり方でコントローラーの引数に直接渡すのが一般的
// 　あくまでリクエスト、レスポンスにかかわる情報を共有するだけで、過度に使わずに簡単なものに留めるのがポイント
// 
// 使うべきでないケース
// フォームデータやリクエストパラメータは ModelBinding や HttpContext.Request を使う。
// アプリケーション全体で共通の情報を管理するなら IHttpContextAccessor か IMemoryCache を使う。
// スレッドセーフであることが求められる場合（Items はスレッドセーフではない）。
// など

app.Use(async (context, next) =>
{
    context.Items["httpContextCurrent"] = DateTime.Now;
    // 呼び出すことで、次のミドルウェアに処理を渡す
    // 呼ばないと、次のミドルウェアが実行されず、リクエストの流れが途中で止ま
    await next.Invoke();
});

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

// リクエストに応じて言語を決定する仕組みの設定
app.UseRequestLocalization( options =>{
        // アプリで使用するカルチャの一覧
        var cultures = new[]{"ja","de","en"};

        // デフォルトのカルチャを設定(今回は[0]=ja)
        options.SetDefaultCulture(cultures[0]);
        // アプリ内部でサポートするカルチャを設定
        // ※内部とはWeb アプリケーションの内部処理 (例えば、日付や数値の表示形式、通貨の単位など)
        options.AddSupportedCultures(cultures);
        // ユーザーインターフェース (UI)でサポートするカルチャを設定
        // ※アプリの文字列リソース（例：メニュー、ボタン、エラーメッセージなど）をどの言語で表示するかのために使います
        options.AddSupportedUICultures(cultures);
    }
);

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
