using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null) instance = new CategoryDAO();
                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        private CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> list = new List<Category>();

            string query = "SELECT * FROM dbo.FoodCategory";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Category category = new Category(item);
                list.Add(category);
            }

            return list;
        }


        public Category GetCategoryByID(int id)
        {
            Category category = null;

            string query = "SELECT * FROM FoodCategory WHERE id = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                category = new Category(item);
                return category;
            }
            return category;
        }

        //Thêm sửa xóa Category June 16
        public bool InsertCategory(string name)
        {
            string query = "INSERT dbo.FoodCategory (name) VALUES (N'"+name+"')";

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateCategory(string name, int id)
        {
            string query = "UPDATE dbo.FoodCategory SET name = N'" + name+"' WHERE id =" +id; ;

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteCategory(int id)
        {
            string query = "DELETE dbo.FoodCategory WHERE id = " + id;

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
