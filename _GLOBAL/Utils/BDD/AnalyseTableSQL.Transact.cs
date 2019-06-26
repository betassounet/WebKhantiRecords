using _Global.Log;
using _GLOBAL.globalModels.SqlTransact;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.BDD {

    // Gere une connexion ODBC
    // Analyse a structure des tables
    
    public class AnalyseTableSQL {

        #region Membres
        OdbcConnection ConnectOdbc;
        bool IsKeepOpen = false;
        string connectionString;
        InfoDataBase infoDataBase;
        #endregion

        public AnalyseTableSQL(string connectionString) {
            this.connectionString = connectionString;
            ConnectOdbc = new OdbcConnection(connectionString);
        }

        #region Ouverture Connection DB

        public bool OpenConnectionDB() {
            if (IsKeepOpen)   // si KeepOpen on n'ouvre pas
                return true;

            if (ConnectOdbc != null) {
                try {
                    if (ConnectOdbc.State != ConnectionState.Open)
                        ConnectOdbc.Open();
                    return true;
                }
                catch (Exception ex) {
                    GlobalLog.Instance().AjouteLog("SingleAnalyseTableSQL", "@OpenConnectionDB  Exception :" + ex.Message);
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region KeepOpen BD pour transactions ou boucles
        public void KeepOpenConnectionDB() {
            bool Statut = OpenConnectionDB();
            IsKeepOpen = Statut;
        }

        public void CloseKeepOpenConnectionDB() {
            CloseConnectionDB();
            IsKeepOpen = false;
        }
        #endregion

        #region Fermeture Connection DB
        public void CloseConnectionDB() {
            if (IsKeepOpen)  // on ferme pas si on est KeepOpen
                return;
            if (ConnectOdbc != null)
                if (ConnectOdbc.State == ConnectionState.Open)
                    ConnectOdbc.Close();
        }
        #endregion

        #region Récupération d'une Commande SQL sans Parametres
        public OdbcCommand GetNewCmdSQL(string query) {
            OdbcCommand Comm = new OdbcCommand();
            Comm.Connection = ConnectOdbc;
            Comm.CommandType = CommandType.Text;
            Comm.CommandText = query;
            Comm.CommandTimeout = 5000;
            return Comm;
        }
        #endregion

        #region Execution query besoin ouverture prealable

        public int QUERY_Simple(string sQuery) {
            int Reponse = -1;
            try {
                var Cmd = GetNewCmdSQL(sQuery); 
                var dr = Cmd.ExecuteReader();
                while (dr.Read()) {
                    var r = dr[0].ToString();  // seulement le premier élément interesse
                    Reponse = int.Parse(r);
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xArticleAS400", "@QUERY_Simple : Exception : " + ex.Message);
            }
            return Reponse;
        }

        public int QUERY_Lignes(string sQuery) {
            int Reponse = -1;
            try {
                var Cmd = GetNewCmdSQL(sQuery);  
                var dr = Cmd.ExecuteReader();
                while (dr.Read()) {
                    for (int i = 0; i < dr.FieldCount; i++) {
                        dr[i].ToString();
                    }
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xArticleAS400", "@QUERY_Lignes : " + ex.Message);
            }
            return Reponse;
        }

        #endregion

        #region methodes query autonome, ouverture intégré

        /// <summary>
        /// Requête select renvoyant un objet dataset.
        /// </summary>
        /// <param name="Requete">Requête</param>
        /// <returns>Object dataset ou null si erreur.</returns>
        public DataSet RequeteSelect(string Requete, ref string sError) {
            sError = "";
            DataSet ds;
            OdbcConnection cnx = new OdbcConnection(connectionString);

            try {
                cnx.Open();

                OdbcCommand cmd = new OdbcCommand(Requete, cnx);
                OdbcDataAdapter da = new OdbcDataAdapter(Requete, cnx);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ex) {
                Debug.WriteLine(", la méthode RequeteSelect a rencontrée une erreur. Code erreur renvoyé: " + ex.Message);
                sError = "Exception sur : " + Requete + "  : " + ex.Message;
                return null;
            }
            finally {
                cnx.Close();
            }
            return ds;
        }

        /// <summary>
        /// Requête select renvoyant un objet dataset.
        /// </summary>
        /// <param name="Requete">Requête</param>
        /// <param name="Table">Nom de la table de la requête.</param>
        /// <returns>Object dataset ou null si erreur.</returns>
        public DataSet RequeteSelect(string Requete, string Table) {
            DataSet ds;
            OdbcConnection cnx = new OdbcConnection(connectionString);

            try {
                cnx.Open();

                OdbcCommand cmd = new OdbcCommand(Requete, cnx);
                OdbcDataAdapter da = new OdbcDataAdapter(Requete, cnx);
                ds = new DataSet();
                da.Fill(ds, Table);
            }
            catch (Exception ex) {
                Debug.WriteLine(", la méthode RequeteSelect a rencontrée une erreur. Code erreur renvoyé: " + ex.Message);
                return null;
            }
            finally {
                cnx.Close();
            }
            return ds;
        }


        /// <summary>
        /// Exécute des requêtes de type insert into, update, delete.
        /// </summary>
        /// <param name="requete">Requête à formuler.</param>
        /// <returns>True si succés, sinon false.</returns>
        public bool RequeteNonQuery(string requete, ref string result) {
            OdbcConnection cnx = new OdbcConnection(connectionString);
            result = "";
            bool isOK = false;
            try {
                cnx.Open();
                OdbcCommand cmd = new OdbcCommand(requete, cnx);
                int qresult = cmd.ExecuteNonQuery();
                if (qresult > 0) {
                    result = "Requete exécuté sur " + qresult.ToString() + " lignes";
                    isOK = true;
                }
                else {
                    result = "WARNING : Requete : " + requete + "\nNON exécuté resultat : " + qresult.ToString();
                    isOK = false;
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(", la méthode RequeteNonQuery a rencontrée une erreur. Code erreur renvoyé: " + ex.Message);
                result = "Exception sur query : " + requete + "\n\nex: " + ex.Message + "\n\n" + ex.InnerException;
            }
            finally {
                result += "\nFinally closed";
                cnx.Close();
            }

            return isOK;
        }

        /// <summary>
        /// Execute une procedure stockée
        /// </summary>
        /// <param name="procedureStocke">Nom de la procédure stockée</param>
        /// <param name="parametres">Paramétres à passer, null si aucun paramétres</param>
        /// <returns>Objet dataset</returns>
        public DataSet ProcedureStocke(string procedureStocke, OdbcParameter[] parametres) {
            DataSet ds;
            OdbcConnection cnx = new OdbcConnection(connectionString);
            try {
                cnx.Open();
                OdbcCommand cmd = new OdbcCommand(procedureStocke, cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parametres != null)
                    foreach (OdbcParameter param in parametres)
                        //cmd.Parameters.Add(param.ParameterName, param.Value);
                        cmd.Parameters.Add(param);

                OdbcDataAdapter da = new OdbcDataAdapter(procedureStocke, cnx);
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ex) {
                Debug.WriteLine(", la méthode ProcedureStocke a rencontrée une erreur. Code erreur renvoyé: " + ex.Message);
                return null;
            }
            finally {
                cnx.Close();
            }
            return ds;
        }

        #endregion

        #region commandes
        private string DeleteTable(string NomTable) {
            string sRep = "";
            RequeteNonQuery("DELETE From " + NomTable, ref sRep);  // on ne travaille que sur la table destination
            return sRep;
        }
        #endregion

        #region commandes structure tables

        public int QUERY_Count(string sTable) {
            string query = "SELECT Count(*) FROM " + sTable;
            int count = 0;
            try {
                OdbcCommand Cmd = null;
                Cmd = GetNewCmdSQL(query);

                var dr = Cmd.ExecuteReader();
                while (dr.Read()) {
                    count = (int)dr[0];
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xArticleAS400", "@xArticleAS400#07 QUERY_CountListeArticlesFamille : Exception : " + ex.Message);
            }
            return count;
        }




        public ReponseShortInfoDataBase GetShortStructure(bool modeForce) {
            var info = GetStructure(modeForce);
            var reponseShortInfoDataBase = new ReponseShortInfoDataBase(info.listInfosTable);
            return reponseShortInfoDataBase;
        }


        public InfoDataBase GetStructure(bool modeForce) {
            if (infoDataBase == null || modeForce) {
                infoDataBase = new InfoDataBase();
                if (OpenConnectionDB()) {
                    // Récupération des donnes sys..
                    QUERY_GetStructure_sys_objects(infoDataBase);   // récupération de tous les obejcts de la database
                    QUERY_GetStructure_sys_foreign_key_columns(infoDataBase);  // Récupération des contraintes foreign key
                    QUERY_GetStructure_sys_columns(infoDataBase);  // Récup des infos sys pour les colonnes
                    QUERY_GetStructure_sys_indexes(infoDataBase);

                    QUERY_GetStructure_TABLES(infoDataBase);
                    QUERY_GetStructure_COLUMS(infoDataBase);
                    QUERY_GetStructure_CONSTRAINT_COLUMN_USAGE(infoDataBase);
                    QUERY_GetStructure_KEY_COLUMN_USAGE(infoDataBase);
                    QUERY_GetStructure_REFERENTIAL_CONSTRAINTS(infoDataBase);

                    foreach (var table in infoDataBase.listInfosTable) {
                        int count = QUERY_Count(table.NomTable);
                        table.NbreCount = count;
                        table.DestinationDifferent = table.NbreCount != table.NbreCountOrigine;  // pour le moment les différences sont sur le count..

                        // Pour les reférence Foreign Key :
                        TableRelationTable tableRelationTableFK = new TableRelationTable() { NomTable = table.NomTable, TypeContrainte = eTypeContrainte.ForeignKey, NbreCountRec = count, NbreRelation = 0, listRelationToTable = new List<RelationToTable>() };
                        var lstTableForeignKey = infoDataBase.listInfosContraintes.Where(c => c.name_parent_object_id == table.NomTable).ToList();
                        if (lstTableForeignKey.Count != 0) {
                            tableRelationTableFK.NbreRelation = lstTableForeignKey.Count;
                            foreach (var t in lstTableForeignKey) {  // On ne descend ici qu'au premier niveau de relation
                                TableRelationTable tableRelationTable = new TableRelationTable() { NomTable = t.name_referenced_object_id, listRelationToTable = new List<RelationToTable>() };
                                RelationToTable relationToTable = new RelationToTable() { NomContrainte = t.name_constraint_object_id, TableRelation = tableRelationTable, IdColumnFrom = t.parent_column_id, IdColumnTo = t.referenced_column_id };
                                tableRelationTableFK.listRelationToTable.Add(relationToTable);
                            }

                        }
                        table.NbreForeignKey = lstTableForeignKey.Count;
                        table.TableRelationTableForeignKey = tableRelationTableFK;
                        infoDataBase.listTableRelationTable.Add(tableRelationTableFK);

                        // Pour les reférence Ref Foreign Key :
                        TableRelationTable tableRelationTableRefFK = new TableRelationTable() { NomTable = table.NomTable, TypeContrainte = eTypeContrainte.RefForeignKey, NbreCountRec = count, NbreRelation = 0, listRelationToTable = new List<RelationToTable>() };
                        var lstTableRefForeignKey = infoDataBase.listInfosContraintes.Where(c => c.name_referenced_object_id == table.NomTable).ToList();
                        if (lstTableRefForeignKey.Count != 0) {
                            tableRelationTableRefFK.NbreRelation = lstTableRefForeignKey.Count;
                            foreach (var t in lstTableRefForeignKey) {
                                TableRelationTable tableRelationTable = new TableRelationTable() { NomTable = t.name_parent_object_id, listRelationToTable = new List<RelationToTable>() };
                                RelationToTable relationToTable = new RelationToTable() { NomContrainte = t.name_constraint_object_id, TableRelation = tableRelationTable, IdColumnFrom = t.referenced_column_id, IdColumnTo = t.parent_column_id };
                                tableRelationTableRefFK.listRelationToTable.Add(relationToTable);
                            }
                        }
                        table.NbreRefForeignKey = lstTableRefForeignKey.Count;
                        table.TableRelationTableRefForeignKey = tableRelationTableRefFK;
                        infoDataBase.listTableRelationTable.Add(tableRelationTableRefFK);


                        // POur l'identity :
                        foreach (var c in table.listInfosColumn) {
                            int Position = int.Parse(c.sPositionOrdinale);
                            bool Identity = c.infosSysColums.IsIdentity == "True";
                            if (Position == 1 && Identity) {
                                table.HasColumnIdentity = true;
                            }
                        }
                    }

                    CloseConnectionDB();
                }
                else {
                    GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@GetStructure #01 GetLignes DB not open !");
                }

                // Premier filtrage général , on ne retient que les tables USer (BASE TABLE), on exclu les vues et table système:
                infoDataBase.listInfosTable = infoDataBase.listInfosTable.Where(c => c.TableType == "BASE TABLE" && c.NomTable != "sysdiagrams").OrderBy(c => c.NomTable).ToList();

            }
            return infoDataBase;
        }



        #region REQUETES SYSTEMES

        public void QUERY_GetStructure_sys_objects(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM sys.sysobjects  ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {                           // signification voir : https://msdn.microsoft.com/fr-fr/library/ms177596(v=sql.120).aspx
                    string name = dr["name"].ToString();      // Nom de l'objet
                    sDebug += "\nname : " + name;
                    string id = dr["id"].ToString().Trim();   // Numero identification de l'objet
                    sDebug += "  id : " + id;
                    string xtype = dr["xtype"].ToString();    // Type d'objet exple : S = TableSystème, P = Procédure stockée, U= Table Utilisateur, IF = Fonction de table Inline, 
                    sDebug += "  xtype : " + xtype;                              //  D : Valeur par défaut ou contrainte DEFAULT, F = Contrainte Foreign Key, PK = Contrainte PrimaryKey, UK = Contrainte UNIQUE
                    string uid = dr["uid"].ToString();
                    sDebug += "  uid : " + uid;               // uid  ID de schèma du propriétaire de l'objet... voir 

                    // On saute ici plusieurs colonnes Non prises en charge...
                    string parent_obj = dr["parent_obj"].ToString();   // Numero d'id de l'objet parent, par exemple l'Id de table s'il s'agit d'un declencheur ou d'une contrainte.
                    sDebug += "  parent_obj : " + parent_obj;
                    string crdate = dr["crdate"].ToString();     // Date de creation de l'objet
                    sDebug += "  crdate : " + crdate;
                    string ftcatid = dr["ftcatid"].ToString();     // Identificateur du catalogue de texte intégral pour tables users enregistrées
                    sDebug += "  ftcatid : " + ftcatid;
                    string type = dr["type"].ToString();     // Type d'objet
                    sDebug += "  type : " + type;

                    InfoSysObject infoSysObject = new InfoSysObject() { ObjectName = name, Id_Object = int.Parse(id), xTypeObject = xtype, ParentIdObject = int.Parse(parent_obj), sDateTimeCreationObject = crdate };
                    infoDataBase.listInfoSysObject.Add(infoSysObject);
                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }


        public void QUERY_GetStructure_sys_foreign_key_columns(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM sys.foreign_key_columns  ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                string sdebug1 = "";
                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string constraint_object_id = dr["constraint_object_id"].ToString();
                    sDebug += "\nconstraint_object_id : " + constraint_object_id;
                    string constraint_column_id = dr["constraint_column_id"].ToString();
                    sDebug += "  constraint_column_id : " + constraint_column_id;
                    string parent_object_id = dr["parent_object_id"].ToString();
                    sDebug += "  parent_object_id : " + parent_object_id;
                    string parent_column_id = dr["parent_column_id"].ToString();
                    sDebug += "  parent_column_id : " + parent_column_id;
                    string referenced_object_id = dr["referenced_object_id"].ToString();
                    sDebug += "  referenced_object_id : " + referenced_object_id;
                    string referenced_column_id = dr["referenced_column_id"].ToString();
                    sDebug += "  referenced_column_id : " + referenced_column_id;

                    InfosContraintes infosContraintes = new InfosContraintes() {
                        constraint_object_id = int.Parse(constraint_object_id),
                        constraint_column_id = int.Parse(constraint_column_id),
                        parent_object_id = int.Parse(parent_object_id),
                        parent_column_id = int.Parse(parent_column_id),
                        referenced_object_id = int.Parse(referenced_object_id),
                        referenced_column_id = int.Parse(referenced_column_id)
                    };

                    var obj1 = infoDataBase.listInfoSysObject.Where(c => c.Id_Object == infosContraintes.constraint_object_id).FirstOrDefault();
                    if (obj1 != null) {
                        infosContraintes.name_constraint_object_id = obj1.ObjectName;
                    }
                    var obj2 = infoDataBase.listInfoSysObject.Where(c => c.Id_Object == infosContraintes.parent_object_id).FirstOrDefault();
                    if (obj2 != null) {
                        infosContraintes.name_parent_object_id = obj2.ObjectName;
                    }
                    var obj3 = infoDataBase.listInfoSysObject.Where(c => c.Id_Object == infosContraintes.referenced_object_id).FirstOrDefault();
                    if (obj3 != null) {
                        infosContraintes.name_referenced_object_id = obj3.ObjectName;
                    }

                    sdebug1 += "\n" + infosContraintes.name_constraint_object_id;
                    sdebug1 += " [" + infosContraintes.constraint_column_id + "] ";
                    sdebug1 += " = ";
                    sdebug1 += string.IsNullOrEmpty(infosContraintes.name_parent_object_id) ? "?? " + infosContraintes.parent_object_id.ToString() + " ??" : infosContraintes.name_parent_object_id;
                    sdebug1 += " [" + infosContraintes.parent_column_id + "] ";
                    sdebug1 += " <-> ";
                    sdebug1 += string.IsNullOrEmpty(infosContraintes.name_referenced_object_id) ? "?? " + infosContraintes.referenced_object_id.ToString() + " ??" : infosContraintes.name_referenced_object_id;
                    sdebug1 += " [" + infosContraintes.referenced_column_id + "] ";

                    infoDataBase.listInfosContraintes.Add(infosContraintes);
                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }

        public void QUERY_GetStructure_sys_columns(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM sys.columns  ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string object_id = dr["object_id"].ToString();
                    sDebug += "\nobject_id : " + object_id;
                    string name = dr["name"].ToString();
                    sDebug += "  name : " + name;
                    string column_id = dr["column_id"].ToString();
                    sDebug += "  column_id : " + column_id;
                    string system_type_id = dr["system_type_id"].ToString();
                    sDebug += "  system_type_id : " + system_type_id;
                    string user_type_id = dr["user_type_id"].ToString();
                    sDebug += "  user_type_id : " + user_type_id;
                    string max_length = dr["max_length"].ToString();
                    sDebug += "  max_length : " + user_type_id;
                    string precision = dr["precision"].ToString();
                    sDebug += "  precision : " + precision;
                    string scale = dr["scale"].ToString();
                    sDebug += "  scale : " + scale;

                    string collation_name = dr["collation_name"].ToString();
                    sDebug += "  collation_name : " + collation_name;
                    string is_nullable = dr["is_nullable"].ToString();
                    sDebug += "  is_nullable : " + is_nullable;
                    string is_ansi_padded = dr["is_ansi_padded"].ToString();
                    sDebug += "  is_ansi_padded : " + is_ansi_padded;
                    string is_rowguidcol = dr["is_rowguidcol"].ToString();
                    sDebug += "  is_rowguidcol : " + is_rowguidcol;
                    string is_identity = dr["is_identity"].ToString();
                    sDebug += "  is_identity : " + is_identity;
                    string is_computed = dr["is_computed"].ToString();
                    sDebug += "  is_computed : " + is_computed;
                    string is_filestream = dr["is_filestream"].ToString();
                    sDebug += "  is_filestream : " + is_filestream;

                    InfosSysColums infosSysColums = new InfosSysColums() { OwnerId = int.Parse(object_id), NameColumns = name, column_Id = int.Parse(column_id), sysTypeId = system_type_id, useTypeId = user_type_id, MaxLenght = int.Parse(max_length), IsNullable = is_nullable, IsIdentity = is_identity, IsComputed = is_computed, IsRowGuidCol = is_rowguidcol };
                    var obj = infoDataBase.listInfoSysObject.Where(c => c.Id_Object == infosSysColums.OwnerId).FirstOrDefault();
                    if (obj != null) {
                        obj.listInfosSysColums.Add(infosSysColums);
                    }

                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }

        public void QUERY_GetStructure_sys_indexes(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM sys.indexes  ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string object_id = dr["object_id"].ToString();
                    sDebug += "\nobject_id : " + object_id;
                    string name = dr["name"].ToString();
                    sDebug += "  name : " + name;
                    string index_id = dr["index_id"].ToString();
                    sDebug += "  index_id : " + index_id;
                    string type = dr["type"].ToString();
                    sDebug += "  type : " + type;
                    string type_desc = dr["type_desc"].ToString();
                    sDebug += "  type_desc : " + type_desc;
                    string referenced_column_id = dr["is_primary_key"].ToString();
                    sDebug += "  is_primary_key : " + referenced_column_id;
                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }


        #endregion


        #region REQUETE VUES SCHEMA D'INFORMATION

        public void QUERY_GetStructure_TABLES(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM INFORMATION_SCHEMA.TABLES ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string TABLE_CATALOG = dr["TABLE_CATALOG"].ToString();
                    sDebug += "\nTABLE_CATALOG : " + TABLE_CATALOG;
                    string TABLE_SCHEMA = dr["TABLE_SCHEMA"].ToString();
                    sDebug += "  TABLE_SCHEMA : " + TABLE_SCHEMA;
                    string TABLE_NAME = dr["TABLE_NAME"].ToString();
                    sDebug += "  TABLE_NAME : " + TABLE_NAME;
                    string TABLE_TYPE = dr["TABLE_TYPE"].ToString();
                    sDebug += "  TABLE_TYPE : " + TABLE_TYPE;

                    InfosTable infosTable = new InfosTable() { NomTable = TABLE_NAME, TableType = TABLE_TYPE };
                    var sysTable = infoDataBase.listInfoSysObject.Where(c => c.ObjectName == TABLE_NAME).FirstOrDefault();
                    if (sysTable != null) {
                        infosTable.infoSysObject = sysTable;
                    }
                    else {
                        infoDataBase.CptNotDefined++;
                    }
                    infoDataBase.listInfosTable.Add(infosTable);
                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }


        public void QUERY_GetStructure_COLUMS(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string TABLE_CATALOG = dr["TABLE_CATALOG"].ToString();
                    sDebug += "\nTABLE_CATALOG : " + TABLE_CATALOG;
                    string TABLE_SCHEMA = dr["TABLE_SCHEMA"].ToString();
                    sDebug += "  TABLE_SCHEMA : " + TABLE_SCHEMA;
                    string TABLE_NAME = dr["TABLE_NAME"].ToString();
                    sDebug += "  TABLE_NAME : " + TABLE_NAME;
                    string COLUMN_NAME = dr["COLUMN_NAME"].ToString();
                    sDebug += "  COLUMN_NAME : " + COLUMN_NAME;
                    string ORDINAL_POSITION = dr["ORDINAL_POSITION"].ToString();
                    sDebug += "  ORDINAL_POSITION : " + ORDINAL_POSITION;
                    string IS_NULLABLE = dr["IS_NULLABLE"].ToString();
                    sDebug += "  IS_NULLABLE : " + IS_NULLABLE;
                    string DATA_TYPE = dr["DATA_TYPE"].ToString();
                    sDebug += "  DATA_TYPE : " + DATA_TYPE;
                    string CHARACTER_MAXIMUM_LENGTH = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    sDebug += "  CHARACTER_MAXIMUM_LENGTH : " + CHARACTER_MAXIMUM_LENGTH;

                    var table = infoDataBase.listInfosTable.Where(c => c.NomTable == TABLE_NAME).FirstOrDefault();
                    if (table != null) {
                        InfosColumn infosColumn = new InfosColumn() { NomColonne = COLUMN_NAME, sPositionOrdinale = ORDINAL_POSITION, isNullable = IS_NULLABLE, sDataType = DATA_TYPE, caracterMaxLenght = CHARACTER_MAXIMUM_LENGTH };
                        var syscolumn = table.infoSysObject.listInfosSysColums.Where(c => c.NameColumns == COLUMN_NAME).FirstOrDefault();
                        if (syscolumn != null) {
                            infosColumn.infosSysColums = syscolumn;
                        }
                        else {
                            infoDataBase.CptNotDefined++;
                        }
                        table.listInfosColumn.Add(infosColumn);
                    }
                    else {
                        infoDataBase.CptNotDefined++;
                    }
                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }


        public void QUERY_GetStructure_CONSTRAINT_COLUMN_USAGE(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string TABLE_CATALOG = dr["TABLE_CATALOG"].ToString();
                    sDebug += "\nTABLE_CATALOG : " + TABLE_CATALOG;
                    string TABLE_SCHEMA = dr["TABLE_SCHEMA"].ToString();
                    sDebug += "  TABLE_SCHEMA : " + TABLE_SCHEMA;
                    string TABLE_NAME = dr["TABLE_NAME"].ToString();
                    sDebug += "  TABLE_NAME : " + TABLE_NAME;
                    string COLUMN_NAME = dr["COLUMN_NAME"].ToString();
                    sDebug += "  COLUMN_NAME : " + COLUMN_NAME;
                    string CONSTRAINT_CATALOG = dr["CONSTRAINT_CATALOG"].ToString();
                    sDebug += "  CONSTRAINT_CATALOG : " + CONSTRAINT_CATALOG;
                    string CONSTRAINT_SCHEMA = dr["CONSTRAINT_SCHEMA"].ToString();
                    sDebug += "  CONSTRAINT_SCHEMA : " + CONSTRAINT_SCHEMA;
                    string CONSTRAINT_NAME = dr["CONSTRAINT_NAME"].ToString();
                    sDebug += "  CONSTRAINT_NAME : " + CONSTRAINT_NAME;

                    var table = infoDataBase.listInfosTable.Where(c => c.NomTable == TABLE_NAME).FirstOrDefault();
                    if (table != null) {
                        var column = table.listInfosColumn.Where(c => c.NomColonne == COLUMN_NAME).FirstOrDefault();
                        if (column != null) {
                            column.ContrainteCatalog = CONSTRAINT_CATALOG;
                            column.ContrainteSchema = CONSTRAINT_SCHEMA;
                            column.ContrainteNom = CONSTRAINT_NAME;
                        }
                    }
                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }


        public void QUERY_GetStructure_KEY_COLUMN_USAGE(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string CONSTRAINT_CATALOG = dr["CONSTRAINT_CATALOG"].ToString();
                    sDebug += "\nCONSTRAINT_CATALOG : " + CONSTRAINT_CATALOG;
                    string CONSTRAINT_SCHEMA = dr["CONSTRAINT_SCHEMA"].ToString();
                    sDebug += "  CONSTRAINT_SCHEMA : " + CONSTRAINT_SCHEMA;
                    string CONSTRAINT_NAME = dr["CONSTRAINT_NAME"].ToString();
                    sDebug += "  CONSTRAINT_NAME : " + CONSTRAINT_NAME;
                    string TABLE_CATALOG = dr["TABLE_CATALOG"].ToString();
                    sDebug += "  TABLE_CATALOG : " + TABLE_CATALOG;
                    string TABLE_SCHEMA = dr["TABLE_SCHEMA"].ToString();
                    sDebug += "  TABLE_SCHEMA : " + TABLE_SCHEMA;
                    string TABLE_NAME = dr["TABLE_NAME"].ToString();
                    sDebug += "  TABLE_NAME : " + TABLE_NAME;
                    string COLUMN_NAME = dr["COLUMN_NAME"].ToString();
                    sDebug += "  COLUMN_NAME : " + COLUMN_NAME;
                    string ORDINAL_POSITION = dr["ORDINAL_POSITION"].ToString();
                    sDebug += "  ORDINAL_POSITION : " + ORDINAL_POSITION;

                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }


        public void QUERY_GetStructure_REFERENTIAL_CONSTRAINTS(InfoDataBase infoDataBase) {
            string queryGetNumCmd = "SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS  ";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sDebug = "";
            try {
                OdbcCommand CmdGetNumCmd = null;
                CmdGetNumCmd = GetNewCmdSQL(queryGetNumCmd);

                var dr = CmdGetNumCmd.ExecuteReader();
                int cpt = 0;
                while (dr.Read()) {
                    string CONSTRAINT_CATALOG = dr["CONSTRAINT_CATALOG"].ToString();
                    sDebug += "\nCONSTRAINT_CATALOG : " + CONSTRAINT_CATALOG;
                    string CONSTRAINT_SCHEMA = dr["CONSTRAINT_SCHEMA"].ToString();
                    sDebug += "  CONSTRAINT_SCHEMA : " + CONSTRAINT_SCHEMA;
                    string CONSTRAINT_NAME = dr["CONSTRAINT_NAME"].ToString();
                    sDebug += "  CONSTRAINT_NAME : " + CONSTRAINT_NAME;
                    string UNIQUE_CONSTRAINT_CATALOG = dr["UNIQUE_CONSTRAINT_CATALOG"].ToString();
                    sDebug += "  UNIQUE_CONSTRAINT_CATALOG : " + UNIQUE_CONSTRAINT_CATALOG;
                    string UNIQUE_CONSTRAINT_SCHEMA = dr["UNIQUE_CONSTRAINT_SCHEMA"].ToString();
                    sDebug += "  UNIQUE_CONSTRAINT_SCHEMA : " + UNIQUE_CONSTRAINT_SCHEMA;
                    string UNIQUE_CONSTRAINT_NAME = dr["UNIQUE_CONSTRAINT_NAME"].ToString();
                    sDebug += "  UNIQUE_CONSTRAINT_NAME : " + UNIQUE_CONSTRAINT_NAME;
                    string MATCH_OPTION = dr["MATCH_OPTION"].ToString();
                    sDebug += "  MATCH_OPTION : " + MATCH_OPTION;
                    string UPDATE_RULE = dr["UPDATE_RULE"].ToString();
                    sDebug += "  UPDATE_RULE : " + UPDATE_RULE;
                    string DELETE_RULE = dr["DELETE_RULE"].ToString();
                    sDebug += "  DELETE_RULE : " + DELETE_RULE;

                    cpt++;
                }
            }
            catch (Exception ex) {
                GlobalLog.Instance().AjouteLog("xAffaireLigneArticleAS400", "@QUERY_GetStructure  Exception :" + ex.Message);
            }
            sw.Stop();
        }

        #endregion
        #endregion
    }


}
