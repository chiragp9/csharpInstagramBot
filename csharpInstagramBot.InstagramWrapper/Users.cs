using System.Net;

namespace csharpInstagramBot.InstagramWrapper
{
    public class Users
    {
        private readonly Helpers _helpers = new Helpers();
        private readonly HttpRequests _myRequests = new HttpRequests();

        private string SessionId { get; }
        private string DsUserId { get; }
        private readonly bool _debug;

        public Users(string currentUser, string sessionId, bool debug)
        {
            SessionId = sessionId;
            _debug = debug;

            DsUserId = _helpers.GetUserId(currentUser);
            _helpers.Log(debug, "DSUserID: " + DsUserId);
        }

        private string FollowCsrf { get; set; }

        public bool Follow(string user, string proxy = "")
        {
            FollowCsrf = _helpers.GetCsrf("https://www.instagram.com/" + user, proxy);
            _helpers.Log(_debug, "Follow CSRF: " + FollowCsrf);

            var userid = _helpers.GetUserId(user, proxy);
            _helpers.Log(_debug, user + "'s id: " + userid);

            var url = "https://www.instagram.com/web/friendships/" + userid + "/follow/";
            var headers = new[]
            {
                "Host: www.instagram.com",
                "User-Agent: " + _helpers.GetUserAgent(proxy),
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.5",
                "Referer: https://www.instagram.com/" + user + "/",
                "X-CSRFToken: " + FollowCsrf,
                "X-Instagram-AJAX: 1",
                "Content-Type: application/x-www-form-urlencoded",
                "X-Requested-With: XMLHttpRequest",
                "Cookie: mid=WFotuQAEAAEj_k7xPdGLSnl7c6r8; csrftoken=" + FollowCsrf + "; s_network=; sessionid=" + SessionId + "; ds_user_id=" + DsUserId + "; ig_pr=1; ig_vw=1600",
                "Connection: keep-alive",
                "Content-Length: 0"
            };

            var request = _myRequests.Create(url, HttpRequests.Method.Post, headers);

            if (proxy != string.Empty && proxy.Contains(":"))
            {
                var splitter = proxy.Split(':');
                var prox = new WebProxy(splitter[0], int.Parse(splitter[1])) {BypassProxyOnLocal = true};
                request.Proxy = prox;
                _helpers.Log(_debug, "Proxy Set: " + proxy);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var html = _myRequests.GetSource(response);
            _helpers.Log(_debug, "Follow HTML: " + html);
            
            if (html.Contains("\"result\": \"following\""))
            {
                _helpers.Log(_debug, "Followed user: " + user);
                return true;
            }
            _helpers.Log(_debug, "Failed to follow user");
            return false;
        }
	}
}

