using System;
using System.Net;

namespace csharpInstagramBot.InstagramWrapper
{
	public class Posts
	{
        private readonly Helpers _helpers = new Helpers();
        private readonly HttpRequests _myRequests = new HttpRequests();

        private string SessionId { get; }
		private string DsUserId { get; }
		private readonly bool _debug;

		public Posts (string currentUser, string sessionId, bool debug)
		{
			SessionId = sessionId;
			_debug = debug;

            DsUserId = _helpers.GetUserId(currentUser);
            _helpers.Log(debug, "DSUserID: " + DsUserId);
		}

		private string LikeCsrf { get; set; }
		private string CommentCsrf { get; set; }

		public bool Like(string photoUrl, string proxy = "")
		{
            LikeCsrf = _helpers.GetCsrf(photoUrl, proxy);
            _helpers.Log(_debug, "Like CSRF " + LikeCsrf);

            var postId = _helpers.GetPostId(photoUrl, proxy);
            _helpers.Log(_debug, "Post ID: " + postId);

            var url = "https://www.instagram.com/web/likes/" + postId + "/like/";
            var headers = new[]
            {
                "Host: www.instagram.com",
                "User-Agent: " + _helpers.GetUserAgent(proxy),
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.5",
                "Referer: " + photoUrl,
                "X-CSRFToken: " + LikeCsrf,
                "X-Instagram-AJAX: 1",
                "Content-Type: application/x-www-form-urlencoded",
                "X-Requested-With: XMLHttpRequest",
                "Cookie: mid=WFotuQAEAAEj_k7xPdGLSnl7c6r8; csrftoken=" + LikeCsrf + "; s_network=; ig_pr=1; ig_vw=1600; sessionid=" + SessionId + "; ds_user_id=" + DsUserId,
                "Connection: keep-alive",
                "Content-Length: 0"
            };

            var request = _myRequests.Create(url, HttpRequests.Method.Post, headers);

            if (proxy != string.Empty && proxy.Contains(":"))
            {
                var splitter = proxy.Split(':');
                var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                prox.BypassProxyOnLocal = true;
                request.Proxy = prox;
                _helpers.Log(_debug, "Proxy Set: " + proxy);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var html = _myRequests.GetSource(response);

            _helpers.Log(_debug, "Like HTML: " + html);
            if (html.Contains("ok"))
            {
                _helpers.Log(_debug, photoUrl + " was liked by user.");
                return true;
            }
		    _helpers.Log(_debug, "Failed to like photo");
		    return false;
		}

		public bool Comment(string photoUrl, string comment, string proxy = "")
		{
            var post = "comment_text=" + comment.Replace(" ", "+");

            CommentCsrf = _helpers.GetCsrf(photoUrl, proxy);
            _helpers.Log(_debug, "Comment CSRF " + CommentCsrf);

            var postId = _helpers.GetPostId(photoUrl, proxy);
            _helpers.Log(_debug, "Post ID: " + postId);

            var url = "https://www.instagram.com/web/comments/" + postId + "/add/";
            var headers = new[]
            {
                "Host: www.instagram.com",
                "User-Agent: " + _helpers.GetUserAgent(proxy),
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.5",
                "Referer: " + photoUrl,
                "X-CSRFToken: " + CommentCsrf,
                "X-Instagram-AJAX: 1",
                "Content-Type: application/x-www-form-urlencoded",
                "X-Requested-With: XMLHttpRequest",
                "Cookie: mid=WFotuQAEAAEj_k7xPdGLSnl7c6r8; csrftoken=" + CommentCsrf + "; s_network=; ig_pr=1; ig_vw=1600; sessionid=" + SessionId + "; ds_user_id=" + DsUserId,
                "Connection: keep-alive"
            };

            var request = _myRequests.Create(url, HttpRequests.Method.Post, headers);

            if (proxy != string.Empty && proxy.Contains(":"))
            {
                var splitter = proxy.Split(':');
                var prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                prox.BypassProxyOnLocal = true;
                request.Proxy = prox;
                _helpers.Log(_debug, "Proxy Set: " + proxy);
            }

            _myRequests.Write(request, post);

            var response = (HttpWebResponse)request.GetResponse();
            var html = _myRequests.GetSource(response);

            _helpers.Log(_debug, "Comment HTML: " + html);
            if (html.Contains("ok"))
            {
                _helpers.Log(_debug, "Comment was added to photo");
                return true;
            }
		    _helpers.Log(_debug, "Comment not added.");
		    return false;
		}
	}
}

