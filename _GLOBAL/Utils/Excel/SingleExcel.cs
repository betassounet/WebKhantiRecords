using _Global.Log;
using _GLOBAL.globalModels.ConfigsClients;
using _GLOBAL.SessionAndConfig;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.Excel {
    //http://excelpackage.codeplex.com/
    //http://excelpackage.codeplex.com/wikipage?title=Creating%20an%20Excel%20spreadsheet%20from%20scratch&referringTitle=Home
    //http://excelpackage.codeplex.com/wikipage?title=Using%20a%20template%20to%20create%20an%20Excel%20spreadsheet&referringTitle=Home


    public class SingleExcel {

        StatutFichier lastStatutFichier;
        XmlConfigInputExcelFile xmlConfigInputExcelFile;

        #region Constructeur Singleton
        protected static SingleExcel instance;

        protected SingleExcel() {
            lastStatutFichier = new StatutFichier();
        }

        public static SingleExcel Instance() {
            if (instance == null)
                instance = new SingleExcel();
            return instance;
        }
        #endregion

        #region Tests

        public void CreateDummyExcelFile(string sPath) {
            FileInfo newFile = new FileInfo(sPath);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile)) {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Tinned Goods");
                // write some titles into column 1
                worksheet.Cell(1, 1).Value = "Product";
                worksheet.Cell(4, 1).Value = "Peas";
                worksheet.Cell(5, 1).Value = "Total";

                // write some values into column 2
                worksheet.Cell(1, 2).Value = "Tins Sold";

                ExcelCell cell = worksheet.Cell(2, 2);
                cell.Value = "15"; // tins of Beans sold
                string calcStartAddress = cell.CellAddress;  // we want this for the formula

                ExcelCell cell2 = worksheet.Cell(3, 2);
                cell2.Value = "32";  // tins Carrots sold
                string calcEndAddress = cell2.CellAddress;  // we want this for the formula

                worksheet.Cell(5, 2).Formula = string.Format("SUM({0}:{1})", calcStartAddress, calcEndAddress);
                worksheet.Column(1).Width = 15;

                worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
                // add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.oddFooter.RightAlignedText =
                string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

                worksheet.InsertRow(3);
                xlPackage.Workbook.Properties.Title = "Sample 1";
                xlPackage.Workbook.Properties.Author = "John Tunnicliffe";
                xlPackage.Workbook.Properties.SetCustomPropertyValue("EmployeeID", "1147");

                xlPackage.Save();
            }
        }


        #endregion

        #region Récupération des articles gestion à partir du fichier excel completé par client

        public string ReadExcelFile(string FileName) {
            bool ConfigExist = true;

            xmlConfigInputExcelFile = SingleSessionConfig.Instance().GetXmlConfigInputExcelFile();

            if (xmlConfigInputExcelFile == null || xmlConfigInputExcelFile.ColInfos == null) {
                xmlConfigInputExcelFile = new XmlConfigInputExcelFile() { ColInfos = new List<ColInfo>(), NbCol = 0, FamilleName = "", ConfigOKForUpdateArticle = false };
                ConfigExist = false;
            }

            //this.xmlConfigInputExcelFile = xmlConfigInputExcelFile;
            int NumOnglet = 1;
            lastStatutFichier = new StatutFichier() { ListExcellLine = new List<ExcelLine>(), ListeColonnes = new List<ExcelColonne>(), ChampsColonnes = new List<string>() };

            try {
                FileInfo template = new FileInfo(FileName);
                using (ExcelPackage xlPackage = new ExcelPackage(template)) {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[NumOnglet];

                    // Calcul du nombre de ligne consécutives
                    int RowBase = 1;
                    string sValeur = "";
                    do {
                        sValeur = worksheet.Cell(RowBase, 1).Value;
                        RowBase++;
                    }
                    while (!string.IsNullOrEmpty(sValeur));

                    // calcul du nombre de colonnes
                    int ColBase = 1;
                    do {
                        sValeur = worksheet.Cell(1, ColBase).Value;
                        ColBase++;
                    }
                    while (!string.IsNullOrEmpty(sValeur));

                    RowBase--;
                    ColBase--;
                    lastStatutFichier.NbRow = RowBase;
                    lastStatutFichier.NbCol = ColBase;

                    for (int row = 1; row < RowBase; row++) {
                        var line = new ExcelLine() { LineRow = row, listValues = new List<string>(), listValuesExtended = new List<ValueExtended>() };
                        for (int col = 1; col < ColBase; col++) {
                            string varCellule = worksheet.Cell(row, col).Value;
                            if (string.IsNullOrEmpty(varCellule)) {
                                //varCellule = "___";
                            }
                            if (row == 1) {
                                ExcelColonne excelColonne = new ExcelColonne() { Nom = varCellule, NumColonne = col };
                                lastStatutFichier.ListeColonnes.Add(excelColonne);
                                lastStatutFichier.ChampsColonnes.Add(varCellule);  // old..
                            }
                            else {
                                line.listValues.Add(varCellule);
                                ValueExtended valueExtended = new ValueExtended() { PosCol = col, Value = varCellule };
                                line.listValuesExtended.Add(valueExtended);
                            }
                        }
                        lastStatutFichier.ListExcellLine.Add(line);
                    }

                    if (ConfigExist) { // Test si compatibilité du fichier excel avec Config Fichier
                        lastStatutFichier.Message = "Config excel available : ";
                        lastStatutFichier.ConfigOKForUpdateArticle = xmlConfigInputExcelFile.ConfigOKForUpdateArticle;
                        if (xmlConfigInputExcelFile.NbCol != lastStatutFichier.NbCol) {
                            lastStatutFichier.Erreur = true;
                            lastStatutFichier.Message += "Col Number not match with config";
                        }
                        else {
                            int NbreDifference = 0;
                            for (int i = 0; i < lastStatutFichier.NbCol; i++) {
                                if (xmlConfigInputExcelFile.ColInfos[i].ColName != lastStatutFichier.ListeColonnes[i].Nom)
                                    NbreDifference++;
                            }
                            if (NbreDifference > 0) {
                                lastStatutFichier.Erreur = true;
                                lastStatutFichier.Message += "Col Name not match with config";
                            }
                        }

                        if (!lastStatutFichier.Erreur) {
                            // SI tout est OK..
                            lastStatutFichier.Message += "Erreur sur xmlConfigInputExcelFile ( a regènèrer)";
                        }

                    }
                    else {
                        lastStatutFichier.Message = "Config excel NOT available : Creation xmlConfigInputExcelFile ";
                        xmlConfigInputExcelFile.NbCol = lastStatutFichier.NbCol;
                        xmlConfigInputExcelFile.ColInfos = new List<ColInfo>();
                        foreach (var v in lastStatutFichier.ListeColonnes) {
                            xmlConfigInputExcelFile.ColInfos.Add(new ColInfo() { ColPosition = v.NumColonne, ColName = v.Nom, ToArticleCodeCritere = "", ToArticleNomCritere = "", ToArticleTypeCritere = 0, IsCritereFamille = false, IsMandatory = true });
                        }
                    }
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("SingleExcel", "ReadExcelFile ex :" + ex.Message);
            }
            return lastStatutFichier.Message;
        }

        #endregion


        public StatutFichier GetLastStatutFichier() {
            return lastStatutFichier;
        }

        public XmlConfigInputExcelFile GetLastXmlConfigInputExcelFile() {
            return xmlConfigInputExcelFile;
        }

    }

    #region Classes de travail

    public class StatutFichier {
        public long ExecutionTimeMs;
        public bool Erreur;
        public string Message;
        public bool ConfigOKForUpdateArticle;
        public int NbRow;
        public int NbCol;
        public List<string> ChampsColonnes;
        public List<ExcelColonne> ListeColonnes;
        public List<ExcelLine> ListExcellLine;
    }

    public class ExcelColonne {
        public int NumColonne;
        public string Nom;
    }

    public class ExcelLine {
        public int LineRow;
        public List<string> listValues;
        public List<ValueExtended> listValuesExtended;
    }

    public class ValueExtended {
        public int PosCol;
        public string Value;
        public ColInfo XmlColInfo;
        public bool IsBasicCodeInterne;
        public bool IsUpdated;
    }

    #endregion
}
