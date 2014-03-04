namespace ExportSchema
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Oracle.ManagedDataAccess.Client;
    using System.Globalization;
    using System.Linq;

    internal class DataAccess
    {
        internal IEnumerable<DbObject> GetDbObjects(string connString, string schema)
        {
            const string Query = "SELECT owner, object_name, decode(object_type, 'PACKAGE BODY', 'PACKAGE_BODY', 'DATABASE LINK', 'DB_LINK', 'MATERIALIZED VIEW', 'MATERIALIZED_VIEW', object_type) "
                                + "FROM dba_objects "
                                + "WHERE object_type <> 'LOB' "
                                + "AND upper(owner) = upper(:schema)";

            var results = new List<DbObject>();
            using (var orclConnection = new OracleConnection(connString))
            {
                orclConnection.Open();
                using (var cmd = new OracleCommand() { Connection = orclConnection, CommandType = CommandType.Text })
                {
                    cmd.CommandText = Query;
                    cmd.Parameters.Add(":schema", schema);

                    var orclRdr = cmd.ExecuteReader();
                    while (orclRdr.Read())
                    {
                        results.Add(new DbObject
                            {
                                Owner = orclRdr.GetString(0),
                                ObjectName = orclRdr.GetString(1),
                                ObjectType = orclRdr.GetString(2),
                            });
                    }
                }
                orclConnection.Close();
            }

            return results;
        }

        internal string GetDdl(string connString, string objectType, string objectName, string owner)
        {
            var returnValue = string.Empty;

            try
            {
                using (var orclConnection = new OracleConnection(connString))
                {
                    orclConnection.Open();
                    using (var cmd = new OracleCommand() { Connection = orclConnection, CommandType = CommandType.Text })
                    {

                        // turn off some of the information we don't need.
                        cmd.CommandText = " begin DBMS_METADATA.SET_TRANSFORM_PARAM (DBMS_METADATA.SESSION_TRANSFORM, 'STORAGE',false); DBMS_METADATA.SET_TRANSFORM_PARAM (DBMS_METADATA.SESSION_TRANSFORM, 'SEGMENT_ATTRIBUTES',false); end;";
                        cmd.ExecuteNonQuery();

                        // perform the ddl fetch
                        cmd.Parameters.Add(":objectType", objectType);
                        cmd.Parameters.Add(":objectName", objectName);
                        cmd.Parameters.Add(":owner", owner);
                        cmd.CommandText = "SELECT DBMS_METADATA.GET_DDL(:objectType, :objectName, :owner) FROM dual";
                        returnValue = cmd.ExecuteScalar().ToString();
                    }
                    orclConnection.Close();
                }
            }
            catch (Exception ex)
            {
                // Ignore errors
                System.Diagnostics.Debug.Write(ex.Message);
            }

            return returnValue;
        }
    }
}