using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AutoComplete.Server
{
    /// <summary>
    /// Server, that provides autocompletion for incoming prefixes by TCP 
    /// </summary>
    public class AutoCompleteServer
    {
        /// <summary>
        /// net listener
        /// </summary>
        private readonly TcpListener _tcpListener;

        /// <summary>
        /// While it is true, server is running
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Server, that provides autocompletion for incoming prefixes by TCP 
        /// </summary>
        /// <param name="port">Port, that server listen to</param>
        public AutoCompleteServer(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Starts the server to listen requests to complete prefixes
        /// </summary>
        /// <param name="vocabularyIndex">Vocabulary vocabularyIndex for getting completion</param>
        public void Start(Dictionary<string, string[]> vocabularyIndex)
        {
            // start listening
            _tcpListener.Start();
            _isRunning = true;

            while (_isRunning)
            {
                // push request handling to threadpool queue 
                ThreadPool.QueueUserWorkItem(Handle, new Tuple<TcpClient, Dictionary<string, string[]>>(_tcpListener.AcceptTcpClient(), vocabularyIndex));
            }
        }

        /// <summary>
        /// Statin handle wrapper
        /// </summary>
        /// <param name="stateInfo"></param>
        static void Handle(object stateInfo)
        {
            var args = (Tuple<TcpClient, Dictionary<string, string[]>>) stateInfo;
            new RequestHandler().Handle(args.Item1, args.Item2);
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            Thread.Sleep(300);
            if (_tcpListener != null)
            {
                _tcpListener.Stop();
            }
        }

        /// <summary>
        /// Handler for single completion request
        /// </summary>
        private class RequestHandler
        {
            /// <summary>
            /// Handles request for complete prefix. Responses for correct request with completion set for prefix.
            /// </summary>
            /// <param name="client">Net client, that is used for read request and writing response</param>
            /// <param name="vocabularyIndex">Vocabulary vocabulary for getting completionr</param>
            public void Handle(TcpClient client, Dictionary<string, string[]> vocabularyIndex)
            {
                try
                {
                    var writer = new StreamWriter(client.GetStream(), Encoding.ASCII);
                    var reader = new StreamReader(client.GetStream(), Encoding.ASCII);
                    ProcessRequest(writer, reader, vocabularyIndex);
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    client.Close();
                }
            }

            /// <summary>
            /// Provides completion response for correct request and error response for incorrect.
            /// </summary>
            /// <param name="writer">For write response</param>
            /// <param name="reader">For read request</param>
            /// <param name="vocabularyIndex">Vocabulary vocabulary for getting completionr</param>
            private void ProcessRequest(StreamWriter writer, StreamReader reader, Dictionary<string, string[]> vocabularyIndex)
            {
                // expected 'get <prefixe>'
                var request = reader.ReadLine();

                // empty request is incorrect
                if (string.IsNullOrEmpty(request))
                {
                    WrongRequest(writer, request);
                    return;
                }

                var requestWords = request.Split(' ');

                // single-word request is incorrect
                if (requestWords.Length != 2)
                {
                    WrongRequest(writer, request);
                    return;
                }

                // request without 'get' at the beginning is incorrect
                if (requestWords[0] != "get")
                {
                    WrongRequest(writer, request);
                    return;
                }

                // provide completion response
                AutoComplete(writer, vocabularyIndex, requestWords[1]);
            }

            /// <summary>
            /// Provides completion response for correct request
            /// </summary>
            /// <param name="writer">For write response</param>
            /// <param name="vocabularyIndex">Vocabulary vocabulary for getting completionr</param>
            /// <param name="prefix">Prefix to complete</param>
            private void AutoComplete(StreamWriter writer, Dictionary<string, string[]> vocabularyIndex, string prefix)
            {
                // if prefix is presented index as a key
                if (vocabularyIndex.ContainsKey(prefix))
                {
                    // provide count of completion variants
                    writer.WriteLine(vocabularyIndex[prefix].Length);

                    // provide completions
                    foreach (var complete in vocabularyIndex[prefix])
                    {
                        writer.WriteLine(complete);
                    }
                }
                else
                {
                    // provide empty completion for prefix, that is not presented in index
                    writer.WriteLine(0);
                }
                writer.Flush();
            }

            /// <summary>
            /// Provides errror response for incorrent request
            /// </summary>
            /// <param name="writer">For write response</param>
            /// <param name="request">Full request string</param>
            private void WrongRequest(StreamWriter writer, string request)
            {
                writer.WriteLine(1);
                writer.WriteLine("Wrong request string[{0}]. Expected 'get <prefix>'", request);
                writer.Flush();
            }
        }
    }
}
