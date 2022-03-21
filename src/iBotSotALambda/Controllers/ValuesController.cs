using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSDataServices;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace iBotSotALambda.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        public ValuesController(IDiagnosticService diagnosticService)
        {
            this.DiagnosticService = diagnosticService;
        }

        public IDiagnosticService DiagnosticService { get; set; }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var result = DiagnosticService.Exec(nameof(ValuesController), diag =>
            {
                return new string[] { "value1", "value2" };
            });
            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
