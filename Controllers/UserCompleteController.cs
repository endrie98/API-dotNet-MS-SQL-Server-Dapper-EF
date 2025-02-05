using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSql _reusableSql;
        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
        {
            string sql = "EXEC TutorialAppSchema.spUsers_Get";
            DynamicParameters sqlParameters = new DynamicParameters();
            string stringParameters = "";

            if (userId != 0)
            {
                stringParameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (isActive)
            {
                stringParameters += ", @Active=@UserActiveParameter";
                sqlParameters.Add("@UserActiveParameter", isActive, DbType.Boolean);
            }

            if (stringParameters.Length > 0) sql += stringParameters.Substring(1);

            return _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user)
        {
            if (_reusableSql.UpsertUser(user)) return Ok();

            string uptOrAdd;
            if (user.UserId != 0) uptOrAdd = " update";
            else uptOrAdd = " add";

            throw new Exception("Failed to" + uptOrAdd + " user");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = "EXEC TutorialAppSchema.spUser_Delete @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) return Ok();

            throw new Exception("Failed to delete user");
        }
    }
}