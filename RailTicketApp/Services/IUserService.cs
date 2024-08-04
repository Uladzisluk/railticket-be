using RailTicketApp.Models;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.Services
{
    public interface IUserService
    {
        public User AuthenticateUser(UserLoginDto loginDto);
        public bool RegisterUser(UserRegistrationDto registrationDto);
    }
}
