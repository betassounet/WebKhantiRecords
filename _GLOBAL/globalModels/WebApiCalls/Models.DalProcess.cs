using _GLOBAL.globalModels.ConfigsClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.globalModels.WebApiCalls {
    // ON rend public les Modeles de Com via WebAPI de façon a ce qu'ils soietn utilisables par application Windows cliente.

   
    #region Models pour gestion Article

    // Pour indiquer au client les modèles a utiliser pour les echanges
    public class ModelsSetup {
        public long ExecutionTimeMs;
        public ModelArticleSetup modelArticleSetup;
    }

    public class ModelArticleSetup {
        public string CodeFamille;
        public string LabelFamille;
        public List<ArticleSetupCritere> listArticleSetupCritere;
    }

    public class ArticleSetupCritere {
        public int Id;
        public string CodeCritere;
        public string ValeurDefaut;
        public eTypeVariablesCriteres TypeCritere;
    }

    public class RepTypeEnumVariableCritere {
        public long ExecutionTimeMs;
        public bool IsOK;
        public string sMessage;
        public List<string> ListEnum;
    }

    public class RepTypeEnumFlag {
        public long ExecutionTimeMs;
        public bool IsOK;
        public string sMessage;
        public List<EnumFlagItem> ListEnumFlag;
    }

    public class EnumFlagItem {
        public string Name;
        public int Value;
    }

    public class RepFamilleArticle {
        public long ExecutionTimeMs;
        public bool IsOK;
        public string sMessage;
        public List<ShortARTICLE_Familles> listArticleFamille;
    }

    public class RepAllArticleRef {
        public long ExecutionTimeMs;
        public bool IsOK;
        public string sMessage;
        public List<string> listArticleRef;
    }

    public class ShortARTICLE_Familles {
        public string CodeFamille;
        public string LabelFamille;
    }

    public class ParamFillData {
        public string ChampCodeInterne;
        public string CodeFamille;
    }

    public class TempClassExcelToDB {
        public string CodeCritere;
        public string NomCritere;
        public int TypeCritere;
        public string ValeurCritere;
    }

    public class ParamSelectCritere {
        public string CodeFamille;
        public List<ChoixCritere> listChoixCritere;
        public PagesParam pagesParam;
        public int Mode;
    }

    public class ChoixCritere {
        public string CodeCritere;
        public string NomCritere;
        public int TypeCritere;
        public string Valeur;
    }

    public class RepSetupCriteresFamille {
        public long ExecutionTimeMs;
        public bool isOK;
        public string sMessage;
        public List<SetupGetCritere> listCriteres;
        public PagesArticles pagesArticles;
        public int Mode;
    }

    public class SetupGetCritere {
        public string NomCritere;
        public string CodeCritere;
        public int TypeCritere;
        public List<string> ListValeurs;
        public string ValeurFiltre;
        public bool IsFiltre;
    }

    public class PagesParam {
        public int NbreTotalArticle;
        public int NumeroPageCourante;
        public int NbreArticleParPage;
    }

    public class PagesArticles {
        public PagesParam pagesParam;
        public List<ModelArticle> listModelArticle;
    }

    public class ModelArticle {
        public long Id;
        public string CodeInterne;
        public string CodeExterne;
        public string Designation;
        public string LabelInterne;
        public string UniteGestion;
        public long IdFamille;
        public List<CritereValeur> listCritereValeur;
    }

    public class CritereValeur {
        public string CodeCritere;
        public string NomCritere;
        public int TypeCritere;
        public string Valeur;
    }


    public class ParamAddPicArticle {
        public int IdArticle;
        public string PicName;
    }

    #endregion


    #region Models pour gestion OF


    public class retListOF_DefTypeUnitJob {
        public bool isOK;
        public string Message;
        public long ExecutionTimeMs;
        public List<OFDefTypeUnitJob> listOFDefTypeUnitJob;

    }

    public class OFDefTypeUnitJob {
        public long Id;
        public string CodeTypeUnit;
        public string LibelleTypeUnit;
        public string Description;
        public string Langage;
    }


    [Flags]
    public enum eFlagEnumOF {
        EnregistrementHeader = 0x00000001,      // marque le début d'un enregistrement OF par son Header.. puis si ça plante après..
        EnregistrementJob = 0x00000002,         // marque l'enregistrement d'un job..  
        EnregistrementOK = 0x00000004,          // marque la fin d'enregistrement OK pour tout l'OF
    }

    [Flags]
    public enum eCreationModeSender {
        ModeWeb = 0x00000001,
        Mode1 = 0x00000002,
        Mode2 = 0x00000004,
        Mode3 = 0x00000008,
        Mode4 = 0x00000010,
    }


    public class DefineWOOneJobArticle {
        public string ExternalRef1;
        public string ExternalRef2;
        public string LibelleOF;
        public int Consigne;
        public int ConsigneMin;
        public int ConsigneMax;
        public int ConsignePoids;
        public string LabelName;
        public string RefCodeArticle;
    }


    public class DefineWOJobsArticle {
        public W_OF_Header WOHeader;
        public List<W_OF_UnitJobArticle> ListOFUnitJobArticle;
    }

    public class W_OF_Header {
        public string ExternalRef1;
        public string ExternalRef2;
        public string LibelleOF;
    }

    public class W_OF_UnitJobArticle {
        public string TypeUnit;
        public int Consigne;
        public int ConsigneMin;
        public int ConsigneMax;
        public int ConsignePoids;
        public string RefCodeArticle;
        public string LabelName;
    }


    public class DefineWOJobsDetails {
        public W_OF_Header WOHeader;
        public List<W_OF_UnitJobDetails> ListOFUnitJobDetails;
    }

    public class W_OF_UnitJobDetails {
        public string TypeUnit;
        public int Consigne;
        public int ConsigneMin;
        public int ConsigneMax;
        public int ConsignePoids;
        public string LabelName;
        public string RefCodeArticle;
        public List<W_OF_Details> ListWODetails;
    }

    public class W_OF_Details {
        public string NomVariable;
        public string ValeurVariable;
        public int TypeVariable;  // doit correspondre a l'enum eTypeVariablesCriteres
    }

    public class DefineWOOneJobsDetails {
        public string ExternalRef1;
        public string ExternalRef2;
        public string LibelleOF;
        public int Consigne;
        public int ConsigneMin;
        public int ConsigneMax;
        public int ConsignePoids;
        public string LabelName;
        public string RefCodeArticle;
        public List<W_OF_Details> ListWODetails;
    }

    public class ParamSelectOF {
        public string sQuery;
        public PagesParam pagesParam;
    }

    public class RepGetListeOF {
        public long ExecutionTimeMs;
        public bool isOK;
        public string sMessage;
        public PagesOF pagesOF;
    }

    public class PagesOF {
        public PagesParam pagesParam;
        public List<ModelWO> listModelWO;
    }


    public class ModelWO {
        public V_OF_Header WOHeader;
        public List<V_OF_UnitJob> ListOFUnitJob;
    }

    public class V_OF_Header {
        public long Id;
        public string ExternalRef1;
        public string ExternalRef2;
        public string LibelleOF;
        public string ParentOFId;
        public int Statut;
        public int FlagEnum;
        public string Commentaire;
        public DateTime DateTimeOp;
        public int CreationModeSender;
        public string LigneProdDediee;
    }

    public class V_OF_UnitJob {
        public long Id;
        public long IdOFRef;
        public string TypeUnit;
        public int Consigne;
        public int ConsigneMin;
        public int ConsigneMax;
        public double ConsignePoids;
        public string LabelName;
        public string RefCodeArticle;
        public bool ExplicitDetails;
        public bool DetailsByArticle;
        public string LocDediee;
        public int QuantiteRun;
        public double PoidsRun;
        public int FlagEnum;
        public List<V_OF_Details> ListWODetails;
        public ModelArticle modelArticle;
    }

    public class V_OF_Details {
        public long Id;
        public long IdOFRef;
        public string NomVariable;
        public string ValeurVariable;
        public long IdOFUnitJob;
        public string CodeVariable;
        public int TypeVariable;
    }


    #endregion
}
