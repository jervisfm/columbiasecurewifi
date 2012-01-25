using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace ColumbiaSecureProxy
{
    /// <summary>
    /// Class that has methods for Manipulating Internet Explorer Proxy Settings. 
    /// </summary>
    public class ProxySetting
    {
        //P/Invoke Code
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        private static bool settingsReturn, refreshReturn;

        private static string KEY = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
        public Int32 OriginalProxyEnable = 0;
        public String OriginalProxyServer = "";

        public ProxySetting() { saveSettings();  }
        
        /// <summary>
        /// Sets the SOCKS Proxy
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public void SetSOCKSProxy(string server, int port){

            saveSettings(); // save previosu proxy settings. 

            string val =  "socks=" + server + ":" + port.ToString(); 

            RegistryKey reg = Registry.CurrentUser.OpenSubKey(KEY, true);
            reg.SetValue("ProxyEnable", 1);
            reg.SetValue("ProxyServer", val);

            //reload so that changes are effective
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

        }

        /// <summary>
        /// Saves current proxy settings into class variables 
        /// OriginalProxyEnable and OriginalProxyServer
        /// </summary>
        public void saveSettings()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(KEY, false);
            OriginalProxyEnable = (Int32)reg.GetValue("ProxyEnable");
            OriginalProxyServer = (String) reg.GetValue("ProxyServer");

            // Update any null objects to equivalent appropriate values
            OriginalProxyServer = (OriginalProxyServer == null) ? "" : OriginalProxyServer;


        }

        /// <summary>
        /// Restore original server settings
        /// </summary>
        public void restore()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(KEY, true);
            reg.SetValue("ProxyEnable", OriginalProxyEnable);
            reg.SetValue("ProxyServer", OriginalProxyServer);

            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

        }

        /// <summary>
        /// Disbales Proxying
        /// </summary>
        public static void Disable()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(KEY, true);
            reg.SetValue("ProxyEnable", 0);
            
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

        }


        
    }



    
}
