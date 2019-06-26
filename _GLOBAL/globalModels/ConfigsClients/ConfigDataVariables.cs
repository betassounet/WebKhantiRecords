using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {

    public enum eFormuleVariable {
        NC,
        Default,
        Egal,
        Concat,
        CB,
        DateTime,
        Fct_NumeroSemaine,
        Fct_JourSemaine,
        Fct_Interfel,
        EgalSameNameOF
    }

    // On définit ici les chemin possible ou trouver l'info
    // Permet de guider le gestionnaire des contexte pour retrouver l'info a qq part.
    [Flags]
    public enum eSourceData {
        OF = 0x00000001,                    // dans contexte d'OF
        LOT = 0x00000002,                   // dans le contexte LOT   
        ARTICLE = 0x00000004,               // dans le contexte ARTICLE 
        GLOBAL = 0x00000008,                 // dans le contexte GLOBAL  
    }

    public class XmlConfigDataVariables {
        public List<ConfigVariable> listConfigVariable;
    }

    public class ConfigVariable {
        [XmlAttribute]
        public string NomVariable;
        [XmlAttribute]
        public eFormuleVariable FormuleVariable;
        [XmlAttribute]
        public string DefaultValue;
        public List<BindingVariable> listParamFormule;
        [XmlAttribute]
        public bool NeedValidation;
    }

    public class BindingVariable {
        [XmlAttribute]
        public eSourceData SourceData;
        [XmlAttribute]
        public string NomVariableBinding;
        [XmlAttribute]
        public string DefaultValue;
    }

    public class VariableRepository {
        [XmlIgnore]
        public long ExecutionTimeMs;
        public List<VariableItem> listVariableItem;
    }

    public class VariableItem {
        public string NomVariable;
        public string ValeurVariable;   // valeurs pour l'init et test formule variable
        public bool IsOK;
        public string sMessageDebug;   // 
        public string sMessageError;
        public List<LocAndValue> listLocValue;
    }

    // Valeur de variable en fonction d'un contexte donné..
    public class LocAndValue {
        public string LocContexte;
        public int LocId;
        public string ValeurVariable;
        public bool IsOK;
        public string sMessageDebug;
        public string sMessageError;
    }
}
