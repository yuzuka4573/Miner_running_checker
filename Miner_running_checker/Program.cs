using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace Miner_running_checker
{
    class profile
    {
        public string ID { get; set; }
        public string Coin { get; set; }
        public string GUS { get; set; }
        public string GUB { get; set; }
    }
    class Program
    {
        public static int PID;
        public static string[,] saver;
        public static List<profile> data = new List<profile>();
        //ID:0 Coin:1 GetUserS:2 GetUserB:3
        public static double[,] results;
        public static int runninTime = 0;
        public static int timerInterval = 0;
        public static string menu = "";
        static void Main(string[] args)
        {

            PID = 0;
            Console.WriteLine("MPOS Mining Pool API Reader");
            Console.WriteLine("V 0.0.3");

            Console.WriteLine("Reading Profile.txt.........\r\n");
            Thread.Sleep(250);
            try
            {
                string line = "";
                ArrayList al = new ArrayList();
                using (StreamReader sr = new StreamReader(
                    "profile.txt", Encoding.GetEncoding("utf-8")))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        al.Add(line);
                    }
                }

                //4line =1set
                if (al.Count % 4 != 0)
                {
                    Console.WriteLine("file check faild...");
                    Console.WriteLine("You type wrong format of \"profile.txt\"! Please check and fix it.");
                    Console.WriteLine("please press key to exit system...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                PID = al.Count / 4;
                /*
                  make save plase with PID value (7 properties)
                  0:Username 1:Shares Valid 2:Shares Invalid 3:Hashrate
                  4:ShareRate 5:Confirmed Coin 6:Unconfirmed Coin
                */

                for (int c = 0; c < al.Count / 4; c++)
                {
                    data.Add(new profile() { ID = al[4 * c].ToString(), Coin = al[4 * c + 1].ToString(), GUS = al[4 * c + 2].ToString(), GUB = al[4 * c + 3].ToString() });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("something occured!!");
                Console.WriteLine(e);
            }

            //sample check
          //  Filecheck();
            Console.WriteLine("file check completed!!");
            //timer set function
            TS();

            saver = new string[PID, 7];
            //comvert data
            Converting();


            //save first Dig result
            results = new double[PID, 4];
            //0:valid  value 1:confirmed coin value 2,3: latest valid[2]/confirmed coin value[3]
            for (int cc = 0; cc < PID; cc++)
            {
               // results[cc, 0] = double.Parse(saver[cc, 1]);
                results[cc, 1] = double.Parse(saver[cc, 5]);
               // results[cc, 2] = results[cc, 0];
                results[cc, 3] = results[cc, 1];
            }
            //display
            Display();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(TimeDisp);
            timer.Interval = 1000 * 60 * timerInterval;
            timer.AutoReset = true;
            timer.Enabled = true;

            while (true)
            {
                Console.WriteLine("Please type commands if you want to get more informations.");
                Console.WriteLine("Type \"help\" to see all of commands");
               menu= Console.ReadLine();
                switch (menu) {

                    default:
                        Console.WriteLine("Exit from system...");
                        Thread.Sleep(2000);
                        Environment.Exit(0);
                        break;

                    case "set":
                        Console.WriteLine("Set timer interval againg.");
                        timer.Dispose();
                        //Re: setting timer 
                        TS();
                        timer = new System.Timers.Timer();
                        timer.Elapsed += new ElapsedEventHandler(TimeDisp);
                        timer.Interval = 1000 * 60 * timerInterval;
                        timer.AutoReset = true;
                        timer.Enabled = true;
                        break;
                    case "help":
                        Console.Write("[");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Command");
                        Console.ResetColor();
                        Console.Write(" : ");
                        Console.Write("Description");
                        Console.WriteLine("]");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("set");
                        Console.ResetColor();
                        Console.Write(" : ");
                        Console.WriteLine("set timer interval agein");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("JUST ENTER or Unknow commands");
                        Console.ResetColor();
                        Console.Write(" : ");
                        Console.WriteLine("Exit from system ");
                        break;
                }
            }
        }


        public static string Gethtml(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);     //using System.Net;
            string html;
            using (var res = (HttpWebResponse)req.GetResponse())
            using (var resSt = res.GetResponseStream())
            using (var sr = new StreamReader(resSt, Encoding.UTF8))     //using System.IO;
            {
                html = sr.ReadToEnd();
            }

            //delete strings
            html = html.Replace("{", "");
            html = html.Replace("}", "");
            html = html.Replace("\":\"", ":");
            html = html.Replace("\"", "");
            html = html.Replace(",", "\r\n");
            html = html.Replace("shares:", "shares:\r\n");
            html = html.Replace("data:", "data:\r\n");
            html = html.Replace("getuserstatus:", "getuserstatus:\r\n");
            html = html.Replace("getuserbalance:", "getuserbalance:\r\n");

            return html;
        }


        public static void Display()
        {

            for (int con = 0; con < PID; con++)
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Pool Data {0}" , con + 1);
                for (int con2 = 0; con2 < 7; con2++)
                {
                    switch (con2)
                    {
                        case 0:
                            Console.Write("User : ");
                            break;
                        case 1:
                            Console.Write("Valid : ");
                            break;
                        case 2:
                            Console.Write("Invalid : ");
                            break;
                        case 3:
                            Console.Write("HashRate : ");
                            break;
                        case 4:
                            Console.Write("ShareRate : ");
                            break;
                        case 5:
                            Console.Write("Confirmed : ");
                            break;
                        case 6:
                            Console.Write("Unconfirmed : ");
                            break;
                    }
                    Console.Write(saver[con, con2]);
                    if (con2 == 3) Console.Write("Hash/Sec");
                    Console.Write("\r\n");
                }
                Calc(con);
                Console.WriteLine("--------------------------------------------------------");

                Console.WriteLine();

            }

        }


        public static void TimeDisp(object sender, EventArgs e)
        {
            runninTime += timerInterval;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("MINER STATES");
            Console.WriteLine("DATE : " + DateTime.Now + "   ({0} min running after)", runninTime);
            Console.ResetColor();
            //comvert data
            Converting();
            for (int num = 0; num < PID; num++)
            {
                results[num, 2] = double.Parse(saver[num, 1]);
                results[num, 3] = double.Parse(saver[num, 5]);
            }
            //display
            Display();

        }


        public static void Converting()
        {

            for (int c2 = 0; c2 < PID; c2++)
            {
                try
                {
                    string statedata = Gethtml(data[c2].GUS.ToString());
                    string[] separator = new string[] { "\r\n" };
                    string[] SDSep = statedata.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int count = 0;
                    foreach (string a in SDSep)
                    {
                        count++;
                        if (a.Contains("username:"))
                        {
                            saver[c2, 0] = a.Substring(9);
                        }
                        else if (a.Contains("valid:"))
                        {
                            if (a.IndexOf('v') != 0)
                            {
                                saver[c2, 2] = a.Substring(8);
                            }
                            else
                            {
                                saver[c2, 1] = a.Substring(6);
                            }

                        }
                        else if (a.Contains("hashrate:"))
                        {
                            saver[c2, 3] = a.Substring(9);
                        }
                        else if (a.Contains("sharerate:"))
                        {
                            saver[c2, 4] = a.Substring(10);
                        }
                    }
                    count = 0;
                    string balancedata = Gethtml(data[c2].GUB.ToString());
                    string[] BDSep = balancedata.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string a in BDSep)
                    {
                        count++;
                        if (a.Contains("confirmed:"))
                        {
                            if (a.IndexOf('c') != 0)
                            {
                                saver[c2, 6] = a.Substring(12);
                            }
                            else
                            {
                                saver[c2, 5] = a.Substring(10);
                            }

                        }
                    }

                }
                catch (Exception f)
                {
                    Console.WriteLine("something occured!!");
                    Console.WriteLine(f);
                }


            }
        }


        public static void Calc(int num)
        {

           // results[num, 2] = double.Parse(saver[num, 1]);
            results[num, 3] = double.Parse(saver[num, 5]);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Dig results");
         //   Console.WriteLine("Shares Valid Amplification : +" + (results[num, 2] - results[num, 0]));
            Console.WriteLine("Confirmed Coin Amplification : +" + (results[num, 3] - results[num, 1]));
            Console.ResetColor();
        }


        public static void Filecheck()
        {
            for (int num = 0; num < PID; num++)
            {
                Console.WriteLine("/////CHECK SYSTEM ACTIVE/////");
                Console.WriteLine(data[num].GUS.ToString() + " == "+ data[num].GUS.ToString().IndexOf("http:"));
                Console.WriteLine(data[num].GUB.ToString()+ " == " + data[num].GUB.ToString().IndexOf("http:"));
                if (data[num].GUS.ToString().IndexOf("http:")==-1)
                {
                    Console.WriteLine("//////////WARNING//////////");
                    Console.WriteLine("There is no pool site API URLs!!");
                    Console.WriteLine("Please check your \"profile.txt\" !!");
                    Console.WriteLine("please press key to exit system...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else if (data[num].GUB.ToString().IndexOf("http:") == -1)
                {
                    Console.WriteLine("//////////WARNING//////////");
                    Console.WriteLine("There is no pool site API URLs!!");
                    Console.WriteLine("Please check your \"profile.txt\" !!");
                    Console.WriteLine("please press key to exit system...");
                    Console.ReadKey();
                    Environment.Exit(0);

                }
            }
        }

        public static void TS() {
            Console.WriteLine("\r\n\r\nTimer Setting");
            Console.WriteLine("Please type check miner states/balance interval with minute (1-1440(1day))");
            try
            {
                timerInterval = int.Parse(Console.ReadLine());
                if (timerInterval <= 0)
                {
                    Console.WriteLine("//////////CAUTION//////////");
                    Console.WriteLine("you type under the 1 min! Timer set 5 min");
                    timerInterval = 5;
                }
                else if (timerInterval > 1440)
                {
                    Console.WriteLine("//////////CAUTION//////////");
                    Console.WriteLine("you type time more than 1 day! Timer set 1Day(1440min)");
                    timerInterval = 5;
                }
            }
            catch (Exception aaa)
            {
                Console.WriteLine("You type no number\r\nTimer set 5 min");

            }
        }
    }
}
