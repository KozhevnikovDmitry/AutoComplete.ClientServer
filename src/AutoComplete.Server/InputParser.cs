using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoComplete.Server
{
    /// <summary>
    /// Statiñ service, that parse input lines to vocabulary
    /// </summary>
    /// <remarks>
    /// During parsing vocabulary is being ordered by word length descending and than by alphabetical order.
    /// It is small preparation for completion algorythm.
    /// </remarks>
    public static class InputParser
    {
        /// <summary>
        /// Parses input lines and returns ordered vocabulary
        /// </summary>
        /// <exception cref="ArgumentException">Input lines is null or empty</exception>
        public static string[] Parse(string[] inputLines)
        {
            // parsing amount
            int vocabularyAmount = Convert.ToInt32(inputLines[0]);

            // parsing vocabulary and frecuencies
            var vocabularyFrequencies = new Dictionary<string, int>();
            for (int i = 1; i < vocabularyAmount + 1; i++)
            {
                var split = inputLines[i].Split(' ');
                vocabularyFrequencies[split[0]] = Convert.ToInt32(split[1]);
            }
            
            // NOTE: vocabulary is ordred by frequency descending and that by alphabetical order
            // method returns only words, but not the frequencies
            return vocabularyFrequencies.OrderByDescending(t => t.Value).ThenBy(t => t.Key).Select(t => t.Key).ToArray();
        }
    }
}