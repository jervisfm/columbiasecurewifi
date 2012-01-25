using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Win32;


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
        private String CUNIX_SSH_KEY = "0x94791195523cb216639e398f61506def718141bb5328caa0d521ffdaf366cf043a18100b84e5a83e6c6c694ccd1ecb87f3528b1e7420506fa02fc3f3c4c9f3f0634d02e38b9484b03990bb056989fb04078a962726521101323af701b523c629b4a3afedac9f5967040ea29df69d9765659ba27fc61125743907f6011e91e3ab,0xd2cd4853d928008c3b5fd2caf0c943356c87fc6d,0x285f4f9821fb823153a928e56c1c21dfe5a4074c3d8ab4c93916683936208841e463d4da7155091388b164270019e56c8292716fec637e35754daf09079b2e40358a65a21dd376c0f4d202a05ef85976255dce523d15f60822d51353c1ba773c3d1ca1f8c024a803aa03aac99ecb1e92d65bc72a98350bda751184010bcca3c0,0x4b94da010531be3ebc560873a731290e388d74c2f514260135aac08b7c2788ed27d48a68468528c82db1e57741725e9684a8efbc7c6283f610a968a6f20747461fc515f2c7d394cd4bfc1d7cf62385a2c5316f483474f125b71a2613d76e9cb0f0d0d78148a3e2d765377935016bebf34ad9cac305c03819659a8aaabb591f7e";
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

            loadSSHKeys(); // this loads the CUNIX SSH key if necessary (i.e. if one was not found in windows registry).
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

        /// <summary>
        /// This loads the Public Key for Cunix.Columbia.edu into the registry
        /// </summary>
        public void loadSSHKeys()
        {
            String reg_key = "dss@22:cunix.columbia.edu"; // key name
            String SSH_REG_PATH = @"Software\SimonTatham\PuTTY\SshHostKeys"; // path of where putty stores ssh keys

            RegistryKey reg = Registry.CurrentUser.OpenSubKey(SSH_REG_PATH, true); // This return NULL when the key does not exist. 

            if (reg == null) //  Key path where putty stores SSH keys needs to be created
            {
                reg = Registry.CurrentUser.CreateSubKey(SSH_REG_PATH);
            }
            
            Object val = reg.GetValue(reg_key);
            if (val == null) // No cached SSH key found, so need to insert it to be able to automate connections. 
            {
                reg.SetValue(reg_key, CUNIX_SSH_KEY);
            }   
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


    }
}
