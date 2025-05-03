using Microsoft.EntityFrameworkCore;
using semsary_backend.Models;

namespace semsary_backend.Enums
{
    [Owned]
    public class Address
    {
        
        public Governorate _gover {  get; set; }
        public string street { get; set; }

    }
}
