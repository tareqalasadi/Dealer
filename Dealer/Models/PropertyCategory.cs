namespace Dealer.Models
{
    public class PropertyCategory
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public byte[] Image { get; set; }
    }
    
    public class PropertyCategoryViewModel
    {
        public string EncryptedId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public byte[] Image { get; set; }
    }

}
