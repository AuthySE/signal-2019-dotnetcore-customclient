using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using signal_2019_dotnetcore.Models;

namespace signal_2019_dotnetcore.Extensions
{
    public static class SessionExtensions
    {
        public static void SetUser(this ISession session, AuthyUser user)
        {
            session.SetString("user", JsonConvert.SerializeObject(user));
        }

        public static AuthyUser GetUser(this ISession session)
        {
            var json = session.GetString("user");

            if (string.IsNullOrEmpty(json))
                return null;

            return JsonConvert.DeserializeObject<AuthyUser>(json);
        }

        public static void SetUsername(this ISession session, string username)
        {
            session.SetString("username", username);
        }

        public static string GetUsername(this ISession session)
        {
            return session.GetString("username");
        }

        
    }
}