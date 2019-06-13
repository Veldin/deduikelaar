using LogSystem;
using System.Collections.Generic;
using Newtonsoft.Json;
using FileReaderWriterSystem;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System;

namespace ApiParser
{
    public static class ApiParserFacade
    {
        private static Queue<ItemOrder> itemOrders = new Queue<ItemOrder>();                // A Queue that has the ItemOrder s in it which holds the order 
        private static List<Story> stories = new List<Story>();                             // A list that holds all the stories that can be shown
        private static List<Question> questions = new List<Question>();                     // A list that holds the questions and answers to ask the user
        private static List<FeedbackStatistic> statistics = new List<FeedbackStatistic>();  // A list that holds objects where the statisics can be calculated off

        public static void Init()
        {
            AddStatistics();

            // Try to get the data from the api
            Task saveItemOrdersAsync = Task.Run(async () => await SaveItemOrdersAsync());
            Task saveStoriesAsync = Task.Run(async () => await SaveStoriesAsync());
            Task saveQuestionsAsync = Task.Run(async () => await SaveQuestionsAsync());

            saveItemOrdersAsync.Wait();
            saveStoriesAsync.Wait();
            saveQuestionsAsync.Wait();

            // Fill all the static collections from the json files
            AddItemOrder();
            AddQuestion();
            AddStory();
        }


        private async static Task<string> CallApi(string url)
        {
            // Create a New HttpClient object and dispose it when done, so the app doesn't leak resources
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);

