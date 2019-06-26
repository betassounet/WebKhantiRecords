using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {


    //-----------------------------------------------------------------------------------------------------
    // CONFIG Pour DATA
    // Donne la configuration de DATA pour le client.
    // Comment on travaille sur les OF.. recus de GPOA, basées sur Articles..
    // Construits a la volée sur Articles, Détermination des familles d'articles concernés 
    // Vérification des données d'entrées sur critères, sur liste..
    // Gestion de la palettisation, commandes pour PLC ( Articles palettes )
    // Vérification des données en général
    //-----------------------------------------------------------------------------------------------------

    #region XmlConfigClientData

    public class XmlConfigClientData {
        [XmlAttribute]
        public string CLientName;
        public ConfigWO configWO;
        public ConfigVidage configVidage;
        public ConfigPacking configPacking;
        public ConfigEcarts configEcarts;
        public ConfigPalettisation configPalettisation;
    }

    public class ConfigWO {
        [XmlAttribute]
        public bool ERPprovideWO;
        [XmlAttribute]
        public int ProfondeurWOQueue;   // profondeur niveau WO 1, 2 ou 3 positions  pour VIDAGE/PACKING/PALETTE, conditionne l'ordre WO
    }

    public class ConfigVidage {
        [XmlAttribute]
        public bool ControleInputCritere;
        [XmlAttribute]
        public bool ControleInputListe;
        [XmlAttribute]
        public bool Bypass;
        [XmlAttribute]
        public bool OnErrorTakePreviousCode;
        [XmlAttribute]
        public int MaxConsecutiveError;
    }

    public class ConfigPacking {
        [XmlAttribute]
        public bool ControleEtiquette;
        [XmlAttribute]
        public bool ChoixBypassEtiquette;
        [XmlAttribute]
        public bool ChoixEtiquette;
        [XmlAttribute]
        public bool EtiquetteDefaut;
        [XmlAttribute]
        public string NomEtiquetteDefaut;
        [XmlAttribute]
        public bool ChoixArticles;
        [XmlAttribute]
        public string NomFamilleArticle;
    }

    public class ConfigEcarts {
        [XmlAttribute]
        public bool EtiquetteDefaut;
        [XmlAttribute]
        public string NomEtiquetteDefaut;
    }

    public class ConfigPalettisation {
        [XmlAttribute]
        public bool ControleParametresWO;
        [XmlAttribute]
        public bool EtiquetteDefaut;
        [XmlAttribute]
        public string NomEtiquetteDefaut;
        [XmlAttribute]
        public bool ChoixArticles;
        [XmlAttribute]
        public string NomFamilleArticle;
    }


    #endregion


}
