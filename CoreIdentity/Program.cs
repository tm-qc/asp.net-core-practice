using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoreIdentity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ここでログイン機能を有効化している
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()//ロール機能を有効化
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ■標準サービスの注入について
// builder.Services.AddXXX();で使うサービスを定義しビルド時に注入する
// ここを書き換えた後は再ビルドで適用される
// 
// AddControllersWithViews()は以下を含む
// 
// ・AddControllersWithViews：MVCアプリ
// ・AddControllers：WebAPI機能
// ・AddMvcCore:MVC基幹機能
// 
// ▼Razor Pages をメインに利用したい場合
// AddControllersWithViewsには含まれないので以下に変更が必要
// この場合AddControllersWithViews(コントローラーなど)は動かなくなる
// 
// builder.Services.AddRazorPages()
// 
// ▼AddControllersWithViewsとAddRazorPagesの併用
// 
// AddRazorPages
// なお、builder.Services.AddControllersWithViews();を定義したまま
// app.MapRazorPages();を定義すればRazorとMVCの併用ができる
// 
// もしくはそもそも全部入りのサービスを注入してもいい
// builder.Services.AddMvc();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

// ログインページはRazorで実装されてるので追加されてる
app.MapRazorPages();

app.Run();
