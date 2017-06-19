using System;
using System.Threading.Tasks;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using SampleApp.Core.Messenging;

namespace SampleApp.Core.Helpers
{
    public static class MessengingHelper
    {
        public static void RequestToast(object requestedBy, string message){
            GetMessenger().Publish(new ToastMvxMessage(requestedBy, message));
		}

        public static void RequestToast(object requestedBy, Exception exception){
            var apiException = exception as Refit.ApiException;

            if (apiException != null)
            {
                switch (apiException.StatusCode){
                    case System.Net.HttpStatusCode.InternalServerError:
                        RequestToast(requestedBy, "We are experiencing technical issues with our REST API");
                        break;
                    default:
                        RequestToast(requestedBy, "Connection to our API failed - check your internet state.");
                        break;
                }
            }

            if (exception is TaskCanceledException){
                RequestToast(requestedBy, "Connection timed out - check your internet connection state.");
            }
            else {
                RequestToast(requestedBy, "Sorry but it looks there is a bug in our app - please contact with application administrator.");
            }

        }

        private static IMvxMessenger GetMessenger() => Mvx.Resolve<IMvxMessenger>();

	}

}
