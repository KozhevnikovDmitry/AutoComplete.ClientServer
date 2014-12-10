using System;
using System.IO;

namespace AutoComplete.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // check arguments count
                if (args.Length != 2)
                {
                    throw new ArgumentOutOfRangeException("args");
                }

                // check file existence
                var filePath = args[0];

                if (!File.Exists(filePath))
                {
                    throw new ArgumentException("vocabularyPath");
                }

                // check port argument
                int port;
                if (!Int32.TryParse(args[1], out port))
                {
                    throw new ArgumentException("port");
                }

                // start autocomplete server
                StartServer(filePath, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Starts autocomplete server
        /// </summary>
        /// <param name="vocabularyPath">Path to file with vocabulary</param>
        /// <param name="port">Port to listen</param>
        static void StartServer(string vocabularyPath, int port)
        {
            // initialize server
            var server = new AutoCompleteServer(port);
            try
            {
                // get raw vocabulary data
                var rawVocabulary = File.ReadAllLines(vocabularyPath);

                // ordered vocabulary
                var vocabulary = InputParser.Parse(rawVocabulary);

                // vocabulary index
                var index = IndexBuilder.BuildIndex(vocabulary, 10);

                // start autocomplete server
                server.Start(index);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
