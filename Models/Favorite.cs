using System.ComponentModel.DataAnnotations.Schema;

namespace foodshop.Models
{
    [Table("favorites")]
    public class Favorite
    {

        public int Id { get; set; }


        public Guid UserId { get; set; }
        public User? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}