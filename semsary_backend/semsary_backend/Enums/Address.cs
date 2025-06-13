using Microsoft.EntityFrameworkCore;
using semsary_backend.Models;

namespace semsary_backend.Enums
{
    public class Address
    {
        public int id { get; set; }
        public Governorate _gover {  get; set; }
        public string? _city { get; set; }  
        public string? street { get; set; }


    }
}
