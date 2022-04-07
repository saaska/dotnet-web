using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;
using ClientsOrders.Pages.Orders;

namespace ClientsOrders.Pages.Clients
{
    public class DetailsModel : PageModel
    {
        private readonly ClientsOrders.Models.MyDbContext _context;

        public DetailsModel(ClientsOrders.Models.MyDbContext context) => _context = context;

        public Client Client { get; set; }

        public IList<OrderDto> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync(
            int? id, int p=1, int pSize=20, string sortBy="", string q="")
        {
            if (id == null) return NotFound();
            Client = await _context.Clients.FirstOrDefaultAsync(m => m.Id == id);
            if (Client == null) return NotFound();
            ViewData["Client"] = Client;
            ViewData["ClientId"] = Client.Id;

            Orders = await DataSet.Filter(
                _context, this, id, p, pSize, sortBy, q
            );

            return Page();
        }
    }
}
