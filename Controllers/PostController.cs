using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId, int userId, string searchParam = "None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();

            if (postId != 0)
            {
                parameters += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            }
            if (userId != 0)
            {
                parameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (searchParam != "None")
            {
                parameters += ", @SearchValue=@SearchParamParameter";
                sqlParameters.Add("@SearchParamParameter", searchParam, DbType.String);
            }

            if (!string.IsNullOrEmpty(parameters)) sql += parameters.Substring(1);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";
            parameters += " @UserId=@UserIdParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);
            sql += parameters;
            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post post)
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Upsert";

            string parameters = " @UserId=@UserIdParameter, @PostTitle=@PostTitleParameter, @PostContent=@PostContentParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParameter", post.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", post.PostContent, DbType.String);


            string uptOrAdd = "add";
            if (post.PostId > 0)
            {
                parameters += ", @PostId=@PostIdParameter"; 
                sqlParameters.Add("@PostIdParameter", post.PostId, DbType.Int32);
                uptOrAdd = "edit";
            }

            sql += parameters;

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) return Ok();

            throw new Exception("Failed to " + uptOrAdd + " new post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = "EXEC TutorialAppSchema.spPost_Delete";

            DynamicParameters sqlParameters = new DynamicParameters();
            string parameters = " @PostId=@PostIdParameter, @UserId=@UserIdParamater";
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            sqlParameters.Add("@UserIdParamater", User.FindFirst("userId")?.Value, DbType.Int32);

            sql += parameters;

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) return Ok();

            throw new Exception("Failed to delete post!");
        }
    }
}