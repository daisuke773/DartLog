using System.Security.Claims;
using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DartLog.Pages.Games
{
    /// <summary>
    /// CountUp の「ゲーム開始」ページ。
    /// 画面では説明と「ゲーム開始」ボタンのみを表示し、
    /// POST されたタイミングで Games テーブルに新しいゲームを作成して
    /// プレイ画面へ遷移する。
    /// </summary>
    public class StartModel : PageModel
    {
        private readonly DartLogContext _context;

        public StartModel(DartLogContext context)
        {
            _context = context;
        }

        // GET：ゲーム開始画面の表示（実処理なし）
        public void OnGet()
        {
        }

        // POST：ゲームを1件作成してプレイ画面に遷移
        public async Task<IActionResult> OnPostAsync()
        {
            // ログイン中のユーザーIDを取得
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                // 認証されていない場合はログインページへ誘導
                return Challenge();
            }

            // ゲームの初期レコードを作成（in_progress = プレイ中）
            var game = new Game
            {
                UserId = userId,
                PlayedAt = DateTime.Now,
                TotalScore = 0,
                Status = "in_progress",
                Memo = null
            };

            // DB に保存
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // 作成した gameId を付けて Play ページへリダイレクト
            return RedirectToPage("/Games/Play", new { gameId = game.Id });
        }
    }
}
