namespace Dealer.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string DescEn { get; set; }
        public string DescAr { get; set; }
        public int CountBeds { get; set; }
        public int CountBaths { get; set; }
        public int Space { get; set; }
        public double Price { get; set; }

        public int CategoryId { get; set; }
        public byte[] Image { get; set; }
        public string Area { get; set; }


    }

    public class PropertyViewModel
    {
        public string EncryptedId { get; set; }
        public string Location { get; set; }
        public string DescEn { get; set; }
        public string DescAr { get; set; }
        public int CountBeds { get; set; }
        public int CountBaths { get; set; }
        public int Space { get; set; }
        public double Price { get; set; }

        public int CategoryId { get; set; }
        public byte[] Image { get; set; }
        public string Area { get; set; }

    }
}
