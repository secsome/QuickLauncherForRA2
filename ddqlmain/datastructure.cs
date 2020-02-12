using System;
using System.Collections.Generic;

namespace ddqlmain
{
    public class Campaigns
    {
        public String Name { get; set; }
        public String Side { get; set; }
        public String ID { get; set; }
        public String SCENERY { get; set; }
    }

    public class Saves
    {
        public String Name { get; set; }
        public String UIName { get; set; }
        public String Time { get; set; }
    }

    public class ScreenResolution
    {
        public List<Tuple<int, int>> Item1;
        public List<String> Item2;
        public ScreenResolution()
        {
            Item1 = new List<Tuple<int, int>>();
            Item2 = new List<string>();
        }
    }
}
