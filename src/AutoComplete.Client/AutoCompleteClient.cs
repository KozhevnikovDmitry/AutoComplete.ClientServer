using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AutoComplete.Client
{
    /// <summary>
    /// Client for getting autompletion from particular server using TCP
    /// </summary>
    public class AutoCompleteClient
    {
        /// <summary>
        /// Address of the server
        /// </summary>
        private readonly string _serverAddress;

        /// <summary>
        /// Port, that server listen to
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// While it is true, client is running
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Client for getting autompletion from particular server
        /// </summary>
        /// <param name="serverAddress">Address of the server</param>
        /// <param name="port">Port, that server listen to</param>
        /// <exception cref="ArgumentNullException">serverAddress</exception>
        public AutoCompleteClient(string serverAddress, int port)
        {
            if (string.IsNullOrEmpty(serverAddress))
            {
                throw new ArgumentNullException(serverAddress);
            }
            _serverAddress = serverAddress;
            _port = port;
        }

        /// <summary>
        /// Starts the autocomplete client. Client is waiting for console input of prefixes.
        /// </summary>
        public void Start()
        {
            _isRunning = true;
            while (_isRunning)
            {
                // read prefix
                var input = Console.ReadLine();

                // get autocomplete from server
                GetAutoComplete(input);
            }
        }

        /// <summary>
        /// Sends prefix to server and prints completion to console.
        /// </summary>
        /// <param name="prefix">Prefix for completion</param>
        private void GetAutoComplete(string prefix)
        {
            // net client
            var client = new TcpClient();
            client.Connect(_serverAddress, _port);

            var reader = new StreamReader(client.GetStream(), Encoding.ASCII);
            var writer = new StreamWriter(client.GetStream(), Encoding.ASCII);

            // send the request
            writer.WriteLine("get " + prefix);
            writer.Flush();

            // completion result accumulator
            string completionAccumulator = string.Empty;
            
            // get completion lines count
            int count = Convert.ToInt32(reader.ReadLine());

            // get completion line by line
            for (int i = 0; i < count; i++)
            {
                completionAccumulator += reader.ReadLine() + "\r\n";
            }

            // print completion
            Console.WriteLine(completionAccumulator);
            client.Close();
        }

        /// <summary>
        /// Stops the autocomplete client
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            Thread.Sleep(300);
        }
    }
}