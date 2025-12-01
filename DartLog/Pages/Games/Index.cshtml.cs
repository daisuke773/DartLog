using System.Security.Claims;
using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DartLog.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly DartLogContext _context;

        public IndexModel(DartLogContext context)
        {
            _context = context;
        }

        // ==== 検索条件 ====
        // プレイヤー＝ログインユーザーなので PlayerId は削除
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinScore { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "date_desc";

        // ==== 画面表示用 ====
        public IList<GameListItem> Games { get; set; } = new List<GameListItem>();

        // サマリー用
        public int TotalCount { get; set; }
        public double? AverageScore { get; set; }
        public int? BestScore { get; set; }
        public DateTime? LastPlayedAt { get; set; }

        public class GameListItem
        {
            public int Id { get; set; }
            // 表示上は「プレイヤー名」扱いのままにしておく（中身はログインユーザー名）
            public string PlayerName { get; set; } = "";
            public DateTime PlayedAt { get; set; }
            public int TotalScore { get; set; }
            public string Status { get; set; } = "";
        }

        public async Task OnGetAsync()
        {
            // ログインユーザーIDを取得
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // AuthorizeFolder で基本ログイン必須のはずだが、念のため
                Games = new List<GameListItem>();
                TotalCount = 0;
                AverageScore = null;
                BestScore = null;
                LastPlayedAt = null;
                return;
            }

            // ベースクエリ：ログインユーザーの completed ゲームのみ
            var query = _context.Games
                .Include(g => g.User)
                .Where(g => g.Status == "completed" && g.UserId == userId)
                .AsQueryable();

            // ==== 絞り込み ====
            if (FromDate.HasValue)
            {
                var from = FromDate.Value.Date;
                query = query.Where(g => g.PlayedAt >= from);
            }

            if (ToDate.HasValue)
            {
                var to = ToDate.Value.Date.AddDays(1); // 翌日0時未満まで
                query = query.Where(g => g.PlayedAt < to);
            }

            if (MinScore.HasValue)
            {
                query = query.Where(g => g.TotalScore >= MinScore.Value);
            }

            // ==== サマリー ====
            TotalCount = await query.CountAsync();
            AverageScore = await query
                .Select(g => (double?)g.TotalScore)
                .AverageAsync();

            BestScore = await query
                .MaxAsync(g => (int?)g.TotalScore);

            LastPlayedAt = await query
                .MaxAsync(g => (DateTime?)g.PlayedAt);

            // ==== 並び順 ====
            query = SortOrder switch
            {
                "date_asc" => query.OrderBy(g => g.PlayedAt),
                "score_desc" => query.OrderByDescending(g => g.TotalScore),
                "score_asc" => query.OrderBy(g => g.TotalScore),
                _ => query.OrderByDescending(g => g.PlayedAt), // date_desc
            };

            // ==== データ取得 ====
            Games = await query
                .Select(g => new GameListItem
                {
                    Id = g.Id,
                    // DisplayName があれば優先、なければ UserName
                    PlayerName = g.User.DisplayName ?? g.User.UserName ?? "",
                    PlayedAt = g.PlayedAt,
                    TotalScore = g.TotalScore,
                    Status = g.Status
                })
                .ToListAsync();
        }
    }
}
