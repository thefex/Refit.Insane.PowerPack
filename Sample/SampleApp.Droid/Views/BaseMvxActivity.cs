using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.V7.Widget;
using Com.Tapadoo.Alerter;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using SampleApp.Core.Messenging;
using SampleApp.Core.ViewModels;

namespace SampleApp.Droid.Views
{
    public abstract class BaseMvxActivity<TViewModel> : MvxAppCompatActivity<TViewModel> where TViewModel : class, IMvxViewModel
	{
		protected Toolbar Toolbar { get; set; }
        readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

		public BaseMvxActivity()
        {
        }

        public BaseMvxActivity(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(LayoutResource);

			Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			if (Toolbar != null)
			{
				SetSupportActionBar(Toolbar);
				SupportActionBar.SetDisplayHomeAsUpEnabled(ShouldShowHome);
				SupportActionBar.SetHomeButtonEnabled(ShouldShowHome);
			}
            var loadable = ViewModel as ILoadable;
            loadable.Load();
        }

        protected override void OnResume()
        {
            base.OnResume();

            _subscriptions.Add(
                Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ToastMvxMessage>(msg => {
                Alerter.Create(this).SetText(msg.Message).SetBackgroundColor(Resource.Color.primaryDark).SetIcon(Resource.Drawable.il2).Show();
            }));
        }

        protected virtual bool ShouldShowHome => true;

        protected override void OnPause()
        {
            base.OnPause();

            foreach (var subscription in _subscriptions.ToList())
                subscription.Dispose();
            _subscriptions.Clear();
        }

        protected abstract int LayoutResource { get; }
    }
}
