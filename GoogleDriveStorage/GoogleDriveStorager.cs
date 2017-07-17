using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveStorage
{

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using Google.Apis.Drive.v3.Data;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    namespace DriveQuickstart
    {
        public class GoogleDriveStorager
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/drive-dotnet-quickstart.json
            private string[] Scopes = { DriveService.Scope.DriveReadonly };
            private string ApplicationName = "Drive API .NET Quickstart";
            private static readonly ILog logger = LogManager.GetLogger(typeof(GoogleDriveStorager));

            public void TestGd()
            {
                UserCredential credential;

                using (var stream =
                    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    logger.Debug(string.Format("Credential file saved to: " + credPath));
                }

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define parameters of request.
                FilesResource.ListRequest listRequest = service.Files.List();
                string pageToken = null;
                listRequest.Q = "mimeType = 'application/vnd.google-apps.folder' and name = 'grainsim' and trashed = false";
                //listRequest.Fields = "nextPageToken, items(id, title)";
                //listRequest.PageToken = pageToken;
                listRequest.PageSize = 10;

                // List files.
                IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                    .Files;

                logger.Debug(string.Format("Files:"));
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        logger.Debug(string.Format("{0} ({1})", file.Name, file.Id));
                    }
                }
                else
                {
                    logger.Debug(string.Format("No files found."));
                }

            }
        }
    }




}


