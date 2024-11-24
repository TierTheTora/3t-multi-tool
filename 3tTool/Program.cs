using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3tTool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Console.Clear();
            bool run = true;

            while (run)
            {
                Console.Title = "3tTool";
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write("    ██████╗ ████████╗████████╗ ██████╗  ██████╗ ██╗     \r\n");
                Console.Write("    ╚════██╗╚══██╔══╝╚══██╔══╝██╔═══██╗██╔═══██╗██║     \r\n");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("     █████╔╝   ██║      ██║   ██║   ██║██║   ██║██║     \r\n");
                Console.Write("     ╚═══██╗   ██║      ██║   ██║   ██║██║   ██║██║     \r\n");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("    ██████╔╝   ██║      ██║   ╚██████╔╝╚██████╔╝███████╗\r\n");
                Console.Write("    ╚═════╝    ╚═╝      ╚═╝    ╚═════╝  ╚═════╝ ╚══════╝\r\n\r\n");
                Console.ResetColor();
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Network info");
                Console.WriteLine("2 - Send message via webhook");
                Console.WriteLine("3 - System info");
                Console.WriteLine("4 - System diagnostics");

                Console.Write("  > ");

                ConsoleKeyInfo k = Console.ReadKey();
                char p = k.KeyChar;

                switch (p)
                {
                    case '0':
                        run = false;
                        break;
                    case '1':
                        await ni();
                        break;
                    case '2':
                        await sm();
                        break;
                    case '3':
                        await si();
                        break;
                    case '4':
                        await sd();
                        break;
                }
            }
        }
        static async Task ni()
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "ni.bat");

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("_3tTool.ni.bat"))
                {
                    if (stream == null)
                    {
                        foreach (var resource in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                        {
                            Console.WriteLine(resource);
                        }
                        return;
                    }

                    using (FileStream fileStream = new FileStream(tempPath, FileMode.Create))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                }))
                {
                    process.WaitForExit();
                }

                File.Delete(tempPath);
            }
            catch (Exception ex) { }
        }
        static async Task sm()
        {
            Console.Clear();
            Console.Title = "3tTool - Send Message via Webhook";

            Console.WriteLine("(\\0 to exit)\n");

            Console.Write("URL: ");
            string webhook = Console.ReadLine();

            if (webhook != "\\0")
            {
                Console.Write("MSG: ");
                string message = Console.ReadLine();

                string json = $"{{\"content\": \"{message}\"}}";

                if (message != "\\0")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = await client.PostAsync(webhook, content);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Message sent successfully.");
                                Console.Write("\nPress any key to continue . . . ");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
                                Console.Write("\nPress any key to continue . . . ");
                                Console.ReadKey();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending message: {ex.Message}");
                            Console.Write("\nPress any key to continue . . . ");
                            Console.ReadKey();
                        }
                    }
                    await sm();
                }
            }
        }
        static async Task si()
        {

            Console.Title = "3tTool - System info";
            Console.Clear();

            Console.WriteLine("\tUserName:       " + Environment.UserName);
            Console.WriteLine("\tMachineName:    " + Environment.MachineName);
            Console.WriteLine("\tUserDomainName: " + Environment.UserDomainName);
            Console.WriteLine("\tOSVersion:      " + Environment.OSVersion.ToString());
            Console.WriteLine("\tVersion:        " + Environment.Version.ToString());

            Console.Write("Press any key to continue . . . ");

            Console.ReadKey();
        }
        static async Task sd()
        {

            Console.Title = "3tTool - System diagnostics";
            Console.Clear();
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            Console.WriteLine("\tCPU Usage:        " + cpuCounter.NextValue() + " %");

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("\tCPU Usage:        " + cpuCounter.NextValue() + " %");

            int uptime = Environment.TickCount;

            TimeSpan uptimeSpan = TimeSpan.FromMilliseconds(uptime);
            if (uptimeSpan.Days < 15)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("\tSystem Uptime:    " + $"{uptimeSpan.Days} days, {uptimeSpan.Hours} hours, {uptimeSpan.Minutes} minutes, {uptimeSpan.Seconds} seconds");

            Console.ResetColor();

            if ((Environment.WorkingSet / 1048576) < 30)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tWorkingSet:       " + Environment.WorkingSet + " Bytes");

            Console.ResetColor();
            Console.WriteLine("\tProcessorCount:   " + Environment.ProcessorCount);

            PerformanceCounter memCounter = new PerformanceCounter("Memory", "Available MBytes");
            Console.WriteLine("\tAvailable Memory: " + memCounter.NextValue() + " MegaBytes");

            PerformanceCounter diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
            PerformanceCounter diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

            if (diskReadCounter.NextValue() / 1048576 >= 500)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tDisk Read Speed:  " + diskReadCounter.NextValue() + " Bytes/sec");

            Console.ResetColor();

            if (diskReadCounter.NextValue() / 1048576 >= 500)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tDisk Write Speed: " + diskWriteCounter.NextValue() + " Bytes/sec");

            Console.ResetColor();

            System.Threading.Thread.Sleep(1000);
            if (diskReadCounter.NextValue() / 1048576 >= 500)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tDisk Read Speed:  " + diskReadCounter.NextValue() + " Bytes/sec");
            Console.ResetColor();

            if (diskReadCounter.NextValue() / 1048576 >= 500)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tDisk Write Speed: " + diskWriteCounter.NextValue() + " Bytes/sec");
            Console.ResetColor();

            Console.Write("Press any key to continue . . . ");

            Console.ReadKey();
        }
    }
}