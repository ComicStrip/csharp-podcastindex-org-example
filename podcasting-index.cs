using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
// needed specifically for this example: 
using System.Security.Cryptography;
using System.IO;

namespace server_test
{
    class program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("running...");
            //Required values
            // WARNING: don't publish these to public repositories or in public places!
            // NOTE: values below are sample values, to get your own values go to https://api.podcastindex.org 
            string apiKey = "ABC"; 
            string apiSecret = "ABC";
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int apiHeaderTime = (int)t.TotalSeconds;

            //Hash them to get the Authorization token
            string hash = "";
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hashed = sha1.ComputeHash(Encoding.UTF8.GetBytes(apiKey + apiSecret + apiHeaderTime));
                var sb = new StringBuilder(hashed.Length * 2);

                foreach (byte b in hashed)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                hash = sb.ToString();
            }

            //Create the web request and add the required headers
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.podcastindex.org/api/1.0/search/byterm?q=bastiat");
            // not working in mono: request.Headers.Add("User-Agent", "SuperPodcastPlayer/1.8");
            request.UserAgent = "SuperPodcastPlayer/1.8";
            request.Headers.Add("X-Auth-Date", apiHeaderTime.ToString());
            request.Headers.Add("X-Auth-Key", apiKey);
            request.Headers.Add("Authorization", hash);

            //Send the request and collect/show the results
            try
            {
                WebResponse webResponse2 = request.GetResponse();
                Stream stream2 = webResponse2.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);

                Console.WriteLine(reader2.ReadToEnd());

                webResponse2.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error.");
                Console.Write(e.Message); // to remove compiler warning and to show error message
            }
        }
    }
}

