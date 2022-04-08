using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace pos_and_inventory_csharp
{
    public partial class frmPOS : Form
    {
        String id;
        String price;
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();


        public frmPOS()
        {
            InitializeComponent();
            lblDate.Text = DateTime.Now.ToLongDateString();
            cn = new SqlConnection(dbcon.MyConnection());
            this.KeyPreview = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            GetTransNo();
            txtSearch.Enabled = true;
            txtSearch.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmSettle frm = new frmSettle(this);
            frm.txtSale.Text = lblDisplayTotal.Text;
            frm.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lblTransno.Text == "0000000000000") { return; }
            frmLookUp frm = new frmLookUp(this);
            frm.LoadRecords();
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmDiscount frm = new frmDiscount(this);
            frm.lblID.Text = id;
            frm.txtPrice.Text = price;
            frm.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }


        public void GetTransNo()
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                string transno;
                int count;
                cn.Open();
                cm = new SqlCommand("select top 1 transno from tblcart where transno like ' % " + sdate + "' order by id desc",cn);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                { 
                    transno = dr[0].ToString();
                    count = int.Parse(transno.Substring(8, 4));
                    lblTransno.Text = sdate + count + 1;

                } else 
                    { 
                    transno = sdate + "0001";
                    lblTransno.Text = transno;
                    } 
                dr.Close();
                cn.Close();
                

            }catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSearch.Text == String.Empty) { return; }
                else
                {
                    cn.Open();
                    cm = new SqlCommand("select * from tblproduct where barcode like '" + txtSearch.Text + "' ", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        frmQty frm = new frmQty(this);
                        frm.Productdetails(dr["pcode"].ToString(), double.Parse(dr["price"].ToString()), lblTransno.Text);
                        dr.Close();
                        cn.Close();
                        frm.ShowDialog();
                    }
                    else
                    {
                    dr.Close();
                    cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void LoadCart()
        {
            try
            {
                dataGridView1.Rows.Clear();
                Boolean hasrecord = false;
                int i = 0;
                double total = 0;
                double discount = 0;
                cn.Open();
                cm = new SqlCommand("select c.id, c.pcode, p.pdesc, c.price, c.qty, c.disc, c.total from tblcart as c inner join tblproduct as p on c.pcode = p.pcode where transno like '" + lblTransno.Text + "' and status like 'Pending'",cn);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    total += Double.Parse(dr["total"].ToString());
                    discount += Double.Parse(dr["disc"].ToString());
                    dataGridView1.Rows.Add(i, dr["id"].ToString(),dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), dr["disc"].ToString(), Double.Parse(dr["total"].ToString()).ToString("#,##0.00"));
                    hasrecord = true;
                }
                dr.Close();
                cn.Close();
                lblTotal.Text = total.ToString("#,##0.00");
                lblDiscount.Text = discount.ToString("#,##0.00");
                GetCartTotal();
                if(hasrecord == true) { btnSttle.Enabled = true; btnDiscount.Enabled = true; } else { btnSttle.Enabled = false; btnDiscount.Enabled = false; }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                cn.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if(MessageBox.Show("Remove this item?", "Remove item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tblcart where id like '" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() +"'", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Item has successfully removed", "Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCart();
                }
            }
        }

        public void GetCartTotal()
        {

            double discount = Double.Parse(lblDiscount.Text);
            double sales = Double.Parse(lblTotal.Text);
            double vat = sales * dbcon.GetVal();
            double vatable = sales - vat;

            lblVat.Text = vat.ToString("#,##0.00");
            lblVatable.Text = vatable.ToString("#,##0.00");
            lblDisplayTotal.Text = sales.ToString("#,##0.00");

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int i = dataGridView1.CurrentRow.Index;
            id = dataGridView1[1, i].Value.ToString();
            price = dataGridView1[4,i].Value.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("hh:MM:ss tt");
            lblDate1.Text = DateTime.Now.ToLongDateString();
        }
    }
}
