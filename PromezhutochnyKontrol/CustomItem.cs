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
    public partial class CustomItem : UserControl
    {
        Form1 form1;


        public CustomItem(Form1 form1, Product product)
        {
            this.form1 = form1;
            this.Product = product;
            InitializeComponent();
        }

        public Product Product { get; set; }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.Gray;
        }

        private void CustomItem_Click(object sender, EventArgs e)
        {
            new Form2(Product, form1).Show();
        }

        private void CustomItem_Load(object sender, EventArgs e)
        {
            label1.Text = Product.ProductType.Title;
            label2.Text = Product.Title;
            label3.Text = Product.Article;
            label4.Text = string.Join(", ", Product.Materials.Select(m => m.Title));
            string imagePath = Product.Image.StartsWith("\\")
                ? "C:\\Users\\d-bel\\Downloads\\Промежуточный контроль\\Сессия 1" + $"{Product.Image}"
                : "C:\\Users\\d-bel\\source\\repos\\PromezhutochnyKontrol\\PromezhutochnyKontrol\\Resources\\picture.png";
            pictureBox1.Image = Bitmap.FromFile(imagePath);
        }

    }
}
