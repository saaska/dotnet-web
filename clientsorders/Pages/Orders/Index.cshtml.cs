using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;

using LazZiya.TagHelpers;

namespace ClientsOrders.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ClientsOrders.Models.SqlServerDbContext _context;

        public IndexModel(ClientsOrders.Models.SqlServerDbContext context) => _context = context;

        public IList<OrderDto> Order { get;set; }

        public async Task OnGetAsync(int p=1, int pSize=20, string sortBy="", string q="")
        {            
            var orders = from o in _context.Orders select o;
            int TotalRecords = await orders.CountAsync();
            int FilteredRecords = TotalRecords;

            // фильтрация
            if (String.IsNullOrEmpty(q)) q = "";
            if (q != "")
            {
                orders = orders.Where(o => o.Name.Contains(q));
                FilteredRecords = await orders.CountAsync();
            }

            // сортировка
            switch (sortBy)
            {
                case "Name":
                    orders = orders.OrderBy(o => o.Name);
                    break;
                case "Name_desc":
                    orders = orders.OrderByDescending(o => o.Name);
                    break;
                case "CreatedOn_desc":
                    orders = orders.OrderByDescending(o => o.CreatedOn);
                    break;
                default:
                    sortBy = "CreatedOn";
                    orders = orders.OrderBy(o => o.CreatedOn);
                    break;
            }

            // пагинация
            int lastPage = (int) Math.Ceiling(1.0f * FilteredRecords / pSize);
            if (lastPage == 0) lastPage = 1;
            p = Math.Clamp(p, 1, lastPage);

            // отбираем из Dto по размеру страницы
            Order = await orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    CreatedOn = o.CreatedOn,
                    Status = o.Status,
                    ClientName = o.Client.Name
                }).Skip(pSize * (p - 1)).Take(pSize).ToListAsync();

            int firstrec = pSize * (p - 1) + 1,
                fetched = Order.Count(),
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
