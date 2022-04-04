using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;

namespace ClientsOrders.Pages.Orders
{
    public class DeleteModel : PageModel
    {
        private readonly ClientsOrders.Models.SqlServerDbContext _context;

        public DeleteModel(ClientsOrders.Models.SqlServerDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string retPath="")
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Orders
                .Include(o => o.Client).FirstOrDefaultAsync(m => m.Id == id);

            if (Order == null)
            {
                return NotFound();
            }

            ViewData["RetPath"] = (retPath != "" && Url.IsLocalUrl(retPath)) ?
                                   retPath : "./Index";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string retPath="")
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Orders.FindAsync(id);

            if (Order != null)
            {
                _context.Orders.Remove(Order);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return LocalRedirect((retPath != "" && Url.IsLocalUrl(retPath)) ?
                                 retPath : "./Index");
        }
    }
}
