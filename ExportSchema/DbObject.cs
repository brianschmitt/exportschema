using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExportSchema
{
    internal class DbObject
    {
        public string Owner { get; set; }

        public string ObjectName { get; set; }

        public string ObjectType { get; set; }

        public string FileName
        {
            get
            {
                var ext = new Dictionary<string, string>
                    {
                        { "FUNCTION", "FNC" },
                        { "PACKAGE", "PKG" },
                        { "PACKAGE_BODY", "PKB" },
                        { "PROCEDURE", "PRC" },
                        { "TABLE", "TAB" },
                        { "TRIGGER", "TRG" },
                        { "TYPE", "TYP" },
                        { "VIEW", "FEW" },
                        { "DB_LINK", "DBL" },
                        { "INDEX", "IDX" },
                        { "LOB", "LOB" },
                        { "SEQUENCE", "SEQ" },
                        { "SYNONYM", "SYN" },
                        { "MATERIALIZED_VIEW", "FEW" },
                        { "TABLE PARTITION", "PRT" }
                    };

                if (ext.ContainsKey(ObjectType))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Owner, ObjectName, ext[ObjectType]);
                }
                return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Owner, ObjectName, ObjectType);
            }
        }
    }
}
