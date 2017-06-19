using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using Refit.Insane.PowerPack.Services;
using SampleApp.Core.Helpers;
using SampleApp.Core.Messenging;
using SampleApp.Core.Model;
using SampleApp.Core.Rest;

namespace SampleApp.Core.ViewModels
{
    public class MainViewModel
        : MvxViewModel, ILoadable
    {
        readonly IRestService restService;

        public MainViewModel(IRestService restService)
        {
            this.restService = restService;
        }


        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();


        public MvxCommand AddClient => new MvxCommand(async () =>
        {
            try
            {
                IsAsynchronousOperationInProgress = true;
                var addClientResponse = await restService.Execute<IClientApi, Client>(api => api.AddClient(default(System.Threading.CancellationToken)));

                if (addClientResponse.IsSuccess)
                {
                    addClientResponse.Results.ImagePath = addClientResponse.Results.ImagePath + "#" + Guid.NewGuid().ToString("N"); // prevent caching of image
                    Clients.Add(addClientResponse.Results);
                }
                else
                    MessengingHelper.RequestToast(this, addClientResponse.FormattedErrorMessages);
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

        public async Task Load()
        {
            try
            {
                IsAsynchronousOperationInProgress = true;

                var getClientsResponse =
                    await restService.Execute<IClientApi, IEnumerable<Client>>(api => api.GetClients(default(System.Threading.CancellationToken)));

                if (!getClientsResponse.IsSuccess)
                {
                    MessengingHelper.RequestToast(this, getClientsResponse.FormattedErrorMessages);
                    return;
                }

                foreach (var item in getClientsResponse.Results)
                {
                    item.ImagePath = item.ImagePath + "#" + Guid.NewGuid().ToString("N"); // prevent caching of image
                    Clients.Add(item);
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
        }

        public MvxCommand RefreshCommand => new MvxCommand(async () =>
        {
            try
            {
                IsAsynchronousOperationInProgress = true;
                IsRefreshing = true;
                Clients.Clear();
                await Load();
            }
            catch (Exception e)
            {
                MessengingHelper.RequestToast(this, e);
            }
            finally
            {
                IsRefreshing = false;
                IsAsynchronousOperationInProgress = false;
            }
        });

        private bool isRefreshing;

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public MvxCommand<Client> ClientTapped => new MvxCommand<Client>((x) =>
        {
            ShowViewModel<ClientDetailsViewModel, Client>(x);
        });


        private bool isAsyncOperationInProgress;
        public bool IsAsynchronousOperationInProgress
        {
            get { return isAsyncOperationInProgress; }
            set { SetProperty(ref isAsyncOperationInProgress, value); }
        }
    }
}
