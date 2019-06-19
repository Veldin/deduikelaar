﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrint
{ 
    class QCClear : IQuickCommand
    {
        private string command { get; }
        private string description { get; }
        private string title { get; }
        private string helpCommand { get; }
        private Command commandClass;

        public QCClear(MainWindow mainWindow, Command commandClass)
        {
            this.commandClass = commandClass;

            command = "clear";
            helpCommand = "help clear";
            description = "This quick command clears the commandResponse";
            title = "clear";
        }

        /// <summary>
        /// This method execute closes the apps
        /// </summary>
        /// <returns>Returns whether is was executed succesfully</returns>
        public bool ExecuteMethod()
        {
            commandClass.ClearCommandResponse();
            return true;
        }

        /// <summary>
        /// This method returns the commmand that should be used to execute the command
        /// </summary>
        /// <returns>Returns the commmand that should be used to execute the command</returns>
        public string GetCommand()
        {
            return command;
        }

        /// <summary>
        /// This method returns a description of this QuickCommand 
        /// </summary>
        /// <returns>Returns a description of this QuickCommand</returns>
        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// This method returns the title of this QuickCommand
        /// </summary>
        /// <returns>Returns the title of this QuickCommand</returns>
        public string GetTitle()
        {
            return title;
        }

        /// <summary>
        /// This method returns the commmand that should be used to execute the helpCommand
        /// </summary>
        /// <returns>returns the commmand that should be used to execute the helpCommand</returns>
        public string GetHelpCommand()
        {
            return helpCommand;
        }

        /// <summary>
        /// This method executes the helpCommand which should inform the user about this command
        /// </summary>
        public void ExecuteHelpCommand()
        {
            commandClass.DisplayLine(description);
        }
    }
}
