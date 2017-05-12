using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruliaParser
{
    class Region
    {
        public int ID { get; set; }
        public string State { get; set; }
        public string RegionName { get; set; }
        public string Link { get; set; }
        public bool Done { get; set; }

        public Region()
        {
            ID = -1;
            State = String.Empty;
            RegionName = String.Empty;
            Link = String.Empty;
        }
        public Region(DataRow row)
        {
            ID = Convert.ToInt32(row[0]);
            State = row[1].ToString();
            RegionName = row[2].ToString();
            Link = row[3].ToString();
            Done = Convert.ToBoolean(row[4]);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1},{2},{3},{4}",ID,State,RegionName,Link,Done?"Parsed":"Not parsed");
        }
    }
   
}
