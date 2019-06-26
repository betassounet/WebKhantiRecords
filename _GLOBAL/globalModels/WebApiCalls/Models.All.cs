using _GLOBAL.globalModels.ConfigsClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.globalModels.WebApiCalls {

    public class ReponseActionBDD {
        public long ExecutionTimeMs;
        public bool isOK;
        public bool AlreadyInit;
        public int Error;
        public string sMessage;
    }

    public class ReponseListePathCritereValeur {
        public long ExecutionTimeMs;
        public bool isOK;
        public int Error;
        public string sMessage;
        public string sPath;
        public int IdExemple;          // L'id pris en exemple
        public List<PathCritereValeurItem> listPathCritereValeurItem;
    }

    public class PathCritereValeurItem {
        public string Path;
        public string Critere;
        public string ExempleValeur;
    }

    public class ParamBool {
        public bool mode;
    }

    public class ParamString {
        public string dataString;
    }

    public class ParamInt {
        public int dataInt;
    }

    public class ParamDT {
        public DateTime dt;
    }

    public class RepCritereValueItem {
        public int IdItem;
        public string Value;
        public int Error;
        public string ErrorMessage;
    }


    public class ArticleKeyValuesType {
        public string Key;
        public string Value;
        public eTypeVariablesCriteres TypeVariablesCriteres;
    }

    public class ReponseString {
        public long ExecutionTimeMs;
        public string sMessage;
    }
}
