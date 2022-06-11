using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Data;
using iText.IO.Font;
using System.Linq;
using System.Diagnostics;

namespace QuanLyQuanCafe
{
    public partial class fTableManager : Form
    {
        private int currenttableid = -1;
        
        public object TableDao { get; private set; }

        private Account loginAccount;
        public Account LoginAccount
        {
            get
            {
                return loginAccount;
            }

            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount.Type);
            }
        }        

        public fTableManager(Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc;            
            LoadTable();
            LoadCategory();
            LoadComboBoxTable(cbSwitchTable);
        }
        #region Method
        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " ("+ loginAccount.DisplayName +") ";
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<QuanLyQuanCafe.DTO.Table> tableList = TableDAO.Instance.LoadTableList();
                foreach (QuanLyQuanCafe.DTO.Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight};
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.Azure;
                        break;
                }

                flpTable.Controls.Add(btn);
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();

            List<QuanLyQuanCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);

            float totalPrice = 0;

            foreach (QuanLyQuanCafe.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }

            CultureInfo culture = new CultureInfo("vi-VN");
            txbTotalPrice.Text = totalPrice.ToString("c", culture);                      
        }

        void LoadComboBoxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        #endregion

        #region Events
        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this, new EventArgs());
        }

        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(this, new EventArgs());
        }

        //Lấy currenttableid
        void btn_Click(object sender, EventArgs e)
        {
            currenttableid = ((sender as Button).Tag as QuanLyQuanCafe.DTO.Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(currenttableid);
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản ("+e.Acc.DisplayName+")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = loginAccount;
            f.InsertFood += F_InsertFood;
            f.DeleteFood += F_DeleteFood;
            f.UpdateFood += F_UpdateFood;
            f.ShowDialog();
        }

        private void F_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as QuanLyQuanCafe.DTO.Table).ID);
        }

        private void F_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as QuanLyQuanCafe.DTO.Table).ID);
        }

        private void F_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as QuanLyQuanCafe.DTO.Table).ID);
                LoadTable();
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            QuanLyQuanCafe.DTO.Table table = lsvBill.Tag as QuanLyQuanCafe.DTO.Table;

            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn đã!");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);
            LoadTable();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            QuanLyQuanCafe.DTO.Table table = lsvBill.Tag as QuanLyQuanCafe.DTO.Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;
            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0].Replace(".", ""));
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;
            
            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Thanh toán hóa đơn cho {0}\nTổng tiền - (Tổng tiền / 100) x Giảm giá\n=>{1} - ({1}/100) x {2} = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    //Lưu nhân viên thanh toán 13/06/2020
                    DataTable tbl = DataProvider.Instance.ExecuteQuery("SELECT UserName FROM Account WHERE isActive = N'TRUE'");
                    DataRow dr = tbl.Rows[0];
                    string Employee = dr["UserName"].ToString();
                    DataProvider.Instance.ExecuteNonQuery("UPDATE Bill SET EmployeeCode = N'" + Employee + "' WHERE id = " + idBill);

                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);
                    LoadTable();
                }
            }
        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {

            int id1 = (lsvBill.Tag as QuanLyQuanCafe.DTO.Table).ID;
            int id2 = (cbSwitchTable.SelectedItem as QuanLyQuanCafe.DTO.Table).ID;
            if (MessageBox.Show(string.Format("Chuyển từ {0} qua {1}?", (lsvBill.Tag as QuanLyQuanCafe.DTO.Table).Name, (cbSwitchTable.SelectedItem as QuanLyQuanCafe.DTO.Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                LoadTable();
            }
        }

        //09/06/2020 xuất hóa đơn pdf
        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            //Đặt font chữ tùy ý cho hóa đơn 13/06/2020
            FontProgramFactory.RegisterFont("E:/datnCNTT2020/VietFontsWeb1_ttf/vuArial.ttf", "vuArial");
            iText.Kernel.Font.PdfFont myFont = iText.Kernel.Font.PdfFontFactory.CreateRegisteredFont("vuArial", "Identity-H", true);


            //int id = ((sender as Button).Tag as QuanLyQuanCafe.DTO.Table).ID;
            string printdt = DateTime.Now.Millisecond.ToString();

            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream("E:/datnCNTT2020/myfiles/Bill_" + printdt + "_" + currenttableid + ".pdf", FileMode.Create, FileAccess.Write)));
            Document document = new Document(pdfDocument);

            document.SetFont(myFont); // SetFont

            List<QuanLyQuanCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(currenttableid);

            int discount = (int)nmDiscount.Value;
            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0].Replace(".", ""));
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;
            DataTable tbl = DataProvider.Instance.ExecuteQuery("SELECT DisplayName FROM Account WHERE isActive = N'TRUE'");
            DataRow dr = tbl.Rows[0];
            string Employee = dr["DisplayName"].ToString();

            DataTable tbl1 = DataProvider.Instance.ExecuteQuery("SELECT name FROM TableFood WHERE id = "+currenttableid);
            DataRow dr1 = tbl1.Rows[0];
            string tablename = dr1["name"].ToString();

            document.Add(new Paragraph("Hóa đơn " + tablename));
            document.Add(new Paragraph(DateTime.Now.ToString()+"\n"));

            var table = new iText.Layout.Element.Table(3);
            table.AddCell(createCell("Tên"));
            table.AddCell(createCell("Số lượng"));
            table.AddCell(createCell("Đơn giá"));

            foreach (QuanLyQuanCafe.DTO.Menu item in listBillInfo)
            {
                table.AddCell(createCell(item.FoodName));
                table.AddCell(createCell(item.Count.ToString()));
                table.AddCell(createCell(item.Price.ToString()));
            }

            document.Add(table);

            document.Add(new Paragraph("\nGiảm giá: " + discount + "%"));
            document.Add(new Paragraph("\nThành tiền: " + finalTotalPrice+" đ"));
            document.Add(new Paragraph("\nNhân viên: " + Employee));
            document.Close();

            MessageBox.Show("In hóa đơn thành công!");
            Process.Start(@"e:datnCNTT2020\myfiles\");
        }
            public Cell createCell(String content)
        {
            Cell cell = new Cell(1, 1);
            cell.Add(new Paragraph(content));
            return cell;
        }
    

        #endregion

        private void lsvBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
        }

        //Xóa món để đổi món 14/06/2020
        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in lsvBill.SelectedItems)
            {
                lsvBill.Items.Remove(eachItem);
                Food selectedfood = FoodDAO.Instance.GetListFood().Where(food => food.Name == eachItem.Text).ToList()[0]; //selected food from list view each item

                BillInfoDAO.Instance.DeleteBillInfoByFoodID(selectedfood.ID); 
            }
        }
    }
}
