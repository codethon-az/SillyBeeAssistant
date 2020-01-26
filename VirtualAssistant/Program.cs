using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using static VirtualAssistant.SillyBeeLuis;

namespace VirtualAssistant
{
    class Program
    {
        private const string SB_API = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/defb523c-ce23-4313-8271-962c13f04182?verbose=true&timezoneOffset=0&subscription-key=ecea6b7640444386b2b041c3601e187e&q=";
        private static List<string> q = new List<string>(){ 
            "Do you like the house?", 
            "Would you like to buy the house?", 
            "Would you like to search for another house?",
            "Do you like the deal on the house?",
            "Do you have funds in other accounts that you would like to bring in?", 
            "Would you like to talk to an advisor?", 
            "Would you like to upload the missing documents?" 
        };
        private static List<string> a = new List<string>()
        {
            "search again", "add account", "check application", "advisor call"
        };
        private static string UNSURE = "not sure what you typed. could you please rephrase?";
        private static string END = "Thanks for chatting... have a nice day";
        private const string CONF = "Utilities.Confirm";
        private const string ADD = "Add account or document";
        private const string BUY = "Buy It";
        private const string HAPPY = "Happy with Finance";
        private const string LIKE = "Like It";
        private const string NONE = "None";
        private const string NO_BUY = "Not buy it";
        private const string NO_HAPPY = "Not happy with finance";
        private const string NO_LIKE = "Not like it";
        private const string SNAP = "Snap It";
        private const string RESTART = "Utilities.StartOver";
        private const string HELP = "Utilities.Help";
        private const string THINK = "Thinking";
        private const string ESC = "Utilities.Escalate";
        private const string REJ = "Utilities.Reject";

        private static bool appChecked = false;
        private static bool accAdded = false;
        static void Main(string[] args)
        {
            var cq = q[0];
            var response = GetNextAction(cq, GetResponse(cq));
            while (response != END)
            {
                if (q.Contains(response))
                {
                    cq = response;
                    response = GetNextAction(cq, GetResponse(cq));
                }
                else if (a.Contains(response))
                {
                    if (response == a[0]) { Console.WriteLine("Great choice! You will now be redirected to our search page!"); break; }
                    else if (response == a[1])
                    {
                        Console.WriteLine("Congratulations! Your account has been added and your AUM has increased by 15%. We are able to offer you a better rate - do you like the new deal?");
                        cq = q[3];
                    }
                    else if (response == a[2])
                    {
                        if (appChecked) { Console.WriteLine("Congratulations! Your application has been submitted."); break; }
                        else { Console.WriteLine("Checking your application."); cq = q[6]; appChecked = true; }
                    }
                    else if (response == a[3])
                    {
                        Console.WriteLine("Calling the advisor.");
                        break;
                    }
                    response = GetNextAction(cq, GetResponse(cq));
                }
                else if (response == UNSURE)
                {
                    response = GetNextAction(cq, GetResponse(response));
                }
            }
            Console.WriteLine(END);
        }

        private static string QueryChatBot(string query)
        {
            string intent = "";
            using (var httpClient = new HttpClient())
            {
                using (var res = httpClient.GetAsync(SB_API + query))
                {
                    var apiResponse = res.Result.Content.ReadAsStringAsync().Result;
                    if (res.Result.IsSuccessStatusCode)
                    {
                        var jsonResponse = JObject.Parse(apiResponse);
                        intent = jsonResponse.SelectToken("intents[0].intent").ToString();
                    }
                    else
                        intent = "unclear";
                }
                return intent;
            }
        }
        
        private static string GetResponse(string query)
        {
            Console.WriteLine(query);
            return QueryChatBot(Console.ReadLine());
        }

        private static string GetNextAction(string query, string response)
        {
            if (query == q[5])
            {
                if (response == CONF || response == THINK || response == ESC || response == HELP || response == ADD)
                    return a[3];
                if (response == REJ)
                    return END;
            }
            if (response == ESC || response == HELP)
                return q[5];
            if (query == q[2])
            {
                if (response == CONF || response == THINK || response == SNAP || response == RESTART)
                    return a[0];
                if (response == REJ || response == NO_LIKE || response == NO_BUY)
                    return q[5];
            }
            if (response == SNAP || response == RESTART)
                return q[2];

            if (query == q[0])
            {

                if (response == CONF || response == LIKE)
                    return q[1];
                if (response == REJ || response == NO_LIKE || response == THINK || response == NO_BUY)
                    return q[2];
                if (response == BUY)
                    return q[3];
                if (response == NO_HAPPY)
                    return q[4];
            }
            if (query == q[1])
            {
                if (response == CONF || response == LIKE || response == BUY)
                    return q[3];
                if (response == REJ || response == NO_LIKE || response == THINK || response == NO_BUY)
                    return q[2];
                if (response == NO_HAPPY)
                    return q[4];
            }
            if (query == q[3])
            {
                if (response == CONF || response == THINK || response == HAPPY || response == LIKE || response == BUY)
                    return a[2];
                if (response == REJ || response == NO_HAPPY)
                    return q[4];
                if (response == NO_LIKE || response == NO_BUY)
                    return q[5];
            }
            if (query == q[4])
            {
                if (response == CONF || response == THINK)
                    return a[1];
                if (response == REJ || response == NO_HAPPY)
                    return q[5];
            }
            if (query == q[6])
            {
                if (response == CONF || response == THINK || response == LIKE || response == BUY || response == HAPPY)
                    return a[2];
                if (response == REJ || response == NO_LIKE || response == NO_BUY || response == NO_HAPPY)
                    return a[3];
            }
            if (response == NONE)
                return UNSURE;
            return UNSURE;
        }
    }
}
