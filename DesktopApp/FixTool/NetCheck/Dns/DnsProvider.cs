using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace NetCheck.Dns
{
    public class DnsProvider 
    {
        public static string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        // Use Google public DNS as default: http://code.google.com/speed/public-dns/docs/using.html
        public static readonly IPAddress[] DefaultDnsServers =
        {
            IPAddress.Parse("8.8.8.8"),
            IPAddress.Parse("8.8.4.4")
        };

        public static List<IPAddress> SystemDnsServers()
        {
            // ditching the old method of reading in the registry for using the .net way
            //    old: http://technet.microsoft.com/en-us/library/cc962470.aspx
            //    new: http://msdn.microsoft.com/en-us/library/system.net.networkinformation.ipinterfaceproperties.dnsaddresses.aspx

            List<IPAddress> servers = new List<IPAddress>();
            NetworkInterface[] intfs = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface intf in intfs)
            {
                if (intf.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties intProps = intf.GetIPProperties();
                    foreach (IPAddress ip in intProps.DnsAddresses)
                    {
                        if (!servers.Contains(ip))
                            servers.Add(ip);
                    }
                }
            }

            return servers;
        }

        public static string IpToArpa(IPAddress ip)
        {
            StringBuilder s = new StringBuilder();
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                foreach (byte b in ip.GetAddressBytes())
                    s.Insert(0, string.Format("{0}.", b));
                s.Append("in-addr.arpa.");
            }
            else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                foreach (byte b in ip.GetAddressBytes())
                    s.Insert(0, string.Format("{0:x}.{1:x}.", (b >> 4) & 0xf, (b >> 0) & 0xf));
                s.Append("ip6.arpa.");
            }

            return s.ToString();
        }

        public static string ProtocolName(int assignedNumber)
        {
            string name = assignedNumber.ToString();  // default name
            string dataBasePath = string.Empty;
            
            try
            {
                dataBasePath = (string)Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters", false).GetValue("DataBasePath");
            }
            catch
            {
                // if we can't access the registry value, just exit returning the default value
                return name;
            }

            if (File.Exists(dataBasePath + "\\protocol"))
            {
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(dataBasePath + "\\protocol");
                    string line;
                    string[] items;
                    while (sr.Peek() > -1)
                    {
                        line = sr.ReadLine().Trim();
                        if (line.Length > 0 && line[0] != '#')
                        {
                            items = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (Convert.ToInt32(items[1]) == assignedNumber)
                                name = items[0];
                        }
                    } // if we get through the entire file without finding a matching entry, then exit returning the default name set earlier
                    sr.Close();
                }
                catch
                {
                    // we don't care what the error was really, just return the default name set earlier
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                }
            }

            return name;
        }

        public static string ServiceName(int assignedNumber, int protocol)
        {
            return ServiceName(assignedNumber, DnsProvider.ProtocolName(protocol));
        }

        public static string ServiceName(int assignedNumber, string protocol)
        {
            string name = assignedNumber.ToString();  // default name
            string dataBasePath = string.Empty;
            
            try
            {
                dataBasePath = (string)Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters", false).GetValue("DataBasePath");
            }
            catch
            {
                // if we can't access the registry value, just exit returning the default value
                return name;
            }
            
            if (File.Exists(dataBasePath + "\\services"))
            {
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(dataBasePath + "\\services");
                    string line;
                    string[] items;
                    while (sr.Peek() > -1)
                    {
                        line = sr.ReadLine().Trim();
                        if (line.Length > 0 && line[0] != '#')
                        {
                            items = line.Split(new char[] { ' ', '\t', '/' }, StringSplitOptions.RemoveEmptyEntries);
                            if (items.Length >= 2)
                                if (Convert.ToInt32(items[1]) == assignedNumber && items[2] == protocol)
                                    name = items[0];
                        }
                    } // if we get through the entire file without finding a matching entry, then exit returning the default name set earlier
                    sr.Close();
                }
                catch
                {
                    // we don't care what the error was really, just return the default name set earlier
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                }
            }

            return name;
        }
    }

}
