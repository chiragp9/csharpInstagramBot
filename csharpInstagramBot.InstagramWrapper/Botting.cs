using System;

namespace csharpInstagramBot.InstagramWrapper
{
    public class Botting
    {
        private readonly bool _debug;
        private readonly Helpers _helpers = new Helpers();

        public Botting(bool debug)
        {
            _debug = debug;
        }

        private static string[] Names()
        {
            return new[]
            {
                "Fighter", "Majestic", "Unicorn", "Dizzy", "Playful", "Swimmer", "Lolipop", "Hole", "Dancer", "Fighter", "Salty", "Butterfly",
                "Snake", "Rabbit", "Dog", "Cat", "Kangaroo", "Giraffe", "Hippopotamus", "Doodler"
            };
        }

        private enum InfoType
        {
            Username, Password, Email, Fullname
        }

        private string GetInfo(InfoType info)
        {
            var myinfo = string.Empty;
            var names = Names();

            var r = new Random();
            switch (info)
            {
                case InfoType.Email:
                    myinfo = names[r.Next(0, names.Length)] + names[r.Next(0, names.Length)] + r.Next(0, 1000000) + "@gmail.com";
                    break;
                case InfoType.Fullname:
                    myinfo = "full name";
                    break;
                case InfoType.Password:
                    myinfo = names[r.Next(0, names.Length)] + r.Next(0, 1000000);
                    break;
                case InfoType.Username:
                    myinfo = names[r.Next(0, names.Length)] + names[r.Next(0, names.Length)] + r.Next(0, 1000000);
                    break;
            }

            _helpers.Log(_debug, "Type: " + info + " // Value: " + myinfo);
            return myinfo;
        }

        public bool Like(string photo, string proxy = "")
        {
            var acc = new Account(_debug);

            var username = GetInfo(InfoType.Username);
            var password = GetInfo(InfoType.Password);
            var email = GetInfo(InfoType.Email);
            var fullname = GetInfo(InfoType.Fullname);

            if (acc.Register(username, email, password, fullname, proxy))
            {
                if (acc.Login(username, password, proxy))
                {
                    var posts = new Posts(username, acc.SessionId, _debug);
                    if (posts.Like(photo, proxy))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        public bool Comment(string photo, string comment, string proxy = "")
        {
            var acc = new Account(_debug);

            var username = GetInfo(InfoType.Username);
            var password = GetInfo(InfoType.Password);
            var email = GetInfo(InfoType.Email);
            var fullname = GetInfo(InfoType.Fullname);

            if (acc.Register(username, email, password, fullname, proxy))
            {
                if (acc.Login(username, password, proxy))
                {
                    var posts = new Posts(username, acc.SessionId, _debug);
                    if (posts.Comment(photo, comment, proxy))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        public bool Follow(string user, string proxy = "")
        {
            var acc = new Account(_debug);

            var username = GetInfo(InfoType.Username);
            var password = GetInfo(InfoType.Password);
            var email = GetInfo(InfoType.Email);
            var fullname = GetInfo(InfoType.Fullname);

            if (acc.Register(username, email, password, fullname, proxy))
            {
                if (acc.Login(username, password, proxy))
                {
                    var users = new Users(username, acc.SessionId, _debug);
                    if (users.Follow(user, proxy))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
    }
}

