using Microsoft.AspNetCore.Mvc;
//su dung doi tuong thao tac IFormCollection
using Microsoft.AspNetCore.Http;
//nhin thay cac file .cs ben trong folder Models
using BTL.Models;
//su dung entity framework
using Microsoft.EntityFrameworkCore;
//phan trang
using X.PagedList;
//nhin thay file CheckLogin.cs tron thu muc Attributes
using BTL.Areas.Admin.Attributes;
//doi tuong thao tac file
using System.IO;
//su dung kieu List
using System.Collections.Generic;
//doi tuong ma hoa password
using BC = BCrypt.Net.BCrypt;



namespace BTL.Controllers
{
    public class CartController : Controller
    {
        //khoi tao doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            //lay cac san pham trong gio hang
            List<Item> _cart = Cart.GetCart(HttpContext.Session);
            if(_cart != null)
            {
                ViewBag._cart = _cart;
                ViewBag._total = _cart.Sum(tbl => (tbl.ProductRecord.Price - (tbl.ProductRecord.Price * tbl.ProductRecord.Discount / 100)) * tbl.Quantity);
            }
            return View();
        }
        //them san pham vao gio hang
        public IActionResult Add(int id)
        {
            //goi ham CartAdd trong class Cart de them phan tu vao gio hang
            Cart.CartAdd(HttpContext.Session, id);
            //di chuyen den rul: /Cart
            return Redirect("/Cart");
        }
        //xoa san pham khoi gio hang
        public IActionResult Remove(int id)
        {
            //goi ham CartRemove trong class Cart de xoa phan tu khoi gio hang
            Cart.CartRemove(HttpContext.Session, id);
            //di chuyen den rul: /Cart
            return Redirect("/Cart");
        }
        //xoa toan bo san pham khoi gio hang
        public IActionResult Destroy()
        {
            //goi ham CartDestroy trong class Cart de xoa tat ca cac phan tu khoi gio hang
            Cart.CartDestroy(HttpContext.Session);
            //di chuyen den rul: /Cart
            return Redirect("/Cart");
        }
        //cap nhat so luong san pham trong gio hang
        public IActionResult Update()
        {
            //lay cac san pham trong gio hang
            List<Item> _cart = Cart.GetCart(HttpContext.Session);
            //duyet cac phan tu trong list _cart
            foreach(var item in _cart)
            {
                //lay so luong cac phan tu
                int quantity = Convert.ToInt32(Request.Form["product_" + item.ProductRecord.Id]);
                //goi ham CartUpdate de update lai so luong san pham
                Cart.CartUpdate(HttpContext.Session, item.ProductRecord.Id, quantity);
            }
            //di chuyen den rul: /Cart
            return Redirect("/Cart");
        }
        //thanh toan gio hang
        public IActionResult Checkout()
        {
            //kiem tra neu user chua dang nhap thi yeu cau dang nhap
            if (HttpContext.Session.GetString("customer_email") == null)
                return Redirect("/Account/Login");
            else
            {
                List<Item> _cart = Cart.GetCart(HttpContext.Session);
                //lay customer_id cua session
                int customer_id = int.Parse(HttpContext.Session.GetString("customer_id"));
                //insert du lieu vao table Orders
                ItemOrders _RecordOrder = new ItemOrders();
                _RecordOrder.CustomerId = customer_id;
                _RecordOrder.Create = DateTime.Now;
                _RecordOrder.Price = _cart.Sum(tbl => tbl.ProductRecord.Price * tbl.Quantity);
                db.Orders.Add(_RecordOrder);
                db.SaveChanges();
                //lay id vua insert
                int order_id = _RecordOrder.Id;
                //duyet cac san pham trong session, moi phan tu se add thanh 1 ban ghi trong OrdersDetail
                foreach (var item in _cart)
                {
                    ItemOrdersDetail _RecordOrdersDetail = new ItemOrdersDetail();
                    _RecordOrdersDetail.OrderId = order_id;
                    _RecordOrdersDetail.ProductId = item.ProductRecord.Id;
                    _RecordOrdersDetail.Price = item.ProductRecord.Price - (item.ProductRecord.Price * item.ProductRecord.Discount) / 100;
                    _RecordOrdersDetail.Quantity = item.Quantity;
                    //---
                    db.OrdersDetail.Add(_RecordOrdersDetail);
                    db.SaveChanges();
                }
                //xoa tat cac cac phan tu trong gio hang
                Cart.CartDestroy(HttpContext.Session);
                return Redirect("/Cart?checkout=success");
            }
        }
    }
}
