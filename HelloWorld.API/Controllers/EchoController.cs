using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HelloWorld.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            dynamic respone = new ExpandoObject();
            respone.HostName = Environment.MachineName;
            respone.LocalIpAddress = Request.HttpContext.Connection.LocalIpAddress.ToString();
            respone.RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            respone.RequestHeaders = Request.HttpContext.Request.Headers;
            respone.EnvVars = Environment.GetEnvironmentVariables();
            return JsonConvert.SerializeObject(respone);
        }
    }
}