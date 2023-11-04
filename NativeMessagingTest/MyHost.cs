using NativeMessaging;
using System.Text.Json.Nodes;

namespace NativeMessagingTest
{
    public class MyHost : Host
    {
        private const bool SendConfirmationReceipt = true;

        public override string Hostname
        {
            get { return "com.google.chrome.example.echo"; }
        }

        public MyHost() : base(SendConfirmationReceipt)
        {

        }

        protected override void ProcessReceivedMessage(JsonObject data)
        {
            SendMessage(data);
        }

    }
}
