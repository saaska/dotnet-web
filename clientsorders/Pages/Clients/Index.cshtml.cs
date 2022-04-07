using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;

namespace ClientsOrders.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly ClientsOrders.Models.MyDbContext _context;

        public IndexModel(ClientsOrders.Models.MyDbContext context) => _context = context;

        public IList<Client> Client { get;set; }

        public async Task OnGetAsync(int p=1, int pSize=20, string sortBy="", string q="")
        {            
            var clients = from o in _context.Clients select o;
            int TotalRecords = await clients.CountAsync();
            int FilteredRecords = TotalRecords;

            // фильтрация
            if (String.IsNullOrEmpty(q) ) q = "";
            if (q != "")
            {
                clients = clients.Where(o => o.Name.Contains(q));
                FilteredRecords = await clients.CountAsync();
            }

            // сортировка
            switch (sortBy)
            {
                case "Name_desc":
                    clients = clients.OrderByDescending(o => o.Name);
                    break;
                default:
                    sortBy = "Name";
                    clients = clients.OrderBy(o => o.Name);
                    break;
            }

            // пагинация
            int lastPage = (int) Math.Ceiling(1.0f * FilteredRecords / pSize);
            if (lastPage == 0) lastPage = 1;
            p = Math.Clamp(p, 1, lastPage);

            // отбираем по размеру страницы
            Client = await clients.Include(c => c.Orders)
                .Skip(pSize * (p - 1)).Take(pSize).ToListAsync();

            int firstrec = pSize * (p - 1) + 1,
                fetched = Client.Count(),
                lastrec = firstrec + fetched - 1;

            ViewData["PageNo"] = p;
            ViewData["PageSize"] = pSize;
            ViewData["TotalRecords"] = TotalRecords;
            ViewData["FilteredRecords"] = FilteredRecords;
            ViewData["Q"] = q;
            ViewData["LocationMsg"] = fetched == 0 ? "No records found" :
                $"Showing records {firstrec}–{lastrec}" +
                (q!="" ? $" of {FilteredRecords} for \"{q}\"" : "");
            ViewData["SortBy"] = sortBy;
            // какую сортировку сделает клик по заголовку
            ViewData["NameSortLink"] =
                sortBy == "Name" ? "Name_desc" : "Name";
            ViewData["CreatedOnSortLink"] =
                sortBy == "CreatedOn_desc" ? "CreatedOn" : "CreatedOn_desc";
        }
    }
}
