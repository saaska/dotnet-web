using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;

namespace ClientsOrders.Pages.Orders
{

    public class EditModel : PageModel
    {
        private readonly ClientsOrders.Models.MyDbContext _context;

        public EditModel(ClientsOrders.Models.MyDbContext context)
            => _context = context;

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
            
            ViewData["ClientSelect"] = new SelectList(_context.Clients, "Id", "Name");

            ViewData["Status"] = new SelectList(
                from v in Enum.GetValues(typeof(Status)).Cast<Status>()
                select new SelectListItem(((int)(v)).ToString(), v.ToString()),
                "Text", "Value"
            );

            ViewData["RetPath"] = (retPath != "" && Url.IsLocalUrl(retPath)) ?
                                   retPath : "./Index";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string retPath="")
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(Order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return LocalRedirect((retPath != "" && Url.IsLocalUrl(retPath)) ?
                                 retPath : "./Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
