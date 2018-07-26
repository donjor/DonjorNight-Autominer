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

namespace Donjornight_Autominer
{
    class Program
    {
        static void Main(string[] args)
        {
            Go();
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 5 seconds.
            aTimer.Interval = 300000;
            aTimer.Enabled = true;

            Console.WriteLine("Press \'z\' to quit");
            while (Console.Read() != 'z') ;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Go();
        }
        static void Go()
        {
            TimeSpan runtime;
            try
            {
                runtime = DateTime.Now - Globals.exeProcess.StartTime;
            }
            catch
            {
                runtime = DateTime.Now - DateTime.Now;
            }
            GetCurrentProfit();
            int coinCount = 0;
            //determine most profitable coin

            foreach (var coinProfit in Globals.profit)
            {
                if (Globals.mostProfitableCoin == -1)
                {
                    Globals.mostProfitableCoin = 0;
                }
                try
                {
                    if (coinProfit > Globals.profit[Globals.mostProfitableCoin])
                    {
                        Globals.mostProfitableCoin = coinCount;
                    }

                }
                catch
                {
                    Console.WriteLine("error getting profit for " + Globals.coins[coinCount]);
                }
                coinCount++;
            }

            //print out nonsense
            coinCount = 0;
            Console.WriteLine();
            foreach (var coin in Globals.coins)
            {
                Console.WriteLine(coin + " profit: " + Globals.profit[coinCount]);
                coinCount++;

            }
            Console.WriteLine();
            Console.WriteLine("Most Profitable Coin: " + Globals.coins[Globals.mostProfitableCoin]);
            Console.WriteLine("Profit: " + Globals.profit[Globals.mostProfitableCoin]);



            //determine if we should switch
            //not mining so go.
            if (Globals.currentlyMining == -1)
            {
                Console.WriteLine();
                Console.WriteLine("NOT CURRENTLY MINING SO MINING START");
                Console.WriteLine();
                //start mining
                LaunchCommandLineApp();
            }
            //if not mining the most profitable coin
            else if (Globals.currentlyMining != Globals.mostProfitableCoin && runtime.TotalMinutes > 60)
            {
                Console.WriteLine();
                Console.WriteLine("NEW COIN MORE PROFITABLE - SWITCHING");
                Console.WriteLine();

                //kill current
                Globals.exeProcess.Kill();
                //switch
                LaunchCommandLineApp();
            }
            else if (Globals.profit[Globals.currentlyMining] * 1.2 < Globals.profit[Globals.mostProfitableCoin])
            {
                Console.WriteLine();
                Console.WriteLine("SWITCHING EARLY > " + Globals.profit[Globals.mostProfitableCoin] / Globals.profit[Globals.currentlyMining] + " % PROFIT!");
                Console.WriteLine();

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
                Console.WriteLine("NOT SWITCHING");
                Console.WriteLine("Elapsed time: " + runtime.TotalMinutes.ToString() + " Mins");

                Console.WriteLine("MINING: " + Globals.coins[Globals.currentlyMining]);
                Console.WriteLine("MINING: " + Globals.coins[Globals.currentlyMining]);
                Console.WriteLine("MINING: " + Globals.coins[Globals.currentlyMining]);
                Console.WriteLine("MINING: " + Globals.coins[Globals.currentlyMining]);

                //Console.WriteLine("**test every coin**");
                //Console.WriteLine("**test every coin**");
                //Console.WriteLine("**test every coin**");
                //Console.WriteLine("**test every coin**");
                //Console.WriteLine("**test every coin**");
                //Console.WriteLine("**test every coin**");
                ////test every coin
                //Globals.mostProfitableCoin = 7;
                //Globals.counter++;
                ////kill current
                //try
                //{
                //    Globals.exeProcess.Kill();
                //}
                //catch { }
                ////switch
                //LaunchCommandLineApp();
            }


        }


