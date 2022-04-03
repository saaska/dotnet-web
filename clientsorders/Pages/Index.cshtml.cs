using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;

namespace ClientsOrders.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ClientsOrders.Models.SqlServerDbContext _context;

        public IndexModel(ClientsOrders.Models.SqlServerDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; }

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                .Include(o => o.Client).ToListAsync();
        }
    }
}
