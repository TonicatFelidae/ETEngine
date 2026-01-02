/*********************************************************
 * Rule The Sea Company.
 * Author: taniafelidae
 * CreateTime: 02/01/2026 12:11:37
 * Description: 
 *********************************************************/
using System;
using UnityScreenNavigator.Runtime.Foundation.Animation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation
{
    public class FakeAnimation : IAnimation
    {
        public float Progress { get; private set; }
        public float Duration { get; }

        public FakeAnimation(float duration = 1.0f)
        {
            Duration = duration;
        }
        
        public void SetTime(float time)
        {
            time = Math.Min(Duration, time);
            Progress = time / Duration;
        }
    }
}
/*Rule The Sea Company.*/