using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RepoLayer.Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        [JsonIgnore]
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        [JsonIgnore]
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
