using _Global.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WebKhantiRecords.NoIp {

    // voir : https://lea-linux.org/documentations/Trucs:Automiser_les_mises-%C3%A0-jour_pour_le_service_NO-IP.COM

    // But automatiser les mises a jour pour le service NO_IP.com
    // NO IP offre un service de redirection IP
    // NO IP fournissait jusqu'à présent un petit programme a installer sur sa machine pour mettre a jopur l'IP de manière automatique.. programme buggé..
    // depuis NOIP a publié un protocole d'uptate d'IP qui repose sur une requette http unique. Ilm n'est pas necessaire de communiquer son IP puisque le service NOIP la detecte lui même.
    //LOGIN=your mail            # NoIp login 
    //PASS=password             # NoIp pass
    //HOST=mysite.no-ip.org     # The site to be updated
    // http://dynupdate.no-ip.com/dns?username=$LOGIN&password=$PASS&hostname=$HOST

    public class NoIpService {

        string LOGIN = "ffauvel@hotmail.com";
        string PASS = "ffNoIP2015";
        string HOST = "testkhanti.servebeer.com";

         #region Constructeur Singleton

        protected static NoIpService instance;

        protected NoIpService() {
        }

        public static NoIpService Instance() {
            if (instance == null)
                instance = new NoIpService();
            return instance;
        }

        #endregion

        public void SendNoIPUpdate() {
            string URL = @"http://dynupdate.no-ip.com/dns?username="+LOGIN+"&password="+PASS+"&hostname="+HOST;
            var webRequest = WebRequest.Create(URL);

            try {
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content)) {
                    var strContent = reader.ReadToEnd();
                }
            }
            catch (Exception ex) {
                SingleLogFileAsXml.Instance().AjouteLog("NoIpService", "SendNoIPUpdate : Exception " + ex.Message);
            }
        }
    }
}