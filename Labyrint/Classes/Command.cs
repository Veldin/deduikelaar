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
        private List<string> logClass;                              // If filled only messages from those classes will be shown
        private MethodInfo methodInfo;                              // This is the method info of a method whereby the parameters still needs to be given
        private List<string> commandHistory;                        // In this list are all the commands saved that are made
        private int historyIndex = -1;                              // This index shows the command that is displayed in the commandHistory. If no command in the commandHistory is displayed the value is -1
        private Dictionary<string, IQuickCommand> quickCommands;    // This dictionary holds all the quickCommands

        public Command(MainWindow engine, TextBox commandBar, TextBox commandResponse)
        {
            // Initiate attributes
            this.engine = engine;
            this.commandBar = commandBar;
            this.commandResponse = commandResponse;
            active = false;
            logClass = new List<string>();
            methodInfo = null;
            quickCommands = new Dictionary<string, IQuickCommand>();
            
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

        public void LogUpdate(LogMessage message)
        {
            // Write message into the commandResponse
            string[] split = message.GetFilePath().Split('\\');
            string className = split[split.Length - 1];

            if (logClass.Contains(className) || logClass.Count == 0)
            {
                DisplayLine(className + " " + message.GetMemberName() + " Line: " + message.GetLineNumber() + " " + message.GetLogLevel().ToString() + ": " + message.GetMessage());
            }
        }

        public void KeyPressed(KeyEventArgs input)
        {
            string key = input.Key.ToString();

            if (key == "DeadCharProcessed")
            {
                key = input.DeadCharProcessedKey.ToString();
            }

            switch (key)
            {
                case "Escape":                // ` key. Activate and deactivated the commandBar
                    SetActive();
                    break;
                case "Return":              // Execute a command or enter a parameters
                    if (!active) { return; }
                    if (methodInfo != null)
                    {
                        ExecuteMethodWithParameters(commandBar.Text);
                    }
                    else
                    {
                        if (!ExecuteQuickCommand(commandBar.Text))
                        {
                            ExecuteMethod(commandBar.Text);
                        }
                    }

                    WriteInput(commandBar.Text);
                    commandBar.Text = "";
                    break;
                case "Up":                  // Go through the command history
                    if (active && historyIndex +1 < commandHistory.Count)
                    {
                        historyIndex++;
                        commandBar.Text = commandHistory[historyIndex];
                    }
                    break;
                case "Down":                // Go through the command history
                    if (active && historyIndex -1 > -1)
                    {
                        historyIndex--;
                        commandBar.Text = commandHistory[historyIndex];
                    }
                    if (historyIndex -1 == -1)
                    {
                        commandBar.Text = "";
                    }
                    break;
                case "Tab":
                    AutoFill();
                    break;
            }

            // Keep the focus in the commandBar
            commandBar.SelectionStart = commandBar.Text.Length;
            commandBar.SelectionLength = 0;
            commandBar.Focus();
        }

        public void AddLogClass(string className)
        {
            logClass.Add(className);
            Log.Debug(className + " added to the logFilter");
        }

        private bool ExecuteQuickCommand(string text)
        {
            if (quickCommands.ContainsKey(text))
            {
                quickCommands[text].ExecuteMethod();
            }

            switch (text)
            {
                case "clear": 
                    commandResponse.Text = "";
                    break;
                case "qqq":
                    //engine.CloseApp();
                    //ExecuteMethod("Labyrint MainWindow.CloseApp");
                    break;
                case "resetControls":
                    engine.ResetControls();
                    break;
                case "who is a good boy?":
                    DisplayLine("Robin is good boy!");
                    break;
                case "SettingsFacade.Save":
                    ExecuteMethod("Settings SettingsFacade.Save");
                    DisplayLine("Settings saved to the .ini file.");
                    break;
                case "logFilter.clear":
                    logClass.Clear();
                    Log.Debug("logFilter cleared");
                    break;
                case "logFilter.AddClass":
                    ExecuteMethod("Labyrint Command.AddLogClass");                 
                    break;
                case "collision":
                    engine.ActivateCollision();
                    break;
                default:
                    return false;
            }

            return true;
        }

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
                    } else
                    {
                        DisplayLine("Method succesfully executed!");
                    }
                    return;
                }
                catch
                {
                    Log.Warning("Failed to execute method: " + method.Name);
                    return;
                }
            }

            // Save the methodInfo to execute the method later
            methodInfo = method;

            // Make a string to show which parameters are needed to execute this method
            string parametersString = "";
            for (int i = 0; i < parameters.Length ; i++)
            {
                if (i == parameters.Length - 1)
                {
                    parametersString += parameters[i].ParameterType.ToString() + " " + parameters[i].Name;
                }
                else
                {
                    parametersString += parameters[i].ParameterType.ToString() + " " +  parameters[i].Name + ", ";
                }
            }

            // Show which parameters are needed to execute this method
            DisplayLine(parametersString);
        }

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
                    Log.Debug("Failed to execute " + methodInfo.Name);
                }
                    
            } else
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
                    Log.Debug("Failed to execute " + methodInfo.Name);
                }
            }

            // Set the methodInfo to null
            methodInfo = null;
        }

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

        private void WriteInput(string text)
        {
            // If stringg is not empty write it in a textFile
            if (text != "")
            {
                // Add it to the commandHistory
                commandHistory.Add(text);

                FileReaderWriterFacade.WriteText(new string[] { text }, FileReaderWriterFacade.GetAppDataPath() + "Log\\CommandBar.txt", true);
            }
        }

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

            string[] splittedText2 = splittedText[1].Split('.');

            if (!ContainsType(splittedText[0], splittedText2[0]))
            {
                FillType(splittedText[0], splittedText2[0]);
                return;
            }

            // It may not be smaller than 2
            if (splittedText2.Length < 2) { return; }

            FillMethod(splittedText[0], splittedText2[0], splittedText2[1]);
        }

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

        private void CreateQuickCommands()
        {
            Assembly QCs = Assembly.Load("Labyrint");
            
            Type[] types = QCs.GetTypes();

            object[] parameters = new object[] { engine, this };

            foreach(Type type in types)
            {
                if (type.ToString().Substring(9,2) != "QC")
                {                    
                    continue;
                }

                IQuickCommand instance = (IQuickCommand)Activator.CreateInstance(type, parameters);

                if (instance != null)
                {
                    quickCommands.Add(instance.GetCommand(), instance);
                    Log.Debug(type + " is created");
                }
                else
                {
                    Log.Warning("Failed to create: " + type);
                }
            }
        }
    }
}
