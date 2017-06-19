using System;
using MvvmCross.Plugins.Messenger;

namespace SampleApp.Core.Messenging
{
    public class ToastMvxMessage : MvxMessage
    {
        public string Message { get; private set; }

        public ToastMvxMessage(object sender, string message) : base(sender)
        {
            Message = message;
        }
    }
}
