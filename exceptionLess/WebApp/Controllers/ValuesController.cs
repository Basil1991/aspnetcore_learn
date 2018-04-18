using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptionless;
using Exceptionless.Models;
using Exceptionless.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers {
    [Route("api/[controller]")]
    public class ValuesController : Controller {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get() {
            try {
                // 发送日志
                ExceptionlessClient.Default.SubmitLog("Logging made easy");

                // 你可以指定日志来源，和日志级别。
                // 日志级别有这几种: Trace, Debug, Info, Warn, Error
                ExceptionlessClient.Default.SubmitLog(typeof(Program).FullName, "This is so easy", "Info");
                ExceptionlessClient.Default.CreateLog(typeof(Program).FullName, "This is so easy", "Info").AddTags("Exceptionless").Submit();

                // 发送 Feature Usages
                ExceptionlessClient.Default.SubmitFeatureUsage("MyFeature");
                ExceptionlessClient.Default.CreateFeatureUsage("MyFeature").AddTags("Exceptionless").Submit();

                // 发送一个 404
                ExceptionlessClient.Default.SubmitNotFound("/somepage");
                ExceptionlessClient.Default.CreateNotFound("/somepage").AddTags("Exceptionless").Submit();

                // 发生一个自定义事件
                ExceptionlessClient.Default.SubmitEvent(new Event { Message = "Low Fuel", Type = "racecar", Source = "Fuel System" });

                throw new Exception("test");
                return new string[] { "value1", "value2" };
            }
            catch (Exception ex) {
                var dic = new Dictionary<string, object>();
                dic.Add("token", "123456");
                ex.ToExceptionless()
                    .SetUserIdentity(new UserInfo {
                        Name = "Jed",
                        Identity = "665544",
                        Data = new Exceptionless.Models.DataDictionary(dic)
                    })
                    .SetProperty("test", "12312312")
                    .Submit();
                return new string[] { "exceptionless" };
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id) {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value) {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
