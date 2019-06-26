using _Global.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.BDD {

    public class SingleStatutBDD {

        public PublishStatutAllBDD publishStatutAllBDD;

        #region Constructeur Singleton
        protected static SingleStatutBDD instance;

        protected SingleStatutBDD() {
            publishStatutAllBDD = new PublishStatutAllBDD() { listStatutItemBDD = new List<StatutItemBDD>() };
        }

        public static SingleStatutBDD Instance() {
            if (instance == null)
                instance = new SingleStatutBDD();
            return instance;
        }
        #endregion


        public string GetConnexionString() {
            string sResult = "";
            try {
                // Recherche des connexion pour BDD :
                var appSetting = ConfigurationManager.AppSettings;
                foreach (var app in appSetting) {
                    var appItem = app;
                }
                var config = ConfigurationManager.ConnectionStrings;
                foreach (var s in config) {
                    string sconfig = s.ToString();
                    var liste = sconfig.Split(';').ToList();
                    string SourceProvider = "";
                    string SourceCatalogue = "";
                    foreach (var param in liste) {
                        if (param.Contains("provider connection string=\"data source=")) {
                            SourceProvider = param.Replace("provider connection string=\"data source=", "");
                        }
                        if (param.Contains("initial catalog=")) {
                            SourceCatalogue = param.Replace("initial catalog=", "");
                        }
                    }
                    if (!string.IsNullOrEmpty(SourceProvider) && !string.IsNullOrEmpty(SourceCatalogue)) {
                        StatutItemBDD statutItemBDD = new StatutItemBDD() { SourceProvider = SourceProvider, SourceCatalogue = SourceCatalogue };
                        publishStatutAllBDD.listStatutItemBDD.Add(statutItemBDD);
                        sResult += "SourceProvider : " + SourceProvider + "  \nSourceCatalogue : " + SourceCatalogue;
                    }
                    else {
                        sResult += "No SourceProvider and NoSourceCatalogue";
                    }
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("SingleStatutBDD", "GetConnexionString ex :" + ex.Message);
                sResult += "Exception : "+ ex.Message;
            }
            return sResult;
        }

        public void FirstConnexionStatut(string BddName, string EntitieName, bool IsOK, string sResult) {
            var bdd = publishStatutAllBDD.listStatutItemBDD.Where(c => c.SourceCatalogue == BddName).FirstOrDefault();
            if (bdd != null) {
                bdd.EntitieName = EntitieName;
                bdd.isOK = IsOK;
                bdd.sResult = sResult;
            }
            else {
                StatutItemBDD statutItemBDD = new StatutItemBDD() { SourceProvider = "NC", SourceCatalogue = "NC", EntitieName = EntitieName, isOK = IsOK, sResult = sResult };
                publishStatutAllBDD.listStatutItemBDD.Add(statutItemBDD);
            }
        }
    }

    public class PublishStatutAllBDD {
        public List<StatutItemBDD> listStatutItemBDD;
    }

    public class StatutItemBDD {
        public string SourceProvider;
        public string SourceCatalogue;
        public string EntitieName;
        public bool isOK;
        public string sResult;
    }

}
