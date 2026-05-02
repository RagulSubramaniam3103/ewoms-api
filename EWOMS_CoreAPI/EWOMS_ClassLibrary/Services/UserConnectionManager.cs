using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.Services
{
    public class UserConnectionManager
    {
        // Support multiple connections per user (e.g. multiple tabs)
        private static readonly Dictionary<string, HashSet<string>> _userConnections = new();

        public void Add(string userId, string connectionId)
        {
            lock (_userConnections)
            {
                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = new HashSet<string>();
                }
                _userConnections[userId].Add(connectionId);
            }
        }

        public void Remove(string connectionId)
        {
            lock (_userConnections)
            {
                var user = _userConnections.FirstOrDefault(x => x.Value.Contains(connectionId));
                if (user.Key != null)
                {
                    user.Value.Remove(connectionId);
                    if (user.Value.Count == 0)
                    {
                        _userConnections.Remove(user.Key);
                    }
                }
            }
        }

        public bool IsOnline(string userId)
        {
            lock (_userConnections)
            {
                return _userConnections.ContainsKey(userId);
            }
        }

        public List<string> GetOnlineUsers()
        {
            lock (_userConnections)
            {
                return _userConnections.Keys.ToList();
            }
        }

        public List<string> GetConnections(string userId)
        {
            lock (_userConnections)
            {
                return _userConnections.ContainsKey(userId) 
                    ? _userConnections[userId].ToList() 
                    : new List<string>();
            }
        }
    }
}
