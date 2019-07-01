using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhantiRecordsMetier {


    public partial class SingleKhanti {



        List<RepoArtiste> CurrentListRepoArtiste;
        ResultatGlobal CurrentResultatGlobal;

        public RepAnalyse2 Analyse2(ParamAnalyse2 param) {
            RepAnalyse2 rep = new RepAnalyse2() { listRepoArtiste = new List<RepoArtiste>(), resultatGlobal = new ResultatGlobal() };


            if (CurrentResultatGlobal != null) {
                rep.resultatGlobal = CurrentResultatGlobal;
            }

            switch (param.sAction) {
                case "OrderName":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.Nom).ToList();
                    break;

                case "OrderNameDesc":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderByDescending(c => c.Nom).ToList();
                    break;

                case "OrderDownload":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.TotalDownLoad).ToList();
                    break;

                case "OrderDownloadDesc":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderByDescending(c => c.TotalDownLoad).ToList();
                    break;

                case "OrderPays":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.NbPays).ToList();
                    break;

                case "OrderPaysDesc":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderByDescending(c => c.NbPays).ToList();
                    break;

                case "OrderPayable":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderBy(c => c.TotalPayable).ToList();
                    break;

                case "OrderPayableDesc":
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste.OrderByDescending(c => c.TotalPayable).ToList();
                    break;

                case "View":
                    var listeTouteLineArtiste = globalLinePayLoad.listLine.Where(c => c.Artist == param.NomArtiste).ToList();
                    string stemp = "";
                    rep.resultsArtiste = new ResultsArtiste() { Nom = param.NomArtiste, Url = GetUrlImageArtiste(param.NomArtiste, ref stemp) };
                    if (CurrentListRepoArtiste != null)
                        rep.listRepoArtiste = CurrentListRepoArtiste;
                    break;

                default:
                    if (globalLinePayLoad != null && globalLinePayLoad.listLine != null) {

                        rep.resultatGlobal.listPeriodes = globalLinePayLoad.listLine.Select(c => c.sPeriode).Distinct().ToList();
                        rep.resultatGlobal.NbArtiste = 0;
                        rep.resultatGlobal.TotalDownload = 0;
                        rep.resultatGlobal.TotalPayable = 0;

                        var listeArtiste = globalLinePayLoad.listLine.Select(c => c.Artist).Distinct().ToList();
                        foreach (var v in listeArtiste) {
                            var listLineArtiste = globalLinePayLoad.listLine.Where(c => c.Artist == v).ToList();
                            var sum = listLineArtiste.Sum(c => c.TotalPayable);
                            var NbDownLoad = listLineArtiste.Sum(c => c.DownloadQty);
                            var NbPays = listLineArtiste.Select(c => c.Territory).Distinct().Count();
                            string NomRecherche = "";
                            string UrlImage = GetUrlImageArtiste(v, ref NomRecherche);
                            rep.listRepoArtiste.Add(new RepoArtiste() { Nom = v, TotalDownLoad = NbDownLoad, TotalPayable = sum, NbPays = NbPays, UrlImage = UrlImage });
                            rep.resultatGlobal.NbArtiste++;
                            rep.resultatGlobal.TotalDownload += NbDownLoad;
                            rep.resultatGlobal.TotalPayable += sum;
                        }
                        CurrentResultatGlobal = rep.resultatGlobal;
                    }
                    else {
                        rep.Message = "Repositoru = null";
                    }
                    break;
            }
            CurrentListRepoArtiste = rep.listRepoArtiste;
            return rep;
        }

        #region GetDetailArtiste par rapport au résultat de l'analyse sur un fichier Excel

        public RepDetailArtisteV2 GetDetailArtisteV2(ParamDetailArtisteV2 param) {

            List<string> listMois = new List<string>(){"JANUARY","FEBRUARY","MARCH","APRIL","MAY","JUNE","JULY","AUGUST","SEPTEMBER","OCTOBER","NOVEMBER","DECEMBER"};

            RepDetailArtisteV2 rep = new RepDetailArtisteV2() { NomArtiste = param.NomArtiste, sMessage = "", listProduct = new List<ProductByCritere>() , listPeriode= new List<PeriodeChecked>(), resultHisto = new ResultHisto() };
            rep.sMessage = "Details pour " + param.NomArtiste;

            try {
                if (globalLinePayLoad != null) {
                    var listInfos = globalLinePayLoad.listLine.Where(c => c.Artist == param.NomArtiste).ToList();
                    var listPeriode = listInfos.Select(c => c.sPeriode).Distinct().ToList();

                    rep.resultHisto.listHistoPeriode = new List<HistoPeriode>();
                    foreach (var v in listPeriode) {
                        rep.listPeriode.Add(new PeriodeChecked() { sPeriode = v, isChecked = true });
                        var listeSurPeriode = listInfos.Where(c => c.sPeriode == v).ToList();
                        var TotalPayablePeriode = listeSurPeriode.Sum(c => c.TotalPayable);
                        var NbDownLoadPeriode = listeSurPeriode.Sum(c => c.DownloadQty);
                        string date = v.Replace("FOR PERIOD ", "");
                        int idMois = 1;
                        foreach (var mois in listMois) {
                            if (date.Contains(mois))
                                break;
                            idMois++;
                        }
                        idMois = idMois % 13;
                        int year = 2018;
                        if (date.Contains("2018"))
                            year = 2018;
                        if (date.Contains("2019"))
                            year = 2019;
                        DateTime dt = new DateTime(year, idMois, 1);
                        long dtlong = UtilityGraphHightChart.GetDateFormatLong(dt);

                        rep.resultHisto.listHistoPeriode.Add(new HistoPeriode() { sPeriode = v, sDate =date,dt = dt, dtlong = dtlong, NbDownLoad = NbDownLoadPeriode, TotalPayable = TotalPayablePeriode });
                    }

                    if (param.listPeriode != null && param.listPeriode.Count != 0) {
                        var listPeriodeDemande = param.listPeriode.Where(c => c.isChecked).Select(c => c.sPeriode).ToList();
                        listInfos = listInfos.Where(c => listPeriodeDemande.Contains(c.sPeriode)).ToList();
                        foreach (var v in rep.listPeriode) {  // on recopie les params d'entrée dans ceux de sortie
                            var exist = param.listPeriode.Where(c => c.sPeriode == v.sPeriode).FirstOrDefault();
                            if (exist != null) {
                                v.isChecked = exist.isChecked;
                            }
                        }
                    }

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
                        foreach (var v in listID) {
                            string stemp = v.Trim();
                            if (!string.IsNullOrEmpty(stemp))
                                sID += stemp + " ";
                        }

                        ProductByCritere productByCritere = new ProductByCritere() { NomProduct = prod, ID = sID, TotalEuros = totalProd, TotalStreaming = totalDownLoad, listKeyQuantityCritereCategorie = new List<KeyQuantity>(), listKeyQuantityCritereDNS = new List<KeyQuantity>(), listKeyQuantityCritereFormatType = new List<KeyQuantity>(), listKeyQuantityCritereTerritory = new List<KeyQuantity>() };
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
                        rep.listKeyTotalQuantityCritereDNS.Add(new KeyQuantity() { key = dms, Quantity = 0 });
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
            catch (Exception ex) {
                rep.sMessage += " Exception :  " + ex.Message;
            }
            return rep;
        }

        #endregion

    }

    public class UtilityGraphHightChart {
        public static long GetDateFormatLong(DateTime dt) {
            var timeSpan = dt - new DateTime(1970, 1, 1, 0, 0, 0); // pas de conversion UTC pour ne pas se decaller en heures 
            return (long)timeSpan.TotalMilliseconds;
        }
    }


    public class RepAnalyse2 {
        public long ExecutionTimeMs;
        public string Message;
        public ResultatGlobal resultatGlobal;
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
        public string UrlImage;
    }

    public class ResultsArtiste {
        public string Nom;
        public string Url;
    }

    public class ResultatGlobal {
        public List<string> listPeriodes;
        public float TotalPayable;
        public int TotalDownload;
        public int NbArtiste;
    }


    public class ParamDetailArtisteV2 {
        public string NomArtiste;
        public List<PeriodeChecked> listPeriode;
    }

    public class RepDetailArtisteV2 {
        public long ExecutionTimeMs;
        public string sMessage;
        public string NomArtiste;
        public int DownLoadQuantity;
        public float GrossRevenue;
        public float NetRevenue;
        public float TotalFee;
        public float TotalPayable;
        public string UrlImage;
        public ResultHisto resultHisto;
        public List<PeriodeChecked> listPeriode;
        public List<ProductByCritere> listProduct;
        public List<KeyQuantity> listKeyTotalQuantityCritereDNS;
        public List<KeyQuantity> listKeyTotalQuantityCritereTerritory;
        public List<KeyQuantity> listKeyTotalQuantityCritereFormatType;
        public List<KeyQuantity> listKeyTotalQuantityCritereCategorie;
    }

    public class PeriodeChecked {
        public string sPeriode;
        public bool isChecked;
    }

    public class ResultHisto {
        public List<HistoPeriode> listHistoPeriode;
    }

    public class HistoPeriode {
        public string sPeriode;
        public string sDate;
        public DateTime dt;
        public long dtlong;
        public float TotalPayable;
        public int NbDownLoad;
    }

    public class EquivalenceDatePaeriode {
        public string sPeriode;
        public DateTime dt;
    }
}
