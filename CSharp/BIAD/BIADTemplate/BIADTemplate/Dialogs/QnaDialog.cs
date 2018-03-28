using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System;
using System.Configuration;

namespace BIADTemplate.Dialogs
{
    [Serializable]
    public class QnaDialog: QnAMakerDialog
    {
        public QnaDialog() :
            base(GetQnaService())
        {
        }

        private static IQnAService GetQnaService()
        {
            var key = ConfigurationManager.AppSettings["QnaSubscriptionKey"];
            var kbId = ConfigurationManager.AppSettings["QnaKnowledgebaseId"];
            var defaultMessage = "Sorry, I couldn't find an answer for that";
            var scoreThreshold = 0.5D;


            var qnaMakerAttribute = new QnAMakerAttribute(key, kbId, defaultMessage, scoreThreshold);
            return new QnAMakerService(qnaMakerAttribute);
        }
    }
}