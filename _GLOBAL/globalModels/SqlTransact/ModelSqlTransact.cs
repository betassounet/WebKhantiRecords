using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.globalModels.SqlTransact {


    public class ReponseInfoDataBase {
        public long ExecutionTimeMs;
        public bool isOK;
        public int Error;
        public string sMessage;
        public InfoDataBase infoDataBase;
    }

    public class ReponseShortInfoDataBase {
        public long ExecutionTimeMs;
        public bool isOK;
        public int Error;
        public string sMessage;
        public List<ShortInfosTable> listShortInfosTable;

        public ReponseShortInfoDataBase() {

        }

        public ReponseShortInfoDataBase(List<InfosTable> listInfosTable) {
            listShortInfosTable = new List<ShortInfosTable>();
            foreach (var v in listInfosTable) {
                listShortInfosTable.Add(new ShortInfosTable(v));
            }
        }
    }


    public class ShortInfosTable {
        public string NomTable;
        public string TableType;
        public bool HasColumnIdentity;
        public int NbreForeignKey;    // Intégrité référentielle : Nombre de Foreign key appliquées sur la table
        public int NbreRefForeignKey;  // Intégrité référentielle  : Nombre de Reférence  Foreign key sur lesquelles pointent d'autres tables
        public int NbreCount;
        public List<ShortInfosColumn> listInfosColumn;
        public ShortInfosTable(InfosTable infosTable) {
            this.NomTable = infosTable.NomTable;
            this.TableType = infosTable.TableType;
            this.HasColumnIdentity = infosTable.HasColumnIdentity;
            this.NbreForeignKey = infosTable.NbreForeignKey;
            this.NbreRefForeignKey = infosTable.NbreRefForeignKey;
            this.NbreCount = infosTable.NbreCount;
            listInfosColumn = new List<ShortInfosColumn>();
            foreach (var v in infosTable.listInfosColumn) {
                listInfosColumn.Add(new ShortInfosColumn(v));
            }
        }
    }

    public class ShortInfosColumn {
        public string NomColonne;
        public string sPositionOrdinale;
        public string sDataType;
        public string isNullable;
        public string caracterMaxLenght;
        public ShortInfosColumn(InfosColumn infosColumn) {
            this.NomColonne = infosColumn.NomColonne;
            this.sPositionOrdinale = infosColumn.sPositionOrdinale;
            this.sDataType = infosColumn.sDataType;
            this.isNullable = infosColumn.isNullable;
            this.caracterMaxLenght = infosColumn.caracterMaxLenght;
        }
    }


    public class InfoDataBase {
        public string NomDataBase;
        public int CptNotDefined;
        public List<InfoSysObject> listInfoSysObject;
        public List<InfosTable> listInfosTable;
        public List<InfosContraintes> listInfosContraintes;
        public List<TableRelationTable> listTableRelationTable;

        public InfoDataBase() {
            listInfoSysObject = new List<InfoSysObject>();
            listInfosTable = new List<InfosTable>();
            listInfosContraintes = new List<InfosContraintes>();
            listTableRelationTable = new List<TableRelationTable>();
        }
    }

    public class InfoSysObject {  // Information sur les objets de la base de donnée

        public InfoSysObject() {
            listInfosSysColums = new List<InfosSysColums>();
        }

        public string ObjectName;
        public int Id_Object;
        public string xTypeObject;
        public int ParentIdObject;
        public string sDateTimeCreationObject;
        public List<InfosSysColums> listInfosSysColums;
    }

    public class InfosSysColums {
        public int OwnerId;
        public string NameColumns;
        public int column_Id;
        public string sysTypeId;
        public string useTypeId;
        public int MaxLenght;
        public string IsNullable;
        public string IsRowGuidCol;
        public string IsIdentity;
        public string IsComputed;
    }


    public class RefreshContraintesTable {
        public TableRelationTable TableRelationTableForeignKey;
        public TableRelationTable TableRelationTableRefForeignKey;
    }

    public enum eTypeContrainte { ForeignKey, RefForeignKey };
    public class TableRelationTable {
        public string NomTable;
        public int NbreCountRec;
        public int NbreCountRecOrigine;
        public bool DifferenceCount;
        public int NbreRelation;
        public eTypeContrainte TypeContrainte;
        public List<RelationToTable> listRelationToTable;
    }

    public class RelationToTable {
        public string NomContrainte;
        public int IdColumnFrom;
        public TableRelationTable TableRelation;
        public int IdColumnTo;
    }


    public class InfosTable {
        public string NomTable;
        public string TableType;
        public bool HasColumnIdentity;
        public InfoSysObject infoSysObject;
        public TableRelationTable TableRelationTableForeignKey;
        public TableRelationTable TableRelationTableRefForeignKey;
        public int NbreForeignKey;    // Intégrité référentielle : Nombre de Foreign key appliquées sur la table
        public int NbreRefForeignKey;  // Intégrité référentielle  : Nombre de Reférence  Foreign key sur lesquelles pointent d'autres tables
        public int NbreCount;
        public int NbreCountOrigine;
        public bool DestinationDifferent;
        public List<InfosColumn> listInfosColumn;
        //public List<TableForeignKey> listTableForeignKey;
        //public List<TableForeignKey> listTableRefForeignKey;
        public string sAction;

        public InfosTable() {
            listInfosColumn = new List<InfosColumn>();
            //listTableForeignKey = new List<TableForeignKey>();
            //listTableRefForeignKey = new List<TableForeignKey>();
        }
    }

    public class InfosColumn {
        public string NomColonne;
        public InfosSysColums infosSysColums;
        public string sPositionOrdinale;
        public string sDataType;
        public string isNullable;
        public string caracterMaxLenght;
        public string ContrainteCatalog;
        public string ContrainteSchema;
        public string ContrainteNom;
        public string infoContrainte;
    }

    //public class TableForeignKey {
    //    public string NomTable;
    //    public int PositionColonneRef;
    //    public int Count;
    //}

    public class InfosContraintes {
        public int constraint_object_id;
        public string name_constraint_object_id;
        public int constraint_column_id;
        public int parent_object_id;
        public string name_parent_object_id;
        public int parent_column_id;
        public int referenced_object_id;
        public string name_referenced_object_id;
        public int referenced_column_id;

    }

}
