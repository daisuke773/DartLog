using DartLog.Data;
using DartLog.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DartLog.Pages.Players
{
    public class IndexModel : PageModel
    {
        private readonly DartLogContext _context;

        public IndexModel(DartLogContext context)
        {
            _context = context;
        }

        public IList<Player> Players { get; set; } = new List<Player>();

        public async Task OnGetAsync()
        {
            Players = await _context.Players
                .OrderBy(p => p.Id)
                .ToListAsync();
        }
    }
}
