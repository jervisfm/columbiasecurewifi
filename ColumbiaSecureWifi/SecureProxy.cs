using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;


namespace ColumbiaSecureProxy
{
    /// <summary>
    /// Creates a SOCKS Proxy that tunnels traffic through CUNIX
    /// </summary>
    public class SecureProxy
    {
        private String putty;
        private String user;
        private String pass;
        private Boolean connected = false;
        public Object[] conn; // first element is a boolean of connection status, 2nd element has Process info for started process. 

        public int port; // Port Number that the server will listen on. 
        public SecureProxy(String f)
        {
            if (File.Exists(f))
            {
                this.putty = f;
            }
            else
            {
                
                this.putty = null;
            }
        }

        public bool IsConnected()
        {
            return connected; 
        }


        /// <summary>
        /// Connects to the Columbia SSH Servers and setups up SOCKS proxy
        /// forwarding on the given local port
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>A 2 element Object array. 
        ///          1st element contains connection success boolean value
        ///          2nd element contains Process object for the putty process that had been started. 
        ///  </returns>
        public Object[] connect(String username, String password, String port)
        {
            // save credentials
            this.user = username;
            this.pass = password;
            
            // Check if given port is not available. If so, choose another one randomly. 
            if (!portOpen(Convert.ToInt32(port)))
            {
                int newPort = getRandomOpenPort();
                this.port = newPort;
                port = newPort.ToString();
            }
            else // use it
            {
                this.port = Convert.ToInt32(port); 
                
            }

            Console.WriteLine("Using POrt " + port);

            /// The Putty Command to auto-connect and setup forwarding w/o starting a shell and disabling interactivity is 
            /// putty -batch -N ssh [UNI]@cunix.columbia.edu -pw [password] -D [localport] 

            String args = "-batch -ssh " + username + "@cunix.columbia.edu -pw " + password + " -D " + port  ;
            Process proc = new Process();
            proc.StartInfo.Arguments =args;
            
            proc.StartInfo.FileName = putty;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.EnableRaisingEvents = true;
            
            //BOTH input/output MUST be redirected, otherwise, process will crash
            // soon after starting. This looks like a bug in Microsoft's code 
            // http://social.msdn.microsoft.com/Forums/en/netfxbcl/thread/4ce40b19-c442-4412-94b2-1ed24bdb9386
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true; // redirect console output
            string s;
            
            
            Console.WriteLine("About to start putty");
            proc.Start();
            
            Console.WriteLine("Started Putty");

            int count = 10 ; // 20 seconds (10 * 2  ) timeout
            DateTime start = DateTime.Now;
            while (!proc.HasExited)
            {
                if (connectionSuccessfully(this.port))
                {
                    this.connected = true;
                    break;
                }
                
                if (count == 0) // reached connect timeout
                {
                    if (connectionSuccessfully(this.port)) // check one last time
                    {
                        this.connected = true;
                        break;
                    }
                    else
                    {
                        this.connected = false;
                        break;
                    }
                }
                Thread.Sleep(2000); // sleep for 2 second
                Console.WriteLine(count);
                count--;
            }

            TimeSpan diff = DateTime.Now.Subtract(start);
            Console.WriteLine("Connction took this many seconds : " + diff.TotalSeconds);

            if (count == 0)
            {
                Console.WriteLine("Counter reached zero !!");
            }

            // double check to make sure that connection is still alive. 
            if (proc.HasExited)
            {
                this.connected = false;
            }


            
            //debugging
            if (connected)
            {
                Console.WriteLine("Connection was successful");
            }
            else
            {
                Console.WriteLine("Connection FAILED"); 
            }

            Object[] arr = new Object[2];
            arr[0] = connected;
            arr[1] = proc;

            return this.conn = arr  ; 
            
            
            
            /*
            proc.WaitForExit();
            if (proc.StartInfo.RedirectStandardOutput)
            {
                s = proc.StandardOutput.ReadToEnd();
                Console.Out.WriteLine("s0 - " + s);

                //s = proc.StandardOutput.ReadLine();
                //Console.Out.WriteLine("s1 - " + s);

                //s = proc.StandardOutput.ReadLine();
                //Console.Out.WriteLine("s1 - " + s);
            }*/


            //To DO: need another thread so see if access was denied due to wrong password. 

            //string output = proc.StandardOutput.ReadToEnd();
            //Console.WriteLine("Output : \n" + output);
            //proc.WaitForExit();


        }

        /// <summary>
        /// Returns a random local port (between 10,000  - 50,000) that is available for a
        /// a server to listen on. 
        /// </summary>
        /// <returns></returns>
        public int getRandomOpenPort()
        {

            Boolean available = false;
            int port = 0;
            while (!available)
            {
                port  = getRandomPort();
                if (portOpen(port))
                {
                    available = true;
                    break;
                }
            }
            return port;

        }

        /// <summary>
        /// Helper method that Returns a random port between 10,000 and 50,000
        /// </summary>
        /// <returns></returns>
        private int getRandomPort()
        {
            Random rand = new Random();
            return rand.Next(10000, 50000);
         
        }

        private void printTCP(TcpConnectionInformation conn)
        {
            Console.Write(conn.LocalEndPoint.Address.ToString() + ":" + conn.LocalEndPoint.Port.ToString());
            Console.Write(" | ");
            Console.Write(conn.RemoteEndPoint.Address.ToString() + ":" + conn.RemoteEndPoint.Port.ToString());
            Console.WriteLine();
        }


        /// <summary>
        /// Determines if the SOCKS proxy has been successfully started at the given port. 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public Boolean connectionSuccessfully(int port)
        {
            return !portOpen(port);
        }

        /// <summary>
        /// Checks if given port is open
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public Boolean portOpen(int port)
        {

            // Using the .NET equivalent of netstat commandline tool 
            bool available = true;

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            
            IPEndPoint [] ips = ipGlobalProperties.GetActiveTcpListeners();
            foreach (IPEndPoint ip in ips)
            {
                //printTCP(tcpconn);
                
                if (ip.Port == port)
                {
                    available = false;
                    break;
                }
            }

            return available;

        }

        public static void threadtest()
        {
            Process p = new Process();
            p.StartInfo.FileName = "ping";
            p.StartInfo.Arguments = "google.com";
            p.Exited += new EventHandler(p_Exited);

            

            p.Start();

            while (!p.HasExited)
            {
                Thread.Sleep(1000);
            }


        }

        

        public static void p_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("The command has been FINISHED running and exited");
        }

        public  void run()
        {

            
        }
        




    }
}
