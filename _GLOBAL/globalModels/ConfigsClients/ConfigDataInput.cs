using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {

    //------------------------------------------------------------------------------------------------------
    // Config pour la définition des parametres d'entrées lot / CB... 
    // Un peu comme le fait le prog de suivi de lot ORPHEA a affiner..
    //------------------------------------------------------------------------------------------------------

    #region Config pour définiton des entrées / CB / tables pour palox / Apports / Lots

    public class XmlConfigInputs {
        public List<CodeBarreDef> listCodeBarreDef;
        public CritereHierarchie critereHierarchie;
        public List<DataBindingTable> listDataBindingTable;
    }

    public enum eTypeCodeBarre { NC, Code39, Interleaved2_5, Code128 }

    public class CodeBarreDef {
        [XmlAttribute]
        public eTypeCodeBarre TypeCode;
        [XmlAttribute]
        public int NbreDigits;
        [XmlAttribute]
        public eTypeTableOrigine TypeTableOrigineBinding;
        public List<ItemDecoupeCode> listItemDecoupeCode;
    }

    public enum eTypeFieldData { NC, Prefixe, Suffixe, CodePalox }

    public class ItemDecoupeCode {
        [XmlAttribute]
        public int Offset;
        [XmlAttribute]
        public int NbreDigit;
        [XmlAttribute]
        public string CheckFixedValue;
        [XmlAttribute]
        public eTypeFieldData TypeFieldData;
    }

    public class CritereHierarchie {
        public List<CritereStock> listCritereStock;
    }

    public class CritereStock {
        public string NomFiltre;
        public string CodeCritere;
        public List<CritereStock> listCritereStock;
    }

    public enum eTypeTableOrigine { NC, PaloxOrphea5, ProcessInput, DirectCB }

    public class DataBindingTable {
        [XmlAttribute]
        public eTypeTableOrigine TypeTableOrigine;
        [XmlAttribute]
        public bool ToTransfertToProcessInputStock;
        public ConfigTransfert configTransfert;
        public List<RegleChampTable> listRegleChampTable;
    }

    public class ConfigTransfert {
        [XmlIgnore]
        public long ExecutionTimeMs;
        [XmlAttribute]
        public DateTime dtMinUpLoad;
        [XmlAttribute]
        public DateTime dtMaxUpLoad;
    }

    [Flags]
    public enum eEditionChamp {
        NC = 0x00000001,
    }

    [Flags]
    public enum eActionChamp {
        RecopieEnTableProcess = 0x00000001,
        CritereFiltreStock = 0x00000002,
        CritereChangementLot = 0x00000004,
        CriterePoidsNet = 0x00000008,
        IdCodeBarre = 0x00000010,
        VerifNotNull = 0x00000020,
    }

    [Flags]
    public enum eTypeVariablesCriteres {
        KeyChoixOperateur = 0x00000001,       // cette variable/critere est utilisable pour selectionner un article par filtre sur IHM
        KeyChoixOF = 0x00000002,              // Un OF pourait choisir un Article sur un critère ? mieux passer par le code interne.. plus simple
        KeyBindORPHEAArticle = 0x00000004,    // critère de correlation avec un Article ORPHEA .. a choisir dans la table des articles ORPHEA
        KeyChangementLotProcess = 0x00000008,        // critère de changement de Lot pour les articles Palox input ( on recopie les palox de la table ORPHEA dans une table process, correspond a nom de champ dans table process..)
        KeyChangementLotNatifOrphea = 0x00000010,        // critère de changement de Lot pour les articles Palox input ( Nom de champ de la BDD Orphea direct ?? a voir )
        KeyCalculPoidsPacking = 0x00000020,           // ce critère doit etre convertible en valeur numerique pour servir de calcul de poids
        ParamPalettisation = 0x00000040,
        KeyConsignePoids = 0x00000080,
        KeyConsigneNbre = 0x00000100,
        ParamContante = 0x00000200,
        ParamProcess = 0x00000400,
        UsedInContexe = 0x00000800,    // SEUL UTILISE pour permettre visu dans contexte de Flux
        SourceArticle = 0x00001000,    //  UTILISE pour permettre reconnaissance provenance
        SourceFlux = 0x00002000,    //  UTILISE pour permettre reconnaissance provenance
        SourceUser = 0x00004000,    //  UTILISE pour permettre reconnaissance provenance
        SourceSystem = 0x00008000,    //  UTILISE pour permettre reconnaissance provenance
    }

    public class RegleChampTable {
        [XmlAttribute]
        public string NomChampOrigine;
        [XmlAttribute]
        public string ToCodeCritere;
        [XmlAttribute]
        public string ToNomCritere;
        [XmlAttribute]
        public string ToLibelleCritere;
        [XmlAttribute]
        public eTypeVariablesCriteres ToTypeCritere;   // Type comportement propre au critère Idem critère Article..
        [XmlAttribute]
        public eActionChamp ActionChamp;      // Actions propres aux critères stock     
    }

    #endregion
}