                    Log.Debug("Data from api call recieved");

                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    Log.Warning("Api call failed. " + url + " can not be found.");
                    return null;
                }
            }
        }

        public static async Task<bool> InformApiAsync()
        {
            // Get all files in the Items folder
            string[] files = FileReaderWriterFacade.CheckFolder(FileReaderWriterFacade.GetAppDataPath() + "Items\\");
            
            // Check if the folder has a Statistics.json file
            bool hasFile = false;
            foreach (string file in files)
            {
                if (file == "Statistics.json")
                {
                    hasFile = true;
                }
            }

            // If the file is not found return false
            if (!hasFile)
            {
                return false;
            }

            // Create a New HttpClient object and dispose it when done, so the app doesn't leak resources
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    StringContent content = new StringContent(FileReaderWriterFacade.ReadFile(FileReaderWriterFacade.GetAppDataPath() + "Items\\Statistics.json"), UnicodeEncoding.UTF8, "application/json");
                    
                    HttpResponseMessage response = await client.PostAsync("http://localhost:8000/api/v1/feedback", content);

                    string success = await response.Content.ReadAsStringAsync();

                    Log.Debug(success);

                    if (success.Contains("success"))
                    {
                        return true;
                    }

                    return false;
                }
                catch (HttpRequestException e)
                {
                    Log.Warning("Api call failed. http://localhost:8000/api/v1/feedback can not be found.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the next ItemOrder
        /// </summary>
        /// <param name="count">Don't use this argument. It's to prevent an infinite loop.</param>
        /// <returns>A ItemOrder which hold an storyId and a feedbackId</returns>
        public static ItemOrder NextItemOrder(int count = 0)
        {
            // Return null if this method has executed itself 10 times to prevent an infinite loop
            if (count > 2)
            {
                Log.Warning("The ItemOrder is missing.");
                return null;
            }

            // Check if the queue is empty
            if (itemOrders.Count > 0)
            {
                // Return the first item of the queue
                return itemOrders.Dequeue();
            }
            else
            {
                // Inform the programmer
                Log.Debug("Refill the ItemOrder queue.");

                // Refill the queue
                AddItemOrder();

                // Try again to pick the first item of the queue
                return NextItemOrder(++count);
            }
        }

        /// <summary>
        /// Get a story
        /// </summary>
        /// <param name="storyId">Give the storyId of the story</param>
        /// <returns>Get a story</returns>
        public static Story GetStory(int storyId)
        {
            // Try to find the story with a matching storyId
            foreach (Story story in stories)
            {
                // If the storyIds match return the story
                if (story.storyId == storyId)
                {
                    return story;
                }
            }

            // If there is no story found, return null
            return null;
        }

        /// <summary>
        /// Get a question
        /// </summary>
        /// <param name="feedbackId">Give the feedbackId of the question</param>
        /// <returns> </returns>
        public static Question GetQuestion(int feedbackId)
        {
            // Try to find the question with a matching questionId
            foreach (Question question in questions)
            {
                // If the questionId match return the question
                if (question.feedbackId == feedbackId)
                {
                    return question;
                }
            }

            // If there is no question found, return null
            return null;
        }

        /// <summary>
        /// This method adds the stories as objects in a list from the json file
        /// </summary>
        public static void AddStory()
        {
            // Clear the list to make sure it is empty
            stories.Clear();

            // Read the file with stories
            string json = FileReaderWriterFacade.ReadFile(FileReaderWriterFacade.GetAppDataPath() + "Items\\Stories.json");
            
            // IsEmpty check
            if (json == null || json.Length < 5)
            {
                Log.Warning("Stories.json is missing or empty");
                return;
            }

            // Convert the json file to the objects
            List<Story> val = JsonConvert.DeserializeObject<List<Story>>(json);

            // Add the stories to the list
            stories.AddRange(val);

            // Give the programmer feedback
            Log.Debug(stories.Count + " stories added");
        }

        /// <summary>
        /// This method adds the questions in a list from a json file
        /// </summary>
        public static void AddQuestion()
        {
            // Clear the list to make sure it is empty
            questions.Clear();

            // Read the file with questions
            string json = FileReaderWriterFacade.ReadFile(FileReaderWriterFacade.GetAppDataPath() + "Items\\Feedback.json");

            // IsEmpty check
            if (json == null || json.Length < 5)
            {
                Log.Warning("Feedback.json is missing or empty");
                return;
            }

            // Convert the json to Questions objects
            List<Question> val = JsonConvert.DeserializeObject<List<Question>>(json);

            // Add the new List to the static one
            questions.AddRange(val);

            // Give the programmer feedback
            Log.Debug(questions.Count + " question(s) added");
        }

        /// <summary>
        /// This method adds ItemOrders to a list from a json file
        /// </summary>
        public static void AddItemOrder()
        {
            lock (itemOrders)
            {
                // Clear the list to make sure it is empty
                itemOrders.Clear();

                // Read the json file with the itemorder
                string json = FileReaderWriterFacade.ReadFile(FileReaderWriterFacade.GetAppDataPath() + "Items\\ItemOrder.json");

                // IsEmpty check
                if (json == null || json.Length < 5)
                {
                    Log.Warning("ItemOrder.json is missing or empty");
                    return;
                }

                // Convert the json to ItemOrder objects
                Queue<ItemOrder> val = JsonConvert.DeserializeObject<Queue<ItemOrder>>(json);

                // Enqueue all itemOrders in the queue
                foreach (ItemOrder i in val)
                {
                    itemOrders.Enqueue(i);
                }

                // Give the programmer feedback
                Log.Debug(itemOrders.Count + " itemOrder(s) added");
            }
        }

        /// <summary>
        /// Add the statistics to the statistics list
        /// </summary>
        public static void AddStatistics()
        {
            // Clear the list to make sure it is empty
            statistics.Clear();

            // Read the json file with the itemorder
            string json = FileReaderWriterFacade.ReadFile(FileReaderWriterFacade.GetAppDataPath() + "Items\\Statistics.json");

            // IsEmpty check
            if (json == null || json.Length < 5)
            {
                Log.Warning("Statistics.json is missing or empty");
                return;
            }

            // Convert the json to ItemOrder objects
            List<FeedbackStatistic> val = JsonConvert.DeserializeObject<List<FeedbackStatistic>>(json);

            // Add the list from the json file to the static list
            statistics.AddRange(val);

            // Give the programmer feedback
            Log.Debug(statistics.Count + " statistics added");
        }

        /// <summary>
        /// Save the stories list in a json file
        /// </summary>
        public static async Task SaveStoriesAsync()
        {
            // Call the json text from the api
            string json = await CallApi("http://localhost:8000/api/v1/stories");

            // Null check
            if (json == null)
            {
                return;
            }

            //// Convert the stories list to json
            //json = JsonConvert.SerializeObject(stories);

            SaveStories(json);
            return;
        }

        /// <summary>
        /// Save a string in a json file.
        /// </summary>
        /// <param name="json">The json that is gonna be save in the json file</param>
        public static void SaveStories(string json)
        {
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, FileReaderWriterFacade.GetAppDataPath() + "Items\\Stories.json", false);

            // Give the programmer feedback
            Log.Debug("Stories saved");
        }

        /// <summary>
        /// Save the questions list in a json file
        /// </summary>
        public static async Task SaveQuestionsAsync()
        {
            // Call the json text from the api
            string json = await CallApi("http://localhost:8000/api/v1/feedback");

            // Null check
            if (json == null)
            {
                return;
            }

            //// Convert the questions list to json
            //json = JsonConvert.SerializeObject(questions);

            SaveQuestions(json);
            return;
        }

        /// <summary>
        /// Save a string in a json file.
        /// </summary>
        /// <param name="json">The json that is gonna be save in the json file</param>
        public static void SaveQuestions(string json)
        {
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, FileReaderWriterFacade.GetAppDataPath() + "Items\\Feedback.json", false);

            // Give the programmer feedback
            Log.Debug("Questions saved");
        }

        /// <summary>
        /// Save the itemorders queue in a json file
        /// </summary>
        public static async Task SaveItemOrdersAsync()
        {
            // Call the json text from the api
            string json = await CallApi("http://localhost:8000/api/v1/order");

            // Null check
            if (json == null)
            {
                Log.Warning("The results of the api call were null");
                return;
            }

            //// Convert the itemOrders list to json
            //json = JsonConvert.SerializeObject(itemOrders);

            SaveItemOrders(json);
            return;
        }

        /// <summary>
        /// Save a string in a json file.
        /// </summary>
        /// <param name="json">The json that is gonna be save in the json file</param>
        public static void SaveItemOrders(string json)
        {
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, FileReaderWriterFacade.GetAppDataPath() + "Items\\ItemOrder.json", false);

            // Give the programmer feedback
            Log.Debug("itemOrders saved");
        }

        /// <summary>
        /// Add a FeedbackStatistic to the json file
        /// </summary>
        /// <param name="storyId">The storyId of the story</param>
        /// <param name="answerId">The answerId of the answer</param>
        public static void SaveFeedbackStatistic(int storyId, int answerId)
        {
            // Create a FeebackStatistic object
            statistics.Add(new FeedbackStatistic(storyId, answerId));

            // Convert the statistics list to json
            string json = JsonConvert.SerializeObject(statistics);
            
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, FileReaderWriterFacade.GetAppDataPath() + "Items\\Statistics.json", false);

            // Give the programmer feedback
            Log.Debug("statistics saved");
        }

        /// <summary>
        /// This method returns whether the itemOrder queue is empty or not 
        /// </summary>
        /// <returns>Returns whether the itemOrder queue is empty or not </returns>
        public static bool IsItemOrdersEmpty()
        {
            if (itemOrders.Count == 0)
            {
                Log.Debug("ItemOrders is empty");
                return true;
            }

            Log.Debug("ItemOrders is not empty");
            return false;
        }

    }
}
