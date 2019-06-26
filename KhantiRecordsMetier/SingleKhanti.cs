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
    public class SingleKhanti
    {

        List<LinePayLoad> CurrentlistLinePayLoad;
        List<string> listUrlImageArtiste;
        RepAnalyseFichiers repAnalyseFichiers;
        GlobalLinePayLoad globalLinePayLoad;

        #region Constructeur Singleton

        protected static SingleKhanti instance;

        protected SingleKhanti() {
            Init();
            LoadRepository();
        }

        public static SingleKhanti Instance() {
            if (instance == null)
                instance = new SingleKhanti();
            return instance;
        }

        #endregion

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


        private void Init() {

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

        public RepTest AnalyseFichier(ParamTest param) {
            RepTest rep = new RepTest() { listInfosArtiste = new List<InfosArtiste>()};
            string sPathDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            sPathDirectory += "\\Files\\UpLoad\\Relevé digital détail mensuel\\";
            string sPathFile = sPathDirectory + param.NomFichier;
            var repExcel = SingleExcel.Instance().AnalyseFichier(sPathFile, param.NomFichier);
            CurrentlistLinePayLoad = repExcel.listLinePayLoad;
            rep.sMessage = repExcel.Message;
            string sPeriode = repExcel.sPeriode;

            if (CurrentlistLinePayLoad != null) {
                var listArtiste = CurrentlistLinePayLoad.Select(c => c.Artist).Distinct().ToList();
                foreach (var art in listArtiste) {
                    InfosArtiste infosArtiste = new InfosArtiste() { NomArtiste = art, UrlImage = "" };

                    infosArtiste.UrlImage = GetUrlImageArtiste(art, ref infosArtiste.NomRecherche);

                    if (param.WithDetails) {
                        var listInfos = CurrentlistLinePayLoad.Where(c => c.Artist == art).ToList();

                        infosArtiste.DownLoadQuantity = listInfos.Sum(c => c.DownloadQty);
                        infosArtiste.GrossRevenue = listInfos.Sum(c => c.GrossRevenue);
                        infosArtiste.NetRevenue = listInfos.Sum(c => c.NetRevenue);
                        infosArtiste.TotalFee = listInfos.Sum(c => c.TotalFee);
                        infosArtiste.TotalPayable = listInfos.Sum(c => c.TotalPayable);

                        rep.TotalDownload+=infosArtiste.DownLoadQuantity ;
                        rep.TotalTotalGrossRevenue+=infosArtiste.GrossRevenue ;
                        rep.TotalTotalNetRevenue += infosArtiste.NetRevenue;
                        rep.TotalTotalFee+=infosArtiste.TotalFee ;
                        rep.TotalTotalPayable+=infosArtiste.TotalPayable;
                    }

                    rep.listInfosArtiste.Add(infosArtiste);

                }

                int CptUpdate = 0;
                if (globalLinePayLoad != null && globalLinePayLoad.listLine != null){
                    var prelist = globalLinePayLoad.listLine.Where(c => c.sFile == param.NomFichier && c.sPeriode == sPeriode ).ToList();
                    foreach (var line in CurrentlistLinePayLoad) {
                        var exist = prelist.Where(c => c.Line == line.Line && c.TotalPayable == line.TotalPayable).FirstOrDefault();
                        if (exist == null) {
                            globalLinePayLoad.listLine.Add(line);
                            CptUpdate++;
                        }
                    }
                    if (CptUpdate > 0) {
                        SaveRepository();
                    }
                }

            }
            return rep;
        }

        private string GetUrlImageArtiste(string art, ref string NomRecherche) {
            string url = @"./img/image-not-found.jpg";
            if (art != null) {
                string tempArt = art.Trim();
                tempArt = tempArt.Replace(" ", "-");
                tempArt = tempArt.ToLower();
                NomRecherche = tempArt;
                if (listUrlImageArtiste != null) {
                    var exist = listUrlImageArtiste.Where(c => c.Contains(tempArt)).FirstOrDefault();
                    if (exist != null) {
                        url = @"http://khanti.fr/wp-content/uploads/2019/05/" + exist;
                    }
                }
            }
            return url;
        }

        public RepDetailArtiste GetDetailArtiste(ParamDetailArtiste param) {
            RepDetailArtiste rep = new RepDetailArtiste() { NomArtiste = param.NomArtiste, sMessage = "", listProduct = new List<ProductByCritere>() };
            rep.sMessage = "Details pour " + param.NomArtiste;

            try {
                if (CurrentlistLinePayLoad != null) {
                    var listInfos = CurrentlistLinePayLoad.Where(c => c.Artist == param.NomArtiste).ToList();

                    var listProduct = listInfos.Select(c => c.Product).Distinct().ToList();
                    var listDMS = listInfos.Select(c => c.DMS).Distinct().ToList();
                    var listTerritory = listInfos.Select(c => c.Territory).Distinct().ToList();
                    var listFormatType = listInfos.Select(c => c.FormatType).Distinct().ToList();
                    var listPriceCategory = listInfos.Select(c => c.PriceCategory).Distinct().ToList();


                    foreach (var prod in listProduct) {

                        var listProd = listInfos.Where(c => c.Product == prod).ToList();

                        var totalDownLoad = listProd.Sum(c => c.DownloadQty);
                        var totalProd = listProd.Sum(c => c.TotalPayable);
                        List<string> listISRC = listProd.Select(c => c.ISRC).Distinct().ToList();
                        List<string> listCB = listProd.Select(c => c.Barcode).Distinct().ToList();
                        List<string> listID = new List<string>();
                        listID.AddRange(listISRC);
                        listID.AddRange(listCB);
                        if (listID.Contains("null"))
                            listID.Remove("null");
                        string sID = "";
                        foreach( var v in listID){
                            string stemp = v.Trim();
                            if(!string.IsNullOrEmpty(stemp))
                            sID +=stemp+" "; 
                        }

                        ProductByCritere productByCritere = new ProductByCritere() { NomProduct = prod, ID = sID, TotalEuros = totalProd, TotalStreaming = totalDownLoad, listKeyQuantityCritereCategorie = new List<KeyQuantity>(), listKeyQuantityCritereDNS = new List<KeyQuantity>(), listKeyQuantityCritereFormatType = new List<KeyQuantity>(), listKeyQuantityCritereTerritory= new List<KeyQuantity>() };
                        foreach (var dms in listDMS) {
                            var listForDms = listProd.Where(c => c.DMS == dms).ToList();
                            var nbDownLoad = listForDms.Sum(c => c.DownloadQty);
                            productByCritere.listKeyQuantityCritereDNS.Add(new KeyQuantity() { key = dms, Quantity = nbDownLoad });
                        }
                        foreach (var terre in listTerritory) {
                            var listForDms = listProd.Where(c => c.Territory == terre).ToList();
                            var nbDownLoad = listForDms.Sum(c => c.DownloadQty);
                            productByCritere.listKeyQuantityCritereTerritory.Add(new KeyQuantity() { key = terre, Quantity = nbDownLoad });
                        }
                        foreach (var format in listFormatType) {
                            var listForDms = listProd.Where(c => c.FormatType == format).ToList();
                            var nbDownLoad = listForDms.Sum(c => c.DownloadQty);
                            productByCritere.listKeyQuantityCritereFormatType.Add(new KeyQuantity() { key = format, Quantity = nbDownLoad });
                        }
                        foreach (var price in listPriceCategory) {
                            var listForDms = listProd.Where(c => c.PriceCategory == price).ToList();
                            var nbDownLoad = listForDms.Sum(c => c.DownloadQty);
                            productByCritere.listKeyQuantityCritereCategorie.Add(new KeyQuantity() { key = price, Quantity = nbDownLoad });
                        }

                        rep.listProduct.Add(productByCritere);

                    }

                    rep.listKeyTotalQuantityCritereDNS = new List<KeyQuantity>();
                    foreach (var dms in listDMS) {
                        rep.listKeyTotalQuantityCritereDNS.Add(new KeyQuantity(){ key = dms, Quantity=0});
                    }
                    foreach (var d in rep.listKeyTotalQuantityCritereDNS) {
                        d.Quantity = 0;
                        foreach (var v in rep.listProduct) {
                            var exist = v.listKeyQuantityCritereDNS.Where(c => c.key == d.key).FirstOrDefault();
                            if (exist != null) {
                                d.Quantity += exist.Quantity;
                            }
                        }
                    }

                    rep.listKeyTotalQuantityCritereTerritory = new List<KeyQuantity>();
                    foreach (var terre in listTerritory) {
                        rep.listKeyTotalQuantityCritereTerritory.Add(new KeyQuantity() { key = terre, Quantity = 0 });
                    }
                    foreach (var d in rep.listKeyTotalQuantityCritereTerritory) {
                        d.Quantity = 0;
                        foreach (var v in rep.listProduct) {
                            var exist = v.listKeyQuantityCritereTerritory.Where(c => c.key == d.key).FirstOrDefault();
                            if (exist != null) {
                                d.Quantity += exist.Quantity;
                            }
                        }
                    }

                    rep.listKeyTotalQuantityCritereFormatType = new List<KeyQuantity>();
                    foreach (var format in listFormatType) {
                        rep.listKeyTotalQuantityCritereFormatType.Add(new KeyQuantity() { key = format, Quantity = 0 });
                    }
                    foreach (var d in rep.listKeyTotalQuantityCritereFormatType) {
                        d.Quantity = 0;
                        foreach (var v in rep.listProduct) {
                            var exist = v.listKeyQuantityCritereFormatType.Where(c => c.key == d.key).FirstOrDefault();
                            if (exist != null) {
                                d.Quantity += exist.Quantity;
                            }
                        }
                    }

                    rep.listKeyTotalQuantityCritereCategorie = new List<KeyQuantity>();
                    foreach (var cat in listPriceCategory) {
                        rep.listKeyTotalQuantityCritereCategorie.Add(new KeyQuantity() { key = cat, Quantity = 0 });
                    }
                    foreach (var d in rep.listKeyTotalQuantityCritereCategorie) {
                        d.Quantity = 0;
                        foreach (var v in rep.listProduct) {
                            var exist = v.listKeyQuantityCritereCategorie.Where(c => c.key == d.key).FirstOrDefault();
                            if (exist != null) {
                                d.Quantity += exist.Quantity;
                            }
                        }
                    }

                    rep.DownLoadQuantity = listInfos.Sum(c => c.DownloadQty);
                    rep.GrossRevenue = listInfos.Sum(c => c.GrossRevenue);
                    rep.NetRevenue = listInfos.Sum(c => c.NetRevenue);
                    rep.TotalFee = listInfos.Sum(c => c.TotalFee);
                    rep.TotalPayable = listInfos.Sum(c => c.TotalPayable);
                    string NomRecherche = "";
                    rep.UrlImage = GetUrlImageArtiste(param.NomArtiste, ref NomRecherche);

                }
                else {
                    rep.sMessage += " Pas d'analyse fichier en cours ";
                }

            }
            catch(Exception ex){
                rep.sMessage += " Exception :  "+ ex.Message;
            }
            return rep;
        }

        List<RepoArtiste> CurrentListRepoArtiste;

        public RepAnalyse2 Analyse2(ParamAnalyse2 param) {
            RepAnalyse2 rep = new RepAnalyse2(){ listRepoArtiste = new List<RepoArtiste>()};

            switch (param.sAction) {
                case "OrderName":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.Nom).ToList();
                    break;
                case "OrderDownload":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.TotalDownLoad).ToList();
                    break;
                case "OrderPays":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.NbPays).ToList();
                    break;
                case "OrderPayable":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.TotalPayable).ToList();
                    break;
                case "View":
                    var listeTouteLineArtiste = globalLinePayLoad.listLine.Where(c => c.Artist == param.NomArtiste).ToList();
                    string stemp="";
                    rep.resultsArtiste = new ResultsArtiste() { Nom = param.NomArtiste, Url = GetUrlImageArtiste(param.NomArtiste, ref stemp) };
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste;
                    break;

                default:
                    if (globalLinePayLoad != null && globalLinePayLoad.listLine != null) {
                        var listeArtiste = globalLinePayLoad.listLine.Select(c => c.Artist).Distinct().ToList();
                        foreach (var v in listeArtiste) {
                            var listLineArtiste = globalLinePayLoad.listLine.Where(c => c.Artist == v).ToList();
                            var sum = listLineArtiste.Sum(c => c.TotalPayable);
                            var NbDownLoad = listLineArtiste.Sum(c => c.DownloadQty);
                            var NbPays = listLineArtiste.Select(c => c.Territory).Distinct().Count();
                            rep.listRepoArtiste.Add(new RepoArtiste() { Nom = v, TotalDownLoad = NbDownLoad, TotalPayable = sum, NbPays = NbPays });
                        }
                    }
                    else {
                        rep.Message = "Repositoru = null";
                    }
                    break;
            }
            CurrentListRepoArtiste = rep.listRepoArtiste;
            return rep;
        }

    }

    public class GlobalLinePayLoad {
        public List<LinePayLoad> listLine;
    }

    public class ParamTest{
        public string sAction;
        public string NomFichier;
        public bool WithDetails;
    }

    public class RepTest{
        public long ExecutionTimeMs;
        public string sMessage;
        public int TotalDownload;
        public float TotalTotalGrossRevenue;
        public float TotalTotalNetRevenue;
        public float TotalTotalFee;
        public float TotalTotalPayable;
        public List<InfosArtiste> listInfosArtiste;
    }

    public class InfosArtiste {
        public string NomArtiste;
        public string NomRecherche;
        public string UrlImage;
        public int DownLoadQuantity;
        public float GrossRevenue;
        public float NetRevenue;
        public float TotalFee;
        public float TotalPayable; 
    }

    public class ParamDetailArtiste {
        public string NomArtiste;
    }

    public class RepDetailArtiste {
        public long ExecutionTimeMs;
        public string sMessage;
        public string NomArtiste;
        public int DownLoadQuantity;
        public float GrossRevenue;
        public float NetRevenue;
        public float TotalFee;
        public float TotalPayable;
        public string UrlImage;
        public List<ProductByCritere> listProduct;
        public List<KeyQuantity> listKeyTotalQuantityCritereDNS;
        public List<KeyQuantity> listKeyTotalQuantityCritereTerritory;
        public List<KeyQuantity> listKeyTotalQuantityCritereFormatType;
        public List<KeyQuantity> listKeyTotalQuantityCritereCategorie;
    }


    public class ProductByCritere {
        public string NomProduct;
        public string ID;
        public int TotalStreaming;
        public float TotalEuros;
        public List<KeyQuantity> listKeyQuantityCritereDNS;
        public List<KeyQuantity> listKeyQuantityCritereTerritory;
        public List<KeyQuantity> listKeyQuantityCritereFormatType;
        public List<KeyQuantity> listKeyQuantityCritereCategorie;
    }

    public class KeyQuantity {
        public string key;
        public int Quantity;
    }

    public class RepAnalyseFichiers {
        public long ExecutionTimeMs;
        public string Message;
        public List<IdFichiers> listIdFichiers;
    }

    public class ParamAnalyseFichiers {
        public string sAction;
    }

    public class IdFichiers {
        public int Id;
        public string sPAth;
        public string sFileName;
        public string extension;
        public string sPeriode;
        public string sMessage;
        public long delayMs;
        public int Error;
    }

    public class RepAnalyse2 {
        public long ExecutionTimeMs;
        public string Message;
        public List<RepoArtiste> listRepoArtiste;
        public ResultsArtiste resultsArtiste;
    }

    public class ParamAnalyse2 {
        public string sAction;
        public string NomArtiste;
    }

    public class RepoArtiste {
        public string Nom;
        public int TotalDownLoad;
        public float TotalPayable;
        public int NbPays;
    }

    public class ResultsArtiste {
        public string Nom;
        public string Url;
    }
}

