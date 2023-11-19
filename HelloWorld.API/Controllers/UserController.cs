using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HelloWorld.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IDistributedCache _cache;

        public UserController(ILogger<UserController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(UserModel user)
        {
            var encodedCachedUser = await _cache.GetAsync(user?.Username);
            if (encodedCachedUser != null)
            {
                var cachedDataString = Encoding.UTF8.GetString(encodedCachedUser);
                var cacheUser = JsonConvert.DeserializeObject<UserModel>(cachedDataString);

                if (cacheUser?.Password == user?.Password)
                {
                    return Ok(new
                    {
                        responseCode = System.Net.HttpStatusCode.OK,
                        result = "Access Granted"
                    });
                }
            }
            return Ok(new
            {
                responseCode = System.Net.HttpStatusCode.OK,
                result = "Access Denied"
            });

        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(UserModel user)
        {
            var cachedUser = JsonConvert.SerializeObject(user);
            var encodedcachedUser = Encoding.UTF8.GetBytes(cachedUser);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            await _cache.SetAsync(user?.Username, encodedcachedUser, options);

            var encodedCachedUsers = await _cache.GetAsync("ListUsers");
            if (encodedCachedUsers != null)
            {
                var cachedDataString = Encoding.UTF8.GetString(encodedCachedUsers);
                var cachedListUsers = JsonConvert.DeserializeObject<List<UserListModel>>(cachedDataString);

                cachedListUsers?.Add(new UserListModel()
                {
                    Username = user.Username,
                    Password = user.Password,
                    RegisteredDateTime = DateTime.Now,
                    ExpiredDateTime = DateTime.Now.AddMinutes(30)
                });

                var objListUsers = JsonConvert.SerializeObject(cachedListUsers);
                var encodedcListUsers = Encoding.UTF8.GetBytes(objListUsers);

                var listUserOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24));
                await _cache.SetAsync("ListUsers", encodedcListUsers, listUserOptions);
            }
            else
            {
                var cachedListUsers = new List<UserListModel> 
                { 
                    new UserListModel()
                    {
                        Username = user.Username,
                        Password = user.Password,
                        RegisteredDateTime = DateTime.Now,
                        ExpiredDateTime = DateTime.Now.AddMinutes(30)
                    } 
                };

                var objListUsers = JsonConvert.SerializeObject(cachedListUsers);
                var encodedcListUsers = Encoding.UTF8.GetBytes(objListUsers);

                var listUserOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24));
                await _cache.SetAsync("ListUsers", encodedcListUsers, listUserOptions);
            }
            return Ok(new
            {
                responseCode = System.Net.HttpStatusCode.OK,
                result = $"User: {user.Username} Registered Success (Registered Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Expired Date: {DateTime.Now.AddHours(1):yyyy-MM-dd HH:mm:ss})"
            });
        }

        [HttpGet("ListUsers")]
        public async Task<IActionResult> ListUserAsync()
        {
            var encodedCachedUsers = await _cache.GetAsync("ListUsers");
            if (encodedCachedUsers != null)
            {
                var cachedDataString = Encoding.UTF8.GetString(encodedCachedUsers);
                var cacheUser = JsonConvert.DeserializeObject<List<UserListModel>>(cachedDataString);

                cacheUser = cacheUser?.Where(x => x.ExpiredDateTime <= DateTime.Now).ToList();
                var objListUsers = JsonConvert.SerializeObject(cacheUser);
                var encodedcListUsers = Encoding.UTF8.GetBytes(objListUsers);

                var listUserOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24));
                await _cache.SetAsync("ListUsers", encodedcListUsers, listUserOptions);

                return Ok(new
                {
                    responseCode = System.Net.HttpStatusCode.OK,
                    result = cacheUser
                });
            }

            return Ok(new
            {
                responseCode = System.Net.HttpStatusCode.OK,
                result = new List<UserListModel>()
            });
        }

        public class UserModel
        {
            public string? Username { get; set; }
            [JsonIgnore]
            public string? Password { get; set; }
            
        }

        public class UserListModel : UserModel
        {
            public DateTime RegisteredDateTime { get; set; }
            public DateTime ExpiredDateTime { get; set; }
        }
    }
}
