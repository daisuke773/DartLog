using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DartLog.Pages.Games
{
    public class PlayModel : PageModel
    {
        private readonly DartLogContext _context;

        public PlayModel(DartLogContext context)
        {
            _context = context;
        }

        private const int Rounds = 8;
        private const int DartsPerRound = 3;
        private const int TotalThrows = Rounds * DartsPerRound; // 24

        // 画面側でも使いたいので公開プロパティにしておく
        public int MaxRounds => Rounds;
        public int MaxDartsPerRound => DartsPerRound;

        // 画面表示用
        public int GameId { get; set; }
        public string PlayerName { get; set; } = "";
        public int CurrentRound { get; set; }
        public int CurrentDart { get; set; }
        public int CurrentTotalScore { get; set; }
        public bool IsFinished { get; set; }
        public int FinalScore { get; set; }

        // 履歴表示用
        public List<ThrowHistoryView> ThrowHistory { get; set; } = new();

        // 入力されたスコア
        [BindProperty]
        [Required(ErrorMessage = "スコアを入力してください。")]
        [Range(0, 60, ErrorMessage = "スコアは 0〜60 の範囲で入力してください。")]
        public int InputScore { get; set; }

        // GET: ゲーム画面表示（gameId 必須）
        public async Task<IActionResult> OnGetAsync(int? gameId)
        {
            if (gameId == null)
            {
                return NotFound();
            }

            GameId = gameId.Value;

            var game = await _context.Games
                .Include(g => g.Player)
                .FirstOrDefaultAsync(g => g.Id == GameId);

            if (game == null)
            {
                return NotFound();
            }

            PlayerName = game.Player.Name;

            var throws = await _context.Throws
                .Where(t => t.GameId == GameId)
                .OrderBy(t => t.RoundNo)
                .ThenBy(t => t.DartNo)
                .ToListAsync();

            var throwCount = throws.Count;
            CurrentTotalScore = throws.Sum(t => t.Score);

            // 履歴用プロパティを作成
            ThrowHistory = BuildHistoryList(throws);

            if (throwCount >= TotalThrows)
            {
                // 全投終了 → ゲーム終了モード
                IsFinished = true;
                FinalScore = CurrentTotalScore;

                // Game テーブル側も完了状態に更新
                if (game.TotalScore != FinalScore || game.Status != "completed")
                {
                    game.TotalScore = FinalScore;
                    game.Status = "completed";
                    game.EndedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                return Page();
            }

            // まだ続きがある → 次のラウンド / 投目を計算
            var currentIndex = throwCount; // 0-based
            CurrentRound = currentIndex / DartsPerRound + 1;
            CurrentDart = currentIndex % DartsPerRound + 1;

            return Page();
        }

        // POST: 1投分のスコア登録
        public async Task<IActionResult> OnPostAsync(int gameId)
        {
            GameId = gameId;

            // 簡単なバリデーション（負の点数はナシ）
            if (InputScore < 0)
            {
                ModelState.AddModelError(nameof(InputScore), "スコアは 0 以上を入力してください。");
            }

            var game = await _context.Games.FindAsync(GameId);
            if (game == null)
            {
                return NotFound();
            }

            var throws = await _context.Throws
                .Where(t => t.GameId == GameId)
                .OrderBy(t => t.RoundNo)
                .ThenBy(t => t.DartNo)
                .ToListAsync();

            var throwCount = throws.Count;

            // すでに24投投げ終わっている場合は何もしない
            if (throwCount >= TotalThrows)
            {
                return RedirectToPage(new { gameId = GameId });
            }

            if (!ModelState.IsValid)
            {
                // GET と同じ情報を再セットして、同じ画面に戻す
                var currentIndexForError = throwCount;
                PlayerName = (await _context.Players.FindAsync(game.PlayerId))?.Name ?? "";
                CurrentTotalScore = throws.Sum(t => t.Score);
                CurrentRound = currentIndexForError / DartsPerRound + 1;
                CurrentDart = currentIndexForError % DartsPerRound + 1;
                IsFinished = false;
                ThrowHistory = BuildHistoryList(throws);

                return Page();
            }

            // 次のラウンド / 投目を計算
            var currentIndex = throwCount;
            var roundNo = currentIndex / DartsPerRound + 1;
            var dartNo = currentIndex % DartsPerRound + 1;

            // Throw を追加
            var th = new Throw
            {
                GameId = GameId,
                RoundNo = roundNo,
                DartNo = dartNo,
                Score = InputScore
            };

            _context.Throws.Add(th);
            await _context.SaveChangesAsync();

            // PRG パターン（Post-Redirect-Get）
            return RedirectToPage(new { gameId = GameId });
        }

        /// <summary>
        /// 投げた履歴（累計スコア付き）を View 用に組み立てる
        /// </summary>
        private List<ThrowHistoryView> BuildHistoryList(List<Throw> throws)
        {
            var result = new List<ThrowHistoryView>();
            int runningTotal = 0;

            // ラウンド / 投目順に累計を計算
            foreach (var t in throws.OrderBy(t => t.RoundNo).ThenBy(t => t.DartNo))
            {
                runningTotal += t.Score;

                result.Add(new ThrowHistoryView
                {
                    RoundNo = t.RoundNo,
                    DartNo = t.DartNo,
                    Score = t.Score,
                    TotalScoreAfter = runningTotal
                });
            }

            // 画面では新しいものから表示したいので逆順に
            return result
                .OrderByDescending(x => x.RoundNo)
                .ThenByDescending(x => x.DartNo)
                .ToList();
        }
    }

    /// <summary>
    /// View 用の履歴表示DTO
    /// </summary>
    public class ThrowHistoryView
    {
        public int RoundNo { get; set; }
        public int DartNo { get; set; }
        public int Score { get; set; }
        public int TotalScoreAfter { get; set; }
    }
}
