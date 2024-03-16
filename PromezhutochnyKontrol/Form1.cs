using Npgsql;
using System.Data;
using System.Windows.Forms;

namespace PromezhutochnyKontrol
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            productTypes = helper.GetProductTypes();
        }
        List<Product> products;
        DataBaseHelper helper = new DataBaseHelper();
        List<ProductType> productTypes;
        List<Label> labels;
        int countpage = 0;
        int pageNumber = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            products = helper.GetProducts();

            //UpdateItems();

            comboBox2.Items.Add("Все типы");

            foreach (var item in productTypes)
            {
                comboBox2.Items.Add(item.Title);
            }
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            products = helper.FindProduct(textBox1.Text, comboBox1.SelectedIndex, productTypes.FirstOrDefault(t => t.Title == comboBox2.SelectedItem));
            UpdateItems();
            comboBox1.SelectedIndexChanged += textBox1_TextChanged;
            comboBox2.SelectedIndexChanged += textBox1_TextChanged;
        }

        public void LoadPages()
        {
            flowLayoutPanel2.Controls.Clear();
            int a = products.Count % 20;
            countpage = a == 0 ? products.Count / 20 : products.Count / 20 + 1;

            Label previousPageLabel = new Label { Text = "<", AutoSize = true };
            previousPageLabel.Click += label_Click;
            flowLayoutPanel2.Controls.Add(previousPageLabel);

            // Добавляем лейблы для страниц
            for (int i = 0; i < countpage; i++)
            {
                if (i <= 3)
                {
                    Label pageLabel = new Label { Text = $"{i + 1}", AutoSize = true };
                    pageLabel.Click += label_Click;
                    flowLayoutPanel2.Controls.Add(pageLabel);
                }
            }

            // Добавляем лейбл ">"
            Label nextPageLabel = new Label { Text = ">", AutoSize = true };
            nextPageLabel.Click += label_Click;
            flowLayoutPanel2.Controls.Add(nextPageLabel);

        }



        public void UpdateItems()
        {

            flowLayoutPanel1.Controls.Clear();

            int startIndex = (pageNumber - 1) * 20;
            int endIndex = Math.Min(startIndex + 20, products.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                CustomItem item = new CustomItem(this, products[i]);
                flowLayoutPanel1.Controls.Add(item);
            }
            LoadPages();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Form2(new Product(), this).ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string findstring = textBox1.Text;

            int indexSort = comboBox1.SelectedIndex;

            ProductType productType = productTypes.FirstOrDefault(t => t.Title == comboBox2.SelectedItem);

            products = helper.FindProduct(findstring, indexSort, productType);
            UpdateItems();
        }

        private void label_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel2.Controls.Count < 4)
            {
                return;
            }
            Label label = (Label)sender;
            if (label.Text == ">")
            {
                if (Convert.ToInt32(flowLayoutPanel2.Controls[flowLayoutPanel2.Controls.Count - 2].Text) < countpage)
                {
                    for (int i = 1; i < flowLayoutPanel2.Controls.Count - 1; i++)
                    {
                        Label currentLabel = (Label)flowLayoutPanel2.Controls[i];
                        int newNumber = int.Parse(currentLabel.Text) + 1;
                        currentLabel.Text = newNumber.ToString();
                    }
                }
            }
            else if (label.Text == "<")
            {
                if (Convert.ToInt32(flowLayoutPanel2.Controls[1].Text) != 1)
                {
                    for (int i = 1; i < flowLayoutPanel2.Controls.Count - 1; i++)
                    {
                        Label currentLabel = (Label)flowLayoutPanel2.Controls[i];
                        int newNumber = int.Parse(currentLabel.Text) - 1;
                        currentLabel.Text = newNumber.ToString();
                    }
                }
            }
            else
            {
                pageNumber = int.Parse(label.Text);
                UpdateItems();
            }
        }
    }
}