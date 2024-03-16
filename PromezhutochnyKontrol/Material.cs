namespace PromezhutochnyKontrol
{
    public class Material
    {
        public int Id { get; set; }

        public int MaterialTypeId { get; set; }

        public MaterialType MaterialType { get; set; }

        public string Title { get; set; }
    }
}