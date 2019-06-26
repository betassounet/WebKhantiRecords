using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.SessionConfigXml {
    
    public class XmlEnvironnementSession {
        public bool AutoStartSurClient;
        public string AutoStartClientName;
        public List<EnvironnementExecution> listEnvironnementExecutionDefault;
    }

    public class EnvironnementExecution {
        [XmlAttribute]
        public string NomEnvironnement;
        [XmlAttribute]
        public bool IsDefault;

        public List<ConnexionDB> listConnexionDB;
        public ConnexionServeurPrint connexionServeurPrint;
        public List<ConnexionOrpheaWeb> listConnexionOrpheaWeb;
        public ConfigWebInterface configWebInterface;
    }

    public class ConnexionDB {
        [XmlAttribute]
        public string NomConnectionBDD;
        [XmlAttribute]
        public bool ToConnect;
        [XmlAttribute]
        public bool ModeConnectionString;
        [XmlAttribute]
        public string NomServeur;
        [XmlAttribute]
        public string NomDB;
        [XmlAttribute]
        public string NomModel;
        [XmlAttribute]
        public string UserId;
        [XmlAttribute]
        public string Password;
        [XmlAttribute]
        public string ExtraConnectionStringOdbc;
    }

    public class ConnexionServeurPrint {
        [XmlAttribute]
        public bool ToConnect;
        [XmlAttribute]
        public string sServeurWCF;                // End point de connection au moteur service Print
        [XmlAttribute]
        public string pathConfigSerciceMoteur;     // acces au fichier de config du service Moteur
        [XmlAttribute]
        public bool IsMoteurOnThisHost;         // Le service impression est il sur ce PC ? si oui le fichier de conf sera accessible et actions sur services aussi en mode administrateur...
        [XmlAttribute]
        public string VersionGestionnaire;     // V0 ou V1
        public List<string>ListNomImprimanteToConnect;
    }

    public class ConnexionOrpheaWeb {
        [XmlAttribute]
        public string NomOrphea;
        [XmlAttribute]
        public bool ToConnect;
        [XmlAttribute]
        public string UrlApi;
        [XmlAttribute]
        public string ListFonctionAutorisee;
    }

    public class ConfigWebInterface {
        public List<string> ListeMenuVisibles;
        public List<ConfigShortButton> ListeShortButton;
    }

    public class ConfigShortButton {
        public string BtnName;
        public string BtnClick;
    }
}
