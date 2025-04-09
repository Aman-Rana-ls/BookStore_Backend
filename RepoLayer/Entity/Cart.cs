using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RepoLayer.Entity
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        public bool IsPurchased { get; set; } = false; 

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual Book Book { get; set; }
    }
}
