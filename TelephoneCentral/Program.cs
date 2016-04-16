using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneCentral
{
    class Program
    {
        static void Main(string[] args)
        {
            TelephoneCentral<int> centr = new TelephoneCentral<int>(30, 0, 1);
            centr.LoadDataFromFile();
            centr.simulation(500D);
            centr.WriteData();

            Console.WriteLine("End of the simulation, to quit press ENTER");
            Console.Read();
        }
    }
}
