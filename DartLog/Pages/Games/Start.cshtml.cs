using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DartLog.Pages.Games
{
    public class StartModel : PageModel
    {
        private readonly DartLogContext _context;

        public StartModel(DartLogContext context)
        {
            _context = context;
        }

        // プレイヤー一覧（ドロップダウン用）
        public IList<Player> Players { get; set; } = new List<Player>();

        // 選択されたプレイヤーID（POSTで受け取る）
        [BindProperty]
        public int SelectedPlayerId { get; set; }

        // GET：プレイヤー選択画面
        public async Task OnGetAsync()
        {
            Players = await _context.Players
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        // POST：ゲーム開始
        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedPlayerId <= 0)
            {
                ModelState.AddModelError(nameof(SelectedPlayerId), "プレイヤーを選択してください。");
                await OnGetAsync();
                return Page();
            }

            // Game を作成（開始時点で IN_PROGRESS）
            var game = new Game
            {
                PlayerId = SelectedPlayerId,
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
