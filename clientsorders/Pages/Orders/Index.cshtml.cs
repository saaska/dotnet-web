using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ClientsOrders.Models;

namespace ClientsOrders.Pages.Orders
{
    public static class DataSet
    {
        public static async Task<IList<OrderDto> > Filter(
            MyDbContext _context,
            PageModel Model,
            int? ClientId,
            int p = 1, int pSize = 20, string sortBy = "", string q = "")
        {
            var orders = from o in _context.Orders select o;
            if (ClientId != null)
                orders = orders.Where(o => o.ClientId == ClientId);
            int TotalRecords = await orders.CountAsync();
            int FilteredRecords = TotalRecords;

            // фильтрация
            if (String.IsNullOrEmpty(q))
            {
                q = "";
            }
            else
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
                case "CreatedOn":
                    orders = orders.OrderBy(o => o.CreatedOn);
                    break;
                default:
                    sortBy = "CreatedOn_desc";
                    orders = orders.OrderByDescending(o => o.CreatedOn);
                    break;
            }

            // пагинация
            int lastPage = (int)Math.Ceiling(1.0f * FilteredRecords / pSize);
            if (lastPage == 0) lastPage = 1;
            p = Math.Clamp(p, 1, lastPage);

            // отбираем из Dto по размеру страницы
            var Orders = await orders.Select(o => new OrderDto
            {
                Id = o.Id,
                Name = o.Name,
                CreatedOn = o.CreatedOn,
                Status = o.Status,
                ClientName = o.Client.Name
            }).Skip(pSize * (p - 1)).Take(pSize).ToListAsync();

            int firstrec = pSize * (p - 1) + 1,
                fetched = Orders.Count(),
                lastrec = firstrec + fetched - 1;

            var VD = Model.ViewData;
            VD["PageNo"] = p;
            VD["PageSize"] = pSize;
            VD["TotalRecords"] = TotalRecords;
            VD["FilteredRecords"] = FilteredRecords;
            VD["Q"] = q;
            VD["LocationMsg"] = fetched == 0 ? 
                "No records found" + (String.IsNullOrEmpty(q) ? "": $" containing \"{q}\""):
                $"Showing records {firstrec}–{lastrec}" +
                (q != "" ? $" of {FilteredRecords} for \"{q}\"" : "");
            VD["SortBy"] = sortBy;
            // какую сортировку сделает клик по заголовку
            VD["NameSortLink"] =
                sortBy == "Name" ? "Name_desc" : "Name";
            VD["CreatedOnSortLink"] =
                sortBy == "CreatedOn_desc" ? "CreatedOn" : "CreatedOn_desc";

            return Orders;
        }
    }


    public class IndexModel : PageModel
    {
        private readonly ClientsOrders.Models.MyDbContext _context;

        public IndexModel(ClientsOrders.Models.MyDbContext context) => _context = context;

        public IList<OrderDto> Orders { get;set; }

        public async Task OnGetAsync(int p=1, int pSize=20, string sortBy="", string q="")
        {
            Orders = await DataSet.Filter(_context, this, null,
                p, pSize, sortBy, q);
        }
    }
}
