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
    
    public partial class SingleSessionConfig {

        // INIT et reférence pour les configs
        // a ajouter config qui vont servir
        // a retirer config qui ne servent pas..


        XmlEnvironnementSession configEnvironnementSession;
        XmlConfigEnvironnementClient configEnvironnementClient;
        XmlConfigClientData configClientData;
        XmlConfigDataVariables configDataVariables;
        XmlConfigInputs configInputs;
        XmlConfigClientImpression configClientImpression;
        XmlConfigInputExcelFile configInputExcelFile;

        XmlConfigGlobalVariable configGlobalVariable;

        XmlConfigExtraParamClient configExtraParamClient;

        XmlScenarioData xmlScenarioData;

        // RECIP /INIT des fichiers de config


        #region Fichier de config d'entrée environnement et client a conserver.

        private XmlEnvironnementSession GetXmlEnvironnementSession(string sPath) {
            XmlEnvironnementSession xmlEnvironnementSession = null;
            if (File.Exists(sPath)) {
                xmlEnvironnementSession = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlEnvironnementSession>(sPath);
                return xmlEnvironnementSession;
            }
            else {
                GlobalLog.Instance().AjouteLog("SingleSessionConfig", "@GetXmlEnvironnementSession : Path n'EXISTE PAS : " + sPath);
            }
            xmlEnvironnementSession = new XmlEnvironnementSession() { AutoStartSurClient = false, AutoStartClientName = "", listEnvironnementExecutionDefault = new List<EnvironnementExecution>() };
            EnvironnementExecution environnementExecution = new EnvironnementExecution() { IsDefault = true, NomEnvironnement = "DEV1", listConnexionDB = new List<ConnexionDB>() };
            ConnexionDB connexionDB = new ConnexionDB(){ NomConnectionBDD="BDDProcess", ToConnect=true, ModeConnectionString=true,  NomServeur="FAUVEL-PORTABLE\\SQLEXPRESS", NomDB="XXPackingProcessV3", NomModel="BdModel.ModelDBxxProcessV3", ExtraConnectionStringOdbc="" };
            environnementExecution.listConnexionDB.Add(connexionDB);
            environnementExecution.connexionServeurPrint = new ConnexionServeurPrint(){ ToConnect=true, sServeurWCF="127.0.0.1", pathConfigSerciceMoteur="D:\\ServiceMoteurImpression\fichierini.config", IsMoteurOnThisHost=true,  VersionGestionnaire="V1", ListNomImprimanteToConnect = new List<string>()};
            environnementExecution.configWebInterface = new ConfigWebInterface(){ ListeMenuVisibles = new List<string>(), ListeShortButton = new List<ConfigShortButton>()};
            environnementExecution.configWebInterface.ListeMenuVisibles.Add("showProcess");
            environnementExecution.configWebInterface.ListeMenuVisibles.Add("showPrinterData");
            environnementExecution.configWebInterface.ListeShortButton.Add(new ConfigShortButton() { BtnName = "TPrint", BtnClick = "Printer" });
            xmlEnvironnementSession.listEnvironnementExecutionDefault.Add(environnementExecution);

            string statutMes = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlEnvironnementSession>(xmlEnvironnementSession, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("SingleSessionConfig", "@GetXmlEnvironnementSession : Config n'existe pas : creation : " + sPath + "   :  " + statutMes);
            return xmlEnvironnementSession;
        }


        private XmlConfigEnvironnementClient GetXmlConfigEnvironnementClient(string sPath) {
            XmlConfigEnvironnementClient xmlConfigEnvironnementClient = null;
            if (File.Exists(sPath)) {
                xmlConfigEnvironnementClient = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigEnvironnementClient>(sPath);


                // Ajout nouvelles caractèristiques à fichier existant :
                bool ToSave = false;

                //foreach (var env in xmlConfigEnvironnementClient.listEnvironnementExecution) {
                //    if (env.listConnexionOrpheaWeb == null || (env.listConnexionOrpheaWeb != null && env.listConnexionOrpheaWeb.Count == 0)) {
                //        env.listConnexionOrpheaWeb = new List<ConnexionOrpheaWeb>();
                //        ConnexionOrpheaWeb con = new ConnexionOrpheaWeb() { ToConnect = false, UrlApi = "http://localhost:55673/" };
                //        env.listConnexionOrpheaWeb.Add(con);
                //        ToSave = true;
                //    }
                //    else {
                //        //foreach (var cOrph in env.listConnexionOrpheaWeb) {
                //        //    cOrph.NomOrphea = "Orphea1";
                //        //    cOrph.ListFonctionAutorisee = "";
                //        //    ToSave = true;
                //        //}
                //    }
                //}

                if (ToSave) {
                    FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigEnvironnementClient>(xmlConfigEnvironnementClient, sPath);
                }
                
                return xmlConfigEnvironnementClient;
            }
            xmlConfigEnvironnementClient = new XmlConfigEnvironnementClient() { NomClient = "NomClient", listEnvironnementExecution = new List<EnvironnementExecution>() };
            EnvironnementExecution environnementExecution = new EnvironnementExecution() { IsDefault = true, NomEnvironnement = "DEV1", listConnexionDB = new List<ConnexionDB>(), listConnexionOrpheaWeb = new List<ConnexionOrpheaWeb>() };
            ConnexionDB connexionDB = new ConnexionDB(){ NomConnectionBDD="BDDProcess", ToConnect=true, ModeConnectionString=true,  NomServeur="FAUVEL-PORTABLE\\SQLEXPRESS", NomDB="XXPackingProcessV3", NomModel="BdModel.ModelDBxxProcessV3", ExtraConnectionStringOdbc="" };
            environnementExecution.listConnexionDB.Add(connexionDB);
            environnementExecution.connexionServeurPrint = new ConnexionServeurPrint(){ ToConnect=true, sServeurWCF="127.0.0.1", pathConfigSerciceMoteur="D:\\ServiceMoteurImpression\\fichierini.config", IsMoteurOnThisHost=true,  VersionGestionnaire="V1", ListNomImprimanteToConnect = new List<string>()};
            environnementExecution.configWebInterface = new ConfigWebInterface(){ ListeMenuVisibles = new List<string>(), ListeShortButton = new List<ConfigShortButton>()};
            environnementExecution.configWebInterface.ListeMenuVisibles.Add("showProcess");
            environnementExecution.configWebInterface.ListeMenuVisibles.Add("showPrinterData");
            environnementExecution.configWebInterface.ListeShortButton.Add(new ConfigShortButton() { BtnName = "TPrint", BtnClick = "Printer" });
            xmlConfigEnvironnementClient.listEnvironnementExecution.Add(environnementExecution);
            xmlConfigEnvironnementClient.listTypeFileConfigNeeded = new List<TypeFileConfigNeeded>();
            xmlConfigEnvironnementClient.listTypeFileConfigNeeded.Add(new TypeFileConfigNeeded() { LibelleFile = "Layout", TypeFileConfig = "XmlConfigClientPackingLayout", NameFileConfig = "ConfigClientPackingLayout.xml" });
            xmlConfigEnvironnementClient.listTypeFileConfigNeeded.Add(new TypeFileConfigNeeded() { LibelleFile = "Data", TypeFileConfig = "XmlConfigClientData", NameFileConfig = "ConfigClientData.xml" });
            xmlConfigEnvironnementClient.listTypeFileConfigNeeded.Add(new TypeFileConfigNeeded() { LibelleFile = "Variables", TypeFileConfig = "InitConfigDataVariables", NameFileConfig = "ConfigDataVariables.xml" });
            xmlConfigEnvironnementClient.listTypeFileConfigNeeded.Add(new TypeFileConfigNeeded() { LibelleFile = "Inputs", TypeFileConfig = "XmlConfigInputs", NameFileConfig = "ConfigInputs.xml" });
            xmlConfigEnvironnementClient.listTypeFileConfigNeeded.Add(new TypeFileConfigNeeded() { LibelleFile = "Impression", TypeFileConfig = "XmlConfigClientImpression", NameFileConfig = "ConfigClientImpression.xml" });

            string statutMes = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigEnvironnementClient>(xmlConfigEnvironnementClient, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("SingleSessionConfig", "@GetXmlEnvironnementSession : Config n'existe pas : creation : " + sPath + "   :  " + statutMes);
            return xmlConfigEnvironnementClient;
        }

        #endregion



        #region Configs annexes suivant les cas d'application, A ABONDER, RETIRER..


        private XmlConfigClientData InitConfigClientData(string sPath) {
            XmlConfigClientData xmlConfigClientData = null;
            if (File.Exists(sPath)) {
                xmlConfigClientData = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigClientData>(sPath);
                return xmlConfigClientData;
            }
            xmlConfigClientData = new XmlConfigClientData() { CLientName = "Test Client", configWO = new ConfigWO(), configPacking = new ConfigPacking(), configEcarts = new ConfigEcarts(), configPalettisation = new ConfigPalettisation(), configVidage = new ConfigVidage() };
            xmlConfigClientData.configWO.ProfondeurWOQueue = 3;
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigClientData>(xmlConfigClientData, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "ConfigClient.Data.xml  n'existe pas : creation : " + sReponse);
            return xmlConfigClientData;
        }

        public XmlConfigDataVariables InitConfigDataVariables(string sPath) {
            XmlConfigDataVariables xmlConfigDataVariables = null;
            if (File.Exists(sPath)) {
                xmlConfigDataVariables = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigDataVariables>(sPath);
                return xmlConfigDataVariables;
            }
            xmlConfigDataVariables = new XmlConfigDataVariables() { listConfigVariable = new List<ConfigVariable>() };
            ConfigVariable configVariable = new ConfigVariable() { NomVariable = "VariableTest", FormuleVariable = eFormuleVariable.NC, NeedValidation = false, DefaultValue = "", listParamFormule = new List<BindingVariable>() };
            BindingVariable bindingVariable = new BindingVariable() { NomVariableBinding = "", SourceData = 0, DefaultValue = "" };
            configVariable.listParamFormule.Add(bindingVariable);
            xmlConfigDataVariables.listConfigVariable.Add(configVariable);
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigDataVariables>(xmlConfigDataVariables, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "ConfigClient.Variables.xml  n'existe pas : creation : " + sReponse);
            return xmlConfigDataVariables;
        }

        private XmlConfigInputs InitConfigInputs(string sPath) {
            XmlConfigInputs xmlConfigInputs = null;
            if (File.Exists(sPath)) {
                xmlConfigInputs = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigInputs>(sPath);
                return xmlConfigInputs;
            }

            xmlConfigInputs = new XmlConfigInputs() { listCodeBarreDef = new List<CodeBarreDef>(), listDataBindingTable = new List<DataBindingTable>(), critereHierarchie = new CritereHierarchie() };
            ItemDecoupeCode itemDecoupeCode = new ItemDecoupeCode() { CheckFixedValue = "", NbreDigit = 12, Offset = 0, TypeFieldData = eTypeFieldData.CodePalox };

            CritereStock critereStockExemple1 = new CritereStock() { CodeCritere = "Espece", listCritereStock = new List<CritereStock>(), NomFiltre = "Test" };
            CritereStock critereStockExemple2 = new CritereStock() { CodeCritere = "Variete", listCritereStock = new List<CritereStock>(), NomFiltre = "" };
            CritereStock critereStockExemple3 = new CritereStock() { CodeCritere = "NomArticle", listCritereStock = new List<CritereStock>(), NomFiltre = "" };
            critereStockExemple2.listCritereStock.Add(critereStockExemple3);
            critereStockExemple1.listCritereStock.Add(critereStockExemple2);
            xmlConfigInputs.critereHierarchie.listCritereStock = new List<CritereStock>();
            xmlConfigInputs.critereHierarchie.listCritereStock.Add(critereStockExemple1);

            CodeBarreDef codeBarreDef = new CodeBarreDef() { TypeCode = eTypeCodeBarre.Code39, NbreDigits = 12, TypeTableOrigineBinding = eTypeTableOrigine.PaloxOrphea5, listItemDecoupeCode = new List<ItemDecoupeCode>() };
            codeBarreDef.listItemDecoupeCode.Add(itemDecoupeCode);
            xmlConfigInputs.listCodeBarreDef.Add(codeBarreDef);
            DataBindingTable dataBindingTable = new DataBindingTable() { TypeTableOrigine = eTypeTableOrigine.PaloxOrphea5, ToTransfertToProcessInputStock = true, listRegleChampTable = new List<RegleChampTable>(), configTransfert = new ConfigTransfert() };
            List<string> listChampViewPaloxOrphea5Utiles = new List<string>(){
                   "Numero_palox",
                   "Numero_produit", 
                   "Nom_article",
                   "Poids", 
                   "Code_article", 
                   "Numero_lot",
                   "Cumul",
                   "Validation_maf",
                   "Validation_externe",
                   "String_palox_libre1",
                   "String_palox_libre2",
                   "String_palox_libre3",
                   "Int_palox_libre1",
                   "Int_palox_libre2",
                   "Int_palox_libre3",
                   "Code_cumul",
                   "Code_adherent_max",
                   "Code_parcelle",
                   "Nom_parcelle",
                   "Code_variete",
                   "Nom_variete",
                   "Code_clone",
                   "Nom_clone",
                   "Code_adherent",
                   "Nom_adherent",
                   "Numero_bon_apport",
                   "Reference",
                   "Nom_espece",
                   "Code_espece",
                   "Commentaire",
                   "String_lot_libre1",
                   "String_lot_libre2",
                   "String_lot_libre3",
                   "String_lot_libre4",
                   "String_lot_libre5",
                   "Int_lot_libre1",
                   "Int_lot_libre2",
                   "Int_lot_libre3",
                   "Int_lot_libre4",
                   "Int_lot_libre5",
                   "Type_palox",
                   "Code_cahier_charge",
                   "Code_article_caracteristique",
                   "Nom_article_caracteristique",
                   "Reference_lot_repasse"
                };

            foreach (var v in listChampViewPaloxOrphea5Utiles) {
                RegleChampTable regleChampTable = new RegleChampTable() { NomChampOrigine = v, ToCodeCritere = v, ToNomCritere = v, ToTypeCritere = 0, ActionChamp = 0 };
                dataBindingTable.listRegleChampTable.Add(regleChampTable);
            }
            xmlConfigInputs.listDataBindingTable.Add(dataBindingTable);

            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigInputs>(xmlConfigInputs, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "ConfigClient.Input.xml  n'existe pas : creation : " + sReponse);
            return xmlConfigInputs;
        }


        public XmlConfigClientImpression InitConfigClientImpression(string sPath) {
            XmlConfigClientImpression xmlConfigClientImpression = null;
            if (File.Exists(sPath)) {
                xmlConfigClientImpression = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigClientImpression>(sPath);
                return xmlConfigClientImpression;
            }
            xmlConfigClientImpression = new XmlConfigClientImpression() { listConfigAttributionImprimante = new List<ConfigAttributionImprimante>(), impressionResolutionVariables = new ImpressionResolutionVariables() };
            xmlConfigClientImpression.listConfigAttributionImprimante.Add(new ConfigAttributionImprimante() { AdresseIP = "192.168.111.150", NomImprimante = "MonImprimante", NomImprimanteMoteur = "MonImprimante", TypePointImpression = eTypePointImpression.NC, MasqueDefaut = "MonMasque" });
            xmlConfigClientImpression.impressionResolutionVariables.DefaultListVariableMasqueCodesoft = new List<VariableMasqueCodesoft>();
            var vmc = new VariableMasqueCodesoft() { NomMasque = "", NomVariable = "", NomVariableDATA = "" };
            xmlConfigClientImpression.impressionResolutionVariables.DefaultListVariableMasqueCodesoft.Add(vmc);
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigClientImpression>(xmlConfigClientImpression, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "ConfigClient.PrintV1.xml  n'existe pas : creation : " + sReponse);
            return xmlConfigClientImpression;
        }

        public XmlConfigInputExcelFile InitXmlConfigInputExcelFile(string sPath) {
            XmlConfigInputExcelFile xmlConfigInputExcelFile = null;
            if (File.Exists(sPath)) {
                xmlConfigInputExcelFile = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigInputExcelFile>(sPath);
                return xmlConfigInputExcelFile;
            }
            xmlConfigInputExcelFile = new XmlConfigInputExcelFile() { FamilleName = "famille", ComputeHeader = "compute", ColInfos = new List<ColInfo>() };
            ColInfo colInfo = new ColInfo() { ColName = "Name", ToArticleNomCritere = "ToArticleNom", ToArticleCodeCritere = "ToArticleCode", ToArticleTypeCritere = eTypeVariablesCriteres.KeyBindORPHEAArticle };
            xmlConfigInputExcelFile.ColInfos.Add(colInfo);
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigInputExcelFile>(xmlConfigInputExcelFile, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "ConfigInputExcelFile.xml  n'existe pas : creation : " + sReponse);
            return xmlConfigInputExcelFile;
        }


        public XmlConfigGlobalVariable InitXmlConfigGlobalVariable(string sPath) {
            XmlConfigGlobalVariable xmlConfigGlobalVariable = null;
            if (File.Exists(sPath)) {
                xmlConfigGlobalVariable = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigGlobalVariable>(sPath);
                return xmlConfigGlobalVariable;
            }
            xmlConfigGlobalVariable = new XmlConfigGlobalVariable() {  listKeyVarDefinition = new List<KeyVarDefinition>()};
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigGlobalVariable>(xmlConfigGlobalVariable, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "InitXmlConfigGlobalVariable.xml  n'existe pas : creation : " + sReponse);
            return xmlConfigGlobalVariable;
        }


        public XmlConfigExtraParamClient InitXmlConfigExtraParamClient(string sPath) {
            XmlConfigExtraParamClient xmlConfigExtraParamClient = null;
            if (File.Exists(sPath)) {
                xmlConfigExtraParamClient = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlConfigExtraParamClient>(sPath);
                return xmlConfigExtraParamClient;
            }
            xmlConfigExtraParamClient = new XmlConfigExtraParamClient() {  ClientName="Toto"};
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlConfigExtraParamClient>(xmlConfigExtraParamClient, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "InitXmlConfigExtraParamClient.xml  n'existe pas : creation : " + sReponse);

            return xmlConfigExtraParamClient;
        }

        public XmlScenarioData InitXmlScenarioData(string sPath) {
            XmlScenarioData xmlScenarioData = null;
            if (File.Exists(sPath)) {
                xmlScenarioData = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<XmlScenarioData>(sPath);
                return xmlScenarioData;
            }
            xmlScenarioData = new XmlScenarioData() { listXmlParamInsertInApport = new List<XmlParamInsertInApport>(), listItemArticleOrphea = new List<XmlItemArticleOrphea>() };
            XmlParamInsertInApport xmlParamInsertInApport = new XmlParamInsertInApport(){ Command="Insert", keyVals = new List<keyVal>()};
            xmlParamInsertInApport.keyVals.Add(new keyVal() { key = "k", value = "v" });
            xmlScenarioData.listXmlParamInsertInApport.Add(xmlParamInsertInApport);
            XmlItemArticleOrphea itemArticleOrphea = new XmlItemArticleOrphea() { ArticleCode = "", ArticleName = "", Variety = "" };
            xmlScenarioData.listItemArticleOrphea.Add(itemArticleOrphea);
            string sReponse = FastSerialisation.Instance().SaveStructInCurrentDirectory<XmlScenarioData>(xmlScenarioData, sPath);
            SingleLogFileAsXml.Instance().AjouteLog("Init", "InitXmlConfigExtraParamClient.xml  n'existe pas : creation : " + sReponse);

            return xmlScenarioData;
        }

        #endregion

    }

}
