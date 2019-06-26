using _Global.Log;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace KhantiRecordsMetier {
    //http://excelpackage.codeplex.com/
    //http://excelpackage.codeplex.com/wikipage?title=Creating%20an%20Excel%20spreadsheet%20from%20scratch&referringTitle=Home
    //http://excelpackage.codeplex.com/wikipage?title=Using%20a%20template%20to%20create%20an%20Excel%20spreadsheet&referringTitle=Home

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    // NEW 03/12/2018
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    // LA dll ExcellPackage a changé elle devient EPPlus.dll
    // Cela permet de corriger les bugs de ExcellPackage 
    // La syntaxe est quasiment la même
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    public class SingleExcel {

        #region Constructeur Singleton
        protected static SingleExcel instance;

        protected SingleExcel() {
        }

        public static SingleExcel Instance() {
            if (instance == null)
                instance = new SingleExcel();
            return instance;
        }
        #endregion

        #region Tests

        public ResultPeriode GetPeriodFile(string sFile) {
            ResultPeriode resultPeriode = new ResultPeriode() { sPeriode = "NC", Message = "" };
            FileInfo newFile = new FileInfo(sFile);
            try {
                using (ExcelPackage xlPackage = new ExcelPackage(newFile)) {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();
                    if (worksheet != null) {
                        var valcell = worksheet.Cells[8, 6].Value;
                        if (valcell != null) {
                            resultPeriode.sPeriode = valcell.ToString();
                        }

                    }
                    else {
                        resultPeriode.Message += "Pb WorkSheet ";
                    }
                }
            }
            catch (Exception ex) {
                resultPeriode.Message += "GetPeriodFile : Exception ex : " + ex.Message;
            }
            return resultPeriode;
        }



        public ResultTest AnalyseFichier(string sFile, string shortFile) {
            ResultTest resultTest = new ResultTest() { Message = "", sFile = shortFile, listLinePayLoad = new List<LinePayLoad>() };
            FileInfo newFile = new FileInfo(sFile);
            try {
                using (ExcelPackage xlPackage = new ExcelPackage(newFile)) {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();
                    if (worksheet != null) {
                        var valcellPeriode = worksheet.Cells[8, 6].Value;
                        if (valcellPeriode != null) {
                            resultTest.sPeriode = valcellPeriode.ToString();
                        }

                        bool findEnd = true;
                        int idxline = 1;
                        int IdxEndFile = 0;
                        while (findEnd) {
                            var valcell = worksheet.Cells[idxline++, 7].Value;
                            if (valcell != null && valcell.ToString().Trim() == "GRAND TOTALS:") {
                                findEnd = false;
                                IdxEndFile = idxline;
                            }
                            if (idxline > 100000) {
                                findEnd = true;
                            }
                        }

                        int CPtNull = 0;
                        for (int i = 12; i < IdxEndFile; i++) {
                            var valcell = worksheet.Cells[i, 1].Value;
                            if (valcell != null) {
                                resultTest.listLinePayLoad.Add(GetLine(worksheet, i, shortFile, resultTest.sPeriode));
                            }
                            else {
                                CPtNull++;
                            }
                        }

                        resultTest.Message = "Excel Analyse " + IdxEndFile.ToString() + " lignes ";
                        if (CPtNull != 0) {
                            resultTest.Message += " CptNull : " + CPtNull.ToString();
                        }
                    }
                }
            }
            catch (Exception ex) {
                resultTest.Message += "TestFile : Exception ex : " + ex.Message;
            }
            return resultTest;
        }

        private LinePayLoad GetLine(ExcelWorksheet worksheet, int NumLine, string sFile, string sPeriode) {
            LinePayLoad line = new LinePayLoad() { Line = NumLine , sFile = sFile, sPeriode = sPeriode };
            line.Artist = worksheet.Cells[NumLine, 1].Value != null ? worksheet.Cells[NumLine, 1].Value.ToString() : "null";
            line.Product = worksheet.Cells[NumLine, 2].Value != null ? worksheet.Cells[NumLine, 2].Value.ToString() : "null";
            line.ISRC = worksheet.Cells[NumLine, 3].Value != null ? worksheet.Cells[NumLine, 3].Value.ToString() : "null";
            line.Barcode = worksheet.Cells[NumLine, 4].Value != null ? worksheet.Cells[NumLine, 4].Value.ToString() : "null";
            line.Territory = worksheet.Cells[NumLine, 5].Value != null ? worksheet.Cells[NumLine, 5].Value.ToString() : "null";
            line.FormatType = worksheet.Cells[NumLine, 6].Value != null ? worksheet.Cells[NumLine, 6].Value.ToString() : "null";
            line.PriceCategory = worksheet.Cells[NumLine, 7].Value != null ? worksheet.Cells[NumLine, 7].Value.ToString() : "null";
            line.DMS = worksheet.Cells[NumLine, 8].Value != null ? worksheet.Cells[NumLine, 8].Value.ToString() : "null";
            string temp1 = worksheet.Cells[NumLine, 9].Value != null ? worksheet.Cells[NumLine, 9].Value.ToString() : "null";
            int.TryParse(temp1, out line.DownloadQty);
            string temp2 = worksheet.Cells[NumLine, 10].Value != null ? worksheet.Cells[NumLine, 10].Value.ToString() : "null";
            float.TryParse(temp2, out line.GrossRevenue);
            string temp3 = worksheet.Cells[NumLine, 11].Value != null ? worksheet.Cells[NumLine, 11].Value.ToString() : "null";
            float.TryParse(temp3, out line.MechanicalDeduction);
            string temp4 = worksheet.Cells[NumLine, 12].Value != null ? worksheet.Cells[NumLine, 12].Value.ToString() : "null";
            float.TryParse(temp4, out line.NetRevenue);
            string temp5 = worksheet.Cells[NumLine, 13].Value != null ? worksheet.Cells[NumLine, 13].Value.ToString() : "null";
            float.TryParse(temp5, out line.Fee);
            string temp6 = worksheet.Cells[NumLine, 14].Value != null ? worksheet.Cells[NumLine, 14].Value.ToString() : "null";
            float.TryParse(temp6, out line.TotalFee);
            string temp7 = worksheet.Cells[NumLine, 15].Value != null ? worksheet.Cells[NumLine, 15].Value.ToString() : "null";
            float.TryParse(temp7, out line.TotalPayable);

            return line;
        }


        // Fonction artifice pour pouvoir entrer un nombre decimal dans une case.. avec l'ancienne dall ExcellPackage
        // avec nouvelle dll , l'affectation à la cellule change, ce n'est plus un string mais un object
        private void AddCellNumber(ExcelWorksheet Worksheet, int Row, int Col, double Nombre) {
            int ValMulttiplie = (int)(Nombre * 1000);
            var cell = Worksheet.Cells[Row,Col];
            //cell.DataType = "Texte";
            cell.Formula = ValMulttiplie.ToString() + "/1000"; 
        }


        #endregion

        #region Utils
        private string GetNormallyString(string chaine) {
            return string.IsNullOrEmpty(chaine) ? "" : chaine;
        }

        #endregion

    }


    public class ExcelLine {
        public int NumLine;
        public string Message;
        public LinePayLoad line;
    }

    public class LinePayLoad {
        [XmlAttribute]
        public string sFile;
        [XmlAttribute]
        public string sPeriode;
        [XmlAttribute]
        public int Line;
        [XmlAttribute]
        public string Artist;
        [XmlAttribute]
        public string Product;
        [XmlAttribute]
        public string ISRC;
        [XmlAttribute]
        public string Barcode;
        [XmlAttribute]
        public string Territory;
        [XmlAttribute]
        public string FormatType;
        [XmlAttribute]
        public string PriceCategory;
        [XmlAttribute]
        public string DMS;
        [XmlAttribute]
        public int DownloadQty;
        [XmlAttribute]
        public float GrossRevenue;
        [XmlAttribute]
        public float MechanicalDeduction;
        [XmlAttribute]
        public float NetRevenue;
        [XmlAttribute]
        public float Fee;
        [XmlAttribute]
        public float TotalFee;
        [XmlAttribute]
        public float TotalPayable;
        [XmlAttribute]
        public float ArtistTotal;
    }


    public class ResultTest {
        public string sFile;
        public string sPeriode;
        public string Message;
        public List<LinePayLoad> listLinePayLoad;
    }

    public class ResultPeriode {
        public string sPeriode;
        public string Message;
    }
}