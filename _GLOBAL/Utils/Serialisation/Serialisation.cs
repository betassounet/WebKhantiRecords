using _Global.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace _GLOBAL.Utils.Serialisation {
    public enum eEncodingFormat { UTF8, UTF16 }

    #region serialisation / deserialisation to / from path

    public class Serialization {

        public static string SerializeObject(Object o, string Path) {
            return SerializeObjectToPath( o, Path);
        }

        // Serialisation d'un objet dans un fichier
        public static string SerializeObjectToPath(Object o, string Path) {
            string sReturn = "OK";
            // Nouveau text writer et xml serializer
            try {
                TextWriter tr = new StreamWriter(Path);
                XmlSerializer sr = new XmlSerializer(o.GetType());
                // serialiser l'objet
                sr.Serialize(tr, o);
                tr.Close();
            }
            catch (Exception exception) {
                sReturn = "Exception : " + exception.Message;
                SingleLogFileAsXml.Instance().AjouteLog("Serialization", "@SerializeObjectToPath : ex :" + exception.Message);
            }
            return sReturn;
        }


        public static Object DeSerializeObject(Type typeObjet, string Path, ref string sMessage) {
            return DeSerializeObjectFromPath(typeObjet, Path, ref sMessage);
        }

        // deserialisation d'un objet de type donné a partir d'un fichier		
        public static Object DeSerializeObjectFromPath(Type typeObjet, string Path, ref string sMessage) {
            sMessage = "OK";
            // reference au type products
            Object o = null;
            // FileStream pour ouvrir l'objet serialisé
            FileStream f;
            try { f = new FileStream(Path, FileMode.Open); }
            catch (Exception exception) {
                SingleLogFileAsXml.Instance().AjouteLog("Serialization", "@DeSerializeObjectFromPath : ex :" + exception.Message);
                return null;
            }
            // nouveau serialiseur
            XmlSerializer newSr = new XmlSerializer(typeObjet);
            // deserialise
            try { o = newSr.Deserialize(f); }
            catch (Exception exception) {
                sMessage = "Exception : " + exception.Message;
                SingleLogFileAsXml.Instance().AjouteLog("Serialization", "@DeSerializeObjectFromPath : ex :" + exception.Message);
            }
            f.Close();
            return o;
        }

    #endregion

        // ATTENTION sur les formats de string UTF8 ( ASCII) ou UTF16 ( UNICODE )
        // il est impossible de melanger serialisation / deserialisation dans des formats difffernts même si on a l'impression d'avoir la même chaine serialisé..il y a des entêtes de chaine au niveau des byte[]...
        // Voir article sur encodage : http://www.undermyhat.org/blog/2009/08/tip-force-utf8-or-other-encoding-for-xmlwriter-with-stringbuilder/

        #region serialisation to string en UTF 8 ( ASCII )

        // Serialisation d'un objet dans un XML string
        public static string SerializeObject_UTF8<T>(T obj) {
            try {
                string xmlstring = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(T));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);  // pas possible a priori serialiser en UTF-16..
                xs.Serialize(xmlTextWriter, obj);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                xmlstring = UTF8ByteArrayToString(memoryStream.ToArray());
                return xmlstring;
            }
            catch {
                return string.Empty;
            }
        }

        // deserialisation a partir de la chaine string
        public static T DeserializeObject_UTF8<T>(string xml) {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }

        #endregion

        #region serialisation to string en UTF16 ( UNICODE, compatible avec s=client silverlight )

        // Serialisation d'un objet dans un XML string
        public static string SerializeObject_UTF16<T>(T obj) {
            try {
                XmlSerializer serialiser = new XmlSerializer(typeof(T));
                StringWriter stringWritter = new StringWriter();
                serialiser.Serialize(stringWritter, obj);
                return stringWritter.ToString();
            }
            catch {
                return string.Empty;
            }
        }

        public static T DeserialiseObject_UTF16<T>(string xml) {
            try {
                XmlSerializer serialiser = new XmlSerializer(typeof(T));
                StringReader stringReader = new StringReader(xml);
                //XmlReader reader = new XmlTextReader(stringReader);  // ca ca marche pas
                return (T)serialiser.Deserialize(stringReader);        // mais a priori pas besoin..
            }
            catch {
                return default(T);
            }
        }

        #endregion


        #region Cloner une classe

        // Une façon pour recuperer un clone d'une instance est de serialiser l'objet a copier et deserialiser dans une autre instance
        public static T GetClone<T>(T o) {
            string s = SerializeObject_UTF8<T>(o);
            return DeserializeObject_UTF8<T>(s);
        }

        #endregion


        #region Utils

        // utilitaire pour serialisation :

        // conversion Byte Array d'Unicode ( UTF-8 ) en string
        private static string UTF8ByteArrayToString(byte[] characters) {
            UTF8Encoding encoding = new UTF8Encoding();
            string s = encoding.GetString(characters);
            return s;
        }

        // conversion string to UTF8 Byte Array
        private static Byte[] StringToUTF8ByteArray(string XmlString) {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(XmlString);
            return byteArray;
        }

        public static string UTF16String(string XmlString) {
            Byte[] bytes = StringToUTF8ByteArray(XmlString);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        #endregion
    }

    #region Classe de test

    public class TestXml {
        public TestXml() {
            infoToto = new InfoToto();
        }

        public TestXml(int Mode) {
            id1 = 2;
            listInt = new List<int>() { 1, 2, 5, 3, 5 };
            ListListe1 = new List<Liste1>();
            ListListe1.Add(new Liste1() { Id = 4 });
            ListListe1.Add(new Liste1() { Id = 5 });
        }

        [XmlAttribute]
        public int id1;

        public List<int> listInt;
        public List<Liste1> ListListe1;
        public List<int> liste2;
        public InfoToto infoToto;
        public List<InfoToto> ListInfoToto;
    }

    public class InfoToto {
        public List<int> liste4;
    }

    public class Liste1 {
        public int Id;
        public List<int> liste3;
    }

    #endregion

}
