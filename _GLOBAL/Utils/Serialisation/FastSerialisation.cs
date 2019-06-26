using _Global.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.Serialisation {

    public class FastSerialisation {

        #region constructeur singleton

        protected static FastSerialisation instance;

        protected FastSerialisation() {
        }

        public static FastSerialisation Instance() {
            if (instance == null)
                instance = new FastSerialisation();
            return instance;
        }
        #endregion

        public T GetSaveStructInCurrentDirectory<T>(string sFileName) {
            string sMessage = "";
            return GetSaveStructInCurrentDirectory<T>(sFileName, ref sMessage);
        }

        public T GetSaveStructInCurrentDirectory<T>(string sFileName, ref string sMessage) {
            T obj = default(T);
            if (File.Exists(sFileName)) {
                try {
                    obj = (T)Serialization.DeSerializeObject(typeof(T), sFileName, ref sMessage);
                    return obj;
                }
                catch (Exception ex) {
                    SingleLogFileAsXml.Instance().AjouteLog("FastSerialisation", "@GetSaveStructInCurrentDirectory : Exception pour file : " + sFileName + "  ex :" + ex.Message);
                }
            }
            else {
                SingleLogFileAsXml.Instance().AjouteLog("FastSerialisation", "File not exist : " + sFileName);
            }
            return obj;
        }

        public string  SaveStructInCurrentDirectory<T>(T obj, string sFileName) {
            string returnValue = "";
            try {
                returnValue = Serialization.SerializeObject(obj, sFileName);
            }
            catch (Exception ex) {
                SingleLogFileAsXml.Instance().AjouteLog("FastSerialisation", "@SaveStructInCurrentDirectory :Exception pour file : " + sFileName + "  ex :" + ex.Message);
            }
            return returnValue;
        }

    }
}
