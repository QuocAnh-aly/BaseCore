using BaseCore.Entities;
using BaseCore.Repository.Authen;
using BaseCore.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Services.Authen;

namespace BaseCore.Services.Authen
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // =========================
        // REGISTER
        // =========================
        public async Task<User> Create(User user, string password)
        {
            byte[] salt;

            user.Password = TokenHelper.HashPassword(password, out salt);
            user.Salt = salt;

            user.Created = DateTime.Now;

            // fix lỗi SQL NULL
            user.IsActive = true;
            user.UserType = user.UserType == 0 ? 1 : user.UserType;

            user.Contact = user.Contact ?? "";
            user.Position = user.Position ?? "";
            user.Image = user.Image ?? "";

            await _userRepository.CreateAsync(user);

            return user;
        }

        // =========================
        // LOGIN
        // =========================
        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
                return null;

            bool valid = TokenHelper.IsValidPassword(password, user.Salt, user.Password);

            if (!valid)
                return null;

            return user;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<User>> GetAll()
        {
            return await _userRepository.GetAllAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<User> GetById(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        // =========================
        // UPDATE
        // =========================
        public async Task Update(User user, string password = null)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);

            if (existingUser == null)
                throw new Exception("User not found");

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Contact = user.Contact;
            existingUser.Position = user.Position;
            existingUser.Image = user.Image;
            existingUser.IsActive = user.IsActive;

            // nếu có đổi password
            if (!string.IsNullOrEmpty(password))
            {
                byte[] salt;
                existingUser.Password = TokenHelper.HashPassword(password, out salt);
                existingUser.Salt = salt;
            }

            await _userRepository.UpdateAsync(existingUser);
        }
        // =========================
        // DELETE
        // =========================
        public async Task Delete(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        // =========================
        // SEARCH
        // =========================
        public async Task<(List<User>, int)> Search(string keyword, int page, int pageSize)
        {
            return await _userRepository.SearchAsync(keyword, page, pageSize);
        }
    }
}