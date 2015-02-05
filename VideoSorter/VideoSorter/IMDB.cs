using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VideoSorter
{
    class IMDB
    {
        private string serviceURL = "http://www.omdbapi.com";

        public Movie GetDetails(Movie movie)
        {

            if (string.IsNullOrEmpty(movie.Title))
            {
                return null;
            }

            return CallRestMethod(movie);
        }

        private Movie CallRestMethod(Movie movie)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceURL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(string.Format("?t={0}&y={1}", movie.Title, movie.Year != "0" ? movie.Year : "")).Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsStringAsync().Result;

                if (dataObjects.IndexOf("Movie not found!") == -1)
                {
                    movie.jsonString = dataObjects;

                    movie = Common.StringToJSONtoMovie(movie.jsonString);
                }
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return movie;
        }

    }
}
