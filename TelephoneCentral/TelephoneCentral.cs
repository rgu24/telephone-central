using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TelephoneCentral
{
    class TelephoneCentral<T>
    {
        String nameOfSystem;

        int distributionNum;
        int channelNum;
        int queueMaxSize;

        Call<T>[] executeCallTable;
        Call<T>[] callWaitingTable;
       
        
        
        private PirorityQueue<double,Event<T>> eventQueue;

        private int witingCallNum=0;
        private double[] missedCallProbability;
        private double[] callNum ;
        private int[] executedCallsNum;
        private double avregeUsabiltyOfChannels=0;
        private double avregeUsabiltyOfQueue=0;
        private int usedChannels = 0;
        private double[] timeTable;
        double avreageTimeOfService = 0;

        private int[] missedCall;

       int streamNum=1;
       Stream[] streams;
        
   
        //METODY
   
    public  TelephoneCentral(int a, int b,int str)
    {
        channelNum=a;
        queueMaxSize=b;
       
        if (queueMaxSize != 0)
        {
            callWaitingTable = new Call<T>[queueMaxSize];
        }
        executeCallTable = new Call<T>[channelNum];
        
        eventQueue = new PirorityQueue<double,Event<T>>();
        streamNum = str;
        timeTable = new double[streamNum];
        streams = new Stream[streamNum];
        
        for (int i = 0; i<streamNum; i++)
        {
            streams[i] = new Stream();
        }
    }

    public void addToWaitingTable(double m, Call<T> n, double time)
    { }
      
     public void simulation(double timeK)
  {
      #region preapering data
      double time = 0;
      Event<T> ev = new Event<T>();
      Call<T> zgl = new Call<T>();

      Call<T>[] firstCall = new Call<T>[streamNum];

      callNum = new double[streamNum];
      executedCallsNum = new int[streamNum];

      missedCall = new int[streamNum];
      Random rnd = new Random();

      #endregion

      #region first iteration
      for (int i = 0; i < streamNum;i++)
      {
          callNum[i] = 1;
          executedCallsNum[i] = 0;

          double m = rnd.NextDouble();

          firstCall[i] = new Call<T>();
          
         firstCall[i].add(streams[i].getCallLengthDistribution().getTime(m),i,
             time,streams[i].size);

         getCallFromOutside(firstCall[i],time);

         Event<T> zdarz = new Event<T>(firstCall[i].getData(),
              time + streams[firstCall[i].getStreamNum()].getNextCallDistribution().getTime(rnd.NextDouble()), 2, i, time, streams[i].size, firstCall[i].getCallLength()); 
         Element<double,Event<T>> el = new Element<double,Event<T>>(zdarz.getExecutionTime(),zdarz);
        
          eventQueue.add(el);

      }
      #endregion
      while (time < timeK)
          {

              bool isEvent = false;
              bool jest_zglosznie = false;

              double timeDoStatystyk=0;
              double timep = 0;

              if (eventQueue.getSize() != 0)
              {
                  Element<double,Event<T>> el = new Element<double,Event<T>>(1.0,ev);
                 
                  el = eventQueue.getMin();
                  ev = el.getData(); 
        
                  isEvent = true;
                  timep = time;

                  time = el.getKey();
                  timeDoStatystyk = time - timep;

                  avregeUsabiltyOfQueue +=Math.Abs(witingCallNum * timeDoStatystyk);
                  avregeUsabiltyOfChannels +=Math.Abs(usedChannels* timeDoStatystyk);

              }
              if (isEvent)
              {

                  switch (ev.getType())
                  {

                      case 0:
                          for (int i = 0; i < ev.size; i++)
                          {
                              deleteCallFromWaiting(ev.ToCall());
                          }
                          break;

                      case 1:
                          for (int i = 0; i < ev.size; i++)
                          {
                              deleteCallFromExecuteTable(ev.ToCall());
                              timeTable[ev.getStream()] += time - ev.getTimeOfArrival();
                          }
                          break;

                      case 2:
                          zgl.add(streams[ev.getStream()].getNextCallDistribution().getTime(rnd.NextDouble()), ev.getData(), 
                              ev.getStream(),time,ev.size);
                          
                          Event<T> zdyrz = new Event<T>(zgl.getData(), 
                              time + streams[zgl.getStreamNum()].getCallLengthDistribution().getTime(rnd.NextDouble()), 2, zgl.getStreamNum(),time,zgl.size,zgl.getCallLength());
                          Element<double,Event<T>> el = new Element<double,Event<T>>(zdyrz.getExecutionTime(),zdyrz);
                          
                          eventQueue.add(el);        
                          jest_zglosznie = true;

                          callNum[ev.getStream()]++;
                          break;

                      default:
                          break;
                  }
              }
                 
             
                   if (witingCallNum != 0)
                  {
                      getCallFromWaiting(time);
                   }

              if (jest_zglosznie)
                  {
                      getCallFromOutside(zgl,time);
                    
                  }

          }
          Statistics(time);
  }
      void Statistics(double endTime)
       {
           missedCallProbability = new double[streamNum];
           int executedCalls = 0;

          for (int i = 0; i < streamNum; i++)
           {
               executedCalls += executedCallsNum[i];
              
              missedCallProbability[i] = ( missedCall[i]/ callNum[i]) * 100;
              avreageTimeOfService += timeTable[i] ;
              Console.WriteLine(missedCallProbability[i]);
           }
          avreageTimeOfService = avreageTimeOfService / executedCalls;
          avregeUsabiltyOfChannels = avregeUsabiltyOfChannels * 100 / (channelNum * endTime);
          avregeUsabiltyOfQueue = avregeUsabiltyOfQueue * 100 / (queueMaxSize * endTime); 

       }
  
        //method finding how many and which channels are free
       int[] isFreeLink(Call<T> zgl)
        {
            int freeLinks = 0;
            int k = 0;
            int[] links = new int[zgl.size];
           for (int i = 0; i < channelNum;i++ )
            {
                if(executeCallTable[i]==null)
                {
                    freeLinks++;
                    links[k] = i;
                    k++;
                }
               if(freeLinks==zgl.size)
               {
                   return links;
               }
            }
           
               links[0] = -1;
           
               return links;
        }
      //method getting call from outside
       public void getCallFromOutside(Call<T> zgl,double time)
        {
           
          
           int[] i=new int[zgl.size];
            i = isFreeLink(zgl);

            if ((i[0] != -1) && (witingCallNum == 0))
            {
                reserveChannels(i, zgl, time);
            }
            else if (witingCallNum < queueMaxSize)
            {
                callWaitingTable[witingCallNum] = zgl;
                witingCallNum++;
                Event<T> zdyrz = new Event<T>(zgl.getData(), time + streams[zgl.getStreamNum()].getMaxTime(), 0, zgl.getStreamNum(),time,zgl.size,zgl.getCallLength());
                Element<double, Event<T>> elm = new Element<double, Event<T>>(zdyrz.getExecutionTime(), zdyrz);
                eventQueue.add(elm);
            }
            else missedCall[zgl.getStreamNum()]++;
        }
        //method getting Call from waitingCallTable and putting them into the executeCallsTable
     public  void getCallFromWaiting(double time)
         
       {
           int[] i=new int[callWaitingTable[0].size];
           i = isFreeLink(callWaitingTable[0]);
          
           if(i[0]!=-1)
           {
               Random rnd = new Random();
               Call<T> zgl = moveQueue();
               reserveChannels(i, zgl,time);
               witingCallNum--;   
           }
       }
    //method deleting Call from WaitingTable
       public void deleteCallFromWaiting(Call<T>zgl)       
       {
         
           int nr=0;
           bool isFound = false;
           for(int i=0;i<queueMaxSize;i++)
           {
               if(callWaitingTable[i]==zgl)
               {
                    nr=i;
                    isFound = true;
                   break;
               }
           }
           if(isFound)
           { 
               for(int i=nr;i<queueMaxSize-2;i++)
               {
                   callWaitingTable[i] = callWaitingTable[i + 1];
               }
               callWaitingTable[queueMaxSize - 1] = null;
               witingCallNum--;
               missedCall[zgl.getStreamNum()]++;
           }
   }
//method deleting the call from execute table because call has ended
       public void deleteCallFromExecuteTable(Call<T> zgl)
           
       {
           int nr = 0;
           int i;
           for ( i = 0; i < channelNum; i++)
           {
               if (executeCallTable[i] != null)
               {
                   if (executeCallTable[i].Equals(zgl))
                   {
                       nr = i;
                       break;
                   }
               }
           }
           if (i == channelNum)
               Console.Out.Write("fail");
           executedCallsNum[zgl.getStreamNum()]++;
           executeCallTable[nr] = null;
           usedChannels--;
       }
       //move FIFO queue
        Call<T> moveQueue()  
     {

         Call<T> zgl;
         zgl = callWaitingTable[0];
            for(int i=0;i<queueMaxSize-2;i++)
            {
                 callWaitingTable[i] = executeCallTable[i + 1];
            }
           
            callWaitingTable[queueMaxSize - 1] = null;
            return zgl;
     }

       void reserveChannels(int[] i,Call<T> zgl,double time)
           //funkcja zajmowania łącza
        {

            Random rnd = new Random();
             
           Event<T> zdyrz = new Event<T>(zgl.getData(),
               time + zgl.getCallLength(), 1, zgl.getStreamNum(), time, zgl.size,zgl.getCallLength());
            Element<double, Event<T>> elm = new Element<double, Event<T>>(zdyrz.getExecutionTime(), zdyrz);
           
            eventQueue.add( elm);
            for (int j = 0; j < zgl.size; j++)
            {
                executeCallTable[i[j]] = new Call<T>();
                executeCallTable[i[j]].add(zgl.callLength,zgl.getData(),zgl.getStreamNum(),zgl.arrivalTime,zgl.size);
                usedChannels++;

               
            }
            
        
        }
        
        public void LoadDataFromFile()
       {
           string[] words;
           try
           {

               StreamReader sr;
               string filePath;
               Console.WriteLine("Drop the file here and press ENTER...");
               
               filePath = Console.ReadLine();
               sr = new StreamReader(filePath);
               String line = "";
               while(line.Length < 2 || line[0]=='#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               #region nazwa
               words = line.Split(' ');
               if (words[0] == "SYSTEM" && words[2] != "")
                   nameOfSystem= words[2];
               else throw (new Exception("Wrong system name"));

               #endregion
               #region kanaly
                line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "CHANNELS" && words[2] != "")
               {
                   channelNum = int.Parse(words[2]);
                   executeCallTable = new Call<T>[channelNum];
                  
               }
               else throw (new Exception("Wrong channel Number"));
               #endregion
               #region kolejka
               line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "QUEUE" && words[2] != "")
               {
                   queueMaxSize = int.Parse(words[2]);
                  if(queueMaxSize>0)
                    callWaitingTable = new Call<T>[queueMaxSize];
                  

               }
               else throw (new Exception("Wrong size of the queue"));
               #endregion

               #region liczba rozkladow
               line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "DISTRIBUTIONS" && words[2] != "")
               {
                   distributionNum = int.Parse(words[2]);


               }
               
               else throw (new Exception("Wrong number of distributions"));
               #endregion
               #region rozklady
               string[] nazwa=new string[distributionNum];
               double[] lambda=new double[distributionNum];
               for (int i = 0; i < distributionNum; i++)
               {
                   
                   line = "";
                   while (line.Length < 2 || line[0] == '#')
                   {
                       line = sr.ReadLine();
                   }
                   words = line.Split(' ');
                   if (words[0] == "NAME" && words[2] != "")
                   {
                       nazwa[i] = words[2];
                   }
                   else throw (new Exception("Wrong name of distribution"));
                   line = "";
                   while (line.Length < 2 || line[0] == '#')
                   {
                       line = sr.ReadLine();
                   }
                   words = line.Split(' ');
                   if (words[0] == "LAMBDA" && words[2] != "")
                   {
                       lambda[i] = double.Parse(words[2]);

                   }
                   else throw (new Exception("Wrong lambda"));

                  
               }
               #endregion
               #region liczba Streami
               line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "STREAMS" && words[2] != "")
                   streamNum = int.Parse(words[2]);
               else throw (new Exception("wrong streams"));
               #endregion
               #region wczytywanie Streami
               streams = new Stream[streamNum];
                int rozT=0;
               int rozO=0;
               for (int i = 0; i < streamNum; i++)
               {
                   string nazwas;
                   int size;
                   double timeOczekiwania;
                   string timeTrwania, odstep;
                   line = "";
                   while (line.Length < 2 || line[0] == '#')
                   {
                       line = sr.ReadLine();
                   }
                   words = line.Split(' ');
                   if (words[0] == "NAME" && words[2] != "" && words[3] == "SIZE" && words[5] != "" &&
                       words[6] == "TIME_WAITING" && words[8] != "" && words[9] == "LENGTH" &&
                       words[11] != "" && words[12] == "BETWEEN" && words[14] != "")
                   {
                       nazwas = words[2];
                       size = int.Parse(words[5]);
                       timeOczekiwania = double.Parse(words[8]);
                       timeTrwania = words[11];
                       odstep = words[14];
                   }
                   else throw (new Exception("wrong data about stream"));

                  
                   for (int j = 0; j < distributionNum;j++ )
                   {
                       if(timeTrwania==nazwa[j])
                       {
                           rozT=j;
                       }
                       if(odstep==nazwa[j])
                       {
                           rozO=j;
                       }
                   }
                  
                   streams[i] = new Stream(nazwas,lambda[rozT] , nazwa[rozT], lambda[rozO], nazwa[rozO],timeOczekiwania,size);  
               }
               #endregion        



           }
           catch(Exception fail)
           {
               Console.WriteLine("Wrong file path try one more time.");
               Console.WriteLine(fail.Message);
           }
           
       }
       public  void WriteData()
      {
          string filePath;
          Console.WriteLine("Please drag output file here ENTER...");

          filePath = Console.ReadLine();
        
              StreamWriter wyniki = new StreamWriter(filePath);

              wyniki.WriteLine("Name of the system: " + nameOfSystem);
              for (int i = 0; i < streamNum; i++)
                  wyniki.WriteLine("Probability of missed calls in Stream" + streams[i].getName() + " is: " + Math.Round(missedCallProbability[i], 2) + "%");
              wyniki.WriteLine("Avrege Usability of channels: " + Math.Round(avregeUsabiltyOfChannels, 2) + "%");
              wyniki.WriteLine("Avrege usability of waiting queue: " + Math.Round(avregeUsabiltyOfQueue, 2) + "%");
              wyniki.WriteLine("Avreage Time of service: " + Math.Round(avreageTimeOfService*100, 2) + " ms");
              wyniki.Close();
        
           
           
      }
}
          
}
     
