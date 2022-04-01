using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using dotnet_web.Models;

namespace dotnet_web
{
    public class ClientListModel : PageModel
    {
        private readonly dotnet_web.Models.SqlServerDbContext _context;

        public ClientListModel(dotnet_web.Models.SqlServerDbContext context)
        {
            _context = context;
        }

        public IList<Client> Client { get;set; }

        public async Task OnGetAsync()
        {
            Client = await _context.Clients.ToListAsync();
        }
    }
}
