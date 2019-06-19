using Di.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FunctionAppDi
{
    public class Function1
    {
        private readonly ILogger _log;
        private readonly IGuidService _guidService;
        private readonly IConfiguration _cfg;

        public Function1(ILogger<Function1> log, IGuidService guidService, IConfiguration cfg)
        {
            _log = log;
            _guidService = guidService;
            _cfg = cfg;
        }

        [FunctionName("DependentFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [ServiceBus("scratch", Connection = "SbConnection")]IAsyncCollector<string> queueCollector)
        {
            _log.LogInformation("Information");
            _log.LogTrace("Trace");
            _log.LogDebug("Debug");
            _log.LogCritical("Critical");
            _log.LogError("Error");
            _log.LogMetric("Metric", 9.001e+3);
            _log.LogWarning("Warning");
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var greeting = _cfg["Greeting"] ?? "Hello";
            _log.LogInformation("Guid Service says {guid}", _guidService.NewGuid());
            _log.LogInformation("{greeting}, {name}!", greeting, name);
            await queueCollector.AddAsync($"{greeting}, {name}!");
            return name != null
                ? (ActionResult)new OkObjectResult($"{greeting}, {name}!")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
