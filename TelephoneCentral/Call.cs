using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneCentral
{
    class Call<T>
    {
        
        
        public double possTimeWaiting;
       private int streamNum;
       public double arrivalTime;
        private T data;
       public int size = 1;//how many links is needed for this call
       
       
       public void add(double cz,T n,int str,double czasp,int roz)
        {
            data = n;
            possTimeWaiting = cz;
            streamNum = str;
            arrivalTime = czasp;
            size = roz;


        }
       public void add(double cz,int str,double czasp,int roz)
       {
           possTimeWaiting = cz;
           streamNum = str;
           arrivalTime = czasp;
           size = roz;
       }
       public double getPossTimeWaiting()
       {
           return possTimeWaiting;
       }
        public T getData()
        {
            return data;
        }
      
       public int getStreamNum()
       {
           return streamNum;
       }
       

    }
}
