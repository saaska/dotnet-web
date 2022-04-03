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
        public int totalRecords { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }

        public async Task OnGetAsync(int p=1, int psize=20)
        {
            pageNo = p;
            pageSize = psize;
            totalRecords = (from o in _context.Orders select o).Count();
            Console.Write($"\n\n Skip {pageSize * (pageNo-1)} ********************\n\n");

            Order = await _context.Orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    CreatedOn = o.CreatedOn,
                    Status = o.Status,
                    ClientName = o.Client.Name
                }).Skip(pageSize * (pageNo-1)).Take(pageSize).ToListAsync();
        }
    }
}
