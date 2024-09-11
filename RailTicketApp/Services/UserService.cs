using Microsoft.EntityFrameworkCore;
using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;
using System.Diagnostics;

namespace RailTicketApp.Services
{
    public class UserService : IUserService
    {

        private readonly DbContextClass _dbContext;
        public UserService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }
        public User AuthenticateUser(UserLoginDto loginDto)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Email == loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.Password))
            {
                return null;
            }

            return user;
        }

        public bool RegisterUser(UserRegistrationDto registrationDto)
        {
            if (_dbContext.Users.Any(u => u.Email == registrationDto.Email))
            {
                return false;
            }

            var hashedPassword = HashPassword(registrationDto.Password);

            var newUser = new User
            {
                Name = registrationDto.Name,
                Email = registrationDto.Email,
                Password = hashedPassword,
                Phone = registrationDto.Phone
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return true;
        }

        public User GetUser(string email)
        {
            var user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                throw new NullReferenceException("There is no user with email " + email + " in db");
            }
            return user;
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
