using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Web.Hosting;

namespace GmailViewer.Models
{
    public class Mseeages
    {
        string[] Scopes = { GmailService.Scope.GmailReadonly };
        string ApplicationName = "Gmail API .NET Quickstart";

        public List<gMessage> ListMinimal { get; set; }

        public void getlist()
        {
            ListMinimal = new List<gMessage>();
            UserCredential credential;

            using (var stream =
                new FileStream(HostingEnvironment.MapPath("~/App_Data/client_secret_50186485062-qtg42d1kc2vldvanujspfolg783ke3on.apps.googleusercontent.com.json"), FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");
                credPath = HostingEnvironment.MapPath("~/App_Data/.credentials/gmail-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            var messageRequest = service.Users.Messages.List("me");
            IList<Message> messages = messageRequest.Execute().Messages;
            // List messages.
            Console.WriteLine("messages:");
            if (messages != null && messages.Count > 0)
            {
                foreach (var labelItem in messages)
                {
                    var item = service.Users.Messages.Get("me", labelItem.Id);
                    item.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                    var _item = item.Execute();

                    string subject = _item.Payload.Headers.FirstOrDefault(x => x.Name == "Subject").Value;
                    DateTime _getdate = Convert.ToDateTime(_item.Payload.Headers.FirstOrDefault(x => x.Name == "Date").Value);
                    string _From = _item.Payload.Headers.FirstOrDefault(x => x.Name == "From").Value;
                    if (_From.Split('\"').Length > 2)
                    {
                        _From = _item.Payload.Headers.FirstOrDefault(x => x.Name == "From").Value.Split('\"')[1];
                    }
                    ListMinimal.Add(new gMessage()
                    {
                        _from = _From,
                        gettime = _getdate.Date.Equals(DateTime.Now.Date) ? (_getdate.Year == DateTime.Now.Year ? _getdate.ToString("MM月dd日") : _getdate.ToString("yy/M/d")) : _getdate.ToString("hh:mm"),
                        isUnread = _item.LabelIds.Contains("UNREAD"),
                        subject = subject,
                        id = _item.Id
                    });
                }
            }
            else
            {
                Console.WriteLine("No labels found.");
            }
        }
    }

    public class gMessage
    {
        public string id { get; set; }
        public string _from { get; set; }
        public string subject { get; set; }
        public string gettime { get; set; }
        public bool isUnread { get; set; }
    }
}
