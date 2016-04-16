using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneCentral
{
    class Event<T>
    {
        int type;//1-end call 0-delete from waiting 2-create new call
        T data;
        int whichStream;
        double executionTime;
        double timeOfArrival=0;
      public int size = 1;//how many links call requires
       public Event()
        {

        }
        
        public Event(T d, double c,int t,int str,double cp,int roz)
        {
            type = t;
            data = d;
            executionTime = c;
            whichStream = str;
            timeOfArrival=cp;
            size = roz;
         
        }

        public void add(T d,double c,int t,int cs,int roz)
        {
            type = t;
            data = d;
            executionTime = c;
            size = roz;
        }

       public int getType()
        {
            return type;
        }

       public T getData()
       {
           return data;
       }

       public double getExecutionTime()
       {
           return executionTime;
       }

       public Call<T> ToCall()
 
       {
           Call<T> zgl=new Call<T>();
           zgl.add(executionTime, data,whichStream,timeOfArrival,size);
           return zgl;
       }
       public int getStream()
       {
           return whichStream;
       }
       public double getTimeOfArrival()
       {
           return timeOfArrival;
       }
    }
}
