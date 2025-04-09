using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RepoLayer.Entity
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string AddressLine { get; set; }

        [Required]
        public string PinCode { get; set; }


        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Type { get; set; } 

        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
