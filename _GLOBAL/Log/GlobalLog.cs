using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Global.Log {
    public class GlobalLog {

        SingleLogFileAsXml singleLogFileAsXml;
        //TestXmlLog testXmlLog;
        bool RecDebug = false;

        #region Constructeur Singleton
        protected static GlobalLog instance;

        protected GlobalLog() {
            singleLogFileAsXml = SingleLogFileAsXml.Instance();
        }

        public static GlobalLog Instance() {
            if (instance == null)
                instance = new GlobalLog();
            return instance;
        }
        #endregion

        public void AjouteLog(string Sender, string LogMessage, bool flgDebug = false) {
            Debug.WriteLine(LogMessage);
            if (!flgDebug)  // par defaut on est pas en mode début on enregistre.
                singleLogFileAsXml.AjouteLog(Sender, LogMessage);
            else {  // sinon on n'enregistre pas par defaut
                if (RecDebug)  // a moins que ce soit precisé ici
                    singleLogFileAsXml.AjouteLog(Sender, LogMessage);
            }
        }

        // pour faire passer tout type d'info vers le client
        public string GetInfosLog() {
            string sResult = "";
            sResult += "Le fichier log est sous :" + singleLogFileAsXml.sPathLogFile + "\n";
            sResult += "ligne suivante pour affichage dans balise pre";
            sResult += "\n\tligne suivante avec retour ligne + tabulation";
            return sResult;
        }

        public List<string> GetLogListSender() {
            TestXmlLog log;
            List<string> listSender = new List<string>();
            if ((log = SingleLogFileAsXml.Instance().GetLogInXml()) != null) {
                if (log.listXmlLineLog != null) {
                    listSender = log.listXmlLineLog.Select(c => c.Sender).Distinct().ToList();
                }
            }
            return listSender;
        }

        public List<string> GetLastLogListSender(string Sender, int NbreHeure) {
            DateTime dtNow = DateTime.Now;
            DateTime dtAvant = dtNow.AddHours(-NbreHeure);
            return GetLogFromSenderWithDate(Sender, dtAvant, dtNow);
        }

        public List<SenderAndErrors> GetLastLogListAllSender(int NbreHeure) {
            List<SenderAndErrors> listSenderAndErrors = new List<SenderAndErrors>();
            List<string> listSender = GetLogListSender();
            DateTime dtNow = DateTime.Now;
            DateTime dtAvant = dtNow.AddHours(-NbreHeure);
            foreach (var v in listSender) {
                listSenderAndErrors.Add(new SenderAndErrors() { Sender = v, CptError = GetLogFromSenderWithDate(v, dtAvant, dtNow).Count });
            }
            return listSenderAndErrors;
        }

        public List<string> GetLogFromSender(string Sender) {
            TestXmlLog log;
            List<string> listLog = new List<string>();
            if ((log = SingleLogFileAsXml.Instance().GetLogInXml()) != null) {
                if (log.listXmlLineLog != null) {
                    var q = log.listXmlLineLog.Where(c => c.Sender == Sender).ToList();
                    if (q != null) {
                        foreach (var v in q)
                            listLog.Add(v.dt.ToLongDateString() + " " + v.dt.ToLongTimeString() + "    " + v.message);
                    }
                }
            }
            return listLog;
        }

        public List<string> GetLogFromSenderWithDate(string Sender, DateTime dtMin, DateTime dtMax) {
            TestXmlLog log;
            List<string> listLog = new List<string>();
            if ((log = SingleLogFileAsXml.Instance().GetLogInXml()) != null) {
                if (log.listXmlLineLog != null) {
                    var q = log.listXmlLineLog.Where(c => c.Sender == Sender && c.dt > dtMin && c.dt < dtMax).ToList();
                    if (q != null) {
                        foreach (var v in q)
                            listLog.Add(v.dt.ToLongDateString() + " " + v.dt.ToLongTimeString() + "    " + v.message);
                    }
                }
            }
            return listLog;
        }
    }

    public class SenderAndErrors {
        public string Sender;
        public int CptError;
    }

}
