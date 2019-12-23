using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhantiRecordsMetier {

    public partial class SingleKhanti {


        public RepUpdateConfig UpdateConfig(ParamUpdateConfig paramUpdateConfig) {
            RepUpdateConfig Rep = new RepUpdateConfig();
            Rep.configParams = LoadConfigFile();
            return Rep;
        }
    
    }

    public class RepUpdateConfig {
        public long ExecutionTimeMs;
        public string Message;
        public ConfigParams configParams;
    }

    public class ParamUpdateConfig {
        public string sAction;
        public ConfigParams configParams;
    }

}
