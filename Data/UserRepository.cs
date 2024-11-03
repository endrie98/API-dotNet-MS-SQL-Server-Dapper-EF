using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public async void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
                await _entityFramework.AddAsync(entityToAdd);
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
                _entityFramework.Remove(entityToRemove);
        }

        public IEnumerable<User> GetUsers()
        {
            return [.. _entityFramework.Users];
        }

        public async Task<User> GetSingleUser(int userId)
        {
            var user = await _entityFramework.Users.FindAsync(userId);
            return user!;
        }

        public async Task<int> EditUser(User user)
        {
            var userFromDb = await _entityFramework.Users.FindAsync(user.UserId);

            if (userFromDb != null)
            {
                userFromDb.FirstName = user.FirstName;
                userFromDb.LastName = user.LastName;
                userFromDb.Email = user.Email;
                userFromDb.Active = user.Active;
                userFromDb.Gender = user.Gender;

                var result = await _entityFramework.SaveChangesAsync() > 0;
                if (result) return 3;
                return 2;
            }
            else
            {
                return 0;
            }
        }

        public UserSalary GetSingleUserSalary(int userSalaryId)
        {
            return _entityFramework.UserSalary.Find(userSalaryId)!;
        }
        public UserJobInfo GetSingleUserJobInfo(int userSalaryId)
        {
            return _entityFramework.UserJobInfo.Find(userSalaryId)!;
        }
    }
}