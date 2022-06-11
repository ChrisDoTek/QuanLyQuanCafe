using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null) instance = new AccountDAO();
                return instance;
            }

            private set
            {
                instance = value;
            }        
        }

        private AccountDAO(){}

        public bool Login(string userName, string passWord)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            byte[] hashData = new MD5CryptoServiceProvider().ComputeHash(temp);

            ////var list = hashData.ToString();
            ////list.Reverse();

            string hashPass = "";
            foreach (byte item in hashData)
            {
                hashPass += item;
            }

            string query = "USP_Login @userName , @passWord";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName , hashPass });
            return result.Rows.Count > 0;
        }

        public bool UpdateAccount(string userName, string displayName, string pass, string newPass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("EXEC USP_UpdateAccount @userName , @displayName , @password , @newPassword", new object[] {userName, displayName, pass, newPass});

            return result > 0;
        }


        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("SELECT UserName, DisplayName, Type FROM Dbo.Account");
        }

        public Account GetAccountByUserName(string userName)
        {
             DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM account WHERE userName = '" + userName +"'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public bool InsertAccount(string name, string displayName, int type)
        {
            string query = string.Format("INSERT dbo.Account (UserName, DisplayName, Type, PassWord) VALUES (N'{0}', N'{1}', {2}, N'{3}')", name, displayName, type, "1962026656160185351301320480154111117132155");

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateAccount(string name, string displayName, int type)
        {
            string query = string.Format("UPDATE dbo.Account SET DisplayName = N'{1}', Type = {2} WHERE UserName = N'{0}'", name, displayName, type);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteAccount(string name)
        {
            string query = string.Format("DELETE Account WHERE UserName = N'{0}'", name);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool ResetPassword(string name)
        {
            string query = string.Format("UPDATE Account SET PassWord = N'1962026656160185351301320480154111117132155' WHERE UserName = N'{0}'", name);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
        
    }
}
