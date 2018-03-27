using BIADTemplate.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs
{
    [Serializable]
    public class ProfileFormDialog: IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            await context.Forward(FormDialog.FromForm(BuildProfileForm), ResumeAfter_ProfileForm, activity, CancellationToken.None);
        }

        private async Task ResumeAfter_ProfileForm(IDialogContext context, IAwaitable<Profile> result)
        {
            var profile = await result;
            await context.PostAsync($"Alright, {profile.FirstName} {profile.LastName} the {profile.Occupation} from {profile.HomeTown}. Got it. Welcome!");
            context.Done(string.Empty);
        }
        private static IForm<Profile> BuildProfileForm()
        {
            return new FormBuilder<Profile>()
                .Field(nameof(Profile.FirstName), "What's your first name?")
                .Field(nameof(Profile.LastName), "Great, and your last name?")
                .Field(nameof(Profile.Occupation), "...and what do you do for a living??")
                .Field(nameof(Profile.HomeTown), "Where are you from?")
                .Build();
        }
    }
}