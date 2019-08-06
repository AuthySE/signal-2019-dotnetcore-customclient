using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using signal_2019_dotnetcore.Extensions;

namespace signal_2019_dotnetcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthyController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUserRepository _userRepository;

        public AuthyController(IHttpClientFactory clientFactory, IUserRepository userRepository)
        {
            _clientFactory = clientFactory;
            _userRepository = userRepository;
        }

        [HttpPost("sms")]
        public async Task<IActionResult> SendSMS()
        {
            var username = HttpContext.Session.GetUsername();

            if(string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _userRepository.GetUser(username);
            var client = _clientFactory.CreateClient("authy");
            var uri = $"/protected/json/sms/{user.Id}?force=true";
            
            var result = await client.GetAsJObjectAsync(uri);

            return Ok(result);
        }

        [HttpPost("voice")]
        public async Task<IActionResult> SendVoice()
        {
            var username = HttpContext.Session.GetUsername();

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _userRepository.GetUser(username);
            var client = _clientFactory.CreateClient("authy");
            var uri = $"/protected/json/call/{user.Id}?force=true";

            var result = await client.GetAsJObjectAsync(uri);

            return Ok(result);
        }

        [HttpPost("onetouch")]
        public async Task<IActionResult> OneTouch()
        {
            var username = HttpContext.Session.GetUsername();

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _userRepository.GetUser(username);
            var client = _clientFactory.CreateClient("authy");
            var uri = $"/onetouch/json/users/{user.Id}/approval_requests";
            var body = new Dictionary<string,string>()
            {
                {"message","Customize this push notification with your messaging"},
                {"details", null}
            };

            var result = await client.PostAsJObjectAsync(uri, body);
            var uuid = result["approval_request"].Value<string>("uuid");

            HttpContext.Session.SetString("onetouch-uuid", uuid);

            return Ok(result);
        }

        [HttpPost("onetouchstatus")]
        public async Task<IActionResult> OneTouchStatus()
        {
            var username = HttpContext.Session.GetUsername();

            if (string.IsNullOrEmpty(username))
                return Unauthorized();
            
            var uuid = HttpContext.Session.GetString("onetouch-uuid");
            if(string.IsNullOrEmpty(uuid))
                return BadRequest("No uuid attached to session.");
                
            var user = _userRepository.GetUser(username);
            var client = _clientFactory.CreateClient("authy");
            var uri = $"/onetouch/json/approval_requests/{uuid}";
            var result = await client.GetAsJObjectAsync(uri);

            var status = result["approval_request"].Value<string>("status");

            if(status == "approved")
                HttpContext.Session.SetString("Authy",status);

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
        {
            var username = HttpContext.Session.GetUsername();

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _userRepository.GetUser(username);
            var client = _clientFactory.CreateClient("authy");
            var uri = $"/protected/json/verify/{request.Token}/{user.Id}";
            var result = await client.GetAsJObjectAsync(uri);

            var success = result.Value<bool>("success");

            if(success)
                HttpContext.Session.SetString("Authy", "approved");

            return Ok(result);
        }

        public class VerifyRequest
        {
            [Required, JsonProperty(PropertyName = "token")]
            public string Token {get;set;}
        }

    }
}