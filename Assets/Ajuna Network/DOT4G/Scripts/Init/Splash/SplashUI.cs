using UnityEngine;

namespace Init.Splash
{
    public class SplashUI : UICanvas
    {
        public Animator animator;

        public bool IsPlaying()
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        }
    }
}
