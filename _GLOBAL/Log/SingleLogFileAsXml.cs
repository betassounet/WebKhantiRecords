using _GLOBAL.Utils.Serialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _Global.Log {

    // Ici on va se servir de la classe qui gere le log file mais on va ecrire dans ce fichier des lignes succeptibles d'être désérialiser dans une classe
    // La classe serialisable est TestXmlLog

    public class SingleLogFileAsXml {

        FichierLog fichierLog;
        public string sPathLogFile = AppDomain.CurrentDomain.BaseDirectory + "LogDebugAsXML.Log";

        #region constructeur singleton

        protected static SingleLogFileAsXml instance;

        protected SingleLogFileAsXml() {
            if (fichierLog == null)
                fichierLog = new FichierLog(sPathLogFile, FichierLog.EModeCreation.Creation);  // ce constructeur va formatter le début du fichier log si necessaire
        }

        public static SingleLogFileAsXml Instance() {
            if (instance == null)
                instance = new SingleLogFileAsXml();
            return instance;
        }

        public void AjouteLog(string Sender, string message) {
            fichierLog.AjouteLogXml(Sender, message);
        }

        // ICI on recupére le fichier dans un string
        // on formatte le string pour le terminer avec les balises xml necessaire 
        // puis on désérialise
        TestXmlLog testXmlLog;
        public TestXmlLog GetLogInXml(string sPath = "") {
            if (sPath == "")
                sPath = AppDomain.CurrentDomain.BaseDirectory + "LogDebugAsXML.Log";
            if (File.Exists(sPath)) {
                StreamReader streamReader = new StreamReader(sPath);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                text += "</listXmlLineLog></TestXmlLog>";
                try {
                    testXmlLog = Serialization.DeserializeObject_UTF8<TestXmlLog>(text);
                }
                catch (Exception ex) {
                    testXmlLog = new TestXmlLog();
                    testXmlLog.listXmlLineLog.Add(new XmlLineLog() { dt = DateTime.Now, Sender = "InternalLOG", message = "Exception : " + ex.Message });
                    return testXmlLog;
                }
            }
            else {
                testXmlLog = new TestXmlLog();
                testXmlLog.listXmlLineLog.Add(new XmlLineLog() { dt = DateTime.Now, Sender = "InternalLOG", message = "File Log not exist  "});
            }
            return testXmlLog;
        }

        public RepOperationLog OperationLog(ParamOperationLog paramOperationLog) {
            RepOperationLog repOperationLog = new RepOperationLog();
            try {
                if (paramOperationLog.sAction == "Delete") {
                    string sPath = AppDomain.CurrentDomain.BaseDirectory + "LogDebugAsXML.Log";
                    if (File.Exists(sPath)) {
                        DateTime dt = DateTime.Now;
                        string Suffixe = dt.DayOfYear.ToString() + "-" + dt.Hour.ToString() + "-" + dt.Minute.ToString() + "-" + dt.Second.ToString();
                        string sPathSave = AppDomain.CurrentDomain.BaseDirectory + "LogDebugAsXML" + Suffixe + "Log.xml";
                        //File.Copy(sPath, sPathSave);
                        ConvertToXMLCompliantAndSaveFileAs(sPathSave);  // on sauve le log direct en xml
                        File.Delete(sPath);
                        // On recree le fichier log..
                        fichierLog = new FichierLog(sPathLogFile, FichierLog.EModeCreation.Creation);  // ce constructeur va formatter le début du fichier log si necessaire
                        repOperationLog.sMessage = "Copy Log to : " + sPathSave;
                    }
                }
                if (paramOperationLog.sAction == "TestLog") {
                    AjouteLog("SenderTest", "TestLog");
                    repOperationLog.sMessage = "Ajout TestLog ";
                }
            }
            catch (Exception ex) {
                repOperationLog.sMessage = "OperationLog : "+ ex.Message;
            }
            return repOperationLog;
        }


        public ResultLogFile GetLogsFiltre(ParamFiltreLog paramFiltreLog) {
            ResultLogFile resultLogFile = new ResultLogFile() { listSenders = new List<string>(), listXmlLineLog = new List<XmlLineLog>() };
            TestXmlLog testXmlLog = GetLogInXml();

            List<XmlLineLog> listXmlLineLog = testXmlLog.listXmlLineLog;
            resultLogFile.listSenders.Add(""); // choix vide
            resultLogFile.listSenders = listXmlLineLog.Select(c => c.Sender).ToList().Distinct().ToList();
            
            if (paramFiltreLog.IsFiltreDate) {
                DateTime dtFiltreMin = paramFiltreLog.dtFiltre.Date;
                DateTime dtFiltreMax = dtFiltreMin.AddDays(1);
                listXmlLineLog = listXmlLineLog.Where(c => c.dt >= dtFiltreMin && c.dt <= dtFiltreMax).ToList();
            }
            if (paramFiltreLog.IsFiltreSender) {
                if (!string.IsNullOrEmpty(paramFiltreLog.SenderFiltre))
                    listXmlLineLog = listXmlLineLog.Where(c => c.Sender == paramFiltreLog.SenderFiltre).ToList();
                if (!string.IsNullOrEmpty(paramFiltreLog.SenderExclureFiltre))
                    listXmlLineLog = listXmlLineLog.Where(c => c.Sender != paramFiltreLog.SenderExclureFiltre).ToList();
            }
            resultLogFile.listXmlLineLog = listXmlLineLog;
            return resultLogFile;
        }

        public string GetInfoFileLog() {
            if (testXmlLog != null) {
                string s = "";
                s += "Desérialisation OK :\n Il y a : " + testXmlLog.listXmlLineLog.Count.ToString() + " messages tout sender confondus ";
            }
            return "Instance desérialisé = Null ";
        }

        // Ici on se fait une copy du fichier de log que l'on formatte pour etre directement exploitable en desérialisation
        public void ConvertToXMLCompliantAndSaveFileAs(string sPathDestination, string sPathOrigine = "") {
            if (sPathOrigine == "") {
                sPathOrigine = sPathLogFile;
            }
            File.Copy(sPathOrigine, sPathDestination, true);
            FichierLog fichierLogDestination = new FichierLog(sPathDestination, FichierLog.EModeCreation.Recopie);  // ce constructeur va formatter le début du fichier log si necessaire
            fichierLogDestination.AjouteLogSimple("\t</listXmlLineLog>");
            fichierLogDestination.AjouteLogSimple("</TestXmlLog>");
            //string sRep = "";
            //TestXmlLog test = (TestXmlLog)Serialization.DeSerializeObjectFromPath(typeof(TestXmlLog), sPathDestination, ref sRep);
        }

        #endregion
    }

    public class ParamOperationLog {
        public string sAction;
    }
    public class RepOperationLog {
        public long ExecutionTimeMs;
        public string sMessage;
    }

    public class ParamFiltreLog {
        public bool IsGetSenders;
        public bool IsFiltreDate;
        public bool IsFiltreSender;
        public DateTime dtFiltre;
        public string SenderFiltre;
        public string SenderExclureFiltre;
    }

    public class ResultLogFile {
        public long ExecutionTimeMs;
        public List<string> listSenders;
        public List<XmlLineLog> listXmlLineLog;
    }

    // Classe qui se serialise sous la forme :

    //    <?xml version="1.0" encoding="utf-8"?>
    //<TestXmlLog xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    //  <listXmlLineLog>
    //    <XmlLineLog Sender="toto" dt="2012-04-16T10:46:36.1359107+02:00" message="Message de toto " />
    //    <XmlLineLog Sender="titi" dt="2012-04-16T10:46:36.1359107+02:00" message="Message de titi " />
    //  </listXmlLineLog>
    //</TestXmlLog>

    public class TestXmlLog {
        public TestXmlLog() {
            if (listXmlLineLog == null)
                listXmlLineLog = new List<XmlLineLog>();
        }
        public List<XmlLineLog> listXmlLineLog { get; set; }
    }

    public class XmlLineLog {
        [XmlAttribute]
        public string Sender { get; set; }
        [XmlAttribute]
        public DateTime dt { get; set; }
        [XmlAttribute]
        public string message { get; set; }
    }


}
