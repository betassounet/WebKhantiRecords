using _Global.Log;
using _GLOBAL.globalModels.ConfigsClients;
using _GLOBAL.globalModels.SessionConfigXml;
using _GLOBAL.Utils.Serialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.SessionAndConfig {

    // Gestion et partage de fichiers configs
    // Sauvegarde des fichiers de config sous la baseDirectory de l'appli : AppDomain.CurrentDomain.BaseDirectory
    // et sous un répertoire ConfigFiles dans lequel on trouve un premier fichier de config qui donne des environnements de dev possibles et si oui ou non on branche sur une config client donnée
    // Chaque config client est classée par répertoire par client sous le répertoire configFiles.
    // On doit trouver pour une config client en premier la config ConfigEnvironnementClient qui donne les environnements de dev / production et aussi les autres possibles fichiers de config..

    
    public partial class SingleSessionConfig {

        EnvironnementProjet environnementProjet;
        EnvironnementGlobal environnementGlobal;

        public EnvironnementExecution EnvironnementExecutionActif;

        public string ProjectClientName = "";

        #region Constructeur Singleton

        protected static SingleSessionConfig instance;

        protected SingleSessionConfig() {
            environnementProjet = new EnvironnementProjet() {
                ClientName = "",
                IsClientSelectedOK = false,
                sPathBase = "",
                sPathBaseClient = ""
            };

            environnementGlobal = new EnvironnementGlobal() { listClientRepertoire = new List<ClientRepertoire>() };
        }

        public static SingleSessionConfig Instance() {
            if (instance == null)
                instance = new SingleSessionConfig();
            return instance;
        }

        #endregion

        // retourne le répertoire pour les fichiers de config pour la base ou pour un client donné
        private string GetDirectoryFile(string NomClient="") {
            environnementProjet.sPathBase = AppDomain.CurrentDomain.BaseDirectory;
            environnementProjet.sPathBase += "configFiles\\";
            environnementProjet.sPathBaseClient = environnementProjet.sPathBase; // par defaut
            NomClient = NomClient.Trim();
            NomClient = NomClient.Replace(' ', '_');
            if (!string.IsNullOrEmpty(NomClient)) {
                environnementProjet.sPathBaseClient = environnementProjet.sPathBase + NomClient;
                if (!Directory.Exists(environnementProjet.sPathBaseClient)) {
                    try {
                        Directory.CreateDirectory(environnementProjet.sPathBaseClient);
                        SingleLogFileAsXml.Instance().AjouteLog("SingleSessionConfig", "@GetDirectoryFile : Creation nouveau répertoire : " + environnementProjet.sPathBaseClient);
                    }
                    catch(Exception ex){
                        SingleLogFileAsXml.Instance().AjouteLog("SingleSessionConfig", "@GetDirectoryFile : PB : " + ex.Message);
                    }
                }
                else {
                    GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@GetDirectoryFile : Environnement Global Existe : " + environnementProjet.sPathBaseClient);
                }
                environnementProjet.sPathBaseClient += "\\";
                return environnementProjet.sPathBaseClient;
            }
            return environnementProjet.sPathBase;
        }

        public string FirstInit() {
            ProjectClientName = "";

            string sPathBase = GetDirectoryFile();  // get répertoire racine pour les configs

            try {
                // exploration des répertoires et fichiers pour infos
                var listDirectoryClient = Directory.GetDirectories(sPathBase).ToList();
                string sTemp = "";
                string sTempFile = "";
                foreach (var d in listDirectoryClient) {
                    sTemp = d.Replace(sPathBase, "");
                    ClientRepertoire clientRepertoire = new ClientRepertoire() { NomRepertoire = sTemp, listFichiers = new List<string>() };
                    var listFiles = Directory.GetFiles(d).ToList();
                    foreach (var f in listFiles) {
                        sTempFile = f.Replace(d + "\\", "");
                        clientRepertoire.listFichiers.Add(sTempFile);
                    }
                    environnementGlobal.listClientRepertoire.Add(clientRepertoire);
                }

                // tentative de récupérations des 2 principaux fichiers de config au demarrage ConfigEnvironnementSession et ConfigEnvironnementClient s'il y a lieu
                string sPathMainConfig = sPathBase + "ConfigEnvironnementSession.xml";

                if (File.Exists(sPathMainConfig)) {
                    GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@FirstInit : Config Environnement Client EXISTE : " + sPathMainConfig);
                }
                else {
                    GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@FirstInit : Config Environnement Client N'EXISTE PAS : " + sPathMainConfig);
                }

                configEnvironnementSession = GetXmlEnvironnementSession(sPathMainConfig);

                if (configEnvironnementSession != null) {
                    GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@FirstInit : configEnvironnementSession OK / AutoStart : " + configEnvironnementSession.AutoStartSurClient + "/  client Name : " + configEnvironnementSession.AutoStartClientName);
                }
                else {
                    GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@FirstInit : configEnvironnementSession = NULL !!");
                }

                if (configEnvironnementSession != null && configEnvironnementSession.AutoStartSurClient && !string.IsNullOrEmpty(configEnvironnementSession.AutoStartClientName)) {

                    ProjectClientName = configEnvironnementSession.AutoStartClientName;

                    DummyModifMainEnvironnementConfig(configEnvironnementSession, sPathMainConfig); // pour mise a jour eventuelle

                    sPathBase = GetDirectoryFile(configEnvironnementSession.AutoStartClientName);
                    environnementProjet.ClientName = configEnvironnementSession.AutoStartClientName;
                    EnvironnementExecutionActif = configEnvironnementSession.listEnvironnementExecutionDefault.Where(c => c.IsDefault).FirstOrDefault(); // set de l'environnement d'exécution actif
                    // Recup / Init des fichiers dans rep client..
                    // a ajuster en fonction utilité des fichiers
                    string sPathClientConfig = sPathBase + "ConfigEnvironnementClient.xml";
                    configEnvironnementClient = GetXmlConfigEnvironnementClient(sPathClientConfig);
                    if (configEnvironnementClient != null && configEnvironnementClient.listTypeFileConfigNeeded != null) {

                        DummyModifClientEnvironnementConfig(configEnvironnementClient, sPathClientConfig); // pour mise a jour eventuelle

                        environnementProjet.IsClientSelectedOK = true;
                        environnementProjet.listTypeFileConfigNeeded = new List<TypeFileConfigNeeded>();
                        environnementProjet.listTypeFileConfigNeeded.Add(new TypeFileConfigNeeded() { LibelleFile = "Config", TypeFileConfig = "XmlConfigEnvironnementClient", NameFileConfig = "ConfigEnvironnementClient.xml" });
                        environnementProjet.listTypeFileConfigNeeded.AddRange(configEnvironnementClient.listTypeFileConfigNeeded);

                        foreach (var conf in configEnvironnementClient.listTypeFileConfigNeeded) {
                            LoadConfigFile(conf);
                        }

                        var environnemntExecutionClient = configEnvironnementClient.listEnvironnementExecution.Where(c => c.IsDefault).FirstOrDefault(); // set de l'environnement d'exécution actif
                        if (environnemntExecutionClient != null)
                            EnvironnementExecutionActif = environnemntExecutionClient;
                    }
                }
                else {
                    GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@FirstInit : Il n'y a pas de Config Environnement : " + sPathMainConfig);
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@FirstInit : Exception : " + ex.Message);
            }
            return ProjectClientName;
        }


        // Fonctions pour mise a jours des 2 principaux fichiers de config si necessaire 
        // One shot puis dévalider 
        private void DummyModifMainEnvironnementConfig(XmlEnvironnementSession config, string sPath) {
            //foreach (var v in config.listEnvironnementExecutionDefault) {
            //    v.connexionOrpheaWeb = new ConnexionOrpheaWeb() { ToConnect = false, UrlApi = "http://localhost:55673/" };
            //}
            //FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlEnvironnementSession>(config, sPath);
        }

        private void DummyModifClientEnvironnementConfig(XmlConfigEnvironnementClient config, string sPath) {
            //foreach (var v in config.listEnvironnementExecution) {
            //    v.connexionOrpheaWeb = new ConnexionOrpheaWeb() { ToConnect = false, UrlApi = "http://localhost:55673/" };
            //}
            //FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigEnvironnementClient>(config, sPath);
        }
            

        // chgargement d'une config en fonction du type de config..
        // ces fichiers de config peuvent être supprimé ou abonder...
        private void LoadConfigFile(TypeFileConfigNeeded typeFileConfigNeeded) {
            switch (typeFileConfigNeeded.TypeFileConfig) {

                case "XmlConfigClientData":
                    configClientData = InitConfigClientData(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;

                case "XmlConfigDataVariables":
                    configDataVariables = InitConfigDataVariables(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;

                case "XmlConfigInputs":
                    configInputs = InitConfigInputs(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;

                case "XmlConfigClientImpression":
                    configClientImpression = InitConfigClientImpression(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;

                case "XmlConfigInputExcelFile":
                    configInputExcelFile = InitXmlConfigInputExcelFile(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;

                case "XmlConfigGlobalVariable":
                    sPathConfigGlobalVariable = environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig;
                    configGlobalVariable = InitXmlConfigGlobalVariable(sPathConfigGlobalVariable);
                    break;

                case "XmlConfigExtraParamClient":
                    sPathConfigExtraParamClient = environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig;
                    configExtraParamClient = InitXmlConfigExtraParamClient(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;

                case "XmlScenarioData":
                    sPathConfigXmlScenarioData = environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig;
                    xmlScenarioData = InitXmlScenarioData(environnementProjet.sPathBaseClient + typeFileConfigNeeded.NameFileConfig);
                    break;


                default:
                    break;
            }
        }

        #region Special BDD

        public ConnexionDB GetConnexionDB(string DBConnectionName) {
            if (EnvironnementExecutionActif != null) {
                return EnvironnementExecutionActif.listConnexionDB.Where(c => c.NomConnectionBDD == DBConnectionName).FirstOrDefault();
            }
            return null;
        }

        public XmlConfigInputs GetXmlConfigInput() {
            return configInputs;
        }

        public XmlConfigInputExcelFile GetXmlConfigInputExcelFile(){
            return configInputExcelFile;
        }

        #endregion

        #region Special OrpheaWeb

        public List<ConnexionOrpheaWeb> GetConnexionOrpheaWeb() {
            if (EnvironnementExecutionActif != null) {
                return EnvironnementExecutionActif.listConnexionOrpheaWeb;
            }
            return null;
        }

        #endregion

        #region Special ConfigPrint

        public ConnexionServeurPrint GetConnexionServeurPrint() {
            if (EnvironnementExecutionActif != null) {
                return EnvironnementExecutionActif.connexionServeurPrint;
            }
            return null;
        }

        public XmlConfigClientImpression GetConfigClientImpression() {
            return configClientImpression;
        }
        #endregion

        #region Special ConfigGlobalVariable
        string sPathConfigGlobalVariable = "";

        public XmlConfigGlobalVariable GetConfigGlobalVariable() {
            return configGlobalVariable;
        }

        public string SaveConfigGlobalVariable() {
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigGlobalVariable>(configGlobalVariable, sPathConfigGlobalVariable);
            return sReponse;
        }

        #endregion

        #region Special pour configExtraParamClient
        string sPathConfigExtraParamClient = "";
        string sPathConfigXmlScenarioData = "";

        public XmlConfigExtraParamClient GetConfigExtraParamClient() {
            return configExtraParamClient;
        }

        public string SaveConfigExtraParamClient() {
            string sReponse = "Config = null";
            if (configExtraParamClient != null) {
                sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigExtraParamClient>(configExtraParamClient, sPathConfigExtraParamClient);
            }
            return sReponse;
        }

        #endregion

        #region Special echantillon de données ( Cadaval )

        public void ClearScenarioDataArticle() {
            if (xmlScenarioData != null) {
                xmlScenarioData.listItemArticleOrphea.Clear();
            }
        }

        public void ClearScenarioDataApport() {
            if (xmlScenarioData != null) {
                xmlScenarioData.listXmlParamInsertInApport.Clear();
            }
        }

        public void AddEchantillonApportERP(XmlParamInsertInApport xmlParamInsertInApport) {
            if (xmlScenarioData != null && xmlScenarioData.listXmlParamInsertInApport!= null){
                xmlScenarioData.listXmlParamInsertInApport.Add(xmlParamInsertInApport);
            }
        }

        public void AddEchantillonArticleERP(XmlItemArticleOrphea xmlItemArticleOrphea) {
            if (xmlScenarioData != null && xmlScenarioData.listItemArticleOrphea != null) {
                xmlScenarioData.listItemArticleOrphea.Add(xmlItemArticleOrphea);
            }
        }

        public string SaveScenarioData() {
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlScenarioData>(xmlScenarioData, sPathConfigXmlScenarioData);
            return sReponse;
        }


        public List<XmlParamInsertInApport> GetListEchantillonApportERP() {
            if (xmlScenarioData != null)
                return xmlScenarioData.listXmlParamInsertInApport;
            else
                return null;
        }

        public List<XmlItemArticleOrphea> GetListArticleOrpheaERP() {
            if (xmlScenarioData != null)
                return xmlScenarioData.listItemArticleOrphea;
            else
                return null;
        }

        #endregion

        #region Gestion des structures XML speciales via le manager de config et pour utiliser au mieux le stockage dans les bons répertoires sans rajouter du surplus partout

        // ICI le gestionnaire de config fait profiter de sa gestion des répertoires dédiée à des client..
        // On rappelle qu'il y a un répertoire par client ( avec le nom du client si possible ) et que tous les fichiers relatifs au fonctionneent de l'appli devraient se trouver dans ces répertoires..
        // Parmis les fichiers de configs il y en a qui sont généralistes.. ( connexions BDD, environnement travail, configs génériques..)
        // pour les fichier XML speciaux a un client ( classe XML speciale sérialisable ).. Le gestionnaire d config ne doit pas specialement connaitre cette classe..
        // c'est les modules dans l'appli concernées par son utilisation qui connaissent et qui peuvent appeller des 2 fonctions suivantes pour récupèration et sauvegarde de leur fichier special..
        // fichier qui est mis dans le même répertoire que  tous les autres fichiers..

        public T GetDedicatedStructInCurrentDirectory<T>(string sShortFileName) {
            string sPathComplet = environnementProjet.sPathBaseClient + sShortFileName;
            return (T)FastSerialisation.Instance().GetSaveStructInCurrentDirectory<T>(sPathComplet);
        }

        public string SaveDedicatedStructInCurrentDirectory<T>(T dedicatedStruct, string sShortFileName) {
            string sPathComplet = environnementProjet.sPathBaseClient + sShortFileName;
            return FastSerialisation.Instance().SaveStructInCurrentDirectory<T>(dedicatedStruct, sPathComplet);
        }

        #endregion

    }

    public class EnvironnementProjet {
        public string sPathBase;
        public string sPathBaseClient;
        public bool IsClientSelectedOK;
        public string ClientName;
        public string sMessage;
        public List<TypeFileConfigNeeded> listTypeFileConfigNeeded;
    }

    public class EnvironnementGlobal {
        public List<ClientRepertoire> listClientRepertoire;
    }

    public class ClientRepertoire {
        public string NomRepertoire;
        public List<string> listFichiers;
    }
}