        public static class Globals
        {
            public static int currentlyMining = -1;
            public static int mostProfitableCoin = -1;
            public static double[] profit = new double[8];
            public static string[] coins = new string[]
                {
                    "DERO",
                    "SUMO",
                    "XTL",
                    "LOKI",
                    "XRN",
                    "RYO",
                    "XHV",
                    "MSR"
                };
            public static int[] algo = new int[]
                {
                    0,
                    0,
                    6,
                    2,
                    2,
                    2,
                    7,
                    8
                };
            public static string[] pool = new string[]
                {
                    "dero.miner.rocks:5555",
                    "sumokoin.miner.rocks:5555",
                    "stellite.miner.rocks:4005",
                    "loki.miner.rocks:5555",
                    "saronite.miner.rocks:5555",
                    "ryo.miner.rocks:5555",
                    "haven.miner.rocks:4005",
                    "masari.miner.rocks:5555"
                };
            public static string[] address = new string[]
                {
                    "dERirD3WyQi4udWH7478H66Ryqn3syEU8bywCQEu3k5ULohQRcz4uoXP12NjmN4STmEDbpHZWqa7bPRiHNFPFgTBPmcBcJSCPAe16v9ceFz27",
                    "SumipDETyjLYi8rqkmyE9c4SftzYzWPCGA3XvcXbGuBYcqDQJWe8wp8NEwNicFyzZgKTSjCjnpuXTitwn6VdBcFZEFXLcTHGMrpio9Z3Bzwma6",
                    "SEiStP7SMy1bvjkWc9dd1t2v1Et5q2DrmaqLqFTQQ9H7JKdZuATcPHUbUL3bRjxzxTDYitHsAPqF8EeCLw3bW8ARe8rYc1at38e5hPuG1Pnj2",
                    "LK8CGQ17G9R3ys3Xf33wCeViD2B95jgdpjAhcRsjuheJ784dumXn7g3RPAzedWpFq364jJKYL9dkQ8mY66sZG9BiCtqCgP7UKp87yTaC9E",
                    "P2PiphQEwi9Ld4fJiSJ7badzM9it8boTeXFStL5jbSiKVprzXp2ngYqS19TCgwaGU2iU1WSHzJEayHv1k5fAsVs4HsSEBA5ibh92SmhDUDYGn",
                    "SumipDETyjLYi8rqkmyE9c4SftzYzWPCGA3XvcXbGuBYcqDQJWe8wp8NEwNicFyzZgKTSjCjnpuXTitwn6VdBcFZEFXLcTgndowUG3B8AQrygV",
                    "hvi1aCqoAZF19J8pijvqnrUkeAeP8Rvr4XyfDMGJcarhbL15KgYKM1hN7kiHMu3fer5k8JJ8YRLKCahDKFgLFgJMYAfnFbkERic2irNC1zEup",
                    "5t5mEm254JNJ9HqRjY9vCiTE8aZALHX3v8TqhyQ3TTF9VHKZQXkRYjPDweT9kK4rJw7dDLtZXGjav2z9y24vXCdRc3AszuV5ApF8Kapsvn"
                };
            public static string[] hashrate = new string[]
            {
                "850", //v7
                "850",
                "850",
                "850", //heavy
                "850",
                "850",
                "850",
                "1600", //light

            };
            public static ProcessStartInfo startInfo = new ProcessStartInfo();
            public static Process exeProcess;
            public static ChromeDriver chrome = new ChromeDriver();
            public static int counter = 0;
        }

