using _GLOBAL.globalModels.ConfigsClients;
using _GLOBAL.globalModels.SessionConfigXml;
using _GLOBAL.Utils.Serialisation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.SessionAndConfig {

    public partial class SingleSessionConfig {


        public RepActionConfigEnvironnementSession ActionOnConfigEnvironnementSession(ParamActionConfigEnvironnementSession paramActionConfigEnvironnementSession) {
            RepActionConfigEnvironnementSession repActionConfigEnvironnementSession = new RepActionConfigEnvironnementSession();
            repActionConfigEnvironnementSession.environnementProjet = environnementProjet;
            repActionConfigEnvironnementSession.environnementGlobal = environnementGlobal;
            if (paramActionConfigEnvironnementSession != null && paramActionConfigEnvironnementSession.sAction != null) {
                if (paramActionConfigEnvironnementSession.sAction == "Get") {
                    repActionConfigEnvironnementSession.configEnvironnementSession = configEnvironnementSession;
                }
                if (paramActionConfigEnvironnementSession.sAction == "Set") {
                    if (paramActionConfigEnvironnementSession.configEnvironnementSession != null && paramActionConfigEnvironnementSession.configEnvironnementSession is XmlEnvironnementSession) {
                        configEnvironnementSession = paramActionConfigEnvironnementSession.configEnvironnementSession;
                        string statutMes = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlEnvironnementSession>(configEnvironnementSession, environnementProjet.sPathBase + "ConfigEnvironnementSession.xml");
                        repActionConfigEnvironnementSession.environnementProjet.sMessage = "Save configEnvironnementSession : " + statutMes;
                        repActionConfigEnvironnementSession.configEnvironnementSession = configEnvironnementSession;
                    }
                }
                if (paramActionConfigEnvironnementSession.sAction == "SetNewClient") {
                    GetDirectoryFile(paramActionConfigEnvironnementSession.newClientName);
                }

            }
            return repActionConfigEnvironnementSession;
        }


        public RepActionFichierConfig ActionFichierConfig(ParamActionFichierConfig paramActionFichierConfig) {
            
            RepActionFichierConfig repActionFichierConfig = new RepActionFichierConfig() {
                sTypeConfig = paramActionFichierConfig.sTypeConfig,
                sMessage = paramActionFichierConfig.sAction + " / " + paramActionFichierConfig.sTypeConfig + " / " + paramActionFichierConfig.NameFileConfig+ "  "
            };

            try {
                string jsonText = "";
                if (paramActionFichierConfig != null && paramActionFichierConfig.jsonObjConfig != null) {
                    jsonText = paramActionFichierConfig.jsonObjConfig.ToString();
                }

                string sAction = paramActionFichierConfig.sAction;
                string sPathWriteFile = environnementProjet.sPathBaseClient + paramActionFichierConfig.NameFileConfig;
                if(sAction == "Set"){
                    repActionFichierConfig.sMessage += "  CompletPathFile : "+ sPathWriteFile;
                }

                switch (paramActionFichierConfig.sTypeConfig) {

                    case "XmlConfigEnvironnementClient":
                        if (sAction == "Get") {
                            repActionFichierConfig.objConfig = configEnvironnementClient;
                        }
                        if (sAction == "Set") {
                            configEnvironnementClient = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigEnvironnementClient>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigEnvironnementClient>(configEnvironnementClient,sPathWriteFile );
                        }
                        break;

                    case "XmlConfigClientData":
                        if (sAction == "Get") {
                            repActionFichierConfig.objConfig = configClientData;
                        }
                        if (sAction == "Set") {
                            configClientData = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigClientData>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigClientData>(configClientData, sPathWriteFile);
                        }
                        break;

                    case "XmlConfigDataVariables":
                         if (sAction == "Get") {
                             repActionFichierConfig.objConfig = configDataVariables;
                        }
                        if (sAction == "Set") {
                            configDataVariables = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigDataVariables>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigDataVariables>(configDataVariables, sPathWriteFile);
                        }
                        break;

                    case "XmlConfigInputs":
                         if (sAction == "Get") {
                             repActionFichierConfig.objConfig = configInputs;
                        }
                        if (sAction == "Set") {
                            configInputs = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigInputs>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigInputs>(configInputs, sPathWriteFile);
                        }
                        break;

                    case "XmlConfigClientImpression":
                        if (sAction == "Get") {
                            repActionFichierConfig.objConfig = configClientImpression;
                        }
                        if (sAction == "Set") {
                            configClientImpression = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigClientImpression>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigClientImpression>(configClientImpression, sPathWriteFile);
                        }
                        break;


                    case "XmlConfigInputExcelFile":
                        if (sAction == "Get") {
                            repActionFichierConfig.objConfig = configInputExcelFile;
                        }
                        if (sAction == "Set") {
                            configInputExcelFile = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigInputExcelFile>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigInputExcelFile>(configInputExcelFile, sPathWriteFile);
                        }
                        break;

                    case "XmlConfigGlobalVariable":
                        if (sAction == "Get") {
                            repActionFichierConfig.objConfig = configGlobalVariable;
                        }
                        if (sAction == "Set") {
                            configGlobalVariable = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigGlobalVariable>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigGlobalVariable>(configGlobalVariable, sPathWriteFile);
                        }
                        break;


                    case "XmlConfigExtraParamClient":
                        if (sAction == "Get") {
                            repActionFichierConfig.objConfig = configExtraParamClient;
                        }
                        if (sAction == "Set") {
                            configExtraParamClient = Newtonsoft.Json.JsonConvert.DeserializeObject<XmlConfigExtraParamClient>(jsonText);
                            repActionFichierConfig.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigExtraParamClient>(configExtraParamClient, sPathWriteFile);
                        }
                        break;


                    default:
                        break;
                }
            }
            catch (Exception ex) {
                repActionFichierConfig.sMessage += "  Pb : " + ex.Message;
            }
            return repActionFichierConfig;
        }


        public RepActionTodoFile ActionTodoFile(ParamTodoFile paramTodoFile) {
            RepActionTodoFile repActionTodoFile = new RepActionTodoFile() { sMessage = "" };
            string sPathWriteFile = environnementProjet.sPathBaseClient;
            string sPath = sPathWriteFile + paramTodoFile.FileName;
            if (paramTodoFile.sAction == "Get") {
                repActionTodoFile.sMessage = "Get File : "+ sPath+"  ";
                if (File.Exists(sPath)) {
                    var todoList = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigTodoList>(sPath);
                    if (todoList != null && todoList.MemoTodoList != null) {
                        repActionTodoFile.sTexte = todoList.MemoTodoList;
                    }
                    else {
                        repActionTodoFile.sMessage += "  Null ";
                    }
                }
                else {
                    repActionTodoFile.sMessage += "   N'existe pas";
                }
            }
            if (paramTodoFile.sAction == "Set") {
                XmlConfigTodoList xmlConfigTodoList = new XmlConfigTodoList(){ MemoTodoList = paramTodoFile.sTexte};
                repActionTodoFile.sMessage = "Set File : "+ sPath+"  ";
                repActionTodoFile.sMessage += FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigTodoList>(xmlConfigTodoList, sPath);
                repActionTodoFile.sTexte = xmlConfigTodoList.MemoTodoList;
            }
            return repActionTodoFile;
        }
    }


    public class ParamActionConfigEnvironnementSession {
        public string sAction;
        public string newClientName;
        public XmlEnvironnementSession configEnvironnementSession;
    }

    public class RepActionConfigEnvironnementSession {
        public long ExecutionTimeMs;
        public EnvironnementGlobal environnementGlobal;
        public EnvironnementProjet environnementProjet;
        public XmlEnvironnementSession configEnvironnementSession;
    }

    public class ParamActionFichierConfig{
        public string sAction;
        public string sTypeConfig;
        public string NameFileConfig;
        public object jsonObjConfig;
    }

    public class RepActionFichierConfig {
        public long ExecutionTimeMs;
        public string sMessage;
        public string sTypeConfig;
        public object objConfig;
    }

    public class ParamTodoFile {
        public string sAction;
        public string sTexte;
        public string FileName;
    }

    public class RepActionTodoFile {
        public long ExecutionTimeMs;
        public string sMessage;
        public string sTexte;
    }

}
