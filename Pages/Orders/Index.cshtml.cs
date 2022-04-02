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

        public IList<OrderDto> Order { get;set; }

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    CreatedOn = o.CreatedOn,
                    Status = o.Status,
                    ClientName = o.Client.Name
                }).ToListAsync();
        }
    }
}
