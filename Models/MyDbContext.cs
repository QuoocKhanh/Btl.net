
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using System.IO;
namespace BTL.Models
{
    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //tao doi tuong de doc thong tin
            var builder = new ConfigurationBuilder();
            //set duong dan cua file
            builder.SetBasePath(Directory.GetCurrentDirectory());
            //add file appsetting.json
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            // doc chuoi ket noi o trong file appsetting.json
            string strDbConnectString = configuration.GetConnectionString("DBConnectString").ToString();
            //ket noi vs csdl thong qua chuoi ket noui
            optionsBuilder.UseSqlServer(strDbConnectString);
        }

        public DbSet<ItemUser> Users { get; set; } //<=> table Users trong csdl
        public DbSet<ItemCategory> Categories { get; set; } //<=> table Categories trong csdl
        public DbSet<ItemProduct> Products { get; set; } //<=> table Products trong csdl
        public DbSet<ItemCategoriesProducts> CategoriesProducts { get; set; } //<=> table CategoriesProducts trong csdl
        public DbSet<ItemNews> News { get; set; }//<=> table News
        public DbSet<ItemRating> Rating { get; set; }//<=> table Rating
        public DbSet<ItemCustomers> Customers { get; set; }//<=> table Customers
        public DbSet<ItemOrders> Orders { get; set; }//<=> table Orders
        public DbSet<ItemOrdersDetail> OrdersDetail { get; set; }//<=> table OrdersDetail
        public DbSet<ItemAdv> Adv { get; set; }//<=> table Adv
        public DbSet<ItemTag> Tags { get; set; }//<=> table Tags
        public DbSet<ItemTagProduct> TagsProducts { get; set; }//<=> table TagsProducts
        public DbSet<ItemSlide> Slides { get; set; }//<=> table Slides
    }
}
