using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gapitest
{
    class Program
    {
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret_50186485062-qtg42d1kc2vldvanujspfolg783ke3on.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

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
            //UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

            //// List labels.
            //IList<Label> labels = request.Execute().Labels;
            //Console.WriteLine("Labels:");
            //if (labels != null && labels.Count > 0)
            //{
            //    foreach (var labelItem in labels)
            //    {
            //        Console.WriteLine("{0}", labelItem.Name);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No labels found.");
            //}
            var messageRequest = service.Users.Messages.List("me");
            IList<Message> messages = messageRequest.Execute().Messages;
            // List messages.
            Console.WriteLine("messages:");
            if (messages != null && messages.Count > 0)
            {
                foreach (var labelItem in messages)
                {
                    var item = service.Users.Messages.Get("me", labelItem.Id);
                    item.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Metadata;

                    var _item = item.Execute();
                    string subject = _item.Payload.Headers.FirstOrDefault(x => x.Name == "Subject").Value; //"INBOX"UNREAD
                    DateTime _getdate = Convert.ToDateTime(_item.Payload.Headers.FirstOrDefault(x => x.Name == "Date").Value);
                    string _From = _item.Payload.Headers.FirstOrDefault(x => x.Name == "From").Value;
                    if(_From.Split('\"').Length > 2)
                    {
                        _From = _item.Payload.Headers.FirstOrDefault(x => x.Name == "From").Value.Split('\"')[1];
                    }
                    Console.WriteLine("{0} : {1} : {2} : {3}", _From, subject, _getdate.ToString("yyyy/MM/dd hh:mm"), string.Join(",", _item.LabelIds));
                }
            }
            else
            {
                Console.WriteLine("No labels found.");
            }

            Console.Read();

        }
    }
}
