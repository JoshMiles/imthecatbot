using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace imthecatbot
{
    class Program
    {
        static catCommands commands;
        static async Task Main(string[] args)
        {

            Console.WriteLine("Starting up.");

            Console.WriteLine("Loading command modules");

            try
            {
                commands = new catCommands();
                Console.WriteLine(commands.listCommands());
            }
            catch
            {
                Console.WriteLine("There was an issue loading the command modules.");
            }


            catBot meow = new catBot(catSecrets.botUsername, catSecrets.botPassword, catSecrets.ip, catSecrets.port, catSecrets.twitchChannel);

            meow.loginBot().SafeFireAndForget();

            Console.WriteLine("Started up.");

            Console.WriteLine("Waking up...");

            await meow.sendMessage("*yawns* I have woken up:)");

            meow.OnMessage += async (sender, twitchChatMessage) =>
            {
                Console.WriteLine($"{twitchChatMessage.Sender} said {twitchChatMessage.Message}");

                foreach(commandClass cc in commands.commands)
                {
                    if (twitchChatMessage.Message.StartsWith($"!{cc.command}"))
                    {
                        string response = cc.response;
                        response = response.Replace("{Sender}", twitchChatMessage.Sender);
                        await meow.sendMessage(response);
                    }
                }
            };
            
            await Task.Delay(-1);

        }
    }
}
