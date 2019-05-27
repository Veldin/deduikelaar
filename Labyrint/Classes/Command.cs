using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileReaderWriterSystem;
using LogSystem;

namespace Labyrint
{
    public class Command : ILogObserver
    {
        private bool active;                // If this bool is true commands can be typed
        private TextBox commandBar;         // The upper textbox where the commands are filled in
        private TextBox commandResponse;    // The lower textbox where the feedback is given
        private List<string> logClass;      // If filled only messages from those classes will be shown
        private MethodInfo methodInfo;      

        public Command(TextBox commandBar, TextBox commandResponse)
        {
            // Initiate attributes
            active = false;
            this.commandBar = commandBar;
            this.commandResponse = commandResponse;
            logClass = new List<string>();
            methodInfo = null;

            // Subscribe to the log
            Log.Subscribe(this);
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

        public void KeyPressed(string key)
        {
            switch (key)
            {
                case "Oem3":
                    SetActive();
                    break;
                case "Return":
                    if (methodInfo != null)
                    {
                        ExecuteMethodWithParameters(commandBar.Text);
                    }
                    else
                    {
                        ExecuteQuickCommand(commandBar.Text);
                        ExecuteMethod(commandBar.Text);
                    }

                    WriteInput(commandBar.Text);
                    ReadInput();
                    commandBar.Text = "";
                    break;

            }
        }

        public void AddLogClass(string className)
        {
            logClass.Add(className);
            Log.Debug(className + " added to the logFilter");
        }

        private void ExecuteQuickCommand(string text)
        {
            switch (text)
            {
                case "clear": 
                    commandResponse.Text = "";
                    break;
                case "quit":
                    ExecuteMethod("Labyrint.Labyrint.MainWindow.CloseApp");
                    //Type t = Type.GetType("Labyrint.MainWindow, Labyrint", true, true);
                    //MethodInfo method = t.GetMethod("CloseApp");
                    //method.Invoke(this, null);
                    break;
                case "who is a good boy?":
                    DisplayLine("Robin is good boy!");
                    break;
                case "logFilter.clear":
                    logClass.Clear();
                    Log.Debug("logFilter cleared");
                    break;
                case "logFilter.AddClass":
                    ExecuteMethod("Labyrint.Labyrint.Command.AddLogClass");                 
                    break;
            }
        }

        private void ExecuteMethod(string text)
        {
            // Split the entered text on .
            string[] wholeText = text.Split('.');

            // If the whole text consist of less than 4 parts it cannot execute a method
            if (wholeText.Length < 4)
            {
                return;
            }

            // Filter the class out of the text (everything except for the first and the last element)
            string classString = "";
            for (int i = 1; i < wholeText.Length - 1; i++)
            {
                if (i == wholeText.Length - 2)
                {
                    classString += wholeText[i] + ", ";
                }
                else
                {
                    classString += wholeText[i] + ".";
                }        
            }

            // Combine the class string with the assambly string
            string typeString = classString + wholeText[0];

            // Get the type of the class
            Type t = Type.GetType(typeString, false, true);

            // If t is null the type could not be found
            if (t == null)
            {
                DisplayLine("U fucked up");
                return;
            }
            
            // Get the method info
            string methodString = wholeText[wholeText.Length - 1];
            MethodInfo method = t.GetMethod(methodString);
            Log.Debug(methodString);
            Log.Debug(method);

            // Get the parameters of the method
            ParameterInfo[] parameters = method.GetParameters();

            // If there are no parameters exectute the method
            if (parameters.Length == 0)
            {               
                method.Invoke(this, null);
                Log.Debug("Method succesfully executed!");
                return;
            }

            // Save the methodInfo to execute the method later
            methodInfo = method;

            // Make a string to show which parameters are needed to execute this method
            string parametersString = "";
            for (int i = 0; i < parameters.Length ; i++)
            {
                if (i == wholeText.Length - 1)
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
                    methodInfo.Invoke(this, new object[] { parameter });
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
                    methodInfo.Invoke(this, new object[] { parameters });
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

        private void DisplayLine(string text)
        {
            //Run it in the UI thread
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                commandResponse.Text = text + "\n" + commandResponse.Text;
            });
        }

        private void WriteInput(string text)
        {
            // If stringg is not empty write it in a textFile
            if (text != "")
            {
                FileReaderWriterFacade.WriteText(new string[] { text }, FileReaderWriterFacade.GetAppDataPath() + "Log\\CommandBar.txt", true);
            }
        }

        private void ReadInput()
        {
            Log.Debug(FileReaderWriterFacade.ReadFile(FileReaderWriterFacade.GetAppDataPath() + "Log\\CommandBar.txt"));
        }
    }
}
