using LogSystem;
using System.Collections.Generic;
using Newtonsoft.Json;
using FileReaderWriterSystem;

namespace ApiParser
{
    public static class ApiParserFacade
    {
        private static Queue<ItemOrder> itemOrders = new Queue<ItemOrder>();            // A Queue that has the ItemOrder s in it which holds the order 
        private static List<Story> stories = new List<Story>();                         // A list that holds all the stories that can be shown
        private static List<Question> questions = new List<Question>();                 // A list that holds the questions and answers to ask the user

        /// <summary>
        /// Get the next ItemOrder
        /// </summary>
        /// <param name="count">Don't use this argument. It's to prevent an infinite loop.</param>
        /// <returns>A ItemOrder which hold an storyId and a feedbackId</returns>
        public static ItemOrder NextItemOrder(int count = 0)
        {
            // Return null if this method has executed itself 10 times to prevent an infinite loop
            if (count > 9)
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
                return NextItemOrder(count++);
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
            string json = FileReaderWriterFacade.ReadFile("Items\\Stories.json");
            
            // IsEmpty check
            if (json == null)
            {
                Log.Warning("Stories.json is missing or empty");
                return;
            }

            // Convert the json file to the objects
            List<Story> val = JsonConvert.DeserializeObject<List<Story>>(json);

            // Add the stories to the list
            stories.AddRange(val);

            // Give the programmer feedback
            Log.Debug("Stories added");
        }

        /// <summary>
        /// This method adds the questions in a list from a json file
        /// </summary>
        public static void AddQuestion()
        {
            // Clear the list to make sure it is empty
            questions.Clear();

            // Read the file with questions
            string json = FileReaderWriterFacade.ReadFile("Items\\Feedback.json");

            // IsEmpty check
            if (json == null)
            {
                Log.Warning("Stories.json is missing or empty");
                return;
            }

            // Convert the json to Questions objects
            List<Question> val = JsonConvert.DeserializeObject<List<Question>>(json);

            // Add the new List to the static one
            questions.AddRange(val);

            // Give the programmer feedback
            Log.Debug("Questions added");
        }

        /// <summary>
        /// This method adds ItemOrders to a list from a json file
        /// </summary>
        public static void AddItemOrder()
        {
            // Clear the list to make sure it is empty
            itemOrders.Clear();

            // Read the json file with the itemorder
            string json = FileReaderWriterFacade.ReadFile("Items\\ItemOrder.json");

            // IsEmpty check
            if (json == null)
            {
                Log.Warning("Stories.json is missing or empty");
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
            Log.Debug("ItemOrders added");
        }

        /// <summary>
        /// Save the stories list in a json file
        /// </summary>
        public static void SaveStories()
        {
            // Convert the stories list to json
            string json = JsonConvert.SerializeObject(stories);

            SaveStories(json);
        }

        /// <summary>
        /// Save a string in a json file.
        /// </summary>
        /// <param name="json">The json that is gonna be save in the json file</param>
        public static void SaveStories(string json)
        {
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, "Items\\Stories.json", false);

            // Give the programmer feedback
            Log.Debug("Stories saved");
        }

        /// <summary>
        /// Save the questions list in a json file
        /// </summary>
        public static void SaveQuestions()
        {
            // Convert the questions list to json
            string json = JsonConvert.SerializeObject(questions);

            SaveQuestions(json);
        }

        /// <summary>
        /// Save a string in a json file.
        /// </summary>
        /// <param name="json">The json that is gonna be save in the json file</param>
        public static void SaveQuestions(string json)
        {
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, "Items\\Feedback.json", false);

            // Give the programmer feedback
            Log.Debug("Questions saved");
        }

        /// <summary>
        /// Save the itemorders queue in a json file
        /// </summary>
        public static void SaveItemOrders()
        {
            // Convert the itemOrders list to json
            string json = JsonConvert.SerializeObject(itemOrders);

            SaveItemOrders(json);
        }

        /// <summary>
        /// Save a string in a json file.
        /// </summary>
        /// <param name="json">The json that is gonna be save in the json file</param>
        public static void SaveItemOrders(string json)
        {
            // Write the json in a json file
            FileReaderWriterFacade.WriteText(new string[] { json }, "Items\\ItemOrder.json", false);

            // Give the programmer feedback
            Log.Debug("itemOrders saved");
        }


    }
}
