using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;

namespace Promact.PasswordlessAuthentication.Services
{
    public class UserHub : Hub
    {
        public static int activeUser = 0;
        public static int totalUser = 0;
        public static Dictionary<string, UserDetails> connectedUsers = new Dictionary<string, UserDetails>();
        

        public override async Task OnConnectedAsync()
        {

            activeUser++;
            await Clients.All.SendAsync("ReceiveMessage", new {activeUser = activeUser
                ,totalUser = totalUser});
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (activeUser > 0)
            {
            
            activeUser--;
            }
            await Clients.All.SendAsync("ReceiveMessage", new
            {
               
                activeUser = activeUser
                ,
                totalUser = totalUser
            });
            await base.OnDisconnectedAsync(exception);
            
        }

        private  async Task ReceiveUserDetailOnConnect(UserDetails user)
        {
            connectedUsers.Add(user.Email, user);
            await Clients.All.SendAsync("ReceiveUserDetailOnConnect", new {currentUser= user, allUser= connectedUsers });
        }
        private async Task ReceiveUserDetailOnDisconnect(UserDetails user)
        {
            
            // Find and remove the user with the specified email address
            var userToRemove = connectedUsers.FirstOrDefault(u => u.Value.Email == user.Email);
            if (userToRemove.Key != null)
            {
                connectedUsers.Remove(userToRemove.Key);
            }

            await Clients.All.SendAsync("ReceiveUserDetailOnDisconnect", user, connectedUsers);
        }


    }
}
