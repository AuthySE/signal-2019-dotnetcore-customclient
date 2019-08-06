using System.Collections.Generic;
using signal_2019_dotnetcore.Models;

namespace signal_2019_dotnetcore
{
    public class UserRepository : IUserRepository
    {
        private static IDictionary<string, AuthyUser> _db = new Dictionary<string, AuthyUser>();

        public bool AddUser(AuthyUser user)
        {
            return _db.TryAdd(user.Username, user);
        }

        public bool Exists(string username)
        {
            return _db.ContainsKey(username);
        }

        public AuthyUser GetUser(string username)
        {
            if(!_db.TryGetValue(username, out var user))
                return null;
            return user;
        }
    }

    public interface IUserRepository
    {
        bool Exists(string username);
        AuthyUser GetUser(string username);
        bool AddUser(AuthyUser user);
    }
}