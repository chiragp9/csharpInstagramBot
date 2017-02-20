using System;
using System.IO;
using System.Net;
using System.Text;

namespace csharpInstagramBot.InstagramWrapper
{
	public class HttpRequests
	{

		public enum Method
		{
			Options, Post, Get, Head, Put, Delete, Trace
		}

		public void SetHeaders(HttpWebRequest request, string[] headers)
		{
			try 
			{
				for (int index = 0, n = headers.Length; index < n; index++)
				{
					var splitter = headers [index].Split(new[] {":"}, StringSplitOptions.None);
					var header = splitter[0];
					var value = headers[index].Replace(header + ": ", string.Empty);

                    switch (header)
                    {
                        case "Accept":
                            request.Accept = value;
                            break;

                        case "Connection":
                            if (value == "keep-alive") request.KeepAlive = true;
                            break;

                        case "Content-Type":
                            request.ContentType = value;
                            break;

                        case "Date":
                            request.Date = DateTime.Parse(value);
                            break;

                        case "Expect":
                            request.Expect = value;
                            break;

                        case "Host":
                            request.Host = value;
                            break;

                        case "Content-Length":
                            request.ContentLength = int.Parse(value);
                            break;

                        case "Referer":
                            request.Referer = value;
                            break;

                        case "TE":
                            request.TransferEncoding = value;
                            break;

                        case "User-Agent":
                            request.UserAgent = value;
                            break;

                        case "Method":
                            request.Method = value;
                            break;

                        default:
                            request.Headers.Add(header, value);
                            break;
                    }
				}
			}
			catch (Exception ex)
			{
				throw new Exception (ex.Message);
			}
		}


		public HttpWebRequest Create(string url, Method method, string[] headers)
		{
			try
			{
				var request = (HttpWebRequest) WebRequest.Create(url);

                switch (method)
                {
                    case Method.Post:
                        request.Method = "POST";
                        break;
                    case Method.Get:
                        request.Method = "GET";
                        break;
                    case Method.Head:
                        request.Method = "HEAD";
                        break;
                    case Method.Options:
                        request.Method = "OPTIONS";
                        break;
                    case Method.Put:
                        request.Method = "PUT";
                        break;
                    case Method.Delete:
                        request.Method = "DELETE";
                        break;
                    case Method.Trace:
                        request.Method = "TRACE";
                        break;
                }


				SetHeaders(request, headers);

				return request;
			} 
			catch (Exception ex)
			{
				throw new Exception (ex.Message);
			}
		}


		public void Write(HttpWebRequest request, string data)
		{
			try 
			{
				var bytes = Encoding.ASCII.GetBytes (data);
				request.ContentLength = bytes.Length;

				using (var stream = request.GetRequestStream ())
				{
					stream.Write (bytes, 0, bytes.Length);
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				throw new Exception (ex.Message);
			}
		}


        public string GetSource(HttpWebResponse response)
        {
            try
            {
                var html = string.Empty;

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    html = reader.ReadToEnd();
                }

                return html;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
	}
}
