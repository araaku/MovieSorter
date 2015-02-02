using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoSorter
{
    class Sanitizer
    {
        Movie _movie;
        private short ReleaseYear_Min = Convert.ToInt16(DateTime.Now.Year - 40); /// Not interested in movies older than 40 years.
        private short ReleaseYear_Max = Convert.ToInt16(DateTime.Now.Year);

        public Sanitizer()
        {
        }

        public Sanitizer(Movie movieObject)
        {
            _movie = movieObject;
        }

        public void Sanitize(Movie movieObject = null)
        {
            if (movieObject != null)
            {
                _movie = movieObject;
            }

            if (_movie == null)
            {
                return;
            }

            TryYear();
            TryTitle();
        }

        /// <summary>
        /// This function checks the string for any year mentioned in it.
        /// </summary>
        /// <returns></returns>
        private void TryYear()
        {
            string[] numbers = Regex.Split(_movie.RawString, @"\D+");
            short temp;

            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value) && short.TryParse(value, out temp))
                {
                    if (temp >= ReleaseYear_Min && temp <= ReleaseYear_Max)
                    {
                        _movie.Year = temp.ToString();

                        /// Release year is not a part of the movie name. Remove the year part from the name by truncating the string.
                        _movie.RawString = _movie.RawString.Substring(0, _movie.RawString.IndexOf(value));
                    }
                }
            }
        }

        private void TryTitle()
        {
            int index = -1;

            /// Words in some names are separated with a dot(.) instead of a space. Replace all dots with space.
            _movie.RawString = _movie.RawString.Replace('.', ' ');

            /// -------------------------------------------------------------------------------------------
            /// truncate the string from the position of the first location found.
            /// This will work for titles like Cars[2006]DvDrip[Eng], Hall Pass (2011)
            /// 

            if ((index = _movie.RawString.IndexOfAny(new char[] { '[', '(' })) > 0)
            {
                _movie.Title = _movie.RawString.Substring(0, index);
            }
            else
            {
                _movie.Title = _movie.RawString;
            }
            /// -------------------------------------------------------------------------------------------
            /// 

            /// If the title contains unwanted words, truncate from there.
            foreach (string value in Common.unwantedWords)
            {
                if ((index = _movie.Title.IndexOf(value)) > -1) {
                    _movie.Title = _movie.Title.Substring(0, index);
                }
            }
        }
    }
}
