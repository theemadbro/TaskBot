using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Prompts;
using Microsoft.Recognizers.Text;
using DateTimeResult = Microsoft.Bot.Builder.Prompts.DateTimeResult;
using PromptStatus = Microsoft.Bot.Builder.Prompts.PromptStatus;

namespace TaskBot.Dialogs.NewUser
{
    public class NewUserDialog : DialogContainer
    {
        public NewUserDialog(): base(Id)
        {
            Dialogs.Add(Id, new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
                    await dc.Context.SendActivity($"Welcome! TaskBot is used for keeping track of upcoming activities, or whatever you need!");
                    await dc.Prompt("namePrompt", "Lets get you set up! What is your name?");
                },
                async (dc, args, next) =>
                {
                    var state = dc.Context.GetConversationState<Dictionary<string, object>>();
                    state["User"] = (string) args["Value"];
                    await dc.Context.SendActivity($"Great! Nice to meet you, {state["User"]}. Now all we need is to teach you the basics.");
                    await dc.Prompt("firstTitlePrompt", "We're going to walk you through making your first task! For simplicities sake, what's the first thing that comes to mind?");
                },
                async (dc, args, next) =>
                {
                    var state = dc.Context.GetConversationState<Dictionary<string, object>>();
                    state["FirstTitle"] = (string) args["Value"];
                    await dc.Prompt("firstDatePrompt", $"'{state["FirstTitle"]}' eh? That works! Now enter a time you'd like to be reminded of this task.");
                },
                async (dc, args, next) =>
                {
                    var state = dc.Context.GetConversationState<Dictionary<string, object>>();
                    dynamic test = args.First().Value;
                    dynamic test2 = test[0].Value;
                    //(new System.Collections.Generic.Mscorlib_CollectionDebugView<Microsoft.Bot.Builder.Prompts.DateTimeResult.DateTimeResolution>(test).Items[0]).Value
                    state["FirstDate"] = (string) test2;
                    
                    await dc.Context.SendActivity($"And there you have it!{Environment.NewLine}Title: {state["FirstTitle"]}{Environment.NewLine}Date: {state["FirstDate"]}");
                }
            });
            Dialogs.Add("namePrompt", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            Dialogs.Add("firstTitlePrompt", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            Dialogs.Add("firstDatePrompt", new Microsoft.Bot.Builder.Dialogs.DateTimePrompt(Culture.English, TimeValidator));
        }

        public static string Id = "NewUserDialog";
        public static NewUserDialog Instance { get; } = new NewUserDialog();

        private static async Task TimeValidator(ITurnContext context, DateTimeResult result)
        {
            if (result.Resolution.Count == 0)
            {
                await context.SendActivity("Sorry, I did not recognize the time that you entered.");
                result.Status = PromptStatus.NotRecognized;
            }

            // Find any recognized time that is not in the past.
            var now = DateTime.Now;
            DateTime time = default(DateTime);
            var resolution = result.Resolution.FirstOrDefault(
                res => DateTime.TryParse(res.Value, out time) && time > now);

            if (resolution != null)
            {
                // If found, keep only that result.
                result.Resolution.Clear();
                result.Resolution.Add(resolution);
            }
            else
            {
                // Otherwise, flag the input as out of range.
                await context.SendActivity("Please enter a time in the future, such as \"tomorrow at 9am\"");
                result.Status = PromptStatus.OutOfRange;
            }
        }
    
    }
}
