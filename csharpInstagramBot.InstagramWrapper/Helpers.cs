using System;
using System.Net;
using System.Text.RegularExpressions;

namespace csharpInstagramBot.InstagramWrapper
{
	public class Helpers
	{
        public string GetCsrf(string page, string proxy = "")
        {
            using (var client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    var splitter = proxy.Split(':');
                    var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                var source = client.DownloadString(page);
                var pattern = "\"csrf_token\": \"(.*?)\"";

                return Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
            }
		}

        public string GetUserId(string user, string proxy = "")
        {
            using (var client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    var splitter = proxy.Split(':');
                    var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                var page = "https://www.instagram.com/" + user;
                var source = client.DownloadString(page);
                var pattern = "\"id\": \"(.*?)\"";

                return Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
            }
        }

		public void Log(bool debug, string text)
		{
			if (debug)
			{
				Console.WriteLine("[" + DateTime.Now.ToLongDateString () + "] [" + DateTime.Now.ToLongTimeString () + "] " + text);
			}
		}

		public string GetUserAgent(string proxy = "")
		{
            using (var client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    var splitter = proxy.Split(':');
                    var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                var useragents = client.DownloadString("http://pastebin.com/mcfXxS5v").Split('\n');

                return useragents[new Random().Next(0, useragents.Length)];
            }
		}

        public string GetPostId(string url, string proxy = "")
        {
            using (var client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    var splitter = proxy.Split(':');
                    var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                var source = client.DownloadString(url);
                var pattern = "\"id\": \"(.*?)\"";

                var postid = string.Empty;
                foreach (Match item in (new Regex(pattern).Matches(source)))
                {
                    var currentid = item.Groups[1].Value.Split('"')[0];
                    if (currentid.Length == 19)
                    {
                        postid = currentid;
                        break;
                    }
                }

                return postid;
            }
        }
	}
}

