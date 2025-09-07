using Microsoft.AspNetCore.SignalR;
using RecomendationService.Application.RepositoryContracts;
using System.Collections.Concurrent;

namespace RecomendationService.WebApi.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly ConcurrentDictionary<Guid, ConcurrentBag<string>> _usersConnectionIDs;
        public NotificationHub(IUserActivityRepository userActivityRepository, ConcurrentDictionary<Guid, ConcurrentBag<string>> usersConnectionIDs)
        {
            _userActivityRepository = userActivityRepository;
            _usersConnectionIDs = usersConnectionIDs;
        }

        public void ConnectToHub(Guid userID)
        {
            if(_usersConnectionIDs.ContainsKey(userID))
            {
                _usersConnectionIDs[userID].Add(Context.ConnectionId);
            }
            else
            {
                _usersConnectionIDs.TryAdd(userID, new ConcurrentBag<string> { Context.ConnectionId });
            }
        } 

        public async Task SendNotification(Guid userID, Guid targetUserID)
        {
            var result = await _userActivityRepository.AddLikeFromUserAsync(targetUserID, userID);

            if (!result.IsSuccess)
            {
                await Clients.Caller.SendAsync("Could not add like");
                return;
            }

            if (_usersConnectionIDs.TryGetValue(targetUserID, out var connections) && connections.Count > 0)
            {
                await Clients.Clients(connections).SendAsync(userID.ToString());
            }
        }

        public void DisconectFromHub(Guid userID)
        {
            ConcurrentBag<string>? connections = _usersConnectionIDs.GetValueOrDefault(userID);

            if(connections != null && connections.Contains(Context.ConnectionId))
            {
                var updatedConnections = new ConcurrentBag<string>(connections.Where(c => c != Context.ConnectionId));
                _usersConnectionIDs[userID] = updatedConnections;
            }
        }
    }
}
