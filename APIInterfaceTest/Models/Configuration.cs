using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterfaceTest.Models
{
    internal class Configuration
    {
        public int Id { get; set; }
        public List<Item> items { get; set; }
        public Category category { get; set; }
        public CoOrdinates coOrd { get; set; }
    }
}
