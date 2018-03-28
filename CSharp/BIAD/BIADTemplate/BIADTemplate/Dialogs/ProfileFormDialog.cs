using BIADTemplate.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs
{
    [Serializable]
    public class ProfileFormDialog: IDialog<string>
    {
        public Task StartAsync(IDialogContext context)
        {
            //Adding "prompt in start" causes our form to prompt right away without waiting for user input
            var form = FormDialog.FromForm(BuildProfileForm, FormOptions.PromptInStart);
            context.Call(form, ResumeAfter_ProfileForm);

            return Task.CompletedTask;
        }


        private async Task ResumeAfter_ProfileForm(IDialogContext context, IAwaitable<Profile> result)
        {
            var profile = await result;
            await context.PostAsync($"Alright, {profile.FirstName} {profile.LastName} the {profile.Occupation} from {profile.HomeTown}. Got it. Welcome!");
            context.Done(string.Empty);
        }
        private static IForm<Profile> BuildProfileForm()
        {
            //FormBuilder greater streamlines waterfall-like functionality
            return new FormBuilder<Profile>()
                .Field(nameof(Profile.FirstName), "What's your first name?")
                .Field(nameof(Profile.LastName), "Great, and your last name?")
                .Field(nameof(Profile.Occupation), "...and what do you do for a living??")
                .Field(nameof(Profile.HomeTown), "Where are you from?")
                .Build();
        }
    }
}