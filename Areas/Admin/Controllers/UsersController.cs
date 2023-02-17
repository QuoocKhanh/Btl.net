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
    public class UsersController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            int curent_page = page ?? 1;

            int record_page = 3;

            List<ItemUser> users = db.Users.OrderByDescending(x => x.Id).ToList();

            return View("Index",users.ToPagedList(curent_page,record_page));
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            ItemUser user = db.Users.Where(n=>n.Id == _id).FirstOrDefault();
            ViewBag.action = "/Admin/Users/UpdatePost/" + _id;
            return View("CreateUpdate",user);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            //cũng có thể sdung dtg request form
            string _password = Request.Form["password"].ToString().Trim();
            //lấy 1 bản ghi
            ItemUser record = db.Users.Where(n => n.Id == id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _name;

                if (!string.IsNullOrEmpty(_password))
                {
                    _password = BC.HashPassword(_password);
                    record.Password = _password;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Users/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();
            string _email = fc["email"].ToString().Trim();
            //cũng có thể sdung dtg request form
            string _password = Request.Form["password"].ToString().Trim();
            //lấy 1 bản ghi
            _password = BC.HashPassword(_password);
            ItemUser record = new ItemUser();
            record.Name = _name;
            record.Email = _email;
            record.Password = _password;

            db.Users.Add(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemUser record = db.Users.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.Users.Remove(record);
            //cap nhat lai table Users
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
    }
}
