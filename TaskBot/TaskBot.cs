using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TaskBot.Dialogs;

namespace TaskBot
{
    public class TaskBot : IBot
    {
        private readonly DialogSet dialogs;

        public TaskBot()
        {
            dialogs = new DialogSet();
            dialogs.Add("introDialog", IntroDialog.Instance);
            dialogs.Add("mainDialog", MainDialog.Instance);
        }
        
        public async Task OnTurn(ITurnContext context)
        {
            
            var state = context.GetConversationState<Dictionary<string, object>>();
            var dialogCon = dialogs.CreateContext(context, state);
            // var userstate = context.
            switch (context.Activity.Type)
            {

                case ActivityTypes.ConversationUpdate:
                    foreach(var newMember in context.Activity.MembersAdded)
                    {
                        if (newMember.Id != context.Activity.Recipient.Id)
                        {
                            await dialogCon.Begin("introDialog");
                            
                        }
                    }
                    break;
                case ActivityTypes.Message:
                    await dialogCon.Continue();
                    if(!context.Responded)
                {
                    await dialogCon.Begin("mainDialog");
                }
                    break;
            }
            

            /*
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var state = context.GetConversationState<Dictionary<string, object>>();
                var dialogCon = dialogs.CreateContext(context, state);

                await dialogCon.Continue();
                if(!context.Responded)
                {
                    await dialogCon.Begin("introDialog");
                }
            }
            */
        }
    }
}
