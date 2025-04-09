using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class BookInputModel
    {
        [Required]
        public string BookName { get; set; }

        [Required]
        public string Author { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal DiscountPrice { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string BookImage { get; set; }

    }
}
