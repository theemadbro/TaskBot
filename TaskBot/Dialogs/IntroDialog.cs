using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using TaskBot.Dialogs;
using TaskBot.Dialogs.NewUser;

namespace TaskBot.Dialogs
{
    public class IntroDialog : DialogContainer
    {
        private IntroDialog() : base(Id)
        {

            Dialogs.Add(DialogId, new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
                    var activity = dc.Context.Activity;

                    await dc.Prompt("ChoicePrompt", $"This is TaskBot!{Environment.NewLine}Have you used this service before?",                    
                        new ChoicePromptOptions
                        {
                            Choices = new List<Choice>()
                            {
                                new Choice()
                                {
                                    Value = "Yes"
                                },
                                new Choice()
                                {
                                    Value = "No"
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
                    if (response == "Yes")
                    {
                        await dc.Context.SendActivity($"Then lets get tasking!");
                        await dc.Begin(MainDialog.Id);
                    }
                    else if (response == "No")
                    {
                        await dc.Begin(NewUserDialog.Id);
                    }
                },
                async (dc, args, next) =>
                {
                    await dc.Replace(Id);
                }
            });
            Dialogs.Add("ChoicePrompt", new ChoicePrompt("en"));
            Dialogs.Add(MainDialog.Id, MainDialog.Instance);
            Dialogs.Add(NewUserDialog.Id, NewUserDialog.Instance);
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
                        Title = "Yes",
                        Value = "Yes"
                    },
                    new CardAction()
                    {
                        Type = ActionTypes.ImBack,
                        Title = "No",
                        Value = "No"
                    }
                }
            }.ToAttachment();
        }

        public static string Id = "IntroDialog";

        public static IntroDialog Instance { get; } = new IntroDialog();
    }
}
