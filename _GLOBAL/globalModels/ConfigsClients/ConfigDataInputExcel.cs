using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {

    #region Config pour récupération d'un fichier Excel pour remplissage et Update Articles.

    public class XmlConfigInputExcelFile {
        [XmlAttribute]
        public int NbCol;
        [XmlAttribute]
        public string FamilleName;
        [XmlAttribute]
        public int ColPosForHeaderCodeInterne;    // Facon basique pour Le nom de la colonne qui va être le codeInterne de l'Article.. voir pour composition..
        [XmlAttribute]
        public int ColPosForHeaderCodeExterne;    // Facon basique pour Le nom de la colonne qui va être le codeExterne de l'Article.. voir pour composition..
        [XmlAttribute]
        public string ComputeHeader;    // voir pour une façon de computer le code Interne..
        [XmlAttribute]
        public bool ConfigOKForUpdateArticle;
        public List<ColInfo> ColInfos;
    }

    public class ColInfo {
        [XmlAttribute]
        public int ColPosition;
        [XmlAttribute]
        public string ColName;
        [XmlAttribute]
        public string ToArticleNomCritere;
        [XmlAttribute]
        public string ToArticleCodeCritere;
        [XmlAttribute]
        public eTypeVariablesCriteres ToArticleTypeCritere;
        [XmlAttribute]
        public bool IsCritereFamille;
        [XmlAttribute]
        public bool IsMandatory;
    }


    #endregion
}
