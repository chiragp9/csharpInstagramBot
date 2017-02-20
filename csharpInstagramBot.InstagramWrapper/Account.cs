using System;
using System.Linq;
using System.Net;

namespace csharpInstagramBot.InstagramWrapper
{
	public class Account
	{
		private readonly bool _debug;

		public Account(bool debug)
		{
			_debug = debug;
		}

		private readonly Helpers _helpers = new Helpers();
		private readonly HttpRequests _myRequests = new HttpRequests ();

		public string SessionId { get; set; }
		private string LoginCsrf { get; set; }
		private string RegisterCsrf { get; set; }
		private string CheckCsrf { get; set; }

		public bool Login(string user, string pass, string proxy = "")
		{
			LoginCsrf = _helpers.GetCsrf ("https://www.instagram.com/accounts/login/", proxy);
			_helpers.Log (_debug, "Grabbed Login CSRF: " + LoginCsrf);

			var post = "username=" + user + "&password=" + pass;
			var url = "https://www.instagram.com/accounts/login/ajax/";

			var headers = new[]
			{
				"Host: www.instagram.com",
				"User-Agent: " + _helpers.GetUserAgent(proxy),
				"Accept: */*",
				"Accept-Language: en-US,en;q=0.5",
				"X-CSRFToken: " + LoginCsrf,
				"X-Instagram-AJAX: 1",
				"Content-Type: application/x-www-form-urlencoded",
				"X-Requested-With: XMLHttpRequest",
				"Referer: https://www.instagram.com/accounts/login/",
				"Cookie: csrftoken=" + LoginCsrf + "; mid=WFOHewAEAAEl7tki8YuogH4lc9rS; ig_pr=1; ig_vw=1366",
				"Connection: keep-alive"
			};

			var request = _myRequests.Create (url, HttpRequests.Method.Post, headers);

            if (proxy != string.Empty && proxy.Contains(":"))
            {
                var splitter = proxy.Split(':');
                var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                prox.BypassProxyOnLocal = true;
                request.Proxy = prox;
                _helpers.Log(_debug, "Proxy Set: " + proxy);
            }

            _myRequests.Write (request, post);

			var response = (HttpWebResponse)request.GetResponse ();

			var html = _myRequests.GetSource (response);
			_helpers.Log (_debug, "Login HTML: " + html);

			if (html.Contains ("\"authenticated\": true"))
			{
				var cookieName = "sessionid";
			    // ReSharper disable once AssignNullToNotNullAttribute
			    var cookie = response.Headers.GetValues ("Set-Cookie").First (x => x.StartsWith (cookieName));
			    SessionId = cookie.Replace (cookieName + "=", string.Empty);
				_helpers.Log (_debug, "Session ID: " + SessionId);
				_helpers.Log (_debug, "Authenticated User: " + user);
				return true;
			}
		    _helpers.Log (_debug, "Authentication failed");
		    return false;
		}


		public bool Register(string username, string email, string password, string fullname, string proxy = "")
		{
			fullname = fullname.Replace (" ", "+");
			email = email.Replace ("@", "%40");

			RegisterCsrf = _helpers.GetCsrf ("https://www.instagram.com/", proxy);
			_helpers.Log (_debug, "Register CSRF: " + RegisterCsrf);

			var url = "https://www.instagram.com/accounts/web_create_ajax/";
			var post = "email=" + email + "&password=" + password + "&username=" + username + "&first_name=" + fullname;

			var headers = new[]
			{
				"Host: www.instagram.com",
				"User-Agent: " + _helpers.GetUserAgent(proxy),
				"Accept: */*",
				"Accept-Language: en-US,en;q=0.5",
				"X-CSRFToken: " + RegisterCsrf,
				"X-Instagram-AJAX: 1",
				"Content-Type: application/x-www-form-urlencoded",
				"X-Requested-With: XMLHttpRequest",
				"Connection: keep-alive",
				"Cookie: mid=WFTOrAAEAAGeFe3by_c5bY_ODw_U; ig_pr=1; ig_vw=1366; csrftoken=" + RegisterCsrf,
				"Referer: https://www.instagram.com/"
			};

			var request = _myRequests.Create (url, HttpRequests.Method.Post, headers);

            if (proxy != string.Empty && proxy.Contains(":"))
            {
                var splitter = proxy.Split(':');
                var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                prox.BypassProxyOnLocal = true;
                request.Proxy = prox;
                _helpers.Log(_debug, "Proxy Set: " + proxy);
            }

            _myRequests.Write (request, post);
			var response = (HttpWebResponse)request.GetResponse ();

			var html = _myRequests.GetSource(response);
			_helpers.Log (_debug, "Register HTML: " + html);

			if (html.Contains("\"account_created\": true"))
			{
				_helpers.Log (_debug, "An account has been created with the username: " + username);
				return true;
			}
		    _helpers.Log(_debug, "Account Creation Failed");
		    return false;
		}

		public bool IsUsernameAvailable(string username)
		{
			CheckCsrf = _helpers.GetCsrf ("https://www.instagram.com/");
			_helpers.Log (_debug, "Check Username CSRF: " + CheckCsrf);

			var url = "https://www.instagram.com/accounts/web_create_ajax/attempt/";
			var post = "email=email%40provider.com&password=password&username=" + username + "&first_name=chris";

			var headers = new[]
			{
				"Host: www.instagram.com",
				"User-Agent: " + _helpers.GetUserAgent(),
				"Accept: */*",
				"Accept-Language: en-US,en;q=0.5",
				"X-CSRFToken: " + CheckCsrf,
				"X-Instagram-AJAX: 1",
				"Content-Type: application/x-www-form-urlencoded",
				"X-Requested-With: XMLHttpRequest",
				"Connection: keep-alive",
				"Cookie: mid=WFTOrAAEAAGeFe3by_c5bY_ODw_U; ig_pr=1; ig_vw=1366; csrftoken=" + CheckCsrf,
				"Referer: https://www.instagram.com/"
			};

			var request = _myRequests.Create (url, HttpRequests.Method.Post, headers);

			_myRequests.Write (request, post);

			var response = (HttpWebResponse)request.GetResponse ();

			var html = _myRequests.GetSource(response);

			if (!html.Contains ("Sorry, that username is taken"))
			{
				_helpers.Log(_debug, username + " is available");
				return true;
			}
		    _helpers.Log(_debug, username + " is taken");
		    return false;
		}
	}
}

