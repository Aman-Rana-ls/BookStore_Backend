using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoLayer.Entity
{
    public class Wishlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishlistId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}
