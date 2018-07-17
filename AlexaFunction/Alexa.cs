using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http;
using System.Collections.Generic;
using System;
using Alexa.NET.Response.Ssml;

namespace AlexaFunction
{
    public static class Alexa
    {
        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("triggered.");
            var data = await req.Content.ReadAsAsync<SkillRequest>();

            var type = data.GetRequestType();

            SkillResponse response = null;

            if (type == typeof(IntentRequest))
            {
                var intentReq = data.Request as IntentRequest;
                switch(intentReq.Intent.Name)
                {
                    case "NowDate":
                        response = NowDate();
                        break;
                    default:
                        response = ResponseBuilder.Tell("すみませんが聞き取れませんでした。");
                        break;

                }
            }
            else
            {
                response = FirstQuestion();
            }

            return new OkObjectResult(response);
        }

        private static SkillResponse NowDate()
        {
            var now = DateTime.Now;
            var date = new SayAs(now.ToString("????MMdd"), InterpretAs.Date);
            var speech = new Speech(new PlainText("今日は"), date, new PlainText("です。"));

            var output = new SsmlOutputSpeech()
            {
                Ssml = speech.ToXml()
            };

            return ResponseBuilder.Tell(output);

        }

        private static SkillResponse FirstQuestion()
        {

            var output = new PlainTextOutputSpeech()
            {
                Text = "TestAppへようこそ。\n何について聞きたいですか?"
            };

            var reprompt = new Reprompt("何について聞きたいですか?");

            return ResponseBuilder.AskWithCard(output, "TestApp", "test", reprompt);

        }
    }
}
