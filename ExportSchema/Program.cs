namespace ExportSchema
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = new[] { 
                 "Database",
                 "User Id", 
                 "Password",
                 "Output Directory"
             };

            var values = new string[settings.Length];

            var counter = 0;
            foreach (var item in settings)
            {
                Console.Write(item + ":");
                values[counter] = Console.ReadLine();
                counter += 1;
            }

            var connString = string.Format("data source={0};user id={1};password={2};",
                values[0],
                values[1],
                values[2]);

            var path = values[3];

            var cleanup = true;
            var dataAccess = new DataAccess();

            var objs = dataAccess.GetDbObjects(connString, values[1]);
            System.IO.Directory.CreateDirectory(path);

            Parallel.ForEach(
                objs,
                item =>
                {
                    var ddl = dataAccess.GetDdl(connString, item.ObjectType, item.ObjectName, item.Owner);
                    var filePath = System.IO.Path.Combine(path, item.FileName);
                    var cleanfile = cleanup ? RemoveCharacters(ddl) : ddl;
                    System.IO.File.WriteAllText(filePath, cleanfile);
                });
        }

        private static string RemoveCharacters(string contents)
        {
            var tempContents = contents
                                .Replace(System.Environment.NewLine, string.Empty);

            return Regex.Replace(tempContents, @"^\s*$", "", RegexOptions.Multiline);
        }
    }
}
