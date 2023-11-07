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
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

        private bool IsTokenSent()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();

                if (authorizationHeader.StartsWith("Bearer "))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetToken()
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Substring("Bearer ".Length);

            return token;
        }

        private string GetEmailFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(GetToken());

            return token.Claims.First(claim => claim.Type == "email").Value;
        }

        private int NewUserId()
        {
            if (_context.Users.Count() > 0)
            {
                return _context.Users.OrderByDescending(x => x.Id).Select(x => x.Id).First() + 1;
            }

            return 0;
        }

        private int NewTokenId()
        {
            if (_context.Users.Count() > 0)
            {
                return _context.BadTokens.OrderByDescending(x => x.Id).Select(x => x.Id).First() + 1;
            }

            return 0;
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

        private bool IsTokenBad()
        {
            if (_context.BadTokens.Where(x => x.Value == GetToken()).Count() != 0)
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
                Id = NewUserId(),
                FullName = data.fullName,
                BirthDate = data.birthDate,
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
        [HttpPost]
        public IActionResult logout()
        {
            if (!IsTokenSent())
            {
                return Unauthorized();
            }

            if (IsTokenBad())
            {
                return Forbid();
            }

            BadToken token = new BadToken
            {
                Id = NewTokenId(),
                Value = GetToken()
            };

            _context.Add(token);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IActionResult profileget()
        {
            if (!IsTokenSent())
            {
                return Unauthorized();
            }

            if (IsTokenBad())
            {
                return Forbid();
            }

            string email = GetEmailFromToken();
            User userDb = _context.Users.Where(x => x.Email == email).First();

            UserDto userProfile = new UserDto
            {
                id = userDb.Id,
                fullName = userDb.FullName,
                birthDate = userDb.BirthDate,
                gender = (userDb.Gender == "M") ? Gender.Male : Gender.Female,
                address = new Guid(userDb.Address),
                email = userDb.Email,
                phoneNumber = userDb.Phone
            };

            return Ok(userProfile);
        }

        [Authorize]
        [HttpPut]
        public IActionResult profileedit(UserEditModel data)
        {
            if (!IsTokenSent())
            {
                return Unauthorized();
            }

            if (IsTokenBad())
            {
                return Forbid();
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

            string email = GetEmailFromToken();
            int userId = _context.Users.Where(x => x.Email == email).First().Id;
            User user = _context.Users.Find(userId);

            user.FullName = data.fullName;
            user.BirthDate = data.birthDate;
            user.Gender = (data.gender == Gender.Male) ? "M" : "F";
            user.Address = data.addressId.ToString();
            user.Phone = data.phoneNumber;

            _context.SaveChanges();

            return Ok();
        }
    }
}
