using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KhantiRecordsMetier {


    public partial class SingleKhanti {

        public DispoDataFromGlobal GetDispoDataFromGlobal(ParamDispoDataFromGlobal param) {
            DispoDataFromGlobal rep = new DispoDataFromGlobal() { listDispoArtist = new List<DispoArtist>() };
            if(globalLinePayLoad != null && globalLinePayLoad.listLine != null) {
                foreach( var v in globalLinePayLoad.listLine) {
                    var artistExist = rep.listDispoArtist.Where(c => c.Artist == v.Artist).FirstOrDefault();
                    if(artistExist == null) {
                        artistExist = new DispoArtist() { Artist = v.Artist, listDispoProduct = new List<DispoProduct>(), dispoOther = new DispoOther() };
                        artistExist.dispoOther.DMSs = new List<string>();
                        artistExist.dispoOther.FormatTypes = new List<string>();
                        artistExist.dispoOther.PriceCategorys = new List<string>();
                        artistExist.dispoOther.Territorys = new List<string>();
                        rep.listDispoArtist.Add(artistExist);
                    }
                    var barcode = v.Barcode.Trim();
                    var isrc = v.ISRC.Trim();
                    if (!string.IsNullOrEmpty(barcode)) {
                        if(barcode == "null") { // si null, barcode = chaine vide..
                            barcode = "";
                        }
                        else {  // s'il y a un barcode - non null - , on ne tient pas compte de l'ISRC  décrit, soit un signle , soit un album
                            isrc = "";
                        }
                    }


                    var productExist = artistExist.listDispoProduct.Where(c => c.Product == v.Product && c.Barcode == barcode && c.ISRC == isrc).FirstOrDefault();
                    if(productExist == null) {
                        productExist = new DispoProduct() { Product = v.Product, Barcode = barcode, ISRC = isrc };
                        artistExist.listDispoProduct.Add(productExist);
                    }

                    if (!artistExist.dispoOther.Territorys.Contains(v.Territory))
                        artistExist.dispoOther.Territorys.Add(v.Territory);
                    if (!artistExist.dispoOther.FormatTypes.Contains(v.FormatType))
                        artistExist.dispoOther.FormatTypes.Add(v.FormatType);
                    if (!artistExist.dispoOther.PriceCategorys.Contains(v.PriceCategory))
                        artistExist.dispoOther.PriceCategorys.Add(v.PriceCategory);
                    if (!artistExist.dispoOther.DMSs.Contains(v.DMS))
                        artistExist.dispoOther.DMSs.Add(v.DMS);
                }

            }

            rep.listDispoArtist = rep.listDispoArtist.OrderBy(c => c.Artist).ToList();
            foreach( var art in rep.listDispoArtist) {
                art.listDispoProduct = art.listDispoProduct.OrderBy(c => c.Product).ToList();
            }

            return rep;
        }

        int Idinc = 0;
        public DispoEntityArtists GetEntityArtists(ParamEntityArtists param) {

            var artistsAdminFile = LoadArtistsAdminFile();

            Idinc = 0;
            DispoEntityArtists rep = new DispoEntityArtists() { listEntityArtist = new List<EntityArtist>() };
            if (globalLinePayLoad != null && globalLinePayLoad.listLine != null) {
                foreach (var v in globalLinePayLoad.listLine) {
                    string artist = v.Artist.Trim();
                    var artistExist = rep.listEntityArtist.Where(c => c.Name == artist).FirstOrDefault();
                    if (artistExist == null) {
                        artistExist = new EntityArtist() { Id = Idinc++, Name = v.Artist, Source = eSource.EXCEL, Payable = ePayable.GROUP, Pourcent = 0, ListRelationEntityArtist = new List<RelationEntityArtist>(), ParentNames = new List<string>(), IdParamAdmin = -1 };
                        rep.listEntityArtist.Add(artistExist);
                    }
                }


                List<EntityArtist> listNewArtist = new List<EntityArtist>();

                foreach (var item in rep.listEntityArtist) {
                    string sName = item.Name;
                    sName = sName.Replace("Feat.", ";");
                    sName = sName.Replace("feat.", ";");
                    sName = sName.Replace("Feat", ";");
                    sName = sName.Replace("feat", ";");
                    sName = sName.Replace("&", ";");

                    var listArtiste = sName.Split(';').ToList();
                    int cptOrdre = 0;
                    foreach (var v in listArtiste) {
                        string newArt = v.Trim();
                        var newExist = listNewArtist.Where(c => c.Name == newArt).FirstOrDefault();
                        if (newExist == null) {
                            newExist = new EntityArtist() { Name = newArt, ParentNames = new List<string>(), Id = Idinc++, Source = eSource.ANALYSE_FEAT, ListRelationEntityArtist = new List<RelationEntityArtist>(), IdParamAdmin = -1 };
                            listNewArtist.Add(newExist);
                        }
                        item.ListRelationEntityArtist.Add(new RelationEntityArtist() { IdEntity = newExist.Id, Name = newExist.Name, OrdreRelation = cptOrdre, PourcentageFeat = cptOrdre == 0 ? 100 : 0 });
                        newExist.ParentNames.Add(item.Name);
                        cptOrdre++;
                    }
                }


                artistsAdminFile = null;

                int IdMax = 0;
                if (artistsAdminFile != null && artistsAdminFile.listArtistAdminItems != null && artistsAdminFile.listArtistAdminItems.Count > 0) {
                    var last = artistsAdminFile.listArtistAdminItems.Last();
                    if (last != null)
                        IdMax = last.IdArtist + 1;
                }
                else {
                    artistsAdminFile = new ArtistsAdminFile() { listArtistAdminItems = new List<ArtistAdminItem>() };
                }

                foreach (var newArt2 in listNewArtist) {
                    newArt2.sPbAscci = "";
                    string rawName = newArt2.Name.Trim().ToLower();
                    for(int i=0; i< rawName.Length; i++) {
                        if (rawName[i] < 32 || rawName[i] > 122) {
                            newArt2.sPbAscci += "Ascci["+ ((int)rawName[i]).ToString()+ "]:" + rawName[i];
                            rawName = rawName.Replace(rawName[i], ' ');
                        }
                    }
                    newArt2.RawName = rawName;
                }


                    foreach (var newArt2 in listNewArtist) {
                    var listAlias = listNewArtist.Where(c => c.RawName == newArt2.RawName).Select(c => c.Name).ToList();
                    int CptMatch = 0;

                    if(newArt2.Name == "Devi Reed") {

                    }

                    ArtistAdminItem selectedArtistAdminItems = null;
                    foreach (var vAdmin in artistsAdminFile.listArtistAdminItems) {
                        if (vAdmin.listAlias != null) {
                            foreach (var v in vAdmin.listAlias) {
                                foreach (var v1 in listAlias) {
                                    if (v == v1) {
                                        CptMatch++;
                                        selectedArtistAdminItems = vAdmin;
                                    }
                                }
                            }
                        }
                    }
                    if (selectedArtistAdminItems == null) {
                        selectedArtistAdminItems = new ArtistAdminItem() { ArtistName = listAlias[0], IdArtist = IdMax++, listAlias = listAlias, LoginArtist = "", PasswordArtist = "", PourcentProdVsArtisteBase = 0 };
                        artistsAdminFile.listArtistAdminItems.Add(selectedArtistAdminItems);
                    }
                    newArt2.IdParamAdmin = selectedArtistAdminItems.IdArtist;
                }
                rep.listEntityArtist.AddRange(listNewArtist);
            }
            SaveArtistsAdminFile();
            rep.artistsAdminFile = artistsAdminFile;

            return rep;
        }

        public NameResolutionPath GetNameResolutionPath(ParamNameResolutionPath Param) {
            NameResolutionPath rep = new NameResolutionPath() { listItemNameResolution = new List<ItemNameResolution>(), listItemArtiste = new List<ItemArtiste>()};
            Idinc = 0;
            int IdArtistInc = 0;

            if (globalLinePayLoad != null && globalLinePayLoad.listLine != null) {
                foreach (var v in globalLinePayLoad.listLine) {
                    string ExcelNameArtist = v.Artist.Trim();
                    var nameExist = rep.listItemNameResolution.Where(c => c.ExcelName == ExcelNameArtist).FirstOrDefault();
                    string sPbAscci = "";
                    string CodeName = GetCodeName(ExcelNameArtist, ref sPbAscci);
                    if (nameExist == null) {
                        nameExist = new ItemNameResolution() { Id = Idinc++, ExcelName = ExcelNameArtist, CodeName = CodeName, sPbAscci = sPbAscci, listItemAlias =new List<ItemAlias>(), listItemArtiste = new List<ItemArtiste>(), listProduct = new List<ItemProduct>() };
                        rep.listItemNameResolution.Add(nameExist);
                    }
                    string _Product = v.Product.Trim();
                    string _ISRC = v.ISRC.Trim();
                    _ISRC = _ISRC == "null" ? "" : _ISRC;
                    string _Barcode = v.Barcode.Trim();
                    _Barcode = _Barcode == "null" ? "" : _Barcode;
                    string _FormatType = v.FormatType.Trim();
                    var productExist = nameExist.listProduct.Where(c => c.Product == _Product && c.ISRC == _ISRC && c.Barcode == _Barcode && c.FormatType == _FormatType).FirstOrDefault();
                    if (productExist == null) {
                        productExist = new ItemProduct() { Product = _Product, ISRC = _ISRC, Barcode = _Barcode, FormatType = _FormatType, DMS = "", PriceCategory = "" };
                        nameExist.listProduct.Add(productExist);
                    }
                    if (!productExist.DMS.Contains(v.DMS.Trim()))
                        productExist.DMS += v.DMS.Trim() + "/";
                    if (!productExist.PriceCategory.Contains(v.PriceCategory.Trim()))
                        productExist.PriceCategory += v.PriceCategory.Trim() + "/";
                    productExist.CptDownload += v.DownloadQty;
                }

                foreach( var v in rep.listItemNameResolution){
                    var listAlias = rep.listItemNameResolution.Where(c => c.CodeName == v.CodeName && c.Id != v.Id).ToList();
                    foreach( var a in listAlias) {
                        v.listItemAlias.Add(new ItemAlias() { Id = a.Id, CodeName = a.CodeName, sPbAscci =a.sPbAscci , ExcelName = a.ExcelName });
                    }
                    v.CptAlias = v.listItemAlias.Count();

                    var listFeat = GetListFeat(v.ExcelName);
                    int CptPosFeat = 0;
                    foreach( var f in listFeat) {
                        string sPbAscci = "";
                        string codeNameArtist= GetCodeName(f, ref sPbAscci);
                        var artExist = rep.listItemArtiste.Where(c => c.ArtistelName == f).FirstOrDefault();
                        if(artExist == null) {
                            var codeArtExist = rep.listItemArtiste.Where(c => c.ArtisteCodeName == codeNameArtist).FirstOrDefault();
                            if(codeArtExist == null) {
                                artExist = new ItemArtiste() { ArtistelName = f, ArtisteCodeName = codeNameArtist, Id = IdArtistInc++ , sPbAscci = sPbAscci };
                                rep.listItemArtiste.Add(artExist);
                            }
                            else {
                                codeArtExist.sPbAscci += sPbAscci;
                                artExist = codeArtExist;
                            }
                        }
                        v.listItemArtiste.Add(artExist);
                        artExist.CptPresenceInRawName++;
                        if (CptPosFeat == 0){
                            artExist.CptPosFirstArtiste++;
                            v.FirstArtisteName = artExist.ArtistelName;
                        }
                        else {
                            artExist.CptPosFeaturing++;
                        }
                        CptPosFeat++;
                    }
                    v.listProduct = v.listProduct.OrderBy(c => c.Product).ToList(); 
                }
            }

            rep.listItemNameResolution = rep.listItemNameResolution.OrderBy(c => c.CodeName).ToList();
            return rep;
        }

        private string GetCodeName(string Name, ref string sPbAscci) {
            string rawName = Name.Trim().ToLower();
            for (int i = 0; i < rawName.Length; i++) {
                if (rawName[i] < 32 || rawName[i] > 122) {
                    sPbAscci += "Ascci[" + ((int)rawName[i]).ToString() + "]:" + rawName[i];
                    rawName = rawName.Replace(rawName[i], '_');
                }
            }
            rawName = rawName.Replace(' ', '_');
            return rawName;
        }

        private List<string> GetListFeat(string ExcelName) {
            List<string> listRep = new List<string>();
            string sName = ExcelName;
            sName = sName.Replace("Feat.", ";");
            sName = sName.Replace("feat.", ";");
            sName = sName.Replace("Feat", ";");
            sName = sName.Replace("feat", ";");
            sName = sName.Replace("&", ";");

            var list = sName.Split(';').ToList();
            foreach( var v in list) {
                listRep.Add(v.Trim());
            }

            return listRep;
        }


        public ArtisteAndResolutionPath GetArtisteAndResolutionPath(ParamArtisteAndResolutionPath Param) {
            ArtisteAndResolutionPath rep = new ArtisteAndResolutionPath() { listArtisteAndRawData = new List<ArtisteAndRawData>() };
            var phase1 = GetNameResolutionPath(new ParamNameResolutionPath());
            phase1.listItemArtiste = phase1.listItemArtiste.OrderBy(c => c.ArtistelName).ToList();
            int IdAdminArtiste = 0;
            foreach (var v in phase1.listItemArtiste) {
                ArtisteAndRawData artisteAndRawData = new ArtisteAndRawData() { NameArtiste = v.ArtistelName };
                artisteAndRawData.artisteDataAdmin = new ArtisteDataAdmin() { Id = IdAdminArtiste++, AnalyseArtisteName = v.ArtistelName, Name = "", IsFirstArtist= false, sRefArtiste = "", listPayableDataAdmin = new List<PayableDataAdmin>() };
                artisteAndRawData.listItemNameResolution = new List<ItemNameResolution>();
                rep.listArtisteAndRawData.Add(artisteAndRawData);
            }

            foreach (var item in phase1.listItemNameResolution) {
                var artisteAdminExist = rep.listArtisteAndRawData.Where(c => c.artisteDataAdmin.AnalyseArtisteName == item.FirstArtisteName).FirstOrDefault();
                if (artisteAdminExist != null) {
                    artisteAdminExist.artisteDataAdmin.IsFirstArtist = true;
                    foreach (var product in item.listProduct) {

                        bool TypeDownLoad=false;
                        string refKey = product.ISRC;
                        if (string.IsNullOrEmpty(refKey)) {
                            TypeDownLoad = true;   // c'est un barcode ?
                            refKey = product.Barcode;
                        }
                        if (!string.IsNullOrEmpty(refKey)) {
                            var CurrentPayable = artisteAdminExist.artisteDataAdmin.listPayableDataAdmin.LastOrDefault();
                            if (CurrentPayable == null) {
                                CurrentPayable = new PayableDataAdmin() { Name = "", sRefProduct = "", listProductDataAdmin = new List<ProductDataAdmin>(), MaxDateTimeValidite = DateTime.Now };
                                artisteAdminExist.artisteDataAdmin.listPayableDataAdmin.Add(CurrentPayable);
                            }
                            var productExist = CurrentPayable.listProductDataAdmin.Where(c => c.BarcodeOrISRC == refKey).FirstOrDefault();
                            if (productExist == null) {
                                productExist = new ProductDataAdmin() { FormatType = product.FormatType, Name = product.Product, BarcodeOrISRC = refKey, TypeDownload = TypeDownLoad, TypeStreaming = !TypeDownLoad, Ref = "", Type = "" };
                                CurrentPayable.listProductDataAdmin.Add(productExist);
                            }
                        }
                    }
                    artisteAdminExist.listItemNameResolution.Add(item);
                }
                else {
                    //PB
                }
                //item.FirstArtisteName
            }

            foreach (var v in rep.listArtisteAndRawData) {
                if(v.NameArtiste == "Ackboo"){
                    var first = v.artisteDataAdmin.listPayableDataAdmin.FirstOrDefault();
                    if (first != null) {
                        PayableDataAdmin clone = new PayableDataAdmin() { MaxDateTimeValidite = first.MaxDateTimeValidite.AddYears(1), listProductDataAdmin = first.listProductDataAdmin };
                        v.artisteDataAdmin.listPayableDataAdmin.Add(clone);
                    }
                        
                }
                if (v.NameArtiste == "The Banyans") {
                    var first = v.artisteDataAdmin.listPayableDataAdmin.FirstOrDefault();
                    if (first != null) {
                        PayableDataAdmin clone = new PayableDataAdmin() { MaxDateTimeValidite = first.MaxDateTimeValidite.AddYears(1), listProductDataAdmin = first.listProductDataAdmin };
                        v.artisteDataAdmin.listPayableDataAdmin.Add(clone);
                        v.artisteDataAdmin.listPayableDataAdmin.Add(clone);
                    }
                        
                }
                
            }
            
            return rep;
        }


    }

    public class ParamDispoDataFromGlobal {
        public string sAction;
    }


    public class DispoDataFromGlobal {
        public long ExecutionTimeMs;
        public string sMessage;
        public List<DispoArtist> listDispoArtist;
    }

    public class DispoArtist {
        public string Artist;
        public List<DispoProduct> listDispoProduct;
        public DispoOther dispoOther;
    }

    public class DispoProduct {
        public string Product;
        public string ISRC;
        public string Barcode;
    }

    public class DispoOther {
        public List<string> Territorys;
        public List<string> FormatTypes;
        public List<string> PriceCategorys;
        public List<string> DMSs;
    }


    public class ParamEntityArtists {
        public string sAction;
    }

    public class DispoEntityArtists {
        public long ExecutionTimeMs;
        public string sMessage;
        public List<EntityArtist> listEntityArtist;
        public ArtistsAdminFile artistsAdminFile;
    }

    public enum eSource { NC, EXCEL, ANALYSE_FEAT, ANALYSE_ALIAS,  MANU }
    public enum ePayable { NC, GROUP, FINAL }

    public class EntityArtist {
        public int Id;
        public string Name;
        public string RawName;
        public string sPbAscci;
        public int IdParamAdmin;
        public List<string> ParentNames;
        public eSource Source;
        public ePayable Payable;
        public int Pourcent;
        public List<RelationEntityArtist> ListRelationEntityArtist;
    }


    public class RelationEntityArtist {
        public int IdEntity;
        public int OrdreRelation;
        public string Name;
        public int PourcentageFeat;
    }

    public class RelationAlias {
        public int IdEntity;
        public int Ordre;
        public string Name;
    }


    public class ArtistsAdminFile {
        public List<ArtistAdminItem> listArtistAdminItems;
    }

    public class ArtistAdminItem {
        [XmlAttribute]
        public int IdArtist; 
        [XmlAttribute]
        public string ArtistName;
        [XmlAttribute]
        public string LoginArtist;
        [XmlAttribute]
        public string PasswordArtist;
        [XmlAttribute]
        public int PourcentProdVsArtisteBase;
        public List<string> listAlias;
    }


    //----------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------


    public class ParamArtisteAndResolutionPath {
        public string sAction;
    }

    public class ArtisteAndResolutionPath {
        public long ExecutionTimeMs;
        public List<ArtisteAndRawData> listArtisteAndRawData;
    }

    public class ArtisteAndRawData {
        public string NameArtiste;
        public List<ItemNameResolution> listItemNameResolution;
        public ArtisteDataAdmin artisteDataAdmin;
    }

    public class ParamNameResolutionPath {
        public string sAction;
    }

    public class NameResolutionPath {
        public long ExecutionTimeMs;
        public List<ItemNameResolution> listItemNameResolution;
        public List<ItemArtiste> listItemArtiste;
    }

    public class ItemNameResolution {
        public int Id;
        public string ExcelName;
        public string CodeName;
        public string sPbAscci;
        public int CptAlias;
        public string FirstArtisteName;
        public List<ItemProduct> listProduct;
        public List<ItemAlias> listItemAlias;
        public List<ItemArtiste> listItemArtiste;
    }

    public class ItemAlias {
        public int Id;
        public string ExcelName;
        public string CodeName;
        public string sPbAscci;
    }

    public class ItemArtiste {
        public int Id;
        public string ArtistelName;
        public string ArtisteCodeName;
        public string sPbAscci;
        public int CptPresenceInRawName;
        public int CptPosFirstArtiste;
        public int CptPosFeaturing;
    }

    public class ItemProduct {
        public string Product;
        public string ISRC;
        public string Barcode;
        public string FormatType;
        public string PriceCategory;
        public string DMS;
        public int CptDownload;
    }


    // Modele Repository Données par Artistes..
    // A la croisé des données reçues par le fichier excell harmonia mundi.. ( toutes les données de telechargement )
    // et du fichier excel résultat Analyse Digitale..
    // Hierarchie de donnée admin a envisager :

    public class ArtisteDataAdmin {
        public int Id;
        public string sRefArtiste;   // Nouvelle notion pour referencer un artiste  // saisie MANU..
        public string Name;       // Nom donné a l'artiste               // saisie MANU..
        // reprise des données Item Artiste issu de l'analyse :
        public string AnalyseArtisteName;
        public bool IsFirstArtist;
        public List<PayableDataAdmin> listPayableDataAdmin;
    }

    public class PayableDataAdmin {
        public int Id;
        public string sRefProduct;    // serait l'actuelle reference KHxxx   // saisie MANU
        public string Name;   // NomAlbum..                 // Saisie MANU
        public List<ProductDataAdmin> listProductDataAdmin;
        public DateTime MaxDateTimeValidite;
    }

    public class ProductDataAdmin {
        public string FormatType;   // en relation avec le format type Digital Track, Digital Album...du fichier excel ...
        public bool TypeStreaming;
        public bool TypeDownload;
        public string Type;   // digital album / Digitalsingle ?? saisie MANU ??
        public string Name;  // Saisie MANU .. correspond nom de product//  DigitalTrack dans streaming , Name dans Download..
        public string Ref; // Ref  Saisie Manu ??
        public string BarcodeOrISRC; // Ref unique ( c'est la clef finale );  // Ref unique ( c'est la clef finale )
    }


    //[XmlAttribute]
    //public string Artist;
    //[XmlAttribute]
    //public string Product;
    //[XmlAttribute]
    //public string ISRC;
    //[XmlAttribute]
    //public string Barcode;
    //[XmlAttribute]
    //public string Territory;
    //[XmlAttribute]
    //public string FormatType;
    //[XmlAttribute]
    //public string PriceCategory;
    //[XmlAttribute]
    //public string DMS;

}
