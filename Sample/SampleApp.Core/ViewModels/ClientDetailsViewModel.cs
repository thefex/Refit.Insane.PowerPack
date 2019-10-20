using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using Refit.Insane.PowerPack.Services;
using SampleApp.Core.Model;
using SampleApp.Core.Rest;
using System.Threading;
using SampleApp.Core.Helpers;
using Refit.Insane.PowerPack.Caching;

namespace SampleApp.Core.ViewModels
{
    public class ClientDetailsViewModel : MvxViewModel<Client>, ILoadable
    {
        readonly IRestService restService;
        IEnumerable<PropertyValue> _properties = Enumerable.Empty<PropertyValue>();

        public ClientDetailsViewModel(IRestService restService)
        {
            this.restService = restService;
        }

        public IEnumerable<PropertyValue> Properties
        {
            get { return _properties; }
            set { SetProperty(ref _properties, value); }
        }

        public Client Client { get; private set; }

        public async Task Load()
        {
            try
            {
                IsAsynchronousOperationInProgress = true;
                var clientId = Client.Id;
                var clientDetailsResponse =
                    await restService.Execute<IClientApi, ClientDetails>
                        (api => api.GetClientDetails(clientId, default(CancellationToken)));

                if (clientDetailsResponse.IsSuccess)
                    Properties = clientDetailsResponse.Results.Properties;
                else
                   MessengingHelper.RequestToast(this, clientDetailsResponse.FormattedErrorMessages);

            }
            catch (Exception e)
            {
                MessengingHelper.RequestToast(this, e);
            }
            finally
            {
                IsAsynchronousOperationInProgress = false;
            }
        }

        public override Task Initialize(Client parameter)
        {
            Client = parameter;
            return Task.FromResult(true);
        }

        public MvxCommand UpdateClientData => new MvxCommand(async () =>
        {
            try
            {
                if (!Properties.Any())
                    return;

                IsAsynchronousOperationInProgress = true;

                var clientDetails = new ClientDetails() { Properties = Properties };
                var clientId = Client.Id;

                var updateClientDetailsResponse = await restService.Execute<IClientApi>(api => api.UpdateClientDetails(
                    clientId,
                    clientDetails,
                    default(CancellationToken)
                ));

                if (!updateClientDetailsResponse.IsSuccess)
                    MessengingHelper.RequestToast(this, updateClientDetailsResponse.FormattedErrorMessages);
                else
                {
                    await RefitCacheService.Instance.UpdateCache<IClientApi, ClientDetails>
                         (api => api.GetClientDetails(
                               clientId, 
                                default(CancellationToken)),
                               new ClientDetails() { Properties = Properties });
                               
                    MessengingHelper.RequestToast(this, "We have successfully updated Client Details data.");
                }
            }
            catch (Exception e)
            {
                MessengingHelper.RequestToast(this, e);
            }
            finally
            {
                IsAsynchronousOperationInProgress = false;
            }
        });

        private bool isAsyncOperationInProgress;
        public bool IsAsynchronousOperationInProgress
        {
            get { return isAsyncOperationInProgress; }
            set { SetProperty(ref isAsyncOperationInProgress, value); }
        }
    }

}