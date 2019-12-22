using _Global.Log;
using _GLOBAL.Utils.Serialisation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KhantiRecordsMetier
{
    public partial class SingleKhanti
    {

        List<string> listUrlImageArtiste;
        List<string> listUrlImageArtisteLocal;
        RepAnalyseFichiers repAnalyseFichiers;
        GlobalLinePayLoad globalLinePayLoad;
        ArtistsAdminFile artistsAdminFile;

        #region Constructeur Singleton

        protected static SingleKhanti instance;

        protected SingleKhanti() {
            //InitRecherchePhotoSurWeb();
            InitImageLocal();
            LoadRepository();
        }

        public static SingleKhanti Instance() {
            if (instance == null)
                instance = new SingleKhanti();
            return instance;
        }

        #endregion


        #region Load And Save GlobalRepository

        private void LoadRepository() {
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPathRepoFile = sPathDirectory + "\\Files\\Repository\\GlobalPayLoad.xml";
            globalLinePayLoad = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<GlobalLinePayLoad>(sPathRepoFile);
            if (globalLinePayLoad == null) {
                globalLinePayLoad = new GlobalLinePayLoad() { listLine = new List<LinePayLoad>() };
            }
        }

        private void SaveRepository() {
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPathRepoFile = sPathDirectory + "\\Files\\Repository\\GlobalPayLoad.xml";
            FastSerialisation.Instance().SaveStructInCurrentDirectory<GlobalLinePayLoad>(globalLinePayLoad, sPathRepoFile);
        }

        private ArtistsAdminFile LoadArtistsAdminFile() {
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPathRepoFile = sPathDirectory + "\\Files\\Repository\\ArtistsAdminFile.xml";
            artistsAdminFile = FastSerialisation.Instance().GetSaveStructInCurrentDirectory<ArtistsAdminFile>(sPathRepoFile);
            if (artistsAdminFile == null) {
                artistsAdminFile = new ArtistsAdminFile() { listArtistAdminItems = new List<ArtistAdminItem>() };
            }
            return artistsAdminFile;
        }

        private void SaveArtistsAdminFile() {
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPathRepoFile = sPathDirectory + "\\Files\\Repository\\ArtistsAdminFile.xml";
            FastSerialisation.Instance().SaveStructInCurrentDirectory<ArtistsAdminFile>(artistsAdminFile, sPathRepoFile);
        }

        #endregion


        #region Init Recherche sur site web khanti.fr des photos

        private void InitRecherchePhotoSurWeb() {

            // On va chercher sur la page du site khanti.fr les url des images des artistes..
            // pour cela on fait une requete web sur le site et on va essayer de parser le contenu..
            var webRequest = WebRequest.Create(@"http://khanti.fr/wp-content/uploads/2019/05/");

            try {
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content)) {
                    var strContent = reader.ReadToEnd();

                    // On a les lignes html qui nous interessent qui sont de ce style :
                    // On veut recupèrer la liste des images derriere le a href=".....
                    //<img src="/__ovh_icons/image2.gif" alt="[IMG]"> <a href="aluna-project.jpg">aluna-project.jpg</a>       2019-05-16 15:42  121K 

                    // On va prendre chaque ligne, ne garder que celles avec "img"
                    var list1 = strContent.Split('\n').ToList();
                    List<string> list2 = new List<string>();
                    foreach (var l in list1) {
                        if (l.Contains("img")) {
                            list2.Add(l);
                        }
                    }
                    // repèrage de a href
                    List<string> list3 = new List<string>();
                    foreach (var l in list2) {
                        int idx = l.IndexOf("a href=");
                        if (idx > 0) {
                            string stemp = l.Substring(idx + 8);
                            int idxFin = stemp.IndexOf(">");
                            if (idxFin > 0) {
                                string stemp2 = stemp.Substring(0, idxFin - 1);
                                if (stemp2.Contains("150x150"))  // que les images en 150x150 tel que ecrit dans le site
                                    list3.Add(stemp2);
                            }
                        }
                    }
                    listUrlImageArtiste = list3.Distinct().ToList();
                }
            }
            catch (Exception ex) {
                SingleLogFileAsXml.Instance().AjouteLog("SingleKhanti", "Init : Exception " + ex.Message);
            }
        }

        #endregion

        void InitImageLocal() {
            listUrlImageArtisteLocal = new List<string>();
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPathImageArtistes = sPathDirectory + "\\img\\Artistes";
            try {
                var listTmp = Directory.GetFiles(sPathImageArtistes).ToList();
                foreach( var f in listTmp) {
                    string fileSeul = f.Replace(sPathImageArtistes + "\\", "");
                    listUrlImageArtisteLocal.Add(fileSeul.Trim());
                }
                
            }
            catch (Exception ex) {
                SingleLogFileAsXml.Instance().AjouteLog("SingleKhanti", "Init : Exception " + ex.Message);
            }
            
        }


        #region Analyse des fichiers excel presents sur disque

        public RepAnalyseFichiers AnalyseFichiers( ParamAnalyseFichiers param) {
            repAnalyseFichiers = new RepAnalyseFichiers() { listIdFichiers = new List<IdFichiers>() };
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            sPathDirectory += "\\Files\\UpLoad\\Relevé digital détail mensuel";
            var listFiles = Directory.GetFiles(sPathDirectory).ToList();
            int idxinc = 0;
            foreach (var v in listFiles) {
                IdFichiers idFichiers = new IdFichiers() { Id = idxinc++, sPAth = v, sFileName = Path.GetFileName(v), extension = Path.GetExtension(v) };
                if (idFichiers.extension == ".xlsx") {
                    Stopwatch sw = Stopwatch.StartNew();
                    var res = SingleExcel.Instance().GetPeriodFile(v);
                    idFichiers.sPeriode = res.sPeriode;
                    idFichiers.sMessage = res.Message;
                    idFichiers.delayMs = sw.ElapsedMilliseconds;
                }
                else {
                    idFichiers.Error = 1;
                    idFichiers.sMessage = "Bad Extension must be xlsx ";
                }
                repAnalyseFichiers.listIdFichiers.Add(idFichiers);
            }
            return repAnalyseFichiers;
        }

        #endregion


        #region Récupèration de l'URL pour photo artiste

        private string GetUrlImageArtiste(string art, ref string NomRecherche) {
            string url = @"./img/image-not-found.jpg";
            if (art != null) {
                string tempArt = art.Trim();
                tempArt = tempArt.Replace(" ", "-");
                tempArt = tempArt.ToLower();
                NomRecherche = tempArt;
                bool isOK = false;
                if (listUrlImageArtiste != null) {
                    var exist = listUrlImageArtiste.Where(c => c.Contains(tempArt)).FirstOrDefault();
                    if (exist != null) {
                        url = @"http://khanti.fr/wp-content/uploads/2019/05/" + exist;
                        isOK = true;
                    }
                }
                // si non OK liste de secours..
                if (!isOK) {
                    var exist = listUrlImageArtisteLocal.Where(c => c.Contains(tempArt)).FirstOrDefault();
                    if (exist != null) {
                        url = @"./img/Artistes/"+ exist;
                    }
                }
            }
            return url;
        }

        #endregion



    }

    public class GlobalLinePayLoad {
        public List<LinePayLoad> listLine;
    }


}

