using System;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace Wifinity
{
    public class Program()
    {
        public static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "-h")
                {
                    Help();

                    Environment.Exit(0);
                }
            }
            else
            {
                ConsoleKey key = Menu();

                switch (key)
                {
                    case ConsoleKey.D1:
                        netList();
                        break;
                    case ConsoleKey.D2:
                        netConnect();
                        break;
                }
            }
        }

        public static void Help()
        {
            Console.WriteLine("Wifinity - NMCLI for Dummies like Me");
            Console.WriteLine("Usage:");
            Console.WriteLine("  Wifinity             : Launch interactive menu");
            Console.WriteLine("  Wifinity -h          : Show this help message");

            Console.WriteLine("[BY IRON]");
        }

        public static ConsoleKey Menu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
 __    __ __  ____ __ __  __ __ ______ _  _
 ||    || || ||    || ||\ || || | || | \\//
 \\ /\ // || ||==  || ||\\|| ||   ||    )/ 
  \V/\V/  || ||    || || \|| ||   ||   //  
                                           ");

            printOp("1. ", "List Networks");
            printOp("2. ", "Connect To Network");
            printOp("3. ", "Disconnect from Network");
            printOp("4. ", "Save Network [WIP]");
            return Console.ReadKey().Key;
        }

        public static void printOp(string a, string b)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(a);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(b);
        }

        public static void netList()
        {
            string nmcli = command("nmcli", "device wifi list");

            if (string.IsNullOrEmpty(nmcli))
            {
                Console.ForegroundColor= ConsoleColor.Green;
                Console.WriteLine("No Networks found or Error Occured");

                Environment.Exit(0);
            }

            string[] networks = nmcli.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("----------");
            foreach (string network in networks)
            {
                string[] info = network.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (info.Length < 8)
                {
                    continue;
                }

                string ssid = info[0];
                string str = info[5];
                string security = info.Length > 7 ? info[7] : "Open";

                printOp("SSID: ", ssid);
                printOp("Signal Strength", str + "%");
                printOp("Security: ", security);
                Console.WriteLine("----------");
            }

            

        }


        public static string command(string command, string args)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();


            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();


            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {error}");
            }

            return output;

        }


        public static void netConnect()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Insert SSID: ");
            Console.ForegroundColor = ConsoleColor.White;
            string ssid = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Enter the password (leave blank for open networks): ");
            Console.ForegroundColor = ConsoleColor.White;
            string password = Console.ReadLine();

            string args = $"device wifi connect \"{ssid}\"";
            if (!string.IsNullOrEmpty(password))
            {
                args += $"device wifi connect \"{ssid}\"";
            }

            string res = command("nmcli", args);

            if (res.Contains("successfully activated"))
            {
                Console.ForegroundColor= ConsoleColor.Green;
                Console.Write("Successfully connected to ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(ssid);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("!");
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Failed to connect to ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(ssid);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!");
            }
        }

        public static void netDisconnect()
        {
            string activeInterface = command("nmcli", "-t -f DEVICE,TYPE device status | grep wifi | cut -d: -f1");

            if (string.IsNullOrEmpty(activeInterface))
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("No active Wi-Fi connection found!");
                Environment.Exit(0);
            }

            string result = command("nmcli", $"device disconnect {activeInterface}");

            if (result.Contains("succesfully disconnected"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Succesfully Disconnected from Network.");
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {result}");
            }
        }




    }
}