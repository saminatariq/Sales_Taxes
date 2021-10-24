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
using System.Data.Sql;
using System.Drawing.Printing;

namespace Sales_Taxes_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
//__________Connection String________________(Please Change Connection String Accordingly)_____________________________________________________________________
        SqlConnection con = new SqlConnection(@"Data Source=SAM-LAPTOP;Initial Catalog=Database_Sales_Taxes;Integrated Security=True");

        //_________________________Print__________________________________________________________________________
        Bitmap rechnung;
        private void button2_Click(object sender, EventArgs e)
        {
            int height=dataGridView1.Height;
            dataGridView1.Height = dataGridView1.RowCount * dataGridView1.RowTemplate.Height * 2;
            rechnung = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            dataGridView1.DrawToBitmap(rechnung, new Rectangle(0, 0, dataGridView1.Width, dataGridView1.Height));
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.PrintPreviewControl.Zoom = 1;
            printPreviewDialog1.ShowDialog();
            dataGridView1.Height = height;
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(rechnung, 0, 0);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
   //___________________________ Price in Textbox____________________________________________________________________
            con.Open();
            if (comboBox2.Text != "")
            {
                SqlCommand tb = new SqlCommand("SELECT Product_Price FROM Product WHERE Product_Name='" + comboBox2.Text + "'", con);
                SqlDataReader dr = tb.ExecuteReader();
                while (dr.Read())
                {
                    textBox1.Text = dr.GetValue(0).ToString();
                }
                con.Close();
            }
  //__________________________Total Price Calculation_________________________________________________________________
            double quantity = double.Parse(numericUpDown1.Text.ToString());
            double tprice = double.Parse(textBox1.Text.ToString());
            double totalprice = quantity * tprice;
            
 //___________________________Sales Tax Calculation___________________________________________________________________
            double stax = 0.0;
            con.Open();
            SqlCommand Check_Type = new SqlCommand("SELECT Product_Type FROM Product WHERE Product_Name='" + comboBox2.Text + "' ", con);
            SqlDataReader C_T = Check_Type.ExecuteReader();

            while (C_T.Read())
            {
                string type= C_T.GetValue(0).ToString().ToLower().Trim();
                if (type!= "book")
                {
                    if (type != "food")
                    {
                        if(type != "medical")
                        {
                            double stax1 = totalprice / 100;
                            stax = stax1 * 10;
                        }

                    }
                }
                // textBox4.Text = C_T.GetValue(0).ToString().ToLower().Trim();


                //if (textBox4.Text.ToLower().Trim() == "book")
                //{
                //stax = 0.0;
                //}
                //else if (textBox4.Text.ToLower().Trim() == "food")
                //{
                //stax = 0.0;
                //}
                //else if (textBox4.Text.ToLower().Trim() == "medical")
                //{
                //  stax = 0.0;
                // }
                //else
                // {
                //double stax1 = totalprice / 100;
                //stax = stax1 * 10;

                //}

            }
            con.Close();

  //________________________________Imported Product Taxes Calculation_________________________________________________
            con.Open();
            double itax = 0.0;
            SqlCommand Check_Imported = new SqlCommand("SELECT Product_Imported FROM Product WHERE Product_Name='" + comboBox2.Text + "'", con);
            SqlDataReader C_I = Check_Imported.ExecuteReader();
            while (C_I.Read())
            {
                string cimp = C_I.GetValue(0).ToString().ToLower().Trim();
                if (cimp == "true")
                {
                    double itax1 = totalprice / 100;
                    itax = itax1 * 5;
                }
                else
                {
                    itax = 0.0;
                }
            }
            con.Close();
            double Sales_Tax = itax + stax;
            Sales_Tax = Math.Round(Sales_Tax, 2);
                
 //________________________________ dataGridView______________________________________________________________
            dataGridView1.Rows.Add(numericUpDown1.Text,comboBox2.Text,textBox1.Text,totalprice,Sales_Tax);

 //_____________________________Sum of Sales Tax Calculation___________________________________________________
            double sales_tax_sum = 0.0;
            for(int i=0; i<dataGridView1.Rows.Count; ++i)
            {
                sales_tax_sum += Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value);
            }
            textBox2.Text = sales_tax_sum.ToString();

            
//____________________________Sum of Price + Sales Tax Calculation________________________________________________
            double grand_total_sum = 0.0;
            for (int j = 0; j < dataGridView1.Rows.Count; ++j)
            {
                grand_total_sum += Convert.ToDouble(dataGridView1.Rows[j].Cells[5].Value);
            }
            grand_total_sum = Math.Round(grand_total_sum,2);
            textBox3.Text = grand_total_sum.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
//__________________________________Product in ComboBox___________________________________________________________
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Product", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteNonQuery();
            con.Close();

            comboBox2.DataSource = ds.Tables[0];
            comboBox2.DisplayMember = "Product_Name";
            comboBox2.ValueMember = "Product_ID";

            
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
           
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
 //___________________________Price of Product with Sales Tax__________________________________________
            double g_total = 0.0;
            for (int k = 0; k < dataGridView1.Rows.Count; ++k)
            {
                double g1 = Convert.ToDouble(dataGridView1.Rows[k].Cells[3].Value);
                double g2 = Convert.ToDouble(dataGridView1.Rows[k].Cells[4].Value);
                g_total = g1 + g2;
                g_total=Math.Round(g_total, 2);
                dataGridView1.Rows[k].Cells[5].Value = g_total;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            Form1 x = new Form1();
            x.Show();

            this.Hide();

        }
    }
}
