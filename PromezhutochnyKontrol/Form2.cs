using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromezhutochnyKontrol
{
    public partial class Form2 : Form
    {
        Product product;
        DataBaseHelper helper;

        List<Material> materials;
        List<ProductType> productTypes;
        Form1 form1;
        public Form2(Product product, Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            helper = new DataBaseHelper();
            this.product = product ?? new Product();
            productTypes = helper.GetProductTypes();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (product.Id == 0)
            {
                button3.Enabled = false;
            }
            else
            {
                button3.Enabled = true;
            }
            foreach (ProductType type in productTypes)
            {
                comboBox1.Items.Add(type.Title);
            }
            textBox2.Text = product.Title;
            textBox1.Text = product.Article;
            textBox3.Text = product.PersonCount.ToString();
            textBox4.Text = product.WorkshopNumber.ToString();
            textBox5.Text = product.MinCost.ToString();
            textBox6.Text = product.Description;
            textBox7.Text = product.Image;
            comboBox1.Text = product.ProductTypeId.ToString();

            comboBox1.Text = product.ProductType == null ? productTypes[0].Title : product.ProductType.Title;

            materials = helper.GetMaterials();
            foreach (var item in materials)
            {
                checkedListBox1.Items.Add(item.Title);
            }

            if (product.Materials != null)
            {
                for (int i = 0; i < product.Materials.Count; i++)
                {
                    for (int j = 0; j < checkedListBox1.Items.Count; j++)
                    {
                        if (product.Materials[i].Title == checkedListBox1.Items[j].ToString())
                        {
                            checkedListBox1.SetItemChecked(j, true);
                        }
                    }
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            helper.AddOrUpdateProduct(new Product
            {
                Title = textBox2.Text,
                Article = textBox1.Text,
                Description = textBox6.Text,
                WorkshopNumber = int.Parse(textBox4.Text),
                PersonCount = int.Parse(textBox3.Text),
                MinCost = decimal.Parse(textBox5.Text),
                Image = textBox7.Text,
                ProductTypeId = productTypes[comboBox1.SelectedIndex].Id
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return;
            }
            else
            {
                textBox7.Text = openFileDialog.FileName;
                pictureBox1.Image = Bitmap.FromFile(openFileDialog.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            helper.DeleteProduct(product);
        }
    }
}
