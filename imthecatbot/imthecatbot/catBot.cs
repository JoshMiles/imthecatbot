using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace imthecatbot
{
    public class catBot
    {
        StreamWriter sw;
        StreamReader sr;
        bool listening = true;

        private string twitchChannel;
        private string botUsername;
        private string botPassword;
        private string ip;
        private int port;

        private TaskCompletionSource<int> connected = new TaskCompletionSource<int>();

        public event TwitchChatEventHandler OnMessage = delegate { };
        public delegate void TwitchChatEventHandler(object sender, TwitchChatMessage e);

        public class TwitchChatMessage : EventArgs
        {
            public string Sender { get; set; }
            public string Message { get; set; }
            public string Channel { get; set; }
        }

        public catBot(string botUsername, string botPassword, string ip, int port, string twitchChannel)
        {
            this.twitchChannel = twitchChannel;
            this.botUsername = botUsername;
            this.botPassword = botPassword;
            this.ip = ip;
            this.port = port;
        }

        public async Task loginBot()
        {
            Console.WriteLine("Attempting login.");


            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);

            SslStream sslStream = new SslStream(tcpClient.GetStream(), false, ValidateServerCertificate, null);

            await sslStream.AuthenticateAsClientAsync(ip);


            sr = new StreamReader(sslStream);
            sw = new StreamWriter(sslStream)
            {
                NewLine = "\t\n",
                AutoFlush = true
            };

            await sw.WriteLineAsync($"PASS {botPassword}");
            await sw.WriteLineAsync($"NICK {botUsername}");
            await sw.WriteLineAsync($"JOIN #{twitchChannel}");
            connected.SetResult(0);

            Console.WriteLine("Login Complete.");
            Console.WriteLine("Listening...");

            while (listening)
            {
                string line = await sr.ReadLineAsync();
                Console.WriteLine(line);

                string[] split = line.Split(" ");

                if (line.StartsWith("PING"))
                {
                    Console.WriteLine("PONGing the PING");
                    await sw.WriteLineAsync($"PONG {split[1]}");
                }

                if (split.Length > 1 && split[1] == "PRIVMSG")
                {
                    int exclamationPointPosition = split[0].IndexOf("!");
                    string username = split[0].Substring(1, exclamationPointPosition - 1);
                    int secondColonPosition = line.IndexOf(':', 1);
                    string message = line.Substring(secondColonPosition + 1);
                    string channel = split[2].TrimStart('#');

                    OnMessage(this, new TwitchChatMessage
                    {
                        Message = message,
                        Sender = username,
                        Channel = channel
                    });
                }

            }
        }

        public async Task sendMessage(string messageText)
        {
            await connected.Task;
            await sw.WriteLineAsync($"PRIVMSG #{twitchChannel} :{messageText}");
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}
