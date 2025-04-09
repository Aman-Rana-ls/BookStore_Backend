using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;

namespace ModelLayer
{
    public class UpdateCartItemModel
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchased { get ; set;} = false;
    }
}
