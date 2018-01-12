using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using System;
using Newtonsoft.Json;
using Ical.Net;
using System.Net.Http.Headers;
using Ical.Net.Serialization;

namespace CalenderMerge {
    public static class IcsMerge {
        [FunctionName("IcsMerge")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "HttpTriggerCSharp/IcsMerge/{calendar}")]HttpRequestMessage req, string calendar, TraceWriter log) {

            log.Info("C# HTTP trigger function processed a request.");
            string blobBaseUrl = ConfigurationManager.AppSettings["AzureBlobStorgageBaseUrl"];
            Uri jsonFileUrl = new Uri(new Uri(blobBaseUrl), calendar + ".json");


            string icsMergeJson = DownloadFileToString(jsonFileUrl);
            IcsMergeDetails mergeDetails = JsonConvert.DeserializeObject<IcsMergeDetails>(icsMergeJson);

            Calendar combinedCalendar = new Calendar();
            foreach (string calendarUrl in mergeDetails.calendars) {
                string calendarToCombineString = DownloadFileToString(new Uri(calendarUrl));
                Calendar calednarToCombine = Calendar.Load(calendarToCombineString);
                combinedCalendar.MergeWith(calednarToCombine);
            }

            CalendarSerializer serializer = new CalendarSerializer();
            string serializedCombinedCalendar = serializer.SerializeToString(combinedCalendar);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(serializedCombinedCalendar));
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = calendar +".ics" };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;

            // Fetching the name from the path parameter in the request URL
            //return req.CreateResponse(HttpStatusCode.OK, "Hello " + calendar);
        }

        private static string DownloadFileToString(Uri url) {
            string file;
            using (var wc = new WebClient()) {
                file = wc.DownloadString(url);
            }

            return file;
        }
    }

    public class IcsMergeDetails {
        public string[] calendars;
    }
}
