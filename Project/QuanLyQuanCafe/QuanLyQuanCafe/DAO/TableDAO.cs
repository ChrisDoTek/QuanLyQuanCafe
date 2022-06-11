using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get
            {
                if (instance == null) instance = new TableDAO();
                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        public static int TableWidth = 100;
        public static int TableHeight = 100;

        private TableDAO() { }

        public void SwitchTable(int id1, int id2)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2", new object[] { id1, id2});
        }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("EXEC USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        //Thêm sửa xóa Table June 16
        public bool InsertTable(string name, string status)
        {
            string query = string.Format("INSERT dbo.TableFood (name, status) VALUES (N'{0}', N'{1}')", name, status);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateTable(int id, string name, string status)
        {
            string query = string.Format("UPDATE dbo.TableFood SET name = N'{0}', status = N'{1}' WHERE id ={2}", name, status, id);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteTable(int id)
        {
            string query = string.Format("DELETE dbo.TableFood WHERE id = {0}", id);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
