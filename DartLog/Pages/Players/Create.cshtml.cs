using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DartLog.Pages.Players
{
    public class CreateModel : PageModel
    {
        private readonly DartLogContext _context;

        public CreateModel(DartLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Player Player { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Players.Add(Player);
            await _context.SaveChangesAsync();

            // Åö ìoò^å„ÇÕàÍóóÉyÅ[ÉWÇ÷
            return RedirectToPage("/Players/Index");
        }
    }
}
