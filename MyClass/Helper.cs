
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BTL.Models;
using System.Collections.Generic;
using System.Linq;
namespace BTL.MyClass
{
    public class Helper
    {
        public static MyDbContext _db = new MyDbContext();
        public static List<ItemAdv> GetAdv(int _position)
        {
            List<ItemAdv> list_record = _db.Adv.Where(n => n.Position == _position).ToList();
            return list_record;
        }
        public static string GetCategoryName(int _CategoryId)
        {
            ItemCategory record = _db.Categories.Where(item => item.Id == _CategoryId).FirstOrDefault();
            return record.Name;


        }
    }
}
