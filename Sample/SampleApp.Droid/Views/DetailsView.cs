using Android.App;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Widget;
using SampleApp.Core.ViewModels;
using XamarinBindings.MaterialProgressBar;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using SampleApp.Droid.Extensions;

namespace SampleApp.Droid.Views
{
    [Activity]
    public class DetailsView : BaseMvxActivity<ClientDetailsViewModel>
    {
        protected override int LayoutResource => Resource.Layout.detailsView;
        View mainView;
        View progressView;

        public DetailsView()
        {
        }

        public DetailsView(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "Client Details";
			var progressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);
			progressBar.IndeterminateDrawable = new IndeterminateProgressDrawable(this);
            progressBar.IndeterminateDrawable.SetColorFilter(
				new Color(ContextCompat.GetColor(BaseContext, Resource.Color.primary)), PorterDuff.Mode.SrcIn);

            progressView = progressBar;
            mainView = FindViewById(Resource.Id.refresher);

            var bindingSet = this.CreateBindingSet<DetailsView, ClientDetailsViewModel>();

            bindingSet.Bind(this)
                      .For(x => x.IsAsynchronousOperationInProgress)
                      .To(x => x.IsAsynchronousOperationInProgress);

            bindingSet.Apply();
        }

		private bool isAsyncOperationInProgres;
		public bool IsAsynchronousOperationInProgress
		{
			get { return isAsyncOperationInProgres; }
			set
			{
				isAsyncOperationInProgres = value;

				if (isAsyncOperationInProgres)
				{
					progressView?.FadeIn();
                    mainView?.FadeOut(ViewStates.Invisible);
				}
				else
				{
                    progressView?.FadeOut(ViewStates.Invisible);
					mainView?.FadeIn();
				}
			}
		}
        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }


    }
}