        static void GetCurrentProfit()
        {

            //hashrates
            string cn = "7";
            string fast = "14";
            string heavy = "7";
            string lite = "14";
            string cnv7 = "7";


            //Cookie ck = new Cookie("estimateHashrate", "2");
            //chrome.Manage().Cookies.AddCookie(ck);

            if (Globals.mostProfitableCoin == -1)
            {
                Globals.chrome.Navigate().GoToUrl("https://miner.rocks/");
                Globals.chrome.ExecuteScript("window.localStorage.setItem('estimateHashrate', '" + cn + "');");
                Globals.chrome.ExecuteScript("window.localStorage.setItem('estimateHashrateFast', '" + fast + "');");
                Globals.chrome.ExecuteScript("window.localStorage.setItem('estimateHashrateHeavy', '" + heavy + "');");
                Globals.chrome.ExecuteScript("window.localStorage.setItem('estimateHashrateLiteV7', '" + lite + "');");
                Globals.chrome.ExecuteScript("window.localStorage.setItem('estimateHashrateV7', '" + cnv7 + "');");
                Globals.chrome.Navigate().GoToUrl("https://miner.rocks/");

            }

            bool loaded = false;
            while (loaded == false)
            {
                string jq = Globals.chrome.ExecuteScript("return jQuery.active").ToString();
                string rs = Globals.chrome.ExecuteScript("return document.readyState").ToString();
                if (jq == "1" & rs == "complete")
                {
                    loaded = true;
                }


            }

            string source = Globals.chrome.PageSource;
            string a = "return document.body.innerHTML;";
            string strDTSource = Globals.chrome.ExecuteScript(a).ToString();

            var doc = new HtmlDocument();
            doc.LoadHtml(strDTSource);

            var nodes = doc.DocumentNode.SelectNodes("//table//tr");
            var table = new DataTable("MyTable");

            //var headers = nodes[0]
            //    .Elements("th")
            //    .Select(th => th.InnerText.Trim());
            //foreach (var header in headers)
            //{
            //    table.Columns.Add(header);

            //
            for (int i = 0; i < 15; i++)
            {
                table.Columns.Add(i.ToString());
            }

            var iRow = 0;
            var rows = nodes.Skip(1).Select(tr => tr
                .Elements("td")
                .Select(td => td.InnerText.Trim())
                .ToArray());
            foreach (var row in rows)
            {
                table.Rows.Add(row);
                string currentCoin = table.Rows[iRow]["12"].ToString();
                string currentCoinProfit = table.Rows[iRow]["14"].ToString();

                int i = Array.IndexOf(Globals.coins, currentCoin);
                if (i != -1)
                {
                    Globals.profit[i] = Convert.ToDouble(currentCoinProfit.Substring(1, currentCoinProfit.Length - 1));
                }

                iRow++;
            }

        }

        static void LaunchCommandLineApp()
        {
            Globals.currentlyMining = Globals.mostProfitableCoin;
            int gpucount = 0;
            string gpu = "";
            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in objvide.Get())
            {
                gpu = gpu + gpucount + ",";
                gpucount++;

            }

            // because rig has intel..
            //gpu = gpu.Substring(0, gpu.Length - 2);


            //Process resetgpu =  Process.Start("resetgpu.bat");
            //resetgpu.WaitForExit();

            string ex1 = AppDomain.CurrentDomain.BaseDirectory;
            //rig
            double diff = Convert.ToDouble(Globals.hashrate[Globals.mostProfitableCoin]) * (gpucount + 1) * 30;

            //main
            diff = Convert.ToDouble(Globals.hashrate[Globals.mostProfitableCoin]) * (gpucount + 1) * 30;

            // Use ProcessStartInfo class
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            Globals.startInfo.CreateNoWindow = false;
            Globals.startInfo.UseShellExecute = false;
            Globals.startInfo.FileName = "cast_xmr-vega.exe";
            Globals.startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //startInfo.Arguments = "-f j -o \"" + ex1 + "\" -z 1.0 -s y ";
            Globals.startInfo.Arguments = "--algo=" + Globals.algo[Globals.mostProfitableCoin] + " -S " + Globals.pool[Globals.mostProfitableCoin] + " -u " + Globals.address[Globals.mostProfitableCoin] + "." + diff.ToString() + " -G " + gpu;

            //reset gpus

            try
            {
                Globals.exeProcess = Process.Start(Globals.startInfo);
            }
            catch
            {
                // Log error.
            }

        }

    }
}
