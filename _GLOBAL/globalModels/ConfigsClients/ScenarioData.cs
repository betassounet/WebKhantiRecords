using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//using System.Xml;

namespace _GLOBAL.globalModels.ConfigsClients {
    
    public class XmlScenarioData {
        public List<XmlParamInsertInApport> listXmlParamInsertInApport;
        public List<XmlItemArticleOrphea> listItemArticleOrphea;
    }

    public class XmlParamInsertInApport {
        [XmlAttribute]
        public long Id;
        [XmlAttribute]
        public string Command;
        public List<keyVal> keyVals;
    }

    public class keyVal{
        [XmlAttribute]
        public string key;
        [XmlAttribute]
        public string value;
    }

    public class XmlItemArticleOrphea {
        [XmlAttribute]
        public long Id;
        [XmlAttribute]
        public string ArticleCode;
        [XmlAttribute]
        public string ArticleName;
        [XmlAttribute]
        public string ArticleShortName;
        [XmlAttribute]
        public string ArticleType;
        [XmlAttribute]
        public string Variety;
    }
}
