using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using dotnet_web.Models;

namespace dotnetweb.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly dotnet_web.Models.SqlServerDbContext _context;

        public IndexModel(dotnet_web.Models.SqlServerDbContext context)
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
