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
using System.Data;
using Microsoft.Data.SqlClient;


namespace BTL.Areas.Admin.Controllers
{
    [Area("Admin")]

    [CheckLogin]
    public class CategoriesController : Controller
    {
        public IConfiguration configuration;
        public MyDbContext db = new MyDbContext();
        public CategoriesController(IConfiguration _configuration)
        {
            this.configuration = _configuration;
            
        }
        public IActionResult Index(int? page)
        {
            //tao doi tg
            DataTable dtCate = new DataTable();
            //lay chuoi ket noi
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            //tu khoa using de thuc hien lenh ben trong khoi using do, khi ket thuc thi khoi lenh ben trong se bi huy di
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //thuc hien truy van, tra ket qua ve bien object
                SqlDataAdapter da = new SqlDataAdapter("Select * from categories where ParentId = 0 order by Id desc ", conn);
                //Do du lieu vao data table co ten la dtcate
                da.Fill(dtCate);
            }
            int current_page = page ?? 1;
            //dinh nghia so ban ghi tren 1 trang
            int record_per_page = 4;
            //do du lieu tu datatable co ten la dtcate vao list de phan trang
            List<ItemCategory> lstCate = new List<ItemCategory>();
            foreach (DataRow item in dtCate.Rows)
            {
                lstCate.Add(new ItemCategory() 
                { 
                    Id = Convert.ToInt32(item["Id"].ToString()), 
                    Name = item["Name"].ToString(), 
                    ParentId = Convert.ToInt32(item["ParentId"].ToString()) 
                });

            }
            return View("Index", lstCate.ToPagedList(current_page, record_per_page));
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            DataTable dtCate = new DataTable();

            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            //tu khoa using de thuc hien lenh ben trong khoi using do, khi ket thuc thi khoi lenh ben trong se bi huy di
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //thuc hien truy van, tra ket qua ve bien object
                SqlDataAdapter da = new SqlDataAdapter("Select * from categories where Id = " + _id, conn);
                //Do du lieu vao data table co ten la dtcate
                da.Fill(dtCate);
            }
            //khoi tao mot item (tuong ung vs 1 row table) cua datatable dtcate
            DataRow itemCate = dtCate.NewRow();
            if (dtCate.Rows.Count > 0)
            {
                itemCate = dtCate.Rows[0];
            }
            int current_id = 0;
            if (itemCate != null)
            {
                current_id = Convert.ToInt32(itemCate["Id"].ToString());

            }
            ViewBag.lstCategory = db.Categories.Where(n => n.ParentId == 0 && n.Id != current_id).OrderByDescending(n => n.Id).ToList();
            ViewBag.action = "/Admin/Categories/UpdatePost/" + _id;
            //liet ke danh muc de truyen ra view
            return View("CreateUpdate", itemCate);

        }
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            string _name = fc["name"].ToString().Trim();
            string _parent_id = fc["parent_id"].ToString().Trim();

            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            //tu khoa using de thuc hien lenh ben trong khoi using do, khi ket thuc thi khoi lenh ben trong se bi huy di
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                conn.Open();
                //thuc hien truy van, tra ket qua ve bien object
                SqlCommand cmd = new SqlCommand("update Categories set Name=@var_name,ParentId=@var_parent_id where Id=@var_id", conn);
                cmd.Parameters.AddWithValue("@var_name", _name);
                cmd.Parameters.AddWithValue("@var_parent_id", _parent_id);
                cmd.Parameters.AddWithValue("@var_id", _id);
                cmd.ExecuteNonQuery();


            }
            return Redirect("/Admin/Categories");
        }

        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }
        //khi ấn nút submit thì trang sẽ ở trạng thái POST
        //url: /Admin/Categories/UpdatePost/Id
        //ở trạng thái POST thì phải khai báo tag sau
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();
            string _parent_id = fc["parent_id"].ToString().Trim();
            //lay chuoi ket noi -> chuoi nay nam trong file appsettings.json
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //update, delete, insert thì phải open đối tượng kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into Categories(Name, ParentId) values (@var_name,@var_parent_id)", conn);
                cmd.Parameters.AddWithValue("@var_name", _name);
                cmd.Parameters.AddWithValue("@var_parent_id", _parent_id);
                //thực thi truy cấn
                cmd.ExecuteNonQuery();
            }
            //di chuyển đến một url khác
            return Redirect("/Admin/Categories");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay chuoi ket noi -> chuoi nay nam trong file appsettings.json
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //update, delete, insert thì phải open đối tượng kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from Categories where Id=@var_id or ParentId=@var_id", conn);
                cmd.Parameters.AddWithValue("@var_id", _id);
                //thực thi truy cấn
                cmd.ExecuteNonQuery();
            }
            //di chuyển đến một url khác
            return Redirect("/Admin/Categories");
        }
    }
}
