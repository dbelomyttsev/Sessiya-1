using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromezhutochnyKontrol
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public string Title { get; set; }
        public ProductType ProductType { get; set; }
        public string Image { get; set; }
        public string Article { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public List<Material> Materials { get; set; }
        public int PersonCount { get; set; }
        public decimal MinCost { get; set; }
        public int WorkshopNumber { get; set; }
    }
}
