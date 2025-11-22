using DartLog.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ★ DbContext（Postgres 接続設定）
builder.Services.AddDbContext<DartLogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ★ Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ★ API のルートを追加
app.MapControllers();

// Razor Pages のルート
app.MapRazorPages();

app.Run();
