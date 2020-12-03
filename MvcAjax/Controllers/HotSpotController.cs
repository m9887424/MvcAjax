using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MvcAjax.Models;
using PagedList;

namespace MvcAjax.Controllers
{
    public class HotSpotController : Controller
    {
        // GET: HotSpot
        public async Task<ActionResult> Index(string districts, string types, string companys, int? page)
        {
            string TargetUri = "https://data.ntpc.gov.tw/api/datasets/04958686-1B92-4B74-889D-9F34409B272B/json/";

            HttpClient client = new HttpClient();

            client.MaxResponseContentBufferSize = Int32.MaxValue;

            var res = await client.GetStringAsync(TargetUri);

            var collect = JsonConvert.DeserializeObject<IEnumerable<HotSpot>>(res);

            //區域 Start
            if (collect != null)
            {
                var dist = collect.OrderBy(x => x.District).Select(x => x.District).Distinct().ToList();
                var districtSelectList = dist.Select(item => new SelectListItem()
                {
                    Text = item,
                    Value = item,
                    Selected = !string.IsNullOrWhiteSpace(districts) && item.Equals(districts,StringComparison.OrdinalIgnoreCase)
                    //不區分大小寫序數比較
                });

                ViewBag.Districts = districtSelectList.ToList();
            }

            ViewBag.SelectedDistrict = districts;

            //區域 End

            //熱點 Start
            if (collect != null)
            {
                var typ = collect.OrderBy(x => x.Type).Select(x => x.Type).Distinct().ToList();
                var typeSelectList = typ.Select(item => new SelectListItem()
                {
                    Text = item,
                    Value = item,
                    Selected = !string.IsNullOrWhiteSpace(types) && item.Equals(types, StringComparison.OrdinalIgnoreCase)
                });

                ViewBag.Types = typeSelectList.ToList();
            }

            ViewBag.SelectedType = types;

            //熱點 End

            //公司 Start
            if (collect != null)
            {
                var comp = collect.OrderBy(x => x.Company).Select(x => x.Company).Distinct().ToList();
                var companySelectList = comp.Select(item => new SelectListItem()
                {
                    Text = item,
                    Value = item,
                    Selected = !string.IsNullOrWhiteSpace(companys) && item.Equals(companys, StringComparison.OrdinalIgnoreCase)
                });

                ViewBag.Companys = companySelectList.ToList();
            }

            ViewBag.SelectedCompany = companys;

            //公司 End

            var source = collect.AsQueryable();

            if (!string.IsNullOrWhiteSpace(districts))
            {
                source = source.Where(x => x.District == districts);
            }

            if (!string.IsNullOrWhiteSpace(types))
            {
                source = source.Where(x => x.Type == types);
            }

            if (!string.IsNullOrWhiteSpace(companys))
            {
                source = source.Where(x => x.Company == companys);
            }

            // return View(source.OrderBy(x => x.District).ToList());

            int pageIndex = page ?? 1;
            int pageSize = 10;
            int totalCount = 0;

            totalCount = source.Count();

            source = source.OrderBy(x => x.District)
                           .Skip((pageIndex - 1) * pageSize)
                           .Take(pageSize);

            var pagedResult =
                new StaticPagedList<HotSpot>(source, pageIndex, pageSize, totalCount);

            return View(pagedResult);

        }
    }
}