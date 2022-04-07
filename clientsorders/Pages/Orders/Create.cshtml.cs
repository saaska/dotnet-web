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
        private readonly ClientsOrders.Models.MyDbContext _context;

        public CreateModel(ClientsOrders.Models.MyDbContext context)
        {
            _context = context;
        }

        public Client Client;

        public IActionResult OnGet(int? ClientId, string retPath="")
        {
            if (ClientId != null)
            {
                Client = _context.Clients.FirstOrDefault<Client>(c => c.Id == ClientId);
            } 
            else
            {
                ViewData["ClientSelect"] = new SelectList(_context.Clients, "Id", "Name");
            }

            ViewData["Status"] = new SelectList(
                from v in Enum.GetValues(typeof(Status)).Cast<Status>()
                select new SelectListItem(((int)(v)).ToString(), v.ToString()),
                "Text", "Value"
            ); 

            ViewData["RetPath"] = (retPath != "" && Url.IsLocalUrl(retPath)) ?
                                   retPath : "./Index";
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; }

        public async Task<IActionResult> OnPostAsync(string retPath="")
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return LocalRedirect((retPath != "" && Url.IsLocalUrl(retPath)) ?
                                 retPath : "./Index");
        }
    }
}