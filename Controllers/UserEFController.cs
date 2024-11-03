using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {
        // private readonly DataContextEF _entityFramework;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserEFController(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            // _entityFramework = new DataContextEF(config);

            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, User>();
                cfg.CreateMap<UserJobInfo, UserJobInfo>();
                cfg.CreateMap<UserSalary, UserSalary>();
            }));
        }

        [HttpGet("GetUsesr")]
        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetUsers();
            // return [.. _entityFramework.Users];
            // return _entityFramework.Users.ToList<User>();
        }

        [HttpGet("GetSingleUser/{userId}")]
        public async Task<IActionResult> GetSingleUser(int userId)
        {
            User? user = await _userRepository.GetSingleUser(userId);
            if (user == null) return NotFound("User not found");

            return Ok(user);
        }

        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser(User user)
        {
            var result = await _userRepository.EditUser(user);
            if (result == 0)
            {
                return NotFound("User not found");
            }
            else if (result == 2)
            {
                return BadRequest("Failed update user");
            }
            else
            {
                return Ok();
            }
            // User? userFromDb = await _entityFramework.Users.FindAsync(user.UserId);
            // if (userFromDb == null) return NotFound("User not found");

            // userFromDb.FirstName = user.FirstName;
            // userFromDb.LastName = user.LastName;
            // userFromDb.Email = user.Email;
            // userFromDb.Active = user.Active;
            // userFromDb.Gender = user.Gender;

            // var result = _userRepository.SaveChanges();
            // if (result) return Ok(userFromDb);

            // return BadRequest("Failed update user");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto userDto)
        {
            // User newUser = new User
            // {
            //     LastName = userDto.LastName ??= "",
            //     FirstName = userDto.FirstName ??= "",
            //     Email = userDto.Email ??= "",
            //     Gender = userDto.Gender ??= "",
            //     Active = userDto.Active
            // };

            User newUser = _mapper.Map<User>(userDto);

            _userRepository.AddEntity(newUser);

            var result = _userRepository.SaveChanges();

            if (result) return Ok(newUser);

            return BadRequest("Failed add user");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            User? user = await _userRepository.GetSingleUser(userId);

            if (user == null) return NotFound("User not found");

            _userRepository.RemoveEntity(user);

            var result = _userRepository.SaveChanges();

            if (result) return Ok("Successfully deleted user");

            return BadRequest("Failed delete user");
        }

        [HttpGet("UserSalarys/{userId}")]
        public UserSalary UserSalarys(int userId)
        {
            return _userRepository.GetSingleUserSalary(userId);
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(AddUserSalaryDto addUserSalaryDto)
        {
            _userRepository.AddEntity(new UserSalary { UserId = addUserSalaryDto.UserId, Salary = addUserSalaryDto.Salary });

            return _userRepository.SaveChanges() ? Ok() : BadRequest("Failed add User salary");
        }

        [HttpPut("UpdateUserSalary")]
        public IActionResult UpdateUserSalary(UpdateUserSalaryDto updateUserSalaryDto)
        {
            var userSalaryFromDb = _userRepository.GetSingleUserSalary(updateUserSalaryDto.UserId);

            if (userSalaryFromDb != null) userSalaryFromDb.Salary = updateUserSalaryDto.Salary;

            return _userRepository.SaveChanges() ? Ok() : BadRequest("Failed update User salary");
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            _userRepository.RemoveEntity(_userRepository.GetSingleUserSalary(userId)!);

            return _userRepository.SaveChanges() ? Ok() : BadRequest("Failed delete User salary");
        }

        [HttpGet("GetUserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfo(int userId)
        {
            return _userRepository.GetSingleUserJobInfo(userId);
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(AddUserJobInfoDto addUserJobInfoDto)
        {
            var newUserJobInfo = new UserJobInfo { UserId = addUserJobInfoDto.UserId, Department = addUserJobInfoDto.Department, JobTitle = addUserJobInfoDto.Department };

            _userRepository.AddEntity(newUserJobInfo);

            return _userRepository.SaveChanges() ? Ok(newUserJobInfo) : BadRequest("Failed add User job info");
        }

        [HttpPut("UpdateUserJobInfo")]
        public IActionResult UpdateUserJobInfo(UserJobInfo userJobInfo)
        {
            var userJobInfoFromDb = _userRepository.GetSingleUserJobInfo(userJobInfo.UserId);

            if (userJobInfoFromDb != null) _mapper.Map(userJobInfo, userJobInfoFromDb);

            return _userRepository.SaveChanges() ? Ok() : BadRequest("Failed update User job info");
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            var userJobInfoFromDb =_userRepository.GetSingleUserJobInfo(userId);

            if (userJobInfoFromDb != null) _userRepository.RemoveEntity(userJobInfoFromDb);

            return _userRepository.SaveChanges() ? Ok() : BadRequest("Failed delete User job info");
        }
    }
}