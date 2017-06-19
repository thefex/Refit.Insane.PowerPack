using System;
using Android.Animation;
using Android.Runtime;

namespace SampleApp.Droid.Extensions
{
    public class AnimatorActionListenerWrapper : Java.Lang.Object, Animator.IAnimatorListener
    {
        private readonly Action<Animator> _onCancel;
        private readonly Action<Animator> _onEnd;
        private readonly Action<Animator> _onRepeat;
        private readonly Action<Animator> _onStart;

        public AnimatorActionListenerWrapper(Action<Animator> onStart, Action<Animator> onEnd, Action<Animator> onCancel,
            Action<Animator> onRepeat)
        {
            _onStart = onStart;
            _onCancel = onCancel;
            _onEnd = onEnd;
            _onRepeat = onRepeat;
        }

        public AnimatorActionListenerWrapper(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public void OnAnimationCancel(Animator animation)
        {
            _onCancel?.Invoke(animation);
        }

        public void OnAnimationEnd(Animator animation)
        {
            _onEnd?.Invoke(animation);
        }

        public void OnAnimationRepeat(Animator animation)
        {
            _onRepeat?.Invoke(animation);
        }

        public void OnAnimationStart(Animator animation)
        {
            _onStart?.Invoke(animation);
        }
    }
}
