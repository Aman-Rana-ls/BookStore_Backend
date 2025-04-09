using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepoLayer.Entity
{
    public class Book
    {
        public int BookId { get; set; }

        public string BookName { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string? Description { get; set; }

        public decimal DiscountPrice { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? BookImage { get; set; }

        public int AdminUserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public virtual AdminUser AdminUser { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        [JsonIgnore]
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>(); 
    }
}
