using System;

namespace AutoComplete.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // check argument count
                if (args.Length != 2)
                {
                    throw new ArgumentOutOfRangeException("args");
                }

                var serverAddress = args[0];

                // check port argument
                int port;
                if (!Int32.TryParse(args[1], out port))
                {
                    throw new ArgumentException("port");
                }

                // start autocomplete client
                StartClient(serverAddress, port);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Starts autocomplete client
        /// </summary>
        /// <param name="serverAddress">Server net address</param>
        /// <param name="port">Port, that server listen</param>
        static void StartClient(string serverAddress, int port)
        {
            // initialize client
            var client = new AutoCompleteClient(serverAddress, port);

            try
            {
                client.Start();
            }
            finally
            {
                client.Stop();
            }
        }
    }
}
