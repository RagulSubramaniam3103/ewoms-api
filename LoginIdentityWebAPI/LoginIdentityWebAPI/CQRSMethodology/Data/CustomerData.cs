using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginIdentityWebAPI.Data
{
    public class CustomerData
    {

    }
    public class CustomerDetails
    {
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public List<CustomerCartPurchased> customerCartPurchaseds { get; set; }

    }
    public class CustomerCartPurchased
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        [ForeignKey("CustomerDetails")]
        public int Customer_Id { get; set; }
        public CustomerDetails CustomerDetails { get; set; }
    }
}
