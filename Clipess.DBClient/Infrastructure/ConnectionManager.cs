using System.Collections.Concurrent;


namespace Clipess.DBClient.Infrastructure
{
    public class ConnectionManager
    {
        public static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        public static void AddConnection(string userId, string connectionId)
        {
            _userConnections.AddOrUpdate(userId, connectionId, (key, oldValue) => connectionId);
        }
        public static void RemoveConnection(string connectionId)
        {
            foreach (var userId in _userConnections.Keys)
            {
                if (_userConnections.TryGetValue(userId, out string connId) && connId == connectionId)
                {
                    _userConnections.TryRemove(userId, out _);
                    break;
                }
            }
        }
        public static IEnumerable<string> GetConnectedUserIds()
        {
            return _userConnections.Keys;
        }
        }
}
