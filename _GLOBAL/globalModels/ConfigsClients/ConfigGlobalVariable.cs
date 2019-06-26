using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {


    public enum eLocalisation { Apport, Orphea, OrpheaWeb, DataStock, };
    public enum eTypeVariable { NC, _int, _long, _string, _double, _bool, _dateTime };
    public enum eTypeGlobalFunction { NC, Special, CopieBDD };

    public enum eTypePath {
        NC,                     // 0
        Fixed,                  // 1
        KeyValToDefine,         // 2
        Reduction,              // 3
        StoreKeyValue,         // 4        
        TableKeyValue,           // 5
        KeyValDef,               // 6
        System,                  //7
        ProcessVarLabel          // 8   
    };


    public class XmlConfigGlobalVariable {
        public List<KeyVarDefinition> listKeyVarDefinition;
    }

    public class KeyVarDefinition {
        [XmlAttribute]
        public string Path;
        [XmlAttribute]
        public string ParentPath;
        [XmlAttribute]
        public string KeyName;
        [XmlAttribute]
        public string KeyCode;
        [XmlAttribute]
        public string KeyLibelle;
        [XmlAttribute]
        public eTypeVariable TypeVariable;
        public List<BindingToVariable> listBindingToVariable;
    }

    public class BindingToVariable {
        [XmlAttribute]
        public string Path;
        [XmlAttribute]
        public eTypeGlobalFunction TypeGlobalFunction;
        [XmlAttribute]
        public string paramFonction;
    }

    // Modele pour representation d3js
    public class ParamGetDataSetTree {
        public string sAction;
        public string id1;
        public string id2;
        public string NodeToAdd;
    }

    public class DataSetTree {
        public long ExecutionTimeMs;
        public AllPath allPath;
        public MessageInfos Message;
        public KeyVarDefinition keyVarDefinitionSelected;
    }

    public class AllPath {
        public List<PathIdentity> listPathIdentity;
    }

    public class PathIdentity {
        public string id;
        public eTypePath TypePath;
        public string Comment;
    }

    public class MessageInfos {
        public bool isMessage;
        public List<string> listLigneMessage;
    }

    public enum eMethodeAccess { NC, ChampApportOrphea, ChampViewPaloxOrphea, ChampBdd }; // A voir... ou autre données pour décrire l'access

    public class VariableSysteme {
        public string Name;
        public string ParentPath;
        public eTypeVariable TypeVariable;
        //public eMethodeAccess MethodeAccess;   // a voir : moyen pour decrire la methode d'acces a cette donnée.. bien que le parentPath soit suffisant
    }

    public class GlobalVariableSysteme {
        public List<VariableSysteme> listVariableSysteme;
    }


}
