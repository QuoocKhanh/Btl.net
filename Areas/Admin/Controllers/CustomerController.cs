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

namespace BTL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class CustomerController : Controller
    {
        public MyDbContext db =new MyDbContext();
        public IActionResult Index(int? page)
        {
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 4;
            //lấy tất cả các bản ghi trong table Users
            List<ItemCustomers> list_record = db.Customers.OrderByDescending(item => item.Id).ToList();
            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemCustomers record = db.Customers.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Customer/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        //khi ấn nút submit thì trạng thái của trang sẽ là POST -> khai báo Attribute [HttpPost]
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            string _email = fc["email"].ToString().Trim();
            string _phone = fc["phone"].ToString().Trim();
            string _address = fc["address"].ToString().Trim();
            //cũng có thể sử dụng đối tượng Request.Form["ten the form"]
            string _password = Request.Form["password"].ToString().Trim();
            //lấy một bản ghi
            ItemCustomers record = db.Customers.Where(item => item.Id == id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _name;
                record.Email = _email;
                record.Phone = _phone;
                record.Address = _address;
                record.Password = _password;
                //nếu password không rỗng thì update password
                if (!String.IsNullOrEmpty(_password))
                {
                    //mã hóa password
                    _password = BC.HashPassword(_password);
                    record.Password = _password;
                }
            }
            //cập nhật lại table
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Customer/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            string _email = fc["email"].ToString().Trim();
            string _phone = fc["phone"].ToString().Trim();
            string _address= fc["address"].ToString().Trim();
            //cũng có thể sử dụng đối tượng Request.Form["ten the form"]
            string _password = Request.Form["password"].ToString().Trim();
            //mã hóa password
            _password = BC.HashPassword(_password);
            //lay mot ban ghi
            ItemCustomers record = new ItemCustomers();
            record.Name = _name;
            record.Email = _email;
            record.Phone = _phone;
            record.Address = _address;
            record.Password = _password;

            //them ban ghi vao table Users
            db.Customers.Add(record);
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        //xoa ban ghi
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemCustomers record = db.Customers.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.Customers.Remove(record);
            //cap nhat lai table Users
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
    }
}
