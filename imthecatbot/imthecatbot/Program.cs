using System;
using System.Threading.Tasks;

namespace imthecatbot
{
    class Program
    {
        string ip = "irc.chat.twitch.tv";
        int port = 6667;
        string password = catSecrets.botPassword;
        string botUsername = catSecrets.botUsername;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
