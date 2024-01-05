using Microsoft.AspNetCore.Mvc;
//thao tac voi IFormCollection
using Microsoft.AspNetCore.Http;
//doi tuong ma hoa password -> co the gan vao mot bien de su dung bien nay
using BC = BCrypt.Net.BCrypt;
//de nhin thay cac class trong folder Models thi phai using dong duoi
using BTL.Models;
//doi tuong phan trang
using X.PagedList;
//su dung kieu List
using System.Collections.Generic;
//de nhin thay file CheckLogin.cs trong thu muc Attributes
using BTL.Areas.Admin.Attributes;
using System.Security.Cryptography;
using System.Data;//sử dụng cho các đối tượng: DataTable, SqlConnection, DataAdapter, DataCommand...
using Microsoft.Data.SqlClient;


namespace BTL.Areas.Admin.Controllers
{
    //khai báo Route Admin để nhận diện trên url đây là Area Admin
    [Area("Admin")]
    //Gọi Attribute CheckLogin để kiểm tra đăng nhập
    [CheckLogin]
    public class BlogController : Controller
    {
        //khởi tạo đối tượng thao tác csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 50;
            //lấy tất cả các bản ghi trong table Products
            List<ItemNews> list_record = db.News.OrderByDescending(item => item.Id).ToList();
            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));
        }
        //mặc định trạng thái của trang là GET
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemNews record = db.News.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Blog/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        //khi ấn nút submit thì trạng thái của trang sẽ là POST -> khai báo Attribute [HttpPost]
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            int _id = id ?? 0;
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();           
            int _hot = fc["hot"] != "" && fc["hot"] == "on" ? 1 : 0;
            string _description = fc["description"].ToString().Trim();
            string _content = fc["content"].ToString().Trim();
            //lấy một bản ghi
            ItemNews record = db.News.Where(item => item.Id == _id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _name;             
                record.Hot = _hot;
                record.Description = _description;
                record.Content = _content;
                //lay thong tin cua file
                string _file_name = "";
                try
                {
                    //lay ten file
                    _file_name = Request.Form.Files[0].FileName;
                }
                catch {; }
                if (!string.IsNullOrEmpty(_file_name))
                {
                    //upload anh moi
                    var timestamp = DateTime.Now.ToFileTime();
                    _file_name = timestamp + "_" + _file_name;
                    //lay duong dan cua file
                    string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", _file_name);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    //update gia tri vao cot Photo trong csdl
                    record.Photo = _file_name;
                    //cập nhật lại table
                    db.SaveChanges();

                }
                
                
                db.SaveChanges();
                

            }
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Blog/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
           
            int _hot = fc["hot"] != "" && fc["hot"] == "on" ? 1 : 0;
            string _description = fc["description"].ToString().Trim();
            string _content = fc["content"].ToString().Trim();
            //lay mot ban ghi
            ItemNews record = new ItemNews();
            record.Name = _name;          
            record.Hot = _hot;
            record.Description = _description;
            record.Content = _content;
            //lay thong tin cua file
            string _file_name = "";
            try
            {
                //lay ten file
                _file_name = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!string.IsNullOrEmpty(_file_name))
            {
                //upload anh moi
                var timestamp = DateTime.Now.ToFileTime();
                _file_name = timestamp + "_" + _file_name;
                //lay duong dan cua file
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", _file_name);
                //upload file
                using (var stream = new FileStream(_Path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Photo trong csdl
                record.Photo = _file_name;
                //cập nhật lại table
                db.SaveChanges();
            }
            //them ban ghi vao table Products
            db.News.Add(record);
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        //xoa ban ghi
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemNews record = db.News.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.News.Remove(record);
            //cap nhat lai table Products
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
    }
}

