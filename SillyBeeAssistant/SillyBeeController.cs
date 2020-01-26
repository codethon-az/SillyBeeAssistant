using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SillyBeeAssistant
{
    [Route("api/[controller]")]
    [ApiController]
    public class SillyBeeController : ControllerBase
    {
        private const string SB_API = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/defb523c-ce23-4313-8271-962c13f04182?verbose=true&timezoneOffset=0&subscription-key=ecea6b7640444386b2b041c3601e187e&q=";
        private List<string> q = new List<string>(){
            "Do you like the house?",
            "Would you like to buy the house?",
            "Would you like to search for another house?",
            "Do you like the deal on the house?",
            "Do you have funds in other accounts that you would like to bring in?",
            "Would you like to talk to an advisor?",
            "Would you like to upload the missing documents?"
        };
        private List<string> a = new List<string>()
        {
            "search again", "add account", "check application", "advisor call"
        };
        private const string UNSURE = "not sure what you typed. could you please rephrase?";
        private const string END = "Thanks for chatting... have a nice day";
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

        private bool appChecked = false;
        private bool accAdded = false;

        [HttpGet]
        public string Get()
        {
            return "Silly Bee assistant is healthy";
        }

        // GET: api/SillyBee/query
        [Route("{query}")]
        public string Get(string query)
        {
            var response = new SillyBeeResponse();
            var intent = GetIntent(query);
            var nextAction = GetNextAction(query, intent);
            if (nextAction == END)
            {
                response.command = END;
                response.commandType = "action";
                return JsonConvert.SerializeObject(response);
            }
            if (q.Contains(nextAction))
            {
                response.command = nextAction;
                response.commandType = "query";
                return JsonConvert.SerializeObject(response);
            }
            if (a.Contains(nextAction))
            {
                response.commandType = "query";
                if (nextAction == a[1])
                {
                    response.command = "Congratulations! Your account has been added and your AUM has increased by 15%. We are able to offer you a better rate - do you like the new deal?";
                    return JsonConvert.SerializeObject(response);
                }
                if (nextAction == a[2])
                {
                    response.command = "Congratulations! Your documents have been collected and your application has been submitted?";
                    return JsonConvert.SerializeObject(response);
                }
                if (nextAction == a[2])
                {
                    response.command = "Great choice! You will now be redirected to our search page!";
                    return JsonConvert.SerializeObject(response);
                }
                if (nextAction == a[3])
                {
                    response.command = "Calling the advisor!";
                    return JsonConvert.SerializeObject(response);
                }
            }
            response.command = UNSURE;
            response.commandType = "query";
            return JsonConvert.SerializeObject(response);
        }


        private string GetIntent(string query)
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
                        intent = UNSURE;
                }
                return intent;
            }
        }

        private string GetNextAction(string query, string intent)
        {
            if (query == q[5])
            {
                if (intent == CONF || intent == THINK || intent == ESC || intent == HELP || intent == ADD)
                    return a[3];
                if (intent == REJ)
                    return END;
            }
            if (intent == ESC || intent == HELP)
                return q[5];
            if (query == q[2])
            {
                if (intent == CONF || intent == THINK || intent == SNAP || intent == RESTART)
                    return a[0];
                if (intent == REJ || intent == NO_LIKE || intent == NO_BUY)
                    return q[5];
            }
            if (intent == SNAP || intent == RESTART)
                return q[2];

            if (query == q[0])
            {

                if (intent == CONF || intent == LIKE)
                    return q[1];
                if (intent == REJ || intent == NO_LIKE || intent == THINK || intent == NO_BUY)
                    return q[2];
                if (intent == BUY)
                    return q[3];
                if (intent == NO_HAPPY)
                    return q[4];
            }
            if (query == q[1])
            {
                if (intent == CONF || intent == LIKE || intent == BUY)
                    return q[3];
                if (intent == REJ || intent == NO_LIKE || intent == THINK || intent == NO_BUY)
                    return q[2];
                if (intent == NO_HAPPY)
                    return q[4];
            }
            if (query == q[3])
            {
                if (intent == CONF || intent == THINK || intent == HAPPY || intent == LIKE || intent == BUY)
                    return a[2];
                if (intent == REJ || intent == NO_HAPPY)
                    return q[4];
                if (intent == NO_LIKE || intent == NO_BUY)
                    return q[5];
            }
            if (query == q[4])
            {
                if (intent == CONF || intent == THINK)
                    return a[1];
                if (intent == REJ || intent == NO_HAPPY)
                    return q[5];
            }
            if (query == q[6])
            {
                if (intent == CONF || intent == THINK || intent == LIKE || intent == BUY || intent == HAPPY)
                    return a[2];
                if (intent == REJ || intent == NO_LIKE || intent == NO_BUY || intent == NO_HAPPY)
                    return a[3];
            }
            if (intent == NONE)
                return UNSURE;
            return UNSURE;
        }
    }
}
