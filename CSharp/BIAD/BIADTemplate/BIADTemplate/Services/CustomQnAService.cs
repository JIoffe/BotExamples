using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BIADTemplate.Services
{
    /// <summary>
    /// Extremely simplified QnA service for the purposes of the Bot-In-A-Day Workshop
    /// </summary>
    [Serializable]
    public class CustomQnAService : IQnAService
    {
        //In a production application, you probably would not want to hard code these values.
        //Some options to Consider:
        //- environment-specific configuration files
        //- configuration on the app service via azure portal
        //- Azure KeyVault

        private const string QNA_HOST = "";
        private const string QNA_KNOWLEDGEBASE = "";
        private const string QNA_SUBSCRIPTION_KEY = "";

        private readonly string _qnaHost;
        private readonly QnAMakerAttribute _qnAMakerAttribute;

        public CustomQnAService(QnAMakerAttribute qnAMakerAttribute, string qnaHost)
        {
            _qnaHost = qnaHost;
            _qnAMakerAttribute = qnAMakerAttribute;
        }

        public Task<bool> ActiveLearnAsync(Uri uri, QnAMakerTrainingRequestBody postBody, string authKey)
        {
            //Active training is not in scope for this workshop, please see documentation online!
            throw new NotImplementedException();
        }

        public Uri BuildFeedbackRequest(string userId, string userQuery, string kbQuestion, string kbAnswer, out QnAMakerTrainingRequestBody postBody, out string authKey, out string knowledgebaseId)
        {
            //Active training is not in scope for this workshop, please see documentation online!
            throw new NotImplementedException();
        }

        public Uri BuildRequest(string queryText, out QnAMakerRequestBody postBody, out string authKey)
        {
            var builder = new UriBuilder($"{_qnaHost}/knowledgebases/{_qnAMakerAttribute.KnowledgebaseId}/generateAnswer");
            postBody = new QnAMakerRequestBody { question = queryText, top = _qnAMakerAttribute.Top };
            authKey = _qnAMakerAttribute.AuthKey;

            return builder.Uri;
        }

        public async Task<QnAMakerResults> QueryServiceAsync(Uri uri, QnAMakerRequestBody postBody, string authKey)
        {
            //Extremely simplified for the sake of the workshop.
            //Note in a production application, you would not want to continually create and dispose of HttpClient in this way!
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", authKey);

                var response = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(postBody), Encoding.UTF8, "application/json"));
                var json = await response.Content.ReadAsStringAsync();

                try
                {
                    var jObject = JObject.Parse(json);
                    var answers = jObject["answers"];

                    if (answers == null)
                        return new QnAMakerResults();

                    return new QnAMakerResults(JsonConvert.DeserializeObject<List<QnAMakerResult>>(answers.ToString()));
                }
                catch (JsonException ex)
                {
                    throw new ArgumentException("Unable to deserialize the QnA service response.", ex);
                }
            }
        }
    }
}