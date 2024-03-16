using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromezhutochnyKontrol
{
    public class DataBaseHelper
    {
        string connstring = "Server=localhost;Port=5432;Username=postgres;Password=123098;Database=LopushokDB";
        
        //вытаскиваем продукты
        public List<Product> GetProducts()
        {
            List <Product> products = new List <Product>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.\"Product\"", connection);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read()) // построчно считываем данные
                {
                    products.Add(new Product
                    {
                        Id = (int)dataReader.GetValue(0),
                        Title = dataReader.GetValue(1).ToString(),
                        ProductTypeId = (int)dataReader.GetValue(2),
                        Article = (string)dataReader.GetValue(3),
                        Image = !dataReader.IsDBNull(5) ? dataReader.GetString(5) : null
                });
                }

            }

            //выиаскиваем тип продукта
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();
                foreach (var item in products)
                {
                    NpgsqlCommand cmd1 = new NpgsqlCommand($"SELECT * FROM public.\"ProductType\" where \"ID\" = {item.ProductTypeId};", connection);
                    
                    NpgsqlDataReader reader = cmd1.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        item.ProductType = new ProductType { Id = (int)reader.GetValue(0), Title = (string)reader.GetValue(1), DefectedOercent = (double)reader.GetValue(2) };
                    }
                    reader.Close();
                }
            }

            //вытаскиваем материалы
            foreach (var item in products)
            {
                item.Materials = GetMaterials(item.Id);
            }


            return products;
        }

        public List<Product> FindProduct(string findstring, int indexSort, ProductType productType)
        {
            string sqlCmd = "SELECT * FROM public.\"Product\"";

            if (!string.IsNullOrEmpty(findstring))
            {
                sqlCmd += $" WHERE \"Title\" ILIKE '%{findstring}%'";
            }

            if (productType != null)
            {
                if (!string.IsNullOrEmpty(findstring))
                {
                    sqlCmd += " AND";
                }
                else
                {
                    sqlCmd += " WHERE";
                }
                sqlCmd += $" \"ProductTypeID\" = {productType.Id}";
            }
            


            switch (indexSort)
            {
                case 0:
                    sqlCmd += " ORDER BY \"Title\" ASC";
                    break;
                case 1:
                    sqlCmd += " ORDER BY \"Title\" DESC";
                    break;
                case 2:
                    sqlCmd += " ORDER BY \"ProductionWorkshopNumber\" ASC";
                    break;
                case 3:
                    sqlCmd += " ORDER BY \"ProductionWorkshopNumber\" DESC";
                    break;
                case 4:
                    sqlCmd += " ORDER BY \"MinCostForAgent\" ASC";
                    break;
                case 5:
                    sqlCmd += " ORDER BY \"MinCostForAgent\" DESC";
                    break;
            }



            List <Product> products = new List<Product>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand($"{sqlCmd}", connection);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read()) // построчно считываем данные
                {
                    products.Add(new Product
                    {
                        Id = (int)dataReader.GetValue(0),
                        Title = dataReader.GetValue(1).ToString(),
                        ProductTypeId = (int)dataReader.GetValue(2),
                        Article = (string)dataReader.GetValue(3),
                        Image = !dataReader.IsDBNull(5) ? dataReader.GetString(5) : null
                    });
                }

            }

            //выиаскиваем тип продукта
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();
                foreach (var item in products)
                {
                    NpgsqlCommand cmd1 = new NpgsqlCommand($"SELECT * FROM public.\"ProductType\" where \"ID\" = {item.ProductTypeId};", connection);

                    NpgsqlDataReader reader = cmd1.ExecuteReader();

                    while (reader.Read())
                    {
                        item.ProductType = new ProductType { Id = (int)reader.GetValue(0), Title = (string)reader.GetValue(1), DefectedOercent = (double)reader.GetValue(2) };
                    }
                    reader.Close();
                }
            }

            //вытаскиваем материалы
            foreach (var item in products)
            {
                item.Materials = GetMaterials(item.Id);
            }

            return products;
        }

        public void AddOrUpdateProduct(Product product)
        {

           

            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();
                if (product.Id == 0)
                {
                    string sqlExpression = $"INSERT INTO \"Product\" (\"Title\", \"ProductTypeID\", \"ArticleNumber\", \"Description\", \"Image\", \"ProductionPersonCount\", \"ProductionWorkshopNumber\", \"MinCostForAgent\") " +
                   $"VALUES (@Title, @ProductTypeID, @ArticleNumber, @Description, @Image, @ProductionPersonCount, @ProductionWorkshopNumber, @MinCostForAgent);";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sqlExpression, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", product.Title);
                        cmd.Parameters.AddWithValue("@ArticleNumber", product.Article);
                        cmd.Parameters.AddWithValue("@ProductTypeID", product.ProductTypeId);
                        cmd.Parameters.AddWithValue("@Description", product.Description);
                        cmd.Parameters.AddWithValue("@Image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductionPersonCount", product.PersonCount);
                        cmd.Parameters.AddWithValue("@ProductionWorkshopNumber", product.WorkshopNumber);
                        cmd.Parameters.AddWithValue("@MinCostForAgent", product.MinCost);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string query = "UPDATE \"Product\" SET \"Title\" = @Title, \"ArticleNumber\" = @ArticleNumber, \"ProductTypeID\" = @ProductTypeID,  \"Description\" = @Description, \"Image\" = @Image, \"ProductionPersonCount\" = @ProductionPersonCount, \"ProductionWorkshopNumber\" = @ProductionWorkshopNumber, \"MinCostForAgent\" = @MinCostForAgent  WHERE \"ID\" = @ProductId";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", product.Id);
                        cmd.Parameters.AddWithValue("@Title", product.Title);
                        cmd.Parameters.AddWithValue("@ArticleNumber", product.Article);
                        cmd.Parameters.AddWithValue("@ProductTypeID", product.ProductTypeId);
                        cmd.Parameters.AddWithValue("@Description", product.Description);
                        cmd.Parameters.AddWithValue("@Image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductionPersonCount", product.PersonCount);
                        cmd.Parameters.AddWithValue("@ProductionWorkshopNumber", product.WorkshopNumber);
                        cmd.Parameters.AddWithValue("@MinCostForAgent", product.MinCost);
                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }

        public void DeleteProduct(Product product)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM \"Product\" WHERE \"Id\" = @Id;", connection);

                cmd.Parameters.AddWithValue("@Id", product.Id);
                cmd.ExecuteNonQuery();
            }
        }
        public List<Material> GetMaterials(int id)
        {
            List<Material> materials = new List<Material>();
            List<int> ints;
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM public.\"ProductMaterial\" where \"ProductID\" = {id};", connection);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();

                ints = new List<int>();

                while (dataReader.Read())
                {
                    ints.Add((int)dataReader.GetValue(2));
                }
                
            }
            
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();
                foreach (var item in ints)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM public.\"Material\" where \"ID\" = {item};", connection);

                    NpgsqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        materials.Add(new Material { Id = dataReader.GetInt32(0), Title = dataReader.GetString(1)});
                    }
                    dataReader.Close();
                }

            }


            return materials;
        }

        public List<Material> GetMaterials()
        {
            List<Material> materials = new List<Material>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.\"Material\"", connection);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read()) // построчно считываем данные
                {
                    materials.Add(new Material
                    {
                        Id = (int)dataReader.GetValue(0),
                        Title = dataReader.GetValue(1).ToString(),
                    });
                }

            }
            return materials;


        }

        public List<ProductType> GetProductTypes()
        {
            List<ProductType> productTypes = new List<ProductType>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
            {
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.\"ProductType\"", connection);
                NpgsqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    productTypes.Add(new ProductType
                    {
                        Id = dataReader.GetInt32(0),
                        Title = dataReader.GetString(1),
                        DefectedOercent = dataReader.GetDouble(2)

                    });
                }
            }
            return productTypes;
        }

        


    }
}
