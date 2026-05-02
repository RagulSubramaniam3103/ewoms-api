using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstProjectWebAPI.MainData.ModelsMigration
{
    public class CustomerAddress
    {
        [Key]
        public int AddressId { get; set; }
        [Required]
        public string Street { get; set; }
        public string LandMark { get; set; }
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public CustomerDetails customer { get; set; }

    }
}
