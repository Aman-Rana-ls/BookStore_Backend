using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class WishListResponceModel
    {
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }  
    }
}
