using _GLOBAL.globalModels.SessionConfigXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _GLOBAL.globalModels.ConfigsClients {

    public class XmlConfigEnvironnementClient {
        public string NomClient;
        public List<EnvironnementExecution> listEnvironnementExecution;
        public List<TypeFileConfigNeeded> listTypeFileConfigNeeded;
    }


    public class TypeFileConfigNeeded {
        [XmlAttribute]
        public string LibelleFile;
        [XmlAttribute]
        public string TypeFileConfig;
        [XmlAttribute]
        public string NameFileConfig;
    }
}
