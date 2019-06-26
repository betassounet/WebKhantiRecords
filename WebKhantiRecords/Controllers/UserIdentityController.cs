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
    public class UserIdentityController : ApiController {

        [HttpPost]
        public ReponseUserIdentity GetMyUserIdentity(ParamUserIdentity Param) {
            ReponseUserIdentity reponseUserIdentity = new ReponseUserIdentity();
            var sw = SinglePerformanceLogger.Instance().StartStopWatch(HttpContext.Current.Request);
            reponseUserIdentity.UserHostAddress = HttpContext.Current.Request.UserHostAddress;
            reponseUserIdentity.UserHostName = HttpContext.Current.Request.UserHostName;
            reponseUserIdentity.UserAgent = HttpContext.Current.Request.UserAgent;
            reponseUserIdentity.UserLanguage = HttpContext.Current.Request.UserLanguages.ToList();
            reponseUserIdentity.ExecutionTimeMs = SinglePerformanceLogger.Instance().StopStopWatch(sw);

            // ICI possibilité de router le client sur une page en fonction de son identité
            // L'appel de cette fonction étant fait au demarrage du site sur le client sur premier controller traversé ( appCtrl en charge de la navigation ) 
            if (reponseUserIdentity.UserHostAddress == "192.168.1.11") {
                reponseUserIdentity.IsAutoRoute = true;
                reponseUserIdentity.AutoRouteURL = "LigneGenerique1";
            }
            if (reponseUserIdentity.UserHostAddress == "::1") {  // Ici c'est localhost
                //reponseUserIdentity.IsAutoRoute = true;
                //reponseUserIdentity.AutoRouteURL = "LigneGenerique/3/ligne 3/appel direct via config";
            }

            return reponseUserIdentity;
        }

    }

    public class ReponseUserIdentity {
        public long ExecutionTimeMs;
        public string UserAgent;
        public string UserHostAddress;
        public string UserHostName;
        public List<string> UserLanguage;
        public bool IsAutoRoute;
        public string AutoRouteURL;
    }

    public class ParamUserIdentity {
        public string action;
    }

}
