using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// DbContext�iPostgreSQL �ڑ��j
// ==============================
builder.Services.AddDbContext<DartLogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Npgsql �̃^�C���X�^���v�݊��ݒ�i�������̂܂܁j
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ==============================
// Identity�i���O�C���@�\�j
// ==============================
// ApplicationUser = �v���C���[�����O�C�����[�U�[
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // ���[���m�F�Ȃ��Ń��O�C�����i�ȈՉ^�p�j
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<DartLogContext>();

// ==============================
// Razor Pages
// ==============================
builder.Services.AddRazorPages(options =>
{
    // �A�v���S�̂���{���O�C���K�{�ɂ���
    options.Conventions.AuthorizeFolder("/");

    // Identity �̃��O�C���^�o�^�y�[�W�͓����A�N�Z�X����
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");
});

// ���� API �R���g���[�����g���Ă���Ȃ炻�̂܂܌p��
builder.Services.AddControllers();

var app = builder.Build();

// ==============================
// �~�h���E�F�A�p�C�v���C��
// ==============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // �J�����Ȃ�}�C�O���[�V�����G���h�|�C���g�i�C�Ӂj
    // app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// �� �F�� �� �F�� �̏��Ԃ��d�v
app.UseAuthentication();
app.UseAuthorization();

// API ���[�g�i�K�v�Ȃ�j
app.MapControllers();

// Razor Pages ���[�g
app.MapRazorPages();

app.Run();
