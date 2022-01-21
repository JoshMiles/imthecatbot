using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace imthecatbot
{
    class Program
    {

        static async Task Main(string[] args)
        {

            Console.WriteLine("Starting up.");

            catBot meow = new catBot(catSecrets.botUsername, catSecrets.botPassword, catSecrets.ip, catSecrets.port, catSecrets.twitchChannel);

            meow.loginBot().SafeFireAndForget();

            Console.WriteLine("Started up.");

            Console.WriteLine("Waking up...");

            await meow.sendMessage("*yawns* I have woken up:)");

            meow.OnMessage += async (sender, twitchChatMessage) =>
            {
                Console.WriteLine($"{twitchChatMessage.Sender} said {twitchChatMessage.Message}");

                if (twitchChatMessage.Message.StartsWith("!meow"))
                {
                    await meow.sendMessage($"MEOW MEOW @{twitchChatMessage.Sender}! MEOWWWW");

                }

            };

            await Task.Delay(-1);

        }
    }
}
