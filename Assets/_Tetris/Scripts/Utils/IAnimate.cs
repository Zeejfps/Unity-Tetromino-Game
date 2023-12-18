using System;
using System.Collections;
using UnityEngine;

public interface IAnimate
{
    Coroutine AnimationRoutine { get; set; }
    
    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine coroutine);
}

public static class AnimationExt
{
    public static Coroutine Animate(this IAnimate animationSrc, float duration, 
        Func<float, float> tweenFunc, Action<float> lerpFunc, Action onComplete = default)
    {
        if (animationSrc.AnimationRoutine != null)
            animationSrc.StopCoroutine(animationSrc.AnimationRoutine);
        animationSrc.AnimationRoutine = animationSrc.StartCoroutine(AnimateRoutine(duration, tweenFunc, lerpFunc, onComplete));
        return animationSrc.AnimationRoutine;
    }

    private static IEnumerator AnimateRoutine(float duration, Func<float, float> tweenFunc,
        Action<float> lerpFunc, Action onComplete = default)
    {
        var time = 0f;
        while (time < duration)
        {
            var t = time / duration;
            t = tweenFunc.Invoke(t);
            lerpFunc.Invoke(t);
            yield return null;
            time += Time.deltaTime;
        }
        lerpFunc.Invoke(tweenFunc.Invoke(1f));
        onComplete?.Invoke();
    }
}