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
    public class DetailsModel : PageModel
    {
        private readonly ClientsOrders.Models.MyDbContext _context;

        public DetailsModel(ClientsOrders.Models.MyDbContext context)
        {
            _context = context;
        }

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
    }
}
