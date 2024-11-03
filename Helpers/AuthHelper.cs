using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            byte[] combinedSalt = Encoding.ASCII.GetBytes(password).Concat(passwordSalt).ToArray();

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: combinedSalt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:TokenKey").Value!));

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool SetPassword(UserForLoginDto userForSetPassword)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password!, passwordSalt);

            string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
                        @Email = @EmailParam, @PasswordHash = @PasswordHashParam, @PasswordSalt = @PasswordSaltParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
            sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);
            // List<SqlParameter> sqlParameters = new List<SqlParameter>();

            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForSetPassword.Email;
            // sqlParameters.Add(emailParameter);

            // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
            // passwordHashParameter.Value = passwordHash;
            // sqlParameters.Add(passwordHashParameter);

            // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
            // passwordSaltParameter.Value = passwordSalt;
            // sqlParameters.Add(passwordSaltParameter);

            return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        }
    }
}