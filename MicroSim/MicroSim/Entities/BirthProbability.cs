using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroSim.Entities
{
    public class BirthProbability
    {
        public byte Age { get; set; }

        public int NbrOfChildren { get; set; }

        public double P { get; set; }
    }
}
