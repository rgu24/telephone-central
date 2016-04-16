using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneCentral
{
    class Distribution
    {
         private double lambda;
        private string name;

        public Distribution(string name, double lambda)
        {
            this.name = name;
            this.lambda = lambda;
        }
//method returning exponetial distribution
        public double getTime(double x)
        {          
            return ((-1) * Math.Log(1 - x) / lambda); 
        }
        public string getName() { return name; }

    }
    }
}
