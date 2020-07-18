using System;
using System.Collections.Generic;
using System.Text;

namespace GoldTracker
{
    public class Market
    {
        public int id { get; set; }
        public string name { get; set; }
        public int buy { get; set; }
        public int sell { get; set; }
        public int supply { get; set; }
        public int demand { get; set; }
    }
}
