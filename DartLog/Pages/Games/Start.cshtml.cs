using System.Security.Claims;
using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DartLog.Pages.Games
{
    public class StartModel : PageModel
    {
        private readonly DartLogContext _context;

        public StartModel(DartLogContext context)
        {
            _context = context;
        }

        // 今はプレイヤー選択をしないので、一覧もバインドも不要
        // 画面側は「ゲーム開始」ボタンだけでOK

        // GET：ゲーム開始画面（説明とボタンだけを表示）
        public void OnGet()
        {
            // 特に何もしない
        }

        // POST：ゲーム開始（ログインユーザーのゲームを1件作成）
        public async Task<IActionResult> OnPostAsync()
        {
            // ログインユーザーIDを取得
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // 認証されていない場合はログインへ
                return Challenge();
            }

            // Game を作成（開始時点で in_progress）
            var game = new Game
            {
                UserId = userId,
                PlayedAt = DateTime.Now,
                TotalScore = 0,
                Status = "in_progress",
                Memo = null
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // gameId を付けて Play ページへ
            return RedirectToPage("/Games/Play", new { gameId = game.Id });
        }
    }
}
