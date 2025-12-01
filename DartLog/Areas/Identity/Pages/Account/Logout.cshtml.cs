// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DartLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DartLog.Areas.Identity.Pages.Account
{
    [AllowAnonymous] // ★ ログアウト後のページ表示に必須
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: ログアウト確認画面表示
        public void OnGet()
        {
        }

        // POST: 実際にログアウト処理を行う
        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("ユーザーがログアウトしました。");

            // ★ returnUrl を一切使わず常にこのページへ戻す（=完了画面を表示）
            return RedirectToPage();
        }
    }
}
