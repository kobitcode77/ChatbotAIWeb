namespace ChatbotAI_BE.Services
{
    public interface IUserConnectionManager
    {
        void Add(string user, string connectionId);
        void Remove(string connectionId);
        IReadOnlyCollection<string> GetConnections(string user);
        string? GetUserByConnection(string connectionId);
        IReadOnlyDictionary<string, HashSet<string>> Snapshot();
    }
}
