using System.Net;
using System.Text.RegularExpressions;

namespace csharpInstagramBot.InstagramWrapper
{
    public class Profile
    {
        private readonly Helpers _helpers = new Helpers();

        public enum InfoType
        {
            FollowerCount, FollowingCount, PostCount, Bio, ProfilePicture, UserId
        }

        public string GetInfo(string user, InfoType type)
        {
            using (var client = new WebClient())
            {
                var source = client.DownloadString("https://www.instagram.com/" + user);

                string pattern;
                var info = string.Empty;

                switch (type)
                {
                    case InfoType.FollowerCount:
                        pattern = "\"followed_by\": {\"count\": (.*?)}";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('}')[0];
                        break;
                    case InfoType.FollowingCount:
                        pattern = "\"follows\": {\"count\": (.*?)}";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('}')[0];
                        break;
                    case InfoType.PostCount:
                        pattern = "\"media\": {\"count\": (.*?),";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split(',')[0];
                        break;
                    case InfoType.Bio:
                        pattern = "\"biography\": \"(.*?)\"";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
                        break;
                    case InfoType.ProfilePicture:
                        pattern = "\"profile_pic_url\": \"(.*?)\"";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
                        break;
                    case InfoType.UserId:
                        info = _helpers.GetUserId(user);
                        break;
                }

                return info;
            }
        }
    }
}
