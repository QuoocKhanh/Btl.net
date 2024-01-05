using BTL.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BTL.Controllers
{
    public class NewsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
           
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 3;
            //lấy tất cả các bản ghi trong table Products
            List<ItemNews> list_record = db.News.OrderByDescending(item => item.Id).ToList();
            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));
        }
        public IActionResult Detail(int? id)
        {
            int _id = id ?? 0;
            ItemNews record = db.News.Where(item => item.Id == _id).FirstOrDefault();
            return View("Detail", record);
        }
    }
}
