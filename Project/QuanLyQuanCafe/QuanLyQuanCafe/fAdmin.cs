using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();

        BindingSource accountList = new BindingSource();

        BindingSource categoryList = new BindingSource();

        BindingSource tableList = new BindingSource();


        public Account loginAccount;

        public fAdmin()
        {
            InitializeComponent();
            LoadAll();
        }


        #region Methods

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void LoadAll()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;//May 28
            dtgvTable.DataSource = tableList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadListCategory();
            LoadAccount();
            LoadFoodCategoryIntoComboBox(cbFoodCategory);
            LoadChangePriceHistory();
            LoadListTable();
            AddTableBinding();
            AddFoodBinding();
            AddAccountBinding();
            AddFoodCategoryBinding();
        }

        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmAccountType.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));

        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        //May 28
        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }
        //May 28

        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.LoadTableList();
        }


        //June 07
        void LoadChangePriceHistory()
        {
            dtgvChangePrice.DataSource = DataProvider.Instance.ExecuteQuery("SELECT * FROM History");
        }
        //June 07

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));

        }

        void AddFoodCategoryBinding()
        {
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
            
        }
        //May 31

        void AddTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));

        }

        void LoadFoodCategoryIntoComboBox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại!");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại!");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Không thể xóa tài khoản đang đăng nhập!");
                return;
            }

            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Xóa nhật tài khoản thất bại!");
            }

            LoadAccount();
        }

        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu tài khoản thất bại!");
            }
        }

        //Thêm sửa xóa Category June 16
        void AddCategory(string name)
        {
            if (CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Thêm danh mục thành công!");
            }
            else
            {
                MessageBox.Show("Thêm danh mục thất bại!");
            }

            LoadListCategory();
        }

        void EditCategory(string name, int id)
        {
            if (CategoryDAO.Instance.UpdateCategory(name, id))
            {
                MessageBox.Show("Cập nhật danh mục thành công!");
            }
            else
            {
                MessageBox.Show("Cập nhật danh mục thất bại!");
            }

            LoadListCategory();
        }

        void DeleteCategory(int id)
        {
            if (CategoryDAO.Instance.DeleteCategory(id))
            {
                MessageBox.Show("Xóa danh mục thành công!");
            }
            else
            {
                MessageBox.Show("Xóa danh mục thất bại!");
            }

            LoadListCategory();
        }

        //Thêm sửa xóa Table June 16
        void AddTable(string name, string status)
        {
            if (TableDAO.Instance.InsertTable(name, status))
            {
                MessageBox.Show("Thêm bàn thành công!");
            }
            else
            {
                MessageBox.Show("Thêm bàn thất bại!");
            }

            LoadListTable();
        }

        void EditTable(int id, string name, string status)
        {
            if (TableDAO.Instance.UpdateTable(id, name, status))
            {
                MessageBox.Show("Cập nhật bàn thành công!");
            }
            else
            {
                MessageBox.Show("Cập nhật bàn thất bại!");
            }

            LoadListTable();
        }

        void DeleteTable(int id)
        {
            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Xóa bàn thành công!");
            }
            else
            {
                MessageBox.Show("Xóa bàn thất bại!");
            }

            LoadListTable();
        }

        #endregion

        #region Events

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;

            AddAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;

            EditAccount(userName, displayName, type);
        }

        //Thêm sửa xóa Category


        private void btnResetPassWord_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            ResetPass(userName);
        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }

        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["idcategory"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = category;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index;

                }
            }

            catch
            {

            }
        }

        private void txbCategoryID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
            }

            catch
            {

            }
        }
        //May 31

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công!");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm món");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {   string name = txbFoodName.Text;
            
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            int foodID = Convert.ToInt32(txbFoodID.Text);
            
            //thay doi gia mon an 06/06/2020
            //float price = (float)nmFoodPrice.Value;

            //07/06/2020
            DateTime dt = DateTime.Now;
            string changedate = dt.ToString();
            DataTable tbl = DataProvider.Instance.ExecuteQuery("SELECT price FROM Food WHERE id =" +foodID);
            DataRow dr = tbl.Rows[0];
            //08/06/2020 cach lay gia tri theo kieu tuy y trong 1 cell trong database dung cho function

            float oldprice = Convert.ToInt32(dr["price"].ToString());
            float newprice = (float)nmFoodPrice.Value;

            if (newprice != oldprice)
                {
                    string query = string.Format("INSERT INTO History VALUES(N'{0}', {1}, {2}, N'{3}', {4})", name, oldprice, newprice, changedate, foodID);
                    DataProvider.Instance.ExecuteNonQuery(query);
                }
            
            //07/06/2020
            

            if (FoodDAO.Instance.UpdateFood(foodID, name, categoryID, newprice))
            {
                MessageBox.Show("Sửa món thành công!");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa món");
            }

            LoadChangePriceHistory(); //Load lại bảng để thấy lịch sử luôn
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int foodID = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(foodID))
            {
                MessageBox.Show("Xoá món thành công!");
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa món");
            }
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private void btnFirstBillPage_Click(object sender, EventArgs e)
        {
            txbPageBill.Text = "1";
        }

        private void btnLastBillPage_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            int lastPage = sumRecord / 10;
            if (sumRecord % 10 != 0)
                lastPage++;
            txbPageBill.Text = lastPage.ToString();
        }

        private void txbPageBill_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
        }

        private void btnPreviousBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);
            if (page > 1)
                page--;
            txbPageBill.Text = page.ToString();
        }

        private void btnNextBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);
            int sumRecord = BillDAO.Instance.GetNumBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            int lastPage = sumRecord / 10;
            if (sumRecord % 10 != 0)
                lastPage++;

            if (page < lastPage)
                page++;
            txbPageBill.Text = page.ToString();


        }

        //Thêm xóa sửa Category
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;
            AddCategory(name);
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int categoryID = Convert.ToInt32(txbCategoryID.Text);
            DeleteCategory(categoryID);

        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;
            int categoryID = Convert.ToInt32(txbCategoryID.Text);
            EditCategory(name, categoryID);
        }

        //Thêm sửa xóa Table
        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbTableStatus.Text;
            AddTable(name, status);
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int tableID = Convert.ToInt32(txbTableID.Text);
            DeleteTable(tableID);
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbTableStatus.Text;
            int tableID = Convert.ToInt32(txbTableID.Text);
            EditTable(tableID, name, status);
        }

        private void fAdmin_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'QuanLyQuanCafeDataSetEmployeeSumPerMonth.USP_EmployeeSumPerMonth' table. You can move, or remove it, as needed.
            this.USP_EmployeeSumPerMonthTableAdapter.Fill(this.QuanLyQuanCafeDataSetEmployeeSumPerMonth.USP_EmployeeSumPerMonth, dtpkFromDate.Value, dtpkToDate.Value);
            this.rpViewer.RefreshReport();
        }

        #endregion


    }
}
