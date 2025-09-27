using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatbotAI_BE.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chats;
        private readonly IUserConnectionManager _connections;

        public ChatHub(IChatService chats, IUserConnectionManager connections)
        {
            _chats = chats;
            _connections = connections;
        }

        // ===== WORLD (THẾ GIỚI) =====
        public async Task SendToWorld(string content)
        {
            var sender = ResolveUser();
            var msg = await _chats.SaveWorldAsync(sender, content);
            await Clients.All.SendAsync("ReceiveWorld", msg.Sender, msg.Content, msg.SendTime);
        }

        // ===== DIRECT (2 NGƯỜI) =====
        public async Task SendToUser(string toUser, string content)
        {
            var sender = ResolveUser();
            if (string.Equals(toUser, sender, StringComparison.OrdinalIgnoreCase)) return;

            var msg = await _chats.SaveDirectAsync(sender, toUser, content);

            // gửi đến mọi connection của người nhận
            foreach (var conn in _connections.GetConnections(toUser))
                await Clients.Client(conn).SendAsync("ReceiveDirect", sender, msg.Content, msg.SendTime);

            // phản chiếu về cho người gửi (mọi connection của sender)
            foreach (var conn in _connections.GetConnections(sender))
                await Clients.Client(conn).SendAsync("ReceiveDirect", sender, msg.Content, msg.SendTime);
        }

        // ===== GROUP (NHIỀU NGƯỜI) =====
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var user = ResolveUser();
            await Clients.Group(groupName).SendAsync("SystemMessage", $"{user} đã tham gia nhóm {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            var user = ResolveUser();
            await Clients.Group(groupName).SendAsync("SystemMessage", $"{user} đã rời nhóm {groupName}");
        }

        public async Task SendToGroup(string groupName, string content)
        {
            var sender = ResolveUser();
            var msg = await _chats.SaveGroupAsync(sender, groupName, content);
            await Clients.Group(groupName).SendAsync("ReceiveGroup", groupName, sender, msg.Content, msg.SendTime);
        }

        // ===== CONNECTION LIFECYCLE =====
        public override async Task OnConnectedAsync()
        {
            var user = ResolveUser();
            _connections.Add(user, Context.ConnectionId);

            await Clients.Caller.SendAsync("SystemMessage", $"Chào mừng {user}!");
            await Clients.AllExcept(Context.ConnectionId).SendAsync("SystemMessage", $"{user} đã tham gia chat");

            await PushOnlineList();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connections.GetUserByConnection(Context.ConnectionId) ?? "Unknown";
            _connections.Remove(Context.ConnectionId);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("SystemMessage", $"{user} đã rời chat");
            await PushOnlineList();
            await base.OnDisconnectedAsync(exception);
        }

        // ===== Helpers =====
        private string ResolveUser()
        {
            var name = Context.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(name)) return name;
            return $"Guest_{Context.ConnectionId[..6]}";
        }

        private Task PushOnlineList()
        {
            var online = _connections.Snapshot().Keys.OrderBy(x => x).ToList();
            return Clients.All.SendAsync("OnlineUsers", online);
        }
    }
}
