using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using TaskBot.Dialogs.NewUser;

namespace TaskBot.Dialogs
{
    public class MainDialog : DialogContainer
    {
        private MainDialog() : base(Id)
        {
            Dialogs.Add(DialogId, new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
                    var activity = dc.Context.Activity;

                    await dc.Prompt("choicePrompt", $"Are you a Returning User or a New User?",                    
                        new ChoicePromptOptions
                        {
                            Choices = new List<Choice>()
                            {
                                new Choice()
                                {
                                    Value = "Returning"
                                },
                                new Choice()
                                {
                                    Value = "New User"
                                }
                            }
                        });
                    await dc.Context.SendActivities(new Activity[]
                        {
                            sendMainCard(activity, mainCard())
                        });
                }, 
                async (dc, args, next) =>
                {
                    var response = (args["Value"] as FoundChoice)?.Value;
                    if (response == "Returning")
                    {
                        await dc.Context.SendActivity("This service is currently under construction. Sorry about that!");
                        //await dc.Begin(ReturningUserDialog.Id);
                    }
                    else if (response == "New User")
                    {
                        await dc.Begin(NewUserDialog.Id);
                    }
                },
                async (dc, args, next) =>
                {
                    await dc.Replace(Id);
                }
            });

           Dialogs.Add(NewUserDialog.Id, NewUserDialog.Instance);
           // Dialogs.Add(ReturningUserDialog.Id, ReturningUserDialog.Instance);
            Dialogs.Add("choicePrompt", new ChoicePrompt("en"));
        }
        
        private Activity sendMainCard(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }
        
        private Attachment mainCard()
        {
            return new HeroCard()
            {
                Buttons = new List<CardAction>()
                {
                    new CardAction()
                    {
                        Type = ActionTypes.ImBack,
                        Title = "Returning",
                        Value = "Returning"
                    },
                    new CardAction()
                    {
                        Type = ActionTypes.ImBack,
                        Title = "New User",
                        Value = "New User"
                    }
                }
            }.ToAttachment();
        }
            
        

        public static string Id = "mainDialog";

        public static MainDialog Instance { get; } = new MainDialog();

    }
}
