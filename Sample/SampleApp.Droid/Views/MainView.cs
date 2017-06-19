using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using SampleApp.Core.ViewModels;
using SampleApp.Droid.Extensions;
using XamarinBindings.MaterialProgressBar;

namespace SampleApp.Droid.Views
{
    [Activity]
    public class MainView : BaseMvxActivity<MainViewModel>
    {
        protected override int LayoutResource => Resource.Layout.mainView;

        View mainView;
        View progressView;

        public MainView()
        {
        }

        public MainView(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			Title = "Client list";
            var progressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);
			progressBar.IndeterminateDrawable = new IndeterminateProgressDrawable(this);
			progressBar.IndeterminateDrawable.SetColorFilter(
				new Color(ContextCompat.GetColor(BaseContext, Resource.Color.primary)), PorterDuff.Mode.SrcIn);

            progressView = progressBar;
            mainView = FindViewById(Resource.Id.refresher);

            var bindingSet = this.CreateBindingSet<MainView, MainViewModel>();

            bindingSet.Bind(this)
                      .For(x => x.IsAsynchronousOperationInProgress)
                      .To(x => x.IsAsynchronousOperationInProgress);

            bindingSet.Apply();
		}

        protected override bool ShouldShowHome => false;

        private bool isAsyncOperationInProgres;
        public bool IsAsynchronousOperationInProgress {
            get { return isAsyncOperationInProgres; }
            set { isAsyncOperationInProgres = value;

                if (isAsyncOperationInProgres)
                {
                    progressView?.FadeIn();
                    mainView?.FadeOut(ViewStates.Invisible);
                } else {
                    progressView?.FadeOut(ViewStates.Invisible);
                    mainView?.FadeIn();
                }
            }
        }
    }
}
