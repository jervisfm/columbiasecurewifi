using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics; 

namespace ColumbiaSecureProxy
{

    /// <summary>
    /// Ensures that the Connection to the CUNIX servers is still valid.
    /// </summary>
    class ConnectionChecker 
    {
        private SecureProxy sw; 
        private Form1 form1;
        private bool notdone = true;
        
        public ConnectionChecker(Form1 form1)
        {
            this.sw = form1.getSecureWifi(); 
            this.form1 = form1;
        }

        /// <summary>
        /// Starts the connections checker. 
        /// </summary>
        public void start()
        {
            Thread t = new Thread(this.doWork);
            t.Start();
            Console.WriteLine("Starting Connection Checking Thread...");
        }


        /// <summary>
        /// Checks if the CUNIX connection is alive in 15 second intervals
        /// </summary>
        private void doWork()
        {
            notdone = true;
            const int SLEEP_TIME = 10 * 1000; // 10 seconds sleep. 
            int c = 0; 
            while (notdone)
            {
                Console.WriteLine(c++); //debugging
                Thread.Sleep(SLEEP_TIME);
                notdone = isAlive(); 
            }

            //Unload Proxy and restore original proxy settings.
            form1.getProxySetting().restore();

            if (notdone == false && !form1.IsDisposed)
            {
                form1.reconnect(); 
                
            }
        }


        public void stop()
        {
            notdone = false; 
        }

        /// <summary>
        /// Checks if the CUNIX connection is still up and running
        /// </summary>
        /// <returns></returns>
        private Boolean isAlive()
        {
            // Write any character to the plink stream. 
            // This will cause the app to quit if the connection
            // has been lost
            Process p = (Process) sw.conn[1];
            //Console.WriteLine("Writing h to plink process..."); // debugging
            
            
            p.StandardInput.Write("h");
            p.StandardInput.Flush();

            Thread.Sleep(100); // wait a bit

            //Check if plink process is still running. 
            if (p.HasExited)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
