using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public async Task JoinUserGroup(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new HubException("Invalid user ID");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            $"user-{userId}"
        );
    }
}