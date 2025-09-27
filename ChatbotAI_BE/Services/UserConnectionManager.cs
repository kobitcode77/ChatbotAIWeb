using System.Collections.Concurrent;

namespace ChatbotAI_BE.Services
{
    public class UserConnectionManager : IUserConnectionManager
    {
        // user -> set of connectionIds
        private readonly ConcurrentDictionary<string, HashSet<string>> _map = new(StringComparer.OrdinalIgnoreCase);
        // connectionId -> user
        private readonly ConcurrentDictionary<string, string> _reverse = new();

        public void Add(string user, string connectionId)
        {
            var set = _map.GetOrAdd(user, _ => new HashSet<string>());
            lock (set) set.Add(connectionId);
            _reverse[connectionId] = user;
        }

        public void Remove(string connectionId)
        {
            if (_reverse.TryRemove(connectionId, out var user) && _map.TryGetValue(user, out var set))
            {
                lock (set)
                {
                    set.Remove(connectionId);
                    if (set.Count == 0) _map.TryRemove(user, out _);
                }
            }
        }

        public IReadOnlyCollection<string> GetConnections(string user)
            => _map.TryGetValue(user, out var set) ? set.ToList() : Array.Empty<string>();

        public string? GetUserByConnection(string connectionId)
            => _reverse.TryGetValue(connectionId, out var u) ? u : null;

        public IReadOnlyDictionary<string, HashSet<string>> Snapshot() => _map;
    }
}
