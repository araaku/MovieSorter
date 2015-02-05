using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;

namespace VideoSorter
{
    public static class Common
    {
        private static string directoryLocation = @"D:\Araaku\Academics";

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// Most famous video extensions
        static string[] extensions = "*.3gp,*.avi,*.bdvm,*.dat,*.dvr-ms,*.flv,*.ifo,*.m2ts,*.m4v,*.mkv,*.mov,*.mp4,*.mpeg,*.mpg,*.mts,*.ogm,*.ogv,*.qt,*.rm,*.sbe,*.ts,*.wmv,*.wtv".Split(',');

        /// Common words used which are generally not a part of the title
        public static string[] unwantedWords = "BRRip,DvDrip,Xvid,MAXSPEED,aXXo".ToLower().Split(',');

        public static IList<string> ReadAllFiles(string _directoryLocation = "")
        {
            if (!string.IsNullOrEmpty(_directoryLocation))
                directoryLocation = _directoryLocation;

            string[] arr = extensions.SelectMany(i => Directory.GetFiles(directoryLocation, i, SearchOption.AllDirectories)).Select(i => Path.GetFileNameWithoutExtension(i).ToLower().Trim()).ToArray();

            Console.WriteLine("Number of video files found: " + arr.Length);

            return arr.ToList();
        }

        public static Movie StringToJSONtoMovie(string jsonString)
        {
            var jsonObject = serializer.DeserializeObject(jsonString) as Dictionary<string, object>;

            return (Movie)ObjectFromDictionary<Movie>(jsonObject);
        }

        private static T ObjectFromDictionary<T>(IDictionary<string, object> dict) where T : class
        {
            Type type = typeof(T);
            T result = (T)Activator.CreateInstance(type);
            foreach (var item in dict)
            {
                type.GetProperty(item.Key).SetValue(result, item.Value, null);
            }
            return result;
        }

        public static void ExportGenericListToExcel<T>(List<T> list, string excelFilePath)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            Application appC = new Application();
            appC.SheetsInNewWorkbook = 1;
            appC.Visible = true;

            Workbook workbook = appC.Workbooks.Add(Type.Missing);
            Sheets worksheets = workbook.Worksheets;
            var worksheet = (Worksheet)worksheets[1];
            worksheet.Name = "Movie Database";


            for (int i = 1; i < properties.Count; i++)
                worksheet.Cells[1, i].Value = properties[i].Name;

            for (int i = 1; i < list.Count; i++)
                for (int j = 1; j < properties.Count; j++)
                    worksheet.Cells[i + 1, j].Value = properties[j].GetValue(list[i]);

            workbook.SaveAs(string.Format("output_{0}.xls", DateTime.Now.Millisecond.ToString()));

        }

    }
}
