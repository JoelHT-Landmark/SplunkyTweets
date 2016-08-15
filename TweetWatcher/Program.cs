using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Tweetinvi;
using Tweetinvi.Models;

namespace TweetWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
        
            var logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();

            logger.Verbose("About to authenticate...");

            var creds = Authenticate();

            Console.Write("Monitor #");
            var hashtag = Console.ReadLine();

            var monitor = new HashtagMonitor(hashtag, creds, logger);
            monitor.Start();

            Console.ReadLine();

            monitor.Stop();
        }

        private static ITwitterCredentials Authenticate()
        {
            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials("H0uQO4dVmOTMNyauymkeFe0oM", "kqR5oDC9PSXwosAP24gG76oarN577YVmgv0Y9ZH6u354fd7z3v");

            // Init the authentication process and store the related `AuthenticationContext`.
            var authenticationContext = AuthFlow.InitAuthentication(appCredentials);

            // Go to the URL so that Twitter authenticates the user and gives him a PIN code.
            Process.Start(authenticationContext.AuthorizationURL);

            // Ask the user to enter the pin code given by Twitter
            Console.Write("Twitter PIN >");
            var pinCode = Console.ReadLine();

            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);

            return userCredentials;
        }
    }
}
