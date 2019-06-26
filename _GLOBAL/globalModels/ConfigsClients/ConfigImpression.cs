using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {

    public enum eTypePointImpression { NC, CaissesAutomatique, CaissesManuel, Palette, Palox, CaisseManuelEtPalette };

    public class XmlConfigClientImpression {
        public List<ConfigAttributionImprimante> listConfigAttributionImprimante;
        public ImpressionResolutionVariables impressionResolutionVariables;
    }

    public class ConfigAttributionImprimante {
        [XmlAttribute]
        public string NomImprimante;
        [XmlAttribute]
        public string NomImprimanteMoteur;
        [XmlAttribute]
        public eTypePointImpression TypePointImpression;
        [XmlAttribute]
        public string AdresseIP;
        [XmlAttribute]
        public string MasqueDefaut;
    }

    public class ImpressionResolutionVariables {
        public List<VariableMasqueCodesoft> DefaultListVariableMasqueCodesoft;
    }

    public class VariableMasqueCodesoft {
        [XmlAttribute]
        public string NomMasque;
        [XmlAttribute]
        public string NomVariable;   // Nom du variable dans le masque
        [XmlAttribute]
        public string NomVariableDATA;   // NEW :  pour directement affecter à une variable globale gérée par les DATA  ( .. on reporte le calcul des variables dans un process global au DATA )
        [XmlAttribute]
        public bool IsDefine;   // Precise si explicitement défini
    }
}
