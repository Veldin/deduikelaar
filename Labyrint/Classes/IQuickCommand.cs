using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrint
{
    public interface IQuickCommand
    {
        // This method should return the command. This is the text that is used to execute this command
        string GetCommand();

        // This method should return the command that will display the description of this QuickCommand in the commandResponse
        string GetHelpCommand();

        // This method should return a one word description about the command 
        string GetTitle();

        // This method should return a small description about what command does
        // This should be set in the commandResponse if the helpCommand is called
        string GetDescription();

        // This method should execute the command. It also needs to return a bool whether it was succesfull or not.
        bool ExecuteMethod();

        // This method should execute the help command
        // This is most often displaying the description of the QuickCommand in the commandResponse of the CommandClass
        void ExecuteHelpCommand();
    }
}
