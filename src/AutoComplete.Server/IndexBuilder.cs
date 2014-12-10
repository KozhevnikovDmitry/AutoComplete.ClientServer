using System.Collections.Generic;
using System.Linq;

namespace AutoComplete.Server
{
    /// <summary>
    /// Static service, that builds some kind of string index for provided vocabulary.
    /// </summary>
    public static class IndexBuilder
    {
        /// <summary>
        /// Returns index for vocabulary. That index will be used for providing auto-completion
        /// </summary>
        /// <param name="vocabulary">Source vocabulary</param>
        /// <param name="completeAmount">Amount of the completion set</param>
        /// <remarks>
        /// Method builds some kind of string index for vocabulary. 
        /// That index has word as a key, and words completion(as single string) set as a value.
        /// </remarks>
        public static Dictionary<string, string[]> BuildIndex(string[] vocabulary, int completeAmount)
        {
            // index accumulator
            var vocabularyIndex = new List<KeyValuePair<string, string[]>>();

            // maximum word's length in vocabulary
            var wordMaxLength = vocabulary.Select(t => t.Length).Max();

            // for every possible word's length in vocabulary
            for (int len = 1; len <= wordMaxLength; len++)
            {
                // add to index completion set for words of 'len' length
                vocabularyIndex.AddRange(vocabulary.Where(t => t.Length >= len)
                                                   .GroupBy(t => t.Substring(0, len))
                                                   .ToDictionary(t => t.Key, t => t.Take(completeAmount).ToArray()));
            }

            return vocabularyIndex.ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
