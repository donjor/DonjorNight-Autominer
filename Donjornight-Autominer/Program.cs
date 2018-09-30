using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Timers;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Donjornight_Autominer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     DONJORNIGHT AUTOMINER");
            Console.WriteLine("     - Donjor (Haven Protocol team)");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;



            //read all the coins!
            string[] coins = File.ReadAllLines("DonjorNight-Coins.txt");

            foreach (var coin in coins)
            {
                if (coin.StartsWith("*"))
                {
                    //ignore
                }
                else
                {
                    //its a coin
                    //don't judge me for this lmao
                    var comma = 0;
                    //resize the array
                    Array.Resize(ref Globals.coinName, Globals.coinName.Length + 1);
                    Globals.coinName[Globals.coinName.Length - 1] = coin.Substring(comma, coin.IndexOf(',', comma) - comma);

                    comma = coin.IndexOf(',', comma + 1);
                    Array.Resize(ref Globals.coinTicker, Globals.coinTicker.Length + 1);
                    Globals.coinTicker[Globals.coinTicker.Length - 1] = coin.Substring(comma + 2, coin.IndexOf(',', comma + 1) - comma - 2);

                    comma = coin.IndexOf(',', comma + 1);
                    Array.Resize(ref Globals.coinAlgo, Globals.coinAlgo.Length + 1);
                    Globals.coinAlgo[Globals.coinAlgo.Length - 1] = coin.Substring(comma + 2, coin.IndexOf(',', comma + 1) - comma - 2);

                    comma = coin.IndexOf(',', comma + 1);
                    Array.Resize(ref Globals.coinPool, Globals.coinPool.Length + 1);
                    Globals.coinPool[Globals.coinPool.Length - 1] = coin.Substring(comma + 2, coin.IndexOf(',', comma + 1) - comma - 2);

                    comma = coin.IndexOf(',', comma + 1);
                    Array.Resize(ref Globals.coinAddress, Globals.coinAddress.Length + 1);
                    Globals.coinAddress[Globals.coinAddress.Length - 1] = coin.Substring(comma + 2, coin.IndexOf(',', comma + 1) - comma - 2);

                };
            }
            TradeOgre();
            Benchmark();
            Go();
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 15 Mins
            aTimer.Interval = 900000;
            aTimer.Enabled = true;

        }

        static void Benchmark()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("**************************************************************");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     BENCHMARKING RELEVANT MINING ALGORITHMS");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("**************************************************************");

            //Go through each algo
            //Array.Resize(ref Globals.coinAlgoHash, Globals.coinName.Length);
            //Globals.chrome.Manage().Window.Minimize();

            var coincount = 0;

            foreach (var algo in Globals.coinAlgo)
            {
                int i = Convert.ToInt16(algo);



                if (Globals.coinAlgoHash[i] == 0)
                {
                    //Not benchmarked
                    
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("");
                    Console.WriteLine("     NOW BENCHMARKING ALGORITHM: " + i);
                    Console.WriteLine("");

                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    Globals.mostProfitableCoin = coincount;
                    Thread.Sleep(500);

                    //Benchmark

                    //Get GPU count
                    //Globals.currentlyMining = Globals.mostProfitableCoin;
                    int gpucount = 0;
                    string gpu = "";
                    ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
                    foreach (ManagementObject obj in objvide.Get())
                    {
                        var gpuname = obj["VideoProcessor"];
                        if (obj["VideoProcessor"].ToString().Contains("Intel"))
                        {
                            //intel = not gpu
                        }
                        else
                        {
                            gpu = gpu + gpucount + ",";
                            gpucount++;

                        }

                    }


                    // Start process.
                    Boolean kill = false;
                    double hash = 0;
                    var hashCount = 0;
                    TimeSpan runtime = DateTime.Now - DateTime.Now;

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cast_xmr-vega.exe"; ;
                        process.StartInfo.Arguments = "--algo=" + Globals.coinAlgo[Globals.mostProfitableCoin] + " -S " + Globals.coinPool[Globals.mostProfitableCoin] + " -u " + Globals.coinAddress[Globals.mostProfitableCoin] + " -G " + gpu;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.Start();

                        StringBuilder output = new StringBuilder();
                        StringBuilder error = new StringBuilder();

                        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        {
                            try
                            {
                                process.OutputDataReceived += (sender, e) =>
                                {
                                    if (e.Data == null)
                                    {
                                        try
                                        {
                                            outputWaitHandle.Set();
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            runtime = DateTime.Now - process.StartTime;
                                        }
                                        catch
                                        {
                                            runtime = DateTime.Now - DateTime.Now;
                                        }

                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        output.AppendLine(e.Data);
                                        Console.WriteLine(e.Data);
                                        if (e.Data.ToString().Contains("RPM |"))
                                        {
                                            if (hashCount == 0)
                                            {
                                                runtime = DateTime.Now - DateTime.Now;
                                            }
                                            if (gpucount < 2)
                                            {
                                                var start = e.Data.ToString().IndexOf("RPM |") + 5;
                                                var end = e.Data.ToString().IndexOf("/s");
                                                double currentHash = Convert.ToDouble(e.Data.ToString().Substring(start, end - 2 - start));

                                                if (hash == 0 && hashCount != 0)
                                                {
                                                    hash = currentHash;
                                                }
                                                else if (hash > 0)
                                                {
                                                    hash = Math.Round((hash + currentHash * gpucount) / 2,2);
                                                }
                                                hashCount++;

                                                Console.WriteLine("");
                                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                                Console.WriteLine("     " + hashCount + "/5 Captured Hashrates | " + Math.Round(runtime.TotalSeconds, 0) + "/45 secs | Algo: " + i + ": " + Globals.coinAlgoName[i] + " - AVERAGE HASHRATE: " + hash + " H/s");
                                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                                Console.WriteLine("");
                                                if (hashCount > 4 * gpucount && runtime.TotalSeconds > 45)
                                                {
                                                    kill = true;
                                                }
                                            }
                                        }
                                        else if (e.Data.ToString().Contains("Hash Rate Avg:"))
                                        {
                                            if (hashCount == 0)
                                            {
                                               runtime = DateTime.Now - DateTime.Now;
                                            }
                                            if (gpucount > 1)
                                            {

                                                var start = e.Data.ToString().IndexOf("Hash Rate Avg:") + 15;
                                                var end = e.Data.ToString().IndexOf("/s");
                                                double currentHash = Convert.ToDouble(e.Data.ToString().Substring(start, end - 2 - start));
                                                
                                                Console.WriteLine("");
                                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                                Console.WriteLine("     Captured Hashrates: " + Math.Round(runtime.TotalSeconds,0) + "/45 secs | " + Math.Round((currentHash / hash * 100 - 100),1) + "% hash increase - target < 1% | Algo: " + i + " - AVERAGE HASHRATE: " + currentHash + " H/s");
                                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                                Console.WriteLine("");


                                                if (hashCount > 2 && (currentHash / hash < 1.001) && runtime.TotalSeconds > 45 && (currentHash / hash > 0.99))
                                                {
                                                    hash = currentHash;
                                                    kill = true;
                                                }

                                                hashCount++;
                                                hash = currentHash;
                                            }
                                        }
                                        else if (e.Data.ToString().Contains("Connecting to Pool failed."))
                                        {
                                            kill = true;
                                        }
                                        else if (runtime.TotalSeconds > 120 && hash > 0)
                                        {
                                            kill = true;
                                        }
                                        else if (runtime.TotalSeconds > 240)
                                        {
                                            kill = true;
                                        }
                                    }
                                };
                                process.ErrorDataReceived += (sender, e) =>
                                {
                                    try
                                    {
                                        if (e.Data == null)
                                        {

                                            errorWaitHandle.Set();
                                        }
                                        else
                                        {
                                            error.AppendLine(e.Data);
                                        }
                                    }
                                    catch
                                    {

                                    }

                                };


                                process.BeginOutputReadLine();
                                process.BeginErrorReadLine();


                                while (kill == false)
                                {
                                    Thread.Sleep(2000);
                                }

                                if (kill == true)
                                {
                                    process.Kill();
                                }

                                if (process.WaitForExit(15) &&
                                    outputWaitHandle.WaitOne(120) &&
                                    errorWaitHandle.WaitOne(120))
                                {
                                    // Process completed. Check process.ExitCode here.
                                }
                                else
                                {
                                    // Timed out.
                                }
                            }

                            catch
                            {

                            }
                        }

                    }

                    if (Convert.ToDouble(hash) > 0)
                    {
                        Globals.coinAlgoHash[i] = hash;
                    }





                }
                coincount++;
            }



        }

        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
           

            Go();
        }
        static void Go()
        {
            Globals.mostProfitableCoinProfit = 0;
            Globals.mostProfitableCoin = -1;
            TimeSpan runtime;
            try
            {
                runtime = DateTime.Now - Globals.exeProcess.StartTime;
            }
            catch
            {
                runtime = DateTime.Now - DateTime.Now;
            }

            try
            {
                TimeSpan ts = DateTime.Now - Globals.lastShareTime;
                if (ts.Minutes > 5 && runtime.Minutes > 5)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine();
                    Console.WriteLine("         Error - No shares detected, cast xmr likely crashed");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;

                    Globals.exeProcess.Kill();
                    Globals.currentlyMining = -1;
                    Benchmark();
                    Go();
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine();
                Console.WriteLine("         Error - 377:" + e.ToString());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
            


            GetCurrentProfit();


            int coinCount = 0;
            //determine most profitable coin
            Console.WriteLine("");
            try
            {


                foreach (double coinProfitPerKH in Globals.profit)
                {
                    if (Globals.mostProfitableCoin == -1)
                    {
                        Globals.mostProfitableCoin = 0;
                    }
                    try
                    {
                        double coinProfit = 0;
                        int algo = Convert.ToInt32(Globals.coinAlgo[coinCount]);
                        double algoHash = Globals.coinAlgoHash[algo];

                        coinProfit = (coinProfitPerKH * algoHash) / 1000;

                        //string print = "     " + Globals.coinName[coinCount] + " Daily Profit: $" + coinProfit;

                        Console.WriteLine("     " + Globals.coinName[coinCount] + " Daily Profit: \t $" + Math.Round(coinProfit, 2) + " \t (" + algoHash + " H/s) " + Globals.coinAlgoName[algo]);

                        int MPC = Globals.mostProfitableCoin;

                        if (Math.Round(coinProfit, 2) > Globals.mostProfitableCoinProfit)
                        {
                            Globals.mostProfitableCoin = coinCount;
                            Globals.mostProfitableCoinProfit = Math.Round(coinProfit, 2);
                        }

                    }
                    catch
                    {
                        Console.WriteLine("     !ERROR!  - Can't get profit for " + Globals.coinName[coinCount]);
                    }
                    coinCount++;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine();
                Console.WriteLine("         Can't determine the most profitable coins:" + e.ToString());
                

                if (Globals.mostProfitableCoin == -1)
                {
                    Console.WriteLine("         Overriding profitability - setting most profitable coin to index 0:");
                    Globals.mostProfitableCoin = 0;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }


            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //print out nonsense
            Console.WriteLine();
            Console.WriteLine("     Most Profitable Coin: \t " + Globals.coinName[Globals.mostProfitableCoin]);
            Console.WriteLine("     Daily Profit: \t \t $" + Globals.mostProfitableCoinProfit);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            //determine if we should switch
            //not mining so go.
            if (Globals.currentlyMining == -1)
            {

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("**************************************************************");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("     NOT CURRENTLY MINING SO MINING START");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("**************************************************************");

                //start mining
                LaunchCommandLineApp();
            }
            //if not mining the most profitable coin
            else if (Globals.currentlyMining != Globals.mostProfitableCoin && runtime.TotalMinutes > 60)
            {

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("**************************************************************");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("     NEW COIN MORE PROFITABLE - SWITCHING");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("**************************************************************");


                //kill current
                Globals.exeProcess.Kill();
                //switch
                LaunchCommandLineApp();
            }

            //int currentAlgo = Convert.ToInt32(Globals.coinAlgo[Globals.currentlyMining]);

            //sndkjhskdjsdkjshdjhksd
            else if ((Globals.profit[Globals.currentlyMining] * Globals.coinAlgoHash[Convert.ToInt32(Globals.coinAlgo[Globals.currentlyMining])]) / 1000 * 1.2 < Globals.mostProfitableCoinProfit)
            {
                Console.WriteLine("**************************************************************");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                //Console.WriteLine("     SWITCHING EARLY > " + Globals.profit[Globals.mostProfitableCoin] / Globals.profit[Globals.currentlyMining] + " % PROFIT!");
                Console.WriteLine("     SWITCHING EARLY > " + ((Globals.mostProfitableCoinProfit / (Globals.profit[Globals.currentlyMining] * Globals.coinAlgoHash[Convert.ToInt32(Globals.coinAlgo[Globals.currentlyMining])]) / 1000) - 100 + " % PROFIT!"));
                Console.WriteLine("");
                Console.WriteLine("     Current Mining Profit: $" + Math.Round(Globals.profit[Globals.currentlyMining] * Globals.coinAlgoHash[Convert.ToInt32(Globals.coinAlgo[Globals.currentlyMining])] / 1000),2);
                Console.WriteLine("     Most Profitable Coin Profit: $" + Globals.mostProfitableCoinProfit);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("**************************************************************");
                Console.WriteLine("");


                //kill current
                try
                {
                    Globals.exeProcess.Kill();
                }
                catch { }
                //switch
                LaunchCommandLineApp();
            }
            else
            {

                Console.WriteLine("**************************************************************");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("     NOT SWITCHING");
                Console.WriteLine("     Elapsed time: " + Math.Round(runtime.TotalMinutes, 1).ToString() + " Mins");
                Console.WriteLine("");
                Console.WriteLine("     MINING: " + Globals.coinName[Globals.currentlyMining]);
                Console.WriteLine("     Current Mining Profit: $" + Globals.profit[Globals.currentlyMining] * Globals.coinAlgoHash[Convert.ToInt32(Globals.coinAlgo[Globals.currentlyMining])] / 1000);
                Console.WriteLine("     Most Profitable Coin Profit: $" + Globals.mostProfitableCoinProfit);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("**************************************************************");

            }


        }


        public static class Globals
        {
            public static int currentlyMining = -1;
            public static int mostProfitableCoin = -1;
            public static double mostProfitableCoinProfit = 0;

            public static string[] coinName = new string[0];
            public static string[] coinTicker = new string[0];
            public static string[] coinAlgo = new string[0];
            
            // all 9 algo options
            public static double[] coinAlgoHash = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            public static string[] coinAlgoName = new string[] { "CryptoNight",  "CryptoNightV7", "CryptoNight-Heavy", "CryptoNight-Lite", "CryptoNightV7-Lite", "CryptoNightIPBC-Lite/TUBE-Heavy", "CryptoNightXTL", "CryptoNightXHV-Heavy", "CryptoNight-Fast" };
            //public static double[] coinAlgoHash = new double[] { 0, 0, 1456.72, 0, 4198, 4179, 1975, 1440, 3689 };

            public static string[] coinPool = new string[0];
            public static string[] coinAddress = new string[0];
            public static double[] profit = new double[0]; //number of coins
            public static ProcessStartInfo startInfo = new ProcessStartInfo();
            public static Process exeProcess;

            public static ChromeDriver chrome;
            public static int counter = 0;
            public static DateTime lastShareTime = DateTime.Now;
        }

        static void GetCurrentProfit()
        {
            Globals.chrome = new ChromeDriver();

            //Globals.chrome.Navigate().GoToUrl("https://cryptoknight.cc/");
            Globals.chrome.Manage().Window.Maximize();

            Array.Resize(ref Globals.profit, Globals.coinName.Length);

            try
            {
                
                 Globals.chrome.Navigate().GoToUrl("https://cryptoknight.cc/");
                
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine();
                Console.WriteLine("         Error - Couldn't load webpage: Attempting to restart" + e.ToString());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                try
                {
                    Globals.chrome.Quit();
                    Globals.chrome = new ChromeDriver();
                    Globals.chrome.Navigate().GoToUrl("https://cryptoknight.cc/");
                    Globals.chrome.Manage().Window.Maximize();
                }
                catch (Exception e2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine();
                    Console.WriteLine("         Error - Couldn't load webpage after restarting: " + e2.ToString());
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;

                }
            }


            try
            {
                bool loaded = false;
                while (loaded == false)


                {
                    string rs = Globals.chrome.ExecuteScript("return document.readyState").ToString();
                    if (rs == "complete")
                    {
                        loaded = true;
                    }
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine();
                Console.WriteLine("         Error - Couldn't finish loading webpage: Attempting to read " + e.ToString());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Globals.chrome.Manage().Window.Maximize();
            }




            try
            {
                string source = Globals.chrome.PageSource;
                string a = "return document.body.innerHTML;";
                string strDTSource = Globals.chrome.ExecuteScript(a).ToString();

                var doc = new HtmlDocument();
                doc.LoadHtml(strDTSource);

                var nodes = doc.DocumentNode.SelectNodes("//table//tr");
                var headnodes = doc.DocumentNode.SelectNodes("//table//thead//tr");
                var table = new DataTable("MyTable");

                //var headers = nodes[0]
                //    .Elements("th")
                //    .Select(th => th.InnerText.Trim());

                var headers = headnodes[0]
                    .Elements("td")
                    .Select(td => td.InnerText.Trim());


                foreach (var header in headers)
                {
                    table.Columns.Add(header.ToString().Replace("&nbsp;", ""));
                }


                //for (int i = 0; i < 15; i++)
                //{
                //    table.Columns.Add(i.ToString());
                //}

                var iRow = 0;
                var rows = nodes.Skip(2).Select(tr => tr
                    .Elements("td")
                    .Select(td => td.InnerText.Trim())
                    .ToArray());
                foreach (var row in rows)
                {
                    table.Rows.Add(row);

                    string currentCoin = table.Rows[iRow]["Coin"].ToString();
                    string currentCoinProfit = table.Rows[iRow]["KH/s/day"].ToString();

                    if (currentCoinProfit != "") //issue with last row..
                    {
                        currentCoin = currentCoin.Replace("&nbsp;", "");
                        currentCoinProfit = currentCoinProfit.Substring(0, currentCoinProfit.Length - 2);
                        int i = Array.IndexOf(Globals.coinName, currentCoin);
                        if (i != -1)
                        {
                            Globals.profit[i] = Convert.ToDouble(currentCoinProfit);
                        }
                    }

                    iRow++;
                }

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine();
                Console.WriteLine("         Error - Couldn't read table from webpage: " + e.ToString());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Go();
            }

            Globals.chrome.Quit();


        }

        static void LaunchCommandLineApp()
        {
            Globals.currentlyMining = Globals.mostProfitableCoin;
            int gpucount = 0;
            string gpu = "";
            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in objvide.Get())
            {
                var gpuname = obj["VideoProcessor"];
                if (obj["VideoProcessor"].ToString().Contains("Intel"))
                {
                    //intel = not gpu
                }
                else
                {
                    gpu = gpu + gpucount + ",";
                    gpucount++;

                }

            }

            double diff = Math.Round(Convert.ToDouble(Globals.coinAlgoHash[Convert.ToInt32(Globals.coinAlgo[Globals.mostProfitableCoin])] * 30),0);


            TimeSpan runtime = DateTime.Now - DateTime.Now;
            Globals.lastShareTime = DateTime.Now;
            //DateTime lastShareTime = DateTime.Now;

            double acceptedShares = 0;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            TimeSpan timeSpan = DateTime.Now - DateTime.Now;
            double counter = 1;

            using (Globals.exeProcess = new Process())
            {
                Globals.exeProcess.StartInfo.FileName = "cast_xmr-vega.exe"; ;
                Globals.exeProcess.StartInfo.Arguments = "--algo=" + Globals.coinAlgo[Globals.mostProfitableCoin] + " -S " + Globals.coinPool[Globals.mostProfitableCoin] + " -u " + Globals.coinAddress[Globals.mostProfitableCoin] + "." + diff.ToString() + " -G " + gpu + "--fastjobswitch --ratewatchdog ";
                Globals.exeProcess.StartInfo.UseShellExecute = false;
                Globals.exeProcess.StartInfo.RedirectStandardOutput = true;
                Globals.exeProcess.StartInfo.RedirectStandardError = true;
                Globals.exeProcess.Start();

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    try
                    {
                        Globals.exeProcess.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                try
                                {
                                    outputWaitHandle.Set();
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    runtime = DateTime.Now - Globals.exeProcess.StartTime;
                                }
                                catch
                                {
                                    runtime = DateTime.Now - DateTime.Now;
                                }

                                if (runtime.TotalMinutes > counter)
                                {
                                    Go();
                                    counter++;
                                }

                                //timeSpan = DateTime.Now - lastShareTime;
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                output.AppendLine(e.Data);
                                Console.WriteLine(e.Data);
                                if (e.Data.ToString().Contains("Shares:"))
                                {
                                    var start = e.Data.ToString().IndexOf("Shares:") + 7;
                                    var end = e.Data.ToString().IndexOf("Accepted");
                                    double currentShares = Convert.ToDouble(e.Data.ToString().Substring(start, end - 1 - start));

                                    if (currentShares > acceptedShares)
                                    {

                                        Globals.lastShareTime = DateTime.Now;
                                    }



                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.WriteLine("");
                                    Console.WriteLine("     " + e.Data);
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    Console.WriteLine("     Currently Mining: " + Globals.coinName[Globals.currentlyMining] + " | Total Mining Runtime: " + Math.Round(runtime.TotalMinutes,1) + " mins | Benchmarked Hashrate: " + Math.Round(Globals.coinAlgoHash[Convert.ToInt32(Globals.coinAlgo[Globals.currentlyMining])], 2) + " H/s | $" + Math.Round(Globals.mostProfitableCoinProfit, 2));
                                    Console.WriteLine("");
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                   
                                    acceptedShares = currentShares;

                                }
                            }
                        };
                        Globals.exeProcess.ErrorDataReceived += (sender, e) =>
                        {
                            try
                            {
                                if (e.Data == null)
                                {

                                    errorWaitHandle.Set();
                                }
                                else
                                {
                                    error.AppendLine(e.Data);
                                }
                            }
                            catch
                            {

                            }

                        };


                        Globals.exeProcess.BeginOutputReadLine();
                        Globals.exeProcess.BeginErrorReadLine();


                        while (timeSpan.TotalMinutes < 15)
                        {
                            //Thread.Sleep(60000);
                            timeSpan = DateTime.Now - Globals.lastShareTime;
                        }


                        if (timeSpan.TotalMinutes > 15)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine();
                            Console.WriteLine("         !!ERROR!! NO SHARES IN 15 MINUTES - Killing app starting again");
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Globals.exeProcess.Kill();
                            Globals.currentlyMining = -1;
                            Benchmark();
                            Go();

                        }         

                        if (Globals.exeProcess.WaitForExit(15) &&
                            outputWaitHandle.WaitOne(120) &&
                            errorWaitHandle.WaitOne(120))
                        {
                            // Process completed. Check process.ExitCode here.
                        }
                        else
                        {
                            // Timed out.
                        }
                    }

                    catch
                    {

                    }
                }

            }


        }
        static void RestartGPUs()
        {

            //Need user input and UAC interaction. Leaving for now
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine();
            Console.WriteLine("         Restarting GPUs");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            int gpucount = 0;
            string gpu = "";
            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in objvide.Get())
            {
                var gpuname = obj["VideoProcessor"];
                if (obj["VideoProcessor"].ToString().Contains("Intel"))
                {
                    //intel = not gpu
                }
                else
                {
                    gpu = gpu + gpucount + ",";
                    gpucount++;

                }

            }

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "switch-radeon-gpu.exe";
                process.StartInfo.Arguments = "-G " + gpu + " restart";
                process.StartInfo.UseShellExecute = false;
                process.Start();


            }

        }

        static void TradeOgre()
        {
            string apiKey = "";
            string apiSecret = "";
            //Get damn apis baby
            try
            {
                string[] apis = File.ReadAllLines("TradeOgre.txt");
                var apiCounter = 0;
                foreach (var api in apis)
                {
                    if (api.StartsWith("*") || api.StartsWith("example"))
                    {
                        //ignore
                    }
                    else
                    {
                        if (apiCounter == 0)
                        {
                            apiKey = api;
                        }
                        else if (apiCounter == 1)
                        {
                            apiSecret = api;
                        }
                        apiCounter++;
                    }

                }

            }
            catch
            {

            }

           

            if (apiKey != "")
            {
                Console.WriteLine("");
                Console.WriteLine("TradeOgre Enabled");
                Console.WriteLine("");


                //Cancel all.

                string jsonOrders = POST("https://tradeogre.com/api/v1/account/orders", "", apiKey, apiSecret);
                //JObject jObjOrders = JObject.Parse(jsonOrders);
                JArray AOrders = JArray.Parse(jsonOrders);


                foreach (var orders in AOrders)
                {
                    JToken currentOrder = orders.SelectToken("uuid");
                    String strCurrentOrder = currentOrder.ToString().Substring(0, currentOrder.ToString().Length);
                    Console.WriteLine("Canceling old orders");
                    Console.WriteLine("Canceling: " + strCurrentOrder);
                    string jsonCancel = POST("https://tradeogre.com/api/v1/order/cancel", "uuid=" + strCurrentOrder, apiKey, apiSecret);
                    Console.WriteLine(jsonCancel);
                }

                //Sell

                //Get Current Balances
                string jsonGet = GET("https://tradeogre.com/api/v1/account/balances", apiKey, apiSecret);

                JObject jObject = JObject.Parse(jsonGet);
                JToken bal = jObject.SelectToken("balances");

                Console.WriteLine("");
                Console.WriteLine("Getting Balances");
                foreach (var coinBal in bal)
                {
                    string coin = coinBal.ToString().Substring(1, coinBal.ToString().IndexOf(":") - 2);
                    string currentBalance = coinBal.ToString().Substring(coinBal.ToString().IndexOf(":") + 3, 10);
                    Console.WriteLine(coin + ": " + currentBalance);

                    if (Convert.ToDouble(currentBalance) > 0.0001 && coin.ToString() != "BTC")
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Selling " + coin);
                        string jsonGetOrderBook = GET("https://tradeogre.com/api/v1/orders/BTC-" + coin.ToString(), apiKey, apiSecret);
                        JObject jObjectGetOrderBook = JObject.Parse(jsonGetOrderBook);
                        JToken sells = jObjectGetOrderBook.SelectToken("sell");

                        string bestsell = sells.First.ToString();
                        string strBestMoney = bestsell.Substring(1, bestsell.IndexOf(":") - 2);
                        double bestMoney = Convert.ToDouble(strBestMoney);

                        strBestMoney = Convert.ToString(((bestMoney * 100000000) - 1));
                        int extraZero = 8 - strBestMoney.Length;

                        for (int ii = 0; ii < extraZero; ii++)
                        {
                            strBestMoney = "0" + strBestMoney;

                        }

                        //convert from sat to btc
                        strBestMoney = "0." + strBestMoney;
                        Console.WriteLine("Selling at: " + strBestMoney);

                        string jsonSell = POST("https://tradeogre.com/api/v1/order/sell", "market=btc-" + coin + "&quantity=" + currentBalance + "&price=" + strBestMoney, apiKey, apiSecret);
                        Console.Write(jsonSell.ToString());
                        Console.WriteLine("");
                    }
                }

            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("TradeOgre Disabled, No API keys detected");
                Console.WriteLine("");
            }
        }

        static string GET(string url, string apiKey, string apiSecret)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Credentials = new System.Net.NetworkCredential(apiKey, apiSecret);
            //request.Headers.Add("Authorization", "{" + apiKey + "}:{" + apiSecret + "}");
            string credidentials = apiKey + ":" + apiSecret;
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));



            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        static string POST(string url, string jsonContent, string apiKey, string apiSecret)
        {


            var request = (HttpWebRequest)WebRequest.Create(url);
            var postData = jsonContent;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            string credidentials = apiKey + ":" + apiSecret;
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString.ToString();
          
        }

    }
}
