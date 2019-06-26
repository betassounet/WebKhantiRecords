using _GLOBAL.SessionAndConfig;
using _GLOBAL.Utils.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebKhantiRecords.Controllers
{
    public class ConfigController : ApiController
    {

        [HttpPost]
        public RepActionConfigEnvironnementSession ActionOnConfigEnvironnementSession(ParamActionConfigEnvironnementSession paramActionConfigEnvironnementSession) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleSessionConfig.Instance().ActionOnConfigEnvironnementSession(paramActionConfigEnvironnementSession);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public RepActionFichierConfig ActionFichierConfig(ParamActionFichierConfig paramActionFichierConfig) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleSessionConfig.Instance().ActionFichierConfig(paramActionFichierConfig);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public RepActionTodoFile ActionTodoFile(ParamTodoFile paramTodoFile) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleSessionConfig.Instance().ActionTodoFile(paramTodoFile);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }
    }
}
