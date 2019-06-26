using _Global.Log;
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
    public class MaintenanceController : ApiController
    {

        [HttpPost]
        public ResultLogFile GetLogsFiltre(ParamFiltreLog paramFiltreLog) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleLogFileAsXml.Instance().GetLogsFiltre(paramFiltreLog);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public RepOperationLog OperationLog(ParamOperationLog paramOperationLog) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleLogFileAsXml.Instance().OperationLog(paramOperationLog);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public ResultPerformanceLogger GetSetPerformanceLogger(ParamPerformanceLogger paramPerformanceLogger) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SinglePerformanceLogger.Instance().GetSetPerformanceLogger(paramPerformanceLogger);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }


    }
}
