using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            return _dapper.LoadData<User>(@"
                SELECT [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active] FROM TutorialAppSchema.Users
            ");
        }

        [HttpGet("GetSingleUser/{userId}")]
        public IActionResult GetSingleUser(int userId)
        {
            var user = _dapper.LoadDataSingle<User>(@"
                SELECT [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active] FROM TutorialAppSchema.Users
                    WHERE UserId = " + userId.ToString());

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            string sql = @"
                UPDATE TutorialAppSchema.Users
                SET 
                [FirstName] = '" + user.FirstName +
                "', [LastName] = '" + user.LastName +
                "', [Email] = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active +
                "' WHERE UserId = " + user.UserId;


            Console.WriteLine(sql);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            return BadRequest("Failed update user");
        }

        [HttpPost("AddUser")]

        public IActionResult AddUser(UserDto user)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.Users(
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
                ) VALUES (" +
                    "'" + user.FirstName +
                    "','" + user.LastName +
                    "','" + user.Email +
                    "','" + user.Gender +
                    "','" + user.Active +
                    "')";

            Console.WriteLine(sql);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            return BadRequest("Failed add user");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            var user = _dapper.LoadDataSingle<User>("SELECT [UserId] FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString());
            if (user == null) return NotFound("User not found");

            string sql = "DELETE FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString();
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            return BadRequest("Failed delete user");
        }

        [HttpGet("GetUsersSalary")]
        public IEnumerable<UserSalary> UserSalarys()
        {
            return _dapper.LoadData<UserSalary>(@"
                SELECT [UserId],
                    [Salary],
                    [AvgSalary] FROM TutorialAppSchema.UserSalary
            ");
        }

        [HttpGet("GetSingleUserSalary/{userId}")]
        public IActionResult GetSingleUserSalary(int userId)
        {
            IEnumerable<UserSalary> userSalary = _dapper.LoadData<UserSalary>(@"
                SELECT [UserId],
                    [Salary],
                    [AvgSalary] FROM TutorialAppSchema.UserSalary WHERE UserId = " + userId.ToString()
            );

            if (userSalary == null) return NotFound("User salary not found");

            return Ok(userSalary);
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(AddUserSalaryDto addUserSalaryDto)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.UserSalary (
                    UserId,
                    Salary
                ) VALUES (" + addUserSalaryDto.UserId
                    + ", " + addUserSalaryDto.Salary
                + ")";

            if (_dapper.ExecuteSql(sql)) return Ok(addUserSalaryDto);

            return BadRequest("Failed add User salary");
        }

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UpdateUserSalaryDto updateUserSalaryDto)
        {
            var userSalaryFromDb = _dapper.LoadDataSingle<UserSalary>(@"
                SELECT [UserId],
                    [Salary],
                    [AvgSalary] FROM TutorialAppSchema.UserSalary WHERE UserId = " + updateUserSalaryDto.UserId.ToString()
                );

            if (userSalaryFromDb == null) return NotFound("User salary not found");

            string sql = @"
                UPDATE TutorialAppSchema.UserSalary
                SET [Salary] = " + updateUserSalaryDto.Salary + " WHERE UserId = " + updateUserSalaryDto.UserId.ToString();

            System.Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                userSalaryFromDb.Salary = updateUserSalaryDto.Salary;
                return Ok(userSalaryFromDb);
            }

            return BadRequest("Failed update User salary");
        }

        [HttpDelete("DeleteUserSalary/{userid}")]
        public IActionResult DeleteUserSalary(int userid)
        {
            var userSalaryFromDb = _dapper.LoadDataSingle<UserSalary>(@"
                SELECT [UserId],
                    [Salary],
                    [AvgSalary] FROM TutorialAppSchema.UserSalary WHERE UserId = " + userid.ToString());

            if (userSalaryFromDb == null) return NotFound("User salary not found");

            string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = " + userid.ToString();

            if (_dapper.ExecuteSql(sql)) return Ok();

            return BadRequest("Failed delete User salary");
        }

        [HttpGet("GetUserJobInfo/{userId}")]
        public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
        {
            return _dapper.LoadData<UserJobInfo>(@"
                SELECT UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
                FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userId);
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(AddUserJobInfoDto addUserJobInfoDto)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.UserJobInfo (
                    UserId,
                    Department,
                    JobTitle
                ) VALUES (" + addUserJobInfoDto.UserId
                    + ", '" + addUserJobInfoDto.Department
                    + "', '" + addUserJobInfoDto.JobTitle
                    + "')";

            if (_dapper.ExecuteSql(sql)) return Ok(addUserJobInfoDto);

            return BadRequest("Failed add User job info");
        }

        [HttpPut("UpdateUserJobInfo")]
        public IActionResult UpdateUserJobInfo(UserJobInfo userJobInfo)
        {
            var userJobInfoFromDb = _dapper.LoadDataSingle<UserJobInfo>(@"
                SELECT [UserId],
                    [JobTitle],
                    [Department] FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userJobInfo.UserId.ToString()
                );

            if (userJobInfoFromDb == null) return NotFound("User job info not found");

            string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
                + userJobInfo.Department
                + "', JobTitle='"
                + userJobInfo.JobTitle
                + "' WHERE UserId =" + userJobInfo.UserId.ToString();

            System.Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                userJobInfoFromDb.JobTitle = userJobInfo.JobTitle;
                userJobInfoFromDb.Department = userJobInfo.Department;
                return Ok(userJobInfoFromDb);
            }

            return BadRequest("Failed update User job info");
        }

        [HttpDelete("DeleteUserJobInfo/{userid}")]
        public IActionResult DeleteUserJobInfo(int userid)
        {
            var userJobInfoFromDb = _dapper.LoadDataSingle<UserJobInfo>(@"
                SELECT [UserId],
                    [JobTitle],
                    [Department] FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userid.ToString()
                );

            if (userJobInfoFromDb == null) return NotFound("User job info not found");

            string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userid.ToString();

            if (_dapper.ExecuteSql(sql)) return Ok();

            return BadRequest("Failed delete User job info");
        }
    }
}