    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ModelLayer
    {
        public class AddressInputModel
        {
            public string FullName { get; set; }
            public string MobileNumber { get; set; }
            public string AddressLine { get; set; }
            public string PinCode { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string AddressType { get; set; } // e.g., Home, Work, Other
        }
    }
