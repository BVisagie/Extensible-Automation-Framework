using System;
using System.Text.RegularExpressions;

namespace EAF.Core.Utilities
{
    public static class SharedMethods
    {
        /// <summary>
        /// Creates a short mostly unique id composed of only alphanumeric characters.
        /// https://stackoverflow.com/a/42026123/3324415
        /// </summary>
        public static string ShortUid()
        {
            return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
        }

        /// <summary>
        /// Returns a random number between the given min and max values
        /// </summary>
        public static int GetRandomNumber(int min, int max)
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(min, max);
        }
    }
}