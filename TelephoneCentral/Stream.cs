using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneCentral
{
    class Stream
    {
        private Distribution nextCallDistribution;
        private Distribution callLengthDistribution;
        private double maxTimeWaiting=1;
        private int frequency=1;
        string name;
        public int size=1;//ile laczy zajmuje
        public Stream()
        {
            nextCallDistribution = new Distribution("name", 0.33);
            callLengthDistribution = new Distribution("name", 1);
            maxTimeWaiting = 1;
            frequency = 1;
            


        }
    public  Stream(string naz,double lambdaT,string nameT,double lambdaO,string nameO,double czas,int roz)
     
        {
            name = naz;
            maxTimeWaiting = czas;
            callLengthDistribution = new Distribution(nameT, lambdaT);
            nextCallDistribution = new Distribution(nameO, lambdaO);
            size = roz;

        }
        public double getMaxTime()
        {
            return maxTimeWaiting;
        }
        public Distribution getNextCallDistribution()
        {
            return nextCallDistribution;
        }
        public int getFrequency()
        {
            return frequency;
        }
        public Distribution getCallLengthistribution()
        {
            return callLengthDistribution;
        }
        public string getName()
        {
            return name;
        }    
    }
    }
}
