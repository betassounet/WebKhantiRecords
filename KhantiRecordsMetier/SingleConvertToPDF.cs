using _Global.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace KhantiRecordsMetier {


    public class SingleConvertToPDF {

         // Apres avoir inclus le wrapper ( dll prince ) en reference : http://www.princexml.com/download/wrappers/
        // utilisation voir : http://www.princexml.com/doc/dotnet/
        Prince prn;
        string sPathDirectory;
        string sInitLog;

        #region Constructeur Singleton
        protected static SingleConvertToPDF instance;

        protected SingleConvertToPDF() {

            try {
                string sPathExe = "C:\\Program Files (x86)\\Prince\\Engine\\bin\\prince.exe";
                if (File.Exists(sPathExe)) {
                    // instantiate Prince by specifying the full path to the engine executable
                    prn = new Prince("C:\\Program Files (x86)\\Prince\\Engine\\bin\\prince.exe");
                    // specify the log file for error and warning messages
                    // make sure that you have write permissions for this file
                    sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                    prn.SetLog(sPathDirectory + "\\HtmlToPdf\\logPrinceHtmlToPdf.txt");
                    // apply a CSS style sheet (optional)

                    prn.AddStyleSheet(sPathDirectory + "\\css\\bootstrap-theme.css");
                    prn.AddStyleSheet(sPathDirectory + "\\css\\bootstrap-theme.min.css");
                    prn.AddStyleSheet(sPathDirectory + "\\css\\bootstrap.min.css");
                    prn.AddStyleSheet(sPathDirectory + "\\css\\UserPourPdf.css");

                    // apply an external JavaScript file (optional)
                    //prn.AddScript("C:\\docs\\js\\test.js");
                }
                else {
                    sInitLog = "Fichier exe Prince n'esiste pas en : " + sPathExe;
                    SingleLogFileAsXml.Instance().AjouteLog("Init", sInitLog);
                }
            }
            catch(Exception ex){
                sInitLog = ex.Message;
                SingleLogFileAsXml.Instance().AjouteLog("Init", "SingleConvertToPDF : Exception : "+ ex.Message);
            }

        }

        public static SingleConvertToPDF Instance() {
            if (instance == null)
                instance = new SingleConvertToPDF();
            return instance;
        }
        #endregion

        public ReponseConvertToPDF ConvertHtmlToPdf(string sHtml) {
            ReponseConvertToPDF reponseConvertToPDF = new ReponseConvertToPDF() { sMessage = "ConvertHtmlToPdf : " + sInitLog };
            string message = "";
            try {
                DateTime dtNow = DateTime.Now;
                string Sufixe = dtNow.DayOfYear.ToString() + "_" + dtNow.Hour.ToString() + "_" + dtNow.Minute.ToString() + "_" + dtNow.Second.ToString();
                string sFileBase = sPathDirectory + "\\HtmlToPdf\\base" + Sufixe + ".html";
                string sFileResultat = sPathDirectory + "\\HtmlToPdf\\resultat" + Sufixe + ".pdf";
                string sFileResultatWEB = "HtmlToPdf/resultat" + Sufixe + ".pdf";
                //string sFilecss = sPathDirectory + "\\HtmlToPdf\\CssFile.css";
                File.AppendAllText(sFileBase, sHtml);
                // convert a HTML document into a PDF file
                //prn.AddStyleSheet(sFilecss);
                prn.Convert(sFileBase, sFileResultat);
                reponseConvertToPDF.sFileResultat = sFileResultatWEB;
                SingleLogFileAsXml.Instance().AjouteLog("Pdf", "SingleConvertToPDF. ConvertHtmlToPdf : " + sFileResultatWEB);
            }
            catch(Exception ex){
                message = ex.Message;
                SingleLogFileAsXml.Instance().AjouteLog("Pdf", "SingleConvertToPDF. ConvertHtmlToPdf : Exception : " + ex.Message);
            }
            return reponseConvertToPDF;
        }

        public ReponseConvertToPDF TestConvertHtmlToPdf(string Sender, string sHtml) {
            ReponseConvertToPDF reponseConvertToPDF = new ReponseConvertToPDF() { sMessage = "ConvertHtmlToPdf : " + sInitLog, Sender = Sender + "/" + sHtml };
            string message = "";
            try {
                string sFileBase = sPathDirectory + "\\HtmlToPdf\\Test1.html";
                string sFileResultat = sPathDirectory + "\\HtmlToPdf\\resultatTest1.pdf";
                // convert a HTML document into a PDF file
                prn.Convert(sFileBase, sFileResultat);
                reponseConvertToPDF.sFileResultat = "HtmlToPdf/resultatTest1.pdf";
            }
            catch (Exception ex) {
                message = ex.Message;
                SingleLogFileAsXml.Instance().AjouteLog("Pdf", "SingleConvertToPDF. ConvertHtmlToPdf : Exception : " + ex.Message);
            }
            return reponseConvertToPDF;
        }

        public ReponseConvertToPDF WebConvertHtmlToPdf(string Sender, string sHtml) {
            SingleLogFileAsXml.Instance().AjouteLog("Pdf", "SingleConvertToPDF.Call for sender : " + Sender);
            var convert = ConvertHtmlToPdf(sHtml);
            convert.Sender = Sender;
            return convert;
        }

    }

    public class ReponseConvertToPDF {
        public long ExecutionTimeMs;
        public bool isOK;
        public int Error;
        public string Sender;
        public string sFileResultat;
        public string sMessage;
    }
}