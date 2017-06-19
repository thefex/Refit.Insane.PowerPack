using Android.Views;

namespace SampleApp.Droid.Extensions
{
    public static class ViewExtensions
    {

		public static View FadeIn(this View view, long duration = 350)
		{
			view.Animation?.Cancel();
			view.Animate()
				.Alpha(1)
				.SetDuration(duration)
				.SetListener(
					new AnimatorActionListenerWrapper(
						x => view.Visibility = ViewStates.Visible,
						x => { },
						x => { },
						x => { }))
				.Start();

			return view;
		}

		public static View FadeOut(this View view, ViewStates viewStateAfterAnimation, long duration = 350)
		{
			view.Animation?.Cancel();
			view.Animate()
				.Alpha(0)
				.SetDuration(duration)
				.SetStartDelay(125)
				.SetListener(
					new AnimatorActionListenerWrapper(
						x => { },
						x => view.Visibility = viewStateAfterAnimation,
						x => { },
						x => { }))
				.Start();

			return view;
		}
    }
}
