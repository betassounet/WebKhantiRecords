using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebKhantiRecords {
    /// <summary>
    /// Description résumée de UpLoadHandler
    /// </summary>
    /// 


    //Ajout nouvel element > Web > gestionnaire générique
    //appelé depuis le client par un <input type=file ... sur url http://..../upLoadHandler.ashx


    public class UpLoadHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            if (context.Request.Form.Count > 0) {
                var collection = context.Request.Form;
                for (int i = 0; i < collection.Count; i++) {
                    var key = collection.AllKeys[i];
                    var val = collection[i];
                }
            }

            if (context.Request.Files.Count > 0) {
                HttpFileCollection files = context.Request.Files;
                string fName = "";
                for (int i = 0; i < files.Count; i++) {
                    HttpPostedFile file = files[i];

                    fName = context.Server.MapPath("Files\\UpLoad\\RawXmlFiles\\"+file.FileName);
                    file.SaveAs(fName);
                }
                context.Response.Write("File/s uploaded succesfully");
            }
            else {
                context.Response.Write("No files uploaded");
            }
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}