using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace _Global.Log {

    public class SingleLogFile {

        FichierLog fichierLog;
        bool DebugLog = true;
        public string sPathLogFile; // = AppDomain.CurrentDomain.BaseDirectory + "LogDebug.Log";

        #region constructeur singleton

        protected static SingleLogFile instance;

        protected SingleLogFile(string sPathName) {
            sPathLogFile = sPathName;
            fichierLog = new FichierLog(sPathLogFile, "#####################################################  Log   ###########################################################\n\r####################################################################################################################################################", 2);
        }

        public static SingleLogFile Instance(string sPathName = "") {
            if (instance == null && !string.IsNullOrEmpty(sPathName))
                instance = new SingleLogFile(sPathName);
            else {
                if (instance == null)
                    throw new System.ArgumentException("SingleLogFile premiere instance sans parametre", "mettre les parametres pour première instance");
            }
            return instance;
        }

        public void AjouteLog(string message) {
            if (DebugLog)
                fichierLog.AjouteLog(message);
        }

        public void SetDebugLogFile(bool DebugLog) {
            this.DebugLog = DebugLog;
        }

        #endregion
    }

    public class FichierLog {
        string NomFic = "";
        string ChaineInit = "";
        Mutex MutLog = new Mutex();

        public FichierLog(string NomFic, string ChaineInit) {
            try {
                this.NomFic = NomFic;
                this.ChaineInit = ChaineInit;
                EffaceLog();
            }
            catch {

            }
        }

        public FichierLog(string NomFic, string ChaineInit, bool fEfface) {
            try {
                this.NomFic = NomFic;
                this.ChaineInit = ChaineInit;
                if (fEfface) {
                    EffaceLog();
                }
                else {
                    AjouteLog(this.ChaineInit);
                }
            }
            catch {
            }
        }

        public FichierLog(string NomFic, string ChaineInit, int NombreJourHistorique) {
            try {
                this.NomFic = NomFic;
                this.ChaineInit = ChaineInit;
                FileInfo ficInf = new FileInfo(NomFic);
                if (ficInf.Exists) {
                    TimeSpan Histo = new TimeSpan(NombreJourHistorique, 0, 0, 0);
                    TimeSpan Dif = DateTime.Now - ficInf.CreationTime;
                    if (Dif > Histo) {
                        // il faut effacer le fichier
                        ficInf.CreationTime = DateTime.Now;
                        EffaceLog();
                    }
                    else {
                        AjouteLog(this.ChaineInit);
                    }
                }
                else {
                    EffaceLog();
                }
            }
            catch {
            }
        }


        // NEW ce constructeur permet la mise en forme d'un fichier de log qui suivra des règles de desérialisation XML
        public enum EModeCreation { Creation, Recopie }
        public FichierLog(string NomFic, EModeCreation ModeCreation) {
            try {
                this.NomFic = NomFic;
                FileInfo ficInf = new FileInfo(NomFic);
                if (ficInf.Exists) {
                    if (ModeCreation == EModeCreation.Creation)
                        AjouteLogXml("Demarrage", "Demarrage de l'application, fichier log existe deja");
                    if (ModeCreation == EModeCreation.Recopie)
                        AjouteLogXml("Recopie", "Recopie du fichier log avec mise en forme XML pour exploitation directe");
                }
                else {
                    AjouteLogSimple("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    AjouteLogSimple("<TestXmlLog xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                    AjouteLogSimple("\t<listXmlLineLog>");
                    AjouteLogXml("Demarrage", "Demarrage de l'application, fichier log vient d'être crée");
                }
            }
            catch {
            }
        }


        public void EffaceLog() {
            MutLog.WaitOne();
            try {
                StreamWriter stw = new StreamWriter(NomFic, false);
                stw.WriteLine(DateTime.Now.ToString() + " : " + ChaineInit);
                stw.Flush();
                stw.Close();
            }
            catch {
            }
            MutLog.ReleaseMutex();
        }

        public void AjouteLog(string Chaine) {
            MutLog.WaitOne();
            try {
                StreamWriter stw = new StreamWriter(NomFic, true);
                stw.WriteLine(DateTime.Now.ToString() + " : " + Chaine);
                stw.Flush();
                stw.Close();
            }
            catch {
            }
            MutLog.ReleaseMutex();
        }

        // NEW fonction d'écriture simple dans le fichier 

        public void AjouteLogSimple(string Chaine) {
            MutLog.WaitOne();
            try {
                StreamWriter stw = new StreamWriter(NomFic, true);
                stw.WriteLine(Chaine);
                stw.Flush();
                stw.Close();
            }
            catch {
            }
            MutLog.ReleaseMutex();
        }

        // NEW fonction pour ecriture suivant format special qui sera deserialisable
        public void AjouteLogXml(string Sender, string message) {
            string messageFormat = "<XmlLineLog Sender=\"" + Sender + "\" dt=\"" + XmlConvert.ToString(DateTime.Now) + "\" message=\"" + message + "\"/>";
            AjouteLogSimple(messageFormat);
        }


    }

}
