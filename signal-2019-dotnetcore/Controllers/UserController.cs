using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using signal_2019_dotnetcore.Extensions;
using signal_2019_dotnetcore.Models;

namespace signal_2019_dotnetcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUserRepository _userRepository;

        public UserController(IHttpClientFactory clientFactory, IUserRepository userRepository)
        {
            _clientFactory = clientFactory;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (_userRepository.Exists(request.Username))
                return BadRequest($"User '{request.Username}' already exists.");

            var form = new Dictionary<string, string>()
            {
                {"user[email]",request.Username},
                {"user[cellphone]",request.PhoneNumber},
                {"user[country_code]",request.CountryCode},
            };

            var client = _clientFactory.CreateClient("authy");

            var response = await client.PostFormAsJObjectAsync("/protected/json/users/new", form);
            var result = response.ToObject<RegistrationResult>();

            if (!result.Success)
                return BadRequest($"Error registering user: {result.Message}");

            var newUser = new AuthyUser
            {
                Username = request.Username,
                CountryCode = request.CountryCode,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password,
                Id = result.User.Id
            };

            if(!_userRepository.AddUser(newUser))
                return BadRequest("Unable to add new user to database.");

            HttpContext.Session.SetUsername(newUser.Username);

            return Ok();
        }

        [HttpGet("/api/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok();
        }

        [HttpPost("/api/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if(!_userRepository.Exists(request.Username))
                return Unauthorized();

            var user = _userRepository.GetUser(request.Username);

            if(user.PasswordHash != request.Password)
                return Unauthorized();

            HttpContext.Session.SetUsername(request.Username);

            return Ok();
        }

        public class LoginRequest
        {
            [Required, JsonProperty(PropertyName = "username")]
            public string Username {get;set;}
            
            [Required, JsonProperty(PropertyName = "password")]
            public string Password {get;set;}
        }

        public class RegisterRequest
        {
            [Required, JsonProperty(PropertyName = "username")]
            public string Username { get; set; }

            [Required, JsonProperty(PropertyName = "phone_number")]
            public string PhoneNumber { get; set; }

            [Required, JsonProperty(PropertyName = "country_code")]
            public string CountryCode { get; set; }

            [Required, JsonProperty(PropertyName = "password")]
            public string Password { get; set; }
        }

        public class RegistrationResult
        {
            public string Message { get; set; }
            public RegistrationResultUser User { get; set; }
            public bool Success { get; set; }
        }

        public class RegistrationResultUser
        {
            public string Id { get; set; }
        }
    }
}