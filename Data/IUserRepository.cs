using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);
        public IEnumerable<User> GetUsers();
        public Task<User> GetSingleUser(int userId);
        public Task<int> EditUser(User user);
        public UserSalary GetSingleUserSalary(int userSalaryId);
        public UserJobInfo GetSingleUserJobInfo(int userSalaryId);
    }
}