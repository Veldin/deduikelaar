using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileReaderWriterSystem;
using LogSystem;
using ApiParser;
using Settings;

namespace Labyrint
{
    public class Command : ILogObserver
    {
        private MainWindow engine;                                  // The engine class that creates this class
        private TextBox commandBar;                                 // The upper textbox where the commands are filled in
        private TextBox commandResponse;                            // The lower textbox where the feedback is given
        private bool active;                                        // If this bool is true commands can be typed
        private MethodInfo methodInfo;                              // This is the method info of a method whereby the parameters still needs to be given
        private List<string> commandHistory;                        // In this list are all the commands saved that are made
        private int historyIndex = -1;                              // This index shows the command that is displayed in the commandHistory. If no command in the commandHistory is displayed the value is -1
        private HashSet<IQuickCommand> quickCommands;               // This dictionary holds all the quickCommands

        public Command(MainWindow engine, TextBox commandBar, TextBox commandResponse)
        {
            // Initiate attributes
            this.engine = engine;
            this.commandBar = commandBar;
            this.commandResponse = commandResponse;
            active = false;
            methodInfo = null;
            quickCommands = new HashSet<IQuickCommand>();
            
            // Get the commandHistory and reverse it
            commandHistory = FileReaderWriterFacade.ReadLines(FileReaderWriterFacade.GetAppDataPath() + "Log\\CommandBar.txt");
            if (commandHistory != null)
            {
                commandHistory.Reverse();
            } else
            {
                commandHistory = new List<string>();
            }

            // Subscribe to the log
            Log.Subscribe(this);

            // Create the quick commands
            CreateQuickCommands();
        }

        /***************************************************************************
         * METHODS FORM INTERFACE OR ABSTRACT
         * ************************************************************************/
        #region mandatoryMethods

        /// <summary>
        /// This method displays the LogMessages in de commandResponse
        /// </summary>
        /// <param name="message">The logMessage from the Log</param>
        public void LogUpdate(LogMessage message)
        {
            // Check if the message is equal or above the given LogLevel in the ini file
            if ((int) message.GetLogLevel() >= SettingsFacade.Get("LogLevel", 2))
            {
                // If the LogLevel in the ini file is info or above, display only the LogLevel and the message.
                // Else the place where the message is called is added to the message
                if (SettingsFacade.Get("LogLevel", 2) > 1)
                {
                    DisplayLine(message.GetLogLevel().ToString() + ": " + message.GetMessage());
                } else
                {
                    // Write message into the commandResponse
                    string[] split = message.GetFilePath().Split('\\');
                    string className = split[split.Length - 1];

                    // Display the data of the LogMessage in the commandResponse
                    DisplayLine(className + " " + message.GetMemberName() + " Line: " + message.GetLineNumber() + " " + message.GetLogLevel().ToString() + ": " + message.GetMessage());
                }              
            }
        }

        #endregion

        /***************************************************************************
         * PUBLIC METHODS
         * ************************************************************************/
        #region publicMetods

        /// <summary>
        /// This method handles the incoming key input
        /// </summary>
        /// <param name="input">The key that has been pressed on the keyboard</param>
        public void KeyPressed(KeyEventArgs input)
        {
            // Get the character(s) of the key that has been pressed
            string key = input.Key.ToString();

            // If the key is a deadCharProcessed get the key
            if (key == "DeadCharProcessed")
            {
                key = input.DeadCharProcessedKey.ToString();
            }

            // In this switch case are the keys handled
            switch (key)
            {
                case "Escape":                // Esc key. Activate and deactivated the commandBar
                    SetActive();
                    break;
                case "Return":                  // Execute a command or enter a parameters
                    // Check if the commandBar is active
                    if (!active) { return; }

                    // If the methodInfo is not null the user is in the process of executing a method with parameters, continue this process
                    // Else execute the method if it is not a quickCommand
                    if (methodInfo != null)
                    {
                        ExecuteMethodWithParameters(commandBar.Text);
                    }
                    else
                    {
                        // Try to execute a QuickCommand, if it fails try to execute the text in the commandBar as method
                        if (!ExecuteQuickCommand(commandBar.Text))
                        {
                            ExecuteMethod(commandBar.Text);
                        }
                    }

                    // Save the typed text by writing the input in a list and in a file
                    WriteInput(commandBar.Text);

                    // Clear the commandBar
                    commandBar.Text = "";
                    break;
                case "Up":                  // Go through the command history
                    // Check if the commandBar is active and if the historyIndex is still in range of the CommandHistory list
                    if (active && historyIndex + 1 < commandHistory.Count)
                    {
                        // Increase the history index
                        historyIndex++;

                        // Set the command history in the commandBar
                        commandBar.Text = commandHistory[historyIndex];
                    }
                    break;
                case "Down":                // Go through the command history

                    // Check if the commandBar is active and if the historyIndex is still in range of the CommandHistory list
                    if (active && historyIndex - 1 > -1)
                    {
                        // Decrease the history index
                        historyIndex--;

                        // Set the command history in the commandBar
                        commandBar.Text = commandHistory[historyIndex];
                    }

                    // If the historyIndex is -1, clear the commandBar
                    if (historyIndex - 1 == -1)
                    {
                        commandBar.Text = "";
                    }
                    break;
                case "Tab":                 // Try to autofill the current text in the commandBar
                    AutoFill();
                    break;
            }

            // Keep the focus in the commandBar
            commandBar.SelectionStart = commandBar.Text.Length;
            commandBar.SelectionLength = 0;
            commandBar.Focus();
        }

