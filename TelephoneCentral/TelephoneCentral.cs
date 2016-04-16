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
        Call<T>[] executeCallTable;
        Call<T>[] callWaitingTable;
        int queueMaxSize;
        int streamNum;
        Stream[] streams;
       
        
        public void LoadDataFromFile()
       {
           string[] words;
           try
           {

               StreamReader sr;
               string filePath;
               Console.WriteLine("Przeciagnij tu plik wejsciowy i wcisnij ENTER...");
               
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
               else throw (new Exception("Zla nazwa systemu"));

               #endregion
               #region kanaly
                line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "KANALY" && words[2] != "")
               {
                   channelNum = int.Parse(words[2]);
                   executeCallTable = new Call<T>[channelNum];
                   //for(int i=0;i<channelNum;i++)
                   //{
                   //    callWaitingTable[i] = new Event<T>();
                   //}
               }
               else throw (new Exception("Zla liczba kanałów"));
               #endregion
               #region kolejka
               line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "KOLEJKA" && words[2] != "")
               {
                   queueMaxSize = int.Parse(words[2]);
                   callWaitingTable = new Call<T>[queueMaxSize];
                   //for (int i = 0; i < queueMaxSize; i++)
                   //{
                   //    waitingQueue[i] = new Event<T>();
                   //}

               }
               else throw (new Exception("Zla liczba pojemności kolejki"));
               #endregion

               #region liczba rozkladow
               line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "ROZKLADY" && words[2] != "")
               {
                   distributionNum = int.Parse(words[2]);


               }
               
               else throw (new Exception("Zla liczba rozkładów"));
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
                   if (words[0] == "NAZWA" && words[2] != "")
                   {
                       nazwa[i] = words[2];
                   }
                   else throw (new Exception("Zla nazwa rozkladu"));
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
                   else throw (new Exception("Zla lambda rozkladu"));

                  
               }
               #endregion
               #region liczba Streami
               line = "";
               while (line.Length < 2 || line[0] == '#')
               {
                   line = sr.ReadLine();
               }
               words = line.Split(' ');
               if (words[0] == "streams" && words[2] != "")
                   streamNum = int.Parse(words[2]);
               else throw (new Exception("Zla liczba Streami"));
               #endregion
               #region wczytywanie Streami
               streams = new Stream[streamNum];
                int rozT=0;
               int rozO=0;
               for (int i = 0; i < streamNum; i++)
               {
                   string nazwas;
                   int rozmiar;
                   double czasOczekiwania;
                   string czasTrwania, odstep;
                   line = "";
                   while (line.Length < 2 || line[0] == '#')
                   {
                       line = sr.ReadLine();
                   }
                   words = line.Split(' ');
                   if (words[0] == "NAZWA" && words[2] != "" && words[3] == "ROZMIAR" && words[5] != "" &&
                       words[6] == "CZAS_OCZEKIWANIA" && words[8] != "" && words[9] == "CZAS_TRWANIA" &&
                       words[11] != "" && words[12] == "ODSTEP" && words[14] != "")
                   {
                       nazwas = words[2];
                       rozmiar = int.Parse(words[5]);
                       czasOczekiwania = double.Parse(words[8]);
                       czasTrwania = words[11];
                       odstep = words[14];
                   }
                   else throw (new Exception("Zle dane Streamia"));

                  
                   for (int j = 0; j < distributionNum;j++ )
                   {
                       if(czasTrwania==nazwa[j])
                       {
                           rozT=j;
                       }
                       if(odstep==nazwa[j])
                       {
                           rozO=j;
                       }
                   }
                  
                   streams[i] = new Stream(nazwas,lambda[rozT] , nazwa[rozT], lambda[rozO], nazwa[rozO],czasOczekiwania,rozmiar);  
               }
               #endregion        



           }
           catch(Exception fail)
           {
               Console.WriteLine("Zla filePath pliku. Sprobuj jeszcze raz.");
               Console.WriteLine(fail.Message);
           }
           
       }
       
          
    }
}
