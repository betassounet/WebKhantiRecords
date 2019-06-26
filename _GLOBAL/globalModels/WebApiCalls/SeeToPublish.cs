using _GLOBAL.globalModels.ConfigsClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.WebApiCalls {


    public class InfoKeyValues {
        [XmlAttribute]
        public string Key;
        [XmlAttribute]
        public string Value;
    }

    public class InfoKeyValuesType {
        [XmlAttribute]
        public string Key;
        [XmlAttribute]
        public string Value;
        [XmlAttribute]
        public eTypeVariablesCriteres TypeVariablesCriteres;
    }

    //[Flags]
    //public enum eTypeVariablesCriteres {
    //    KeyChoixOperateur = 0x00000001,       // cette variable/critere est utilisable pour selectionner un article par filtre sur IHM
    //    KeyChoixOF = 0x00000002,              // Un OF pourait choisir un Article sur un critère ? mieux passer par le code interne.. plus simple
    //    KeyBindORPHEAArticle = 0x00000004,    // critère de correlation avec un Article ORPHEA .. a choisir dans la table des articles ORPHEA
    //    KeyChangementLotProcess = 0x00000008,        // critère de changement de Lot pour les articles Palox input ( on recopie les palox de la table ORPHEA dans une table process, correspond a nom de champ dans table process..)
    //    KeyChangementLotNatifOrphea = 0x00000010,        // critère de changement de Lot pour les articles Palox input ( Nom de champ de la BDD Orphea direct ?? a voir )
    //    KeyCalculPoidsPacking = 0x00000020,           // ce critère doit etre convertible en valeur numerique pour servir de calcul de poids
    //    ParamPalettisation = 0x00000040,
    //    KeyConsignePoids = 0x00000080,
    //    KeyConsigneNbre = 0x00000100,
    //    ParamContante = 0x00000200,
    //    ParamProcess = 0x00000400,
    //    UsedInContexe = 0x00000800,    // SEUL UTILISE pour permettre visu dans contexte de Flux
    //    SourceArticle = 0x00001000,    //  UTILISE pour permettre reconnaissance provenance
    //    SourceFlux = 0x00002000,    //  UTILISE pour permettre reconnaissance provenance
    //    SourceUser = 0x00004000,    //  UTILISE pour permettre reconnaissance provenance
    //    SourceSystem = 0x00008000,    //  UTILISE pour permettre reconnaissance provenance
    //}

}
