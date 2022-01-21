using System;
using System.Collections.Generic;
using System.IO;

namespace imthecatbot
{
    public class catCommands
    {
        public List<commandClass> commands = new List<commandClass>();

        public catCommands()
        {
            loadCommands();
        }

        public string listCommands()
        {
            string cmdList = "";
            foreach(commandClass cc in commands)
            {
                cmdList += $"!{cc.command} - {cc.response}";
            }
            return cmdList;
        }
        

        void loadCommands()
        {
            string[] files = Directory.GetFiles(@"data/commands", "*.catCommand", SearchOption.AllDirectories);

            for(int i = 0; i < files.Length; i++)
            {
                string[] fileContent = File.ReadAllLines(files[i]);

                commandClass command = new commandClass();
                command.command = fileContent[0];
                command.response = fileContent[1];

                commands.Add(command);
                Console.WriteLine($"Loaded !{command.command}");
            }
        }
    }
}
