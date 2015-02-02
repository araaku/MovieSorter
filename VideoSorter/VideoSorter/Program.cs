using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace VideoSorter
{
    class Program
    {
        private static Sanitizer sanitizer = new Sanitizer();
        private static Movie movie;
        private static IMDB imdb = new IMDB();
        static void Main(string[] args)
        {

            
            IList<string> fileNames = Common.ReadAllFiles();
            List<Movie> sanitizedList = new List<Movie>();

            sanitizedList.Add(new Movie());

            if (fileNames != null)
            {
                foreach (string value in fileNames)
                {
                    if (string.IsNullOrEmpty(value)) continue;

                    movie = new Movie();

                    movie.RawString = value;

                    sanitizer.Sanitize(movie);

                    Console.WriteLine("Getting details for: " + movie.Title);
                    sanitizedList.Add(imdb.GetDetails(movie));
                }
            }

            Console.WriteLine("Total files read from disk: {0}", fileNames.Count);
            Console.WriteLine("Total files sanitized: {0}", sanitizedList.Count);

            /// Export to Excel
            Common.ExportGenericListToExcel(sanitizedList, "");

        }
    }
}
