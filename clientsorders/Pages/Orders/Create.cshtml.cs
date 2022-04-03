using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClientsOrders.Models;

namespace ClientsOrders.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ClientsOrders.Models.SqlServerDbContext _context;

        public CreateModel(ClientsOrders.Models.SqlServerDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["Status"] = new SelectList(
                from v in Enum.GetValues(typeof(Status)).Cast<Status>()
                select new SelectListItem(((int)(v)).ToString(), v.ToString()),
                "Text", "Value"
            ); 
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}