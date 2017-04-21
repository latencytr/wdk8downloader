﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace wdk8_downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MemoryStream ms = new MemoryStream(wdk8_downloader.Properties.Resources._0))
            {
                using (XmlReader reader = XmlReader.Create(ms))
                {

                    DirectoryInfo dir = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\wdkdownload");

                    while (reader.Read())
                    {
                        if (reader.Name == "Payload" && (reader.GetAttribute("Packaging") != "embedded"))
                        {
                            string filePath = reader.GetAttribute("FilePath");
                            UriBuilder ub = new UriBuilder("http://download.microsoft.com/download/0/8/C/08C7497F-8551-4054-97DE-60C0E510D97A/wdk/" + filePath.Replace("\\", "/"));
                            WebProxy proxy = new WebProxy("localProxyIp:8080", true);
                            proxy.Credentials = new NetworkCredential("username", "password");
                            WebRequest.DefaultWebProxy = proxy;
                            using (WebClient client = new WebClient())
                            {
                                client.Proxy = proxy;
                                string fileName = dir.FullName + "\\" + filePath;
                                if(!Directory.Exists(Path.GetDirectoryName(fileName)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                                }
                                try
                                {
                                    client.DownloadFile(ub.Uri, fileName);
                                }
                                catch (WebException ex)
                                {
                                    Console.WriteLine("File Not Download : {0}\nErrorMessage: {1}\nExceptionDetails: {2}\n\n", fileName, ex.Message, ex.StackTrace);
                                }
                            }
                        }
                    }

                    Console.WriteLine("\n\n\nAll process completed!\n");
                }

            }
        }

        static void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("{0} file downloaded", sender.ToString());
        }
    }
}
