using KhantiRecordsMetier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using WebKhantiRecords.NoIp;

namespace WebKhantiRecords
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            SingleKhanti.Instance();

            //NoIpService.Instance().SendNoIPUpdate();
        }
    }
}
