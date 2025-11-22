using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DartLog.Pages.Players
{
    public class EditModel : PageModel
    {
        private readonly DartLogContext _context;

        public EditModel(DartLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Player Player { get; set; } = null!;

        // 編集画面の表示（GET）
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            Player = player;
            return Page();
        }

        // 編集内容の保存（POST）
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // そのまま更新
            _context.Attach(Player).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