        /// <summary>
        /// This method displays a line of text in the commandResponse
        /// </summary>
        /// <param name="text">The text that is gonna be displayed in the commandResponse</param>
        public void DisplayLine(string text)
        {
            // Try to invoke the UI thread
            try
            {
                //Run it in the UI thread
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    commandResponse.Text = text + "\n" + commandResponse.Text;
                });
            }
            catch
            {
                Log.Debug("Failed to invoke the Ui thread");
            }
           
        }

        /// <summary>
        /// This method clears the commandResponse
        /// </summary>
        public void ClearCommandResponse()
        {
            // Clear the commandResponse
            commandResponse.Text = "";
        }

        /// <summary>
        /// This method provides help with using the quickCommands
        /// </summary>
        public void Help()
        {
            // Loop through every quickCommand to display all quick commands
            foreach (IQuickCommand quickCommand in quickCommands)
            {
                // Display the command of the quick command
                DisplayLine(quickCommand.GetCommand());
            }

            // Give the user help for using the quickCommands
            DisplayLine("To get more information about the quick command type: help <command>");
            DisplayLine("Below are all the quick commands.");
        }

        #endregion

        /***************************************************************************
         * PRIVATE METHODS
         * ************************************************************************/
        #region privateMethods

        /// <summary>
        /// This method will check if there are QuickCommands that can be executed by the given text and executed them if possible
        /// </summary>
        /// <param name="text">A command of a QuickCommand</param>
        /// <returns>Returns a bool whether a QuickCommand is executed</returns>
        private bool ExecuteQuickCommand(string text)
        {
            // Loop through every quickCommand
            foreach (IQuickCommand quickCommand in quickCommands)
            {
                // Check of the text given is a command
                if (quickCommand.GetCommand() == text)
                {
                    // Exectute the method of the QuickCommand and return true
                    quickCommand.ExecuteMethod();
                    return true;
                }

                // Check if the text given is a help command
                if (quickCommand.GetHelpCommand() == text)
                {
                    // Execute the helpCommand and return true
                    quickCommand.ExecuteHelpCommand();
                    return true;
                }
            }

            // Return false because no quickCommand is executed
            return false;
        }

        /// <summary>
        /// This method tries to execute a method from another class.
        /// The text herefore needed needs to be written in the format below
        /// AssemblyName TypeName.MethodName
        /// 
        /// If the method has parameters, will the be needed when they are asked later
        /// </summary>
        /// <param name="text">The text necessary to execute a method</param>
        private void ExecuteMethod(string text)
        {
            // Check the assembly
            if (!ContainsAssembly(text))
            {
                DisplayLine("Assembly not found");
                return;
            }

            // Split the text on the space (seperating the assembly and the type/Class)
            string[] splittedText = text.Split(' ');

            // Split the text on on dot (seperating the type/class and the method
            string[] splittedText2 = splittedText[1].Split('.');

            // Check the type/class
            if (!ContainsType(splittedText[0], splittedText2[0]))
            {
                DisplayLine("Class not found");
                return;
            }

            // Get the Method
            MethodInfo method = GetMethod(splittedText[0], splittedText2[0], splittedText2[1]);


            // If method is null, return
            if (method == null)
            {
                DisplayLine("No method found");
                return;
            }

            // Get the parameters of the method
            ParameterInfo[] parameters = method.GetParameters();

            // If there are no parameters exectute the method
            if (parameters.Length == 0)
            {
                try
                {
                    // Invoke the method
                    var returnValue = method.Invoke(this, null);

                    // If there is a return type print it, else print execute conformation
                    if (returnValue != null)
                    {
                        DisplayLine(returnValue.ToString());
                    }
                    else
                    {
                        DisplayLine("Method succesfully executed!");
                    }
                    return;
                }
                catch
                {
                    // Give the user/programmer feedback
                    Log.Warning("Failed to execute method: " + method.Name);
                    return;
                }
            }

            // Save the methodInfo to execute the method later
            methodInfo = method;

            // Make a string to show which parameters are needed to execute this method
            string parametersString = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i == parameters.Length - 1)
                {
                    parametersString += parameters[i].ParameterType.ToString() + " " + parameters[i].Name;
                }
                else
                {
                    parametersString += parameters[i].ParameterType.ToString() + " " + parameters[i].Name + ", ";
                }
            }

            // Show which parameters are needed to execute this method
            DisplayLine(parametersString);
        }

        /// <summary>
        /// This method tries to execute a method with parameters
        /// </summary>
        /// <param name="text">This is a string with all the parameters seperated by a comma</param>
        private void ExecuteMethodWithParameters(string text)
        {
            // Get the parametersInfo from the methodInfo
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            // Check if it is one or multiple parameters
            if (parameterInfos.Length == 1)
            {
                // Convert the input to the necessary parameterTypes
                object parameter = Convert.ChangeType(text, parameterInfos[0].ParameterType);

                // Try to execute the method
                try
                {
                    // Invoke the method
                    var returnValue = methodInfo.Invoke(this, new object[] { parameter });

                    // If there is a return type print it, else print execute conformation
                    if (returnValue != null)
                    {
                        DisplayLine(returnValue.ToString());
                    }
                    else
                    {
                        DisplayLine("Method succesfully executed!");
                    }
                }
                catch
                {
                    // Give the programmer feedback
                    Log.Debug("Failed to execute " + methodInfo.Name);
                }

            }
            else
            {
                // Split the input by the comma's
                string[] parametersStrings = text.Split(',');

                // Create the parameters array
                object[] parameters = new object[parametersStrings.Length];

                // Loop through all parametersStrings to convert them to the necessary type and add them to the object array
                for (int i = 0; i < parametersStrings.Length; i++)
                {
                    object parameter = Convert.ChangeType(parametersStrings[i], parameterInfos[0].ParameterType);

                    parameters[i] = (parameter);

                }

                // Try to execute the method
                try
                {
                    // Invoke the method
                    var returnValue = methodInfo.Invoke(this, parameters);

                    // If there is a return type print it, else print execute conformation
                    if (returnValue != null)
                    {
                        DisplayLine(returnValue.ToString());
                    }
                    else
                    {
                        DisplayLine("Method succesfully executed!");
                    }
                }
                catch
                {
                    // Give the programmer feedback
                    Log.Debug("Failed to execute " + methodInfo.Name);
                }
            }

            // Set the methodInfo to null
            methodInfo = null;
        }

        /// <summary>
        /// Set the Command classs active or deactive based on the current state.
        /// Also change the visibility of the commandBar and commandResponse depanding on the active bool
        /// </summary>
        private void SetActive()
        {
            active = !active;

            if (!active)
            {
                commandBar.Visibility = Visibility.Hidden;
                commandResponse.Visibility = Visibility.Hidden;
            }
            else
            {
                commandBar.Visibility = Visibility.Visible;
                commandResponse.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// This method saves the entered text in a list and in an file in the appdata
        /// </summary>
        /// <param name="text">The entered text</param>
        private void WriteInput(string text)
        {
            // If stringg is not empty write it in a textFile
            if (text != "")
            {
                // Add it to the commandHistory
                commandHistory.Add(text);

                // Write the text in the CommandBar.txt file
                FileReaderWriterFacade.WriteText(new string[] { text }, FileReaderWriterFacade.GetAppDataPath() + "Log\\CommandBar.txt", true);
            }
        }

        /// <summary>
        /// This method is tries to autofill the text in the commandBar to an string that can execute a method
        /// </summary>
        private void AutoFill()
        {
            // Fix the assembly
            if (!ContainsAssembly(commandBar.Text))         // Check if the assembly name is already typed
            {
                FillAssembly(commandBar.Text);              // Try to fill the assembly name
                return;
            }

            // Split the text on the space
            string[] splittedText = commandBar.Text.Split(' ');

            // It may not be smaller than 2
            if (splittedText.Length < 2) { return; }

            // Split the second part of the splittedText  at the point
            string[] splittedText2 = splittedText[1].Split('.');

            // Check if the texts contains a type
            if (!ContainsType(splittedText[0], splittedText2[0]))
            {
                // Try to autofill the type and return
                FillType(splittedText[0], splittedText2[0]);
                return;
            }

            // It may not be smaller than 2
            if (splittedText2.Length < 2) { return; }

            // Try to autofill the mehod
            FillMethod(splittedText[0], splittedText2[0], splittedText2[1]);
        }

        /// <summary>
        /// This method tries to autofill the assembly. If the assembly name can be multiple assemblies, display them all in the commmandResponse
        /// </summary>
        /// <param name="text">The assemblyName</param>
        private void FillAssembly(string text)
        {
            // Get all assemblies in this solution
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            // Create a new empty list
            List<string> fillable = new List<string>();

            // Check which assemlbly names can be filled
            foreach (Assembly assembly in assemblies)
            {
                // Get the name of the assembly name
                string name = assembly.GetName().Name;

                // Check if the assembly name is long enhough to fill the text
                if (name.Length < text.Length)
                {
                    break;
                }

                // Check if the already typed text is equal to the beginning of the assembly name
                if (name.Substring(0, text.Length).Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    fillable.Add(name);
                }
            }    

            if (fillable.Count == 1)        // If there is only one fillable assembly name, fill it in de commandBar
            {
                commandBar.Text = commandBar.Text.Substring(0, commandBar.Text.Length - text.Length) + fillable[0] + " ";
            }
            else if (fillable.Count > 1)    // If there are multiple fillable assembly names, display the in the CommandResponse
            {
                foreach (string assembly in fillable)
                {
                    DisplayLine(assembly);
                }
            }
        }

        /// <summary>
        /// This method checks if the given assemblyName is indeed an assemblyName
        /// </summary>
        /// <param name="assemlbyName">A string of the assemblyName</param>
        /// <returns>Returns whether he given assemblyName is indeed an assemblyName</returns>
        private bool IsAssembly(string assemlbyName)
        {
            // Get all assemblies in this solution
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Loop through all assemblies
            foreach (Assembly assembly in assemblies)
            {
                // Get the name of the assembly name
                string name = assembly.GetName().Name;

                // Check if the already typed text is equal to the beginning of the assembly name
                if (name.Equals(assemlbyName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method checks whether an assembly name contains the given string
        /// </summary>
        /// <param name="text">The exact name of the assembly</param>
        /// <returns>Returns a bool whether an assembly name containts the given text</returns>
        private bool ContainsAssembly(string text)
        {
            // Get all assemblies in this solution
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Loop through all assemblies
            foreach (Assembly assembly in assemblies)
            {
                // Get the name of the assembly name
                string name = assembly.GetName().Name;

                // Check if the already typed text is equal to the beginning of the assembly name
                if (text.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method tries to autofill the type. If the typeName can be multiple types, display them all in the commmandResponse
        /// </summary>
        /// <param name="assemblyName">The exect name of the assembly where the type is in</param>
        /// <param name="typeName">The text where a string of the type suppose to be is</param>
        private void FillType(string assemblyName, string typeName)
        {
            // Load the typed assembly
            Assembly assembly = Assembly.Load(assemblyName);

            // Get all types of the assembly
            Type[] types = assembly.GetTypes();

            // Create a new empty list
            List<string> fillable = new List<string>();

            // Check which assemlbly names can be filled
            foreach (Type type in types)
            {
                // Get the name of the assembly name
                string name = type.Name;

                // Check if the assembly name is long enhough to fill the text
                if (name.Length < typeName.Length)
                {
                    break;
                }

                // Check if the already typed text is equal to the beginning of the assembly name
                if (name.Substring(0, typeName.Length).Equals(typeName, StringComparison.OrdinalIgnoreCase))
                {
                    fillable.Add(name);
                }
            }

            if (fillable.Count == 1)        // If there is only one fillable assembly name, fill it in de commandBar
            {
                commandBar.Text = commandBar.Text.Substring(0, commandBar.Text.Length - typeName.Length) + fillable[0] + ".";
            }
            else if (fillable.Count > 1)    // If there are multiple fillable assembly names, display the in the CommandResponse
            {
                foreach (string type in fillable)
                {
                    DisplayLine(type);
                }
            }
        }

        /// <summary>
        /// This method checks if the string that should be a type a type is
        /// </summary>
        /// <param name="assemblyName">The exect name of the assembly where the type is in</param>
        /// <param name="typeName">The text where a string of the type suppose to be is</param>
        /// <returns>Returns whether the typeName is indeed a type</returns>
        private bool ContainsType(string assemblyName, string typeName)
        {
            // Load the typed assembly
            Assembly assembly = Assembly.Load(assemblyName);

            Type[] types = assembly.GetTypes();

            // Loop through all assemblies
            foreach (Type type in types)
            {
                // Get the name of the assembly name
                string name = type.Name;

                // Check if the already typed text is equal to the beginning of the assembly name
                if (typeName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method tries to auto fill the method name in the commandBar or if there are multiple options avalible. Display them in the commandResponse
        /// </summary>
        /// <param name="assemblyName">>A string of the exact name of the assembly where the method is in</param>
        /// <param name="typeName">The exect name of the type where the method is in</param>
        /// <param name="methodName">The text that is typed where the method is suppose to be</param>
        private void FillMethod(string assemblyName, string typeName, string methodName)
        {
            // Difine a type
            Type type = null;

            // Try to load the type
            try
            {
                // Load the assembly
                Assembly assembly = Assembly.Load(assemblyName);

                // Loop through all the Types in the assembly
                foreach(Type tempType in assembly.GetTypes())
                {
                    // Check if it is the correct type
                    if (tempType.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)){
                        type = tempType;
                    }
                }

                if(type == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                DisplayLine("assembly or type is not valid");
                return;
            }

            // Get the methods
            MethodInfo[] methods = type.GetMethods();

            // Create a new empty list
            List<string> fillable = new List<string>();

            // Check which assemlbly names can be filled
            foreach (MethodInfo method in methods)
            {
                // Get the name of the assembly name
                string name = method.Name;

                // Check if the assembly name is long enhough to fill the text
                if (name.Length < methodName.Length)
                {
                    break;
                }

                // Check if the already typed text is equal to the beginning of the assembly name
                if (name.Substring(0, methodName.Length).Equals(methodName, StringComparison.OrdinalIgnoreCase))
                {
                    fillable.Add(name);
                }
            }

            if (fillable.Count == 1)        // If there is only one fillable assembly name, fill it in de commandBar
            {
                commandBar.Text = commandBar.Text.Substring(0, commandBar.Text.Length - methodName.Length) + fillable[0];
            }
            else if (fillable.Count > 1)    // If there are multiple fillable assembly names, display the in the CommandResponse
            {
                foreach (string method in fillable)
                {
                    DisplayLine(method);
                }
            }
        }

        /// <summary>
        /// This method returns the methodInfo of a method
        /// </summary>
        /// <param name="assemblyName">A string of the exact name of the assembly where the method is in</param>
        /// <param name="typeName">The exect name of the type where the method is in</param>
        /// <param name="methodName">The exact name of the method</param>
        /// <returns>The methodInfo of the method</returns>
        private MethodInfo GetMethod(string assemblyName, string typeName, string methodName)
        {
            try
            {
                // Difine a type
                Type type = null;

                // Load the assembly
                Assembly assembly = Assembly.Load(assemblyName);

                // Loop through all the Types in the assembly
                foreach (Type tempType in assembly.GetTypes())
                {
                    // Check if it is the correct type
                    if (tempType.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    {
                        type = tempType;
                        break;
                    }
                }

                if (type == null)
                {
                    throw new Exception();
                }

                // Loop through all the methods in the type
                foreach (MethodInfo method in type.GetMethods())
                {
                    // Check if it is the correct type
                    if (method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                    {
                        return method;
                    }
                }

                // The method is not found, so throw an exception
                throw new Exception();
            }
            catch
            {
                DisplayLine("assembly, type or method is not valid");
                return null;
            }
        }

        /// <summary>
        /// This method creates all the QuickCommand classes and sets them in the quickCommand list
        /// </summary>
        private void CreateQuickCommands()
        {
            // Load the Laybrint assembly
            Assembly labyrint = Assembly.Load("Labyrint");
            
            // Get all types from the labyrint assembly
            Type[] types = labyrint.GetTypes();

            // Create an object array with the parameters for the QuickCommands
            object[] parameters = new object[] { engine, this };

            // Loop through every type in the labyrint assembly
            foreach(Type type in types)
            {
                // If the type does not has the IQuickCommand interface go to the next type
                if (type.GetInterface("IQuickCommand") == null)
                {                    
                    continue;
                }

                // Create een instance of the type
                IQuickCommand instance = (IQuickCommand)Activator.CreateInstance(type, parameters);

                // If the instance is not null add it to de quickcommands dictionary and give the programmer feedback
                // Else give the programmer feedback of the failure
                if (instance != null)
                {
                    quickCommands.Add(instance);
                    Log.Debug(type + " is created");
                }
                else
                {
                    Log.Warning("Failed to create: " + type);
                }
            }
        }

        #endregion
    }
}
