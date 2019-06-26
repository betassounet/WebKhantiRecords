using _GLOBAL.Utils.Performance;
using KhantiRecordsMetier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebKhantiRecords.Controllers
{
    public class TestController : ApiController
    {

        [HttpPost]
        public RepTest AnalyseFichier(ParamTest param) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleKhanti.Instance().AnalyseFichier(param);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public RepDetailArtiste GetDetailArtiste(ParamDetailArtiste param) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleKhanti.Instance().GetDetailArtiste(param);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public RepAnalyseFichiers AnalyseFichiers(ParamAnalyseFichiers param) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleKhanti.Instance().AnalyseFichiers(param);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

        [HttpPost]
        public RepAnalyse2 Analyse2(ParamAnalyse2 param) {
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            var returnValue = SingleKhanti.Instance().Analyse2(param);
            returnValue.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);
            return returnValue;
        }

    }
}
