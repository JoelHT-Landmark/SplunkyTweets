using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TweetWatcher
{
    public class HashtagMonitor : TwitterMonitor
    {
        private string hashtag;
        private readonly IUserStream messageStream;
        private readonly ILogger logger;

        public HashtagMonitor(string hashtag, ITwitterCredentials credentials, ILogger logger)
        {
            this.logger = logger;

            this.logger.Verbose("About to create filtered twitter stream for {hashtag}", hashtag);

            this.hashtag = hashtag.ToLowerInvariant().Trim();

            this.messageStream = Stream.CreateUserStream(credentials.Clone());

            ////messageStream.AddTrack(hashtag);
            ////messageStream.MatchingTweetReceived += (sender, args) =>
            ////{
            ////    Console.WriteLine("A tweet containing '" + this.hashtag + "' has been found: '" + args.Tweet + "'");
            ////    this.logger.Information("Tweet from {user}: {message}", args.Tweet.Source, args.Tweet.Text);
            ////};

            this.messageStream.StreamStopped += MessageStream_StreamStopped;
            this.messageStream.TweetCreatedByAnyone += MessageStream_TweetReceived;
            this.logger.Verbose("Successfully created filtered twitter stream {streamId}", this.messageStream.GetHashCode());
        }

        private void MessageStream_TweetReceived(object sender, Tweetinvi.Events.TweetReceivedEventArgs e)
        {
            Console.WriteLine("Tweet from {0}: {1}", e.Tweet.Source, e.Tweet.Text);
        }

        private void MessageStream_StreamStopped(object sender, Tweetinvi.Events.StreamExceptionEventArgs e)
        {
            Console.WriteLine("Stopped - {0}: {1}", e.DisconnectMessage, e.Exception.Message);
        }

        public void Start()
        {
            this.logger.Verbose("Starting monitoring Twitter for '#{hashtag}' using stream {streamId}.", this.hashtag, this.messageStream.GetHashCode());
            messageStream.StartStream();
        }

        public void Stop()
        {
            this.logger.Verbose("Stopping stream {streamId}.", this.messageStream.GetHashCode());
            this.messageStream.StopStream();
        }
    }
}
