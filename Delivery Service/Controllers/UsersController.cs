using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Delivery_Service.Schemas.Enums;
using System.Net;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("api/account/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public UsersController(DeliveryDbContext context)
        {
            _context = context;
        }

        private string EncodePassword(string password)
        {
            using SHA256 hash = SHA256.Create();
            return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        private string GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyz");

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "Delivery",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Audience = "audience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private int LastUserId()
        {
            return _context.Users.OrderByDescending(x => x.Id).Select(x => x.Id).First();
        }

        private bool IsEmailUnique(string email)
        {
            if (_context.Users.Where(x => x.Email == email).Count() == 0)
            {
                return true;
            }
            return false;
        }

        private bool EmailExists(string email)
        {
            if (_context.Users.Where(x => x.Email == email).Count() != 0)
            {
                return true;
            }
            return false;
        }

        private bool AddressExists(Guid addressGuid)
        {
            if (_context.AsHouses.Where(x => x.Objectguid == addressGuid).Count() != 0)
            {
                return true;
            }
            return false;
        }

        private bool IsPasswordValid(string email, string password)
        {
            string encodedEnteredPassword = EncodePassword(password);
            string encodedUserPassword = _context.Users.Where(x => x.Email == email).First().Password;

            if (encodedEnteredPassword == encodedUserPassword)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public IActionResult register(UserRegisterModel data)
        {
            if (!IsEmailUnique(data.email))
            {
                Response response = new Response
                {
                    status = "Некорректный адрес эл. почты",
                    message = "Пользователь с таким адресом электронной почты уже существует."
                };

                return BadRequest(response);
            }

            if (!AddressExists(data.addressId))
            {
                Response response = new Response
                {
                    status = "Некорректный адрес проживания",
                    message = "Введённый адрес проживания отсутствует в базе данных."
                };

                return BadRequest(response);
            }

            User user = new User
            {
                Id = LastUserId() + 1,
                FullName = data.fullName,
                BirthDate = DateOnly.FromDateTime(data.birthDate),
                Gender = (data.gender == Gender.Male) ? "M" : "F",
                Phone = data.phoneNumber,
                Email = data.email,
                Address = data.addressId.ToString(),
                Password = EncodePassword(data.password)
            };

            _context.Add(user);
            _context.SaveChanges();

            TokenResponse tokenResponse = new TokenResponse
            {
                token = GenerateToken(data.email)
            };

            return Ok(tokenResponse);
        }

        [HttpPost]
        public IActionResult login(LoginCredentials credentials)
        {
            if (!EmailExists(credentials.email))
            {
                Response response = new Response
                {
                    status = "Некорректный адрес эл. почты",
                    message = "Пользователя с таким адресом электронной почты не существует."
                };

                return BadRequest(response);
            }

            if (!IsPasswordValid(credentials.email, credentials.password))
            {
                Response response = new Response
                {
                    status = "Неверный пароль",
                    message = "Введённый пароль не подходит."
                };

                return BadRequest(response);
            }

            TokenResponse tokenResponse = new TokenResponse
            {
                token = GenerateToken(credentials.email)
            };

            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpGet]
        public IActionResult profile()
        {
            return Ok();
        }
    }
}
