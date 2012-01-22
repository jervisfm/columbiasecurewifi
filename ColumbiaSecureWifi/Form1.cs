using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ColumbiaSecureProxy
{
    public partial class Form1 : Form
    {
        private Process proc;
        private String putty;
        private SecureProxy sw; 
        public Form1()
        {
            InitializeComponent();
            this.putty = loadPutty();
            sw = new SecureProxy(putty); 
            Console.Out.WriteLine("Hello");
            this.Disposed += new EventHandler(Form1_Disposed);
            
            //test();
            
        }

        public SecureProxy getSecureWifi()
        {
            return sw;
        }

        public void Form1_Disposed(object sender, EventArgs e)
        {
            Console.WriteLine("Dispose method called...");
            if (proc != null)
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
            
        }

        



        private void test()
        {
            String path = loadPutty();
            SecureProxy sw = new SecureProxy(path);
            string port = Convert.ToString(8080);
            sw.connect("","",port);

            return;
            if (sw.portOpen(446))
            {

                Console.WriteLine("PORT IS OPEN");
            }
            else
            {
                Console.WriteLine("PORT IS CLOSED");
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Extracts putty froom exe image and saves it into a temporary file
        /// </summary>
        /// <returns> the file into which putty was unloaded into.
        /// </returns>
        private String loadPutty()
        {

            String tmpDir = Path.GetTempPath();
            String path = Path.Combine(tmpDir, "plinkcolumbia.exe");

            // Unload putty exe into path
            byte[] putty = ColumbiaSecureProxy.Properties.Resources.plink;
            try
            {
                File.WriteAllBytes(path, putty);
            }
            catch (IOException)
            {
                Console.Out.WriteLine("Putty File already exists and is in use" + path); 
            }
            Console.Out.WriteLine("Saved putty to file - " + path); 
            return path;
            
        }


        public void updateServer(String s)
        {
            Action a = delegate()
            {
                textBox_Server.Text = s;
            };
            BeginInvoke(a);
        }

        /// <summary>
        /// Thread safe update status textbox method
        /// </summary>
        /// <param name="s"></param>
        public void updateStatus(String s)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new updateStatusDlg(pUpdateStatus),s);
            }
            else
            {
                pUpdateStatus(s);
            }
        }

        /// <summary>
        /// Re-connects to CUNIX after a connection is lost. 
        /// </summary>
        public void reconnect()
        {
            Action a = delegate()
            {
                Console.WriteLine("reconnect method called");
                textBox_Server.Text = "";
                textBox_status.Text = "Connection Lost. Reconnecting ...";
                Console.WriteLine("connect button clicked");

                textBox_Password.ReadOnly = false;
                textBox_Username.ReadOnly = false; 

                //setCredsReadOnly(false); 

                btnConnect_Click(null, null);
            };

            runOnUI(a); 
        }

        /// <summary>
        /// Runs the given action on the UI Thread and in a threadsafe way.
        /// </summary>
        /// <param name="a"></param>
        private void runOnUI(Action a)
        {
            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.BeginInvoke(null, null); 
            }
        }

        /// <summary>
        /// Updates the UI to indicate that connection has been disconnected
        /// </summary>
        public void disconnect()
        {
            Action a = delegate()
            {
                // Re-enable connect button
                btnConnect.Visible = true;
                btnConnect.Enabled = true;
 
                // Update the status info. 
                textBox_Server.Text = "";
                textBox_status.Text = "Connection Lost. Please Re-connect";
            };
            
            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.BeginInvoke(null, null);
            }
        }

        /// <summary>
        /// Sets the user credentials textbox to readonly
        /// </summary>
        /// <param name="readOnly"></param>
        private void setCredsReadOnly(bool readOnly)
        {
            Action a = delegate()
            {
                textBox_Password.ReadOnly = readOnly;
                textBox_Username.ReadOnly = readOnly; 
            };
            runOnUI(a); 
        }

        /// <summary>
        /// Thread safe hide method for Connect
        /// </summary>
        /// <param name="hide"></param>
        private void hideConnectBtn(bool hide)
        {
            Action a = delegate()
            {
                btnConnect.Visible = !hide;
                btnConnect.Enabled = true;
            };
            if (InvokeRequired)
            {

                BeginInvoke(a);

            }
            else
            {
                a.BeginInvoke(null,null);
            }
        }


        private delegate void updateStatusDlg(String s); 
        // Update label method meant to be from Form's Main thread only
        public void pUpdateStatus(String s)
        {
            textBox_status.Text = s; 
        }

        private void connectCallback(IAsyncResult result)
        {
            
            Console.WriteLine("YEAAH , we got the callback");
            if (sw.IsConnected())
            {
                Console.WriteLine("things look good");
                String msg = "Connected to CUNIX. ";
                String msg2 = "localhost : " + sw.port;
                updateStatus(msg);
                updateServer(msg2); 
                hideConnectBtn(true);

                //Set Credentials to readonly
                setCredsReadOnly(true);

                //Start the Connection checker. 
                ConnectionChecker cc = new ConnectionChecker(this);
                cc.start(); 

            }
            else
            {
                
                updateStatus("Connection attempt failed. Please try again");
                Console.WriteLine("Oohoh, Connection FAILED");
                hideConnectBtn(false);
            }
        }

        private delegate Boolean connectDlg(string user, string pass, string port);
        private Boolean wConnect(String user, String pass, String port)
        {            
            Object[] arr = sw.connect(user, pass, port);
            proc = (Process) arr[1]; 
            return (Boolean) arr[0];
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            String user = textBox_Username.Text;
            String pass = textBox_Password.Text;
            String port = "8080"; //try a default port of 8080
            

            textBox_status.Text = "Connecting ...";
            btnConnect.Enabled = false;

            connectDlg cDlg = new connectDlg(wConnect);
            IAsyncResult ias = cDlg.BeginInvoke(user, pass, port, new AsyncCallback(connectCallback), null);
            
            
            
        }
    }
}
