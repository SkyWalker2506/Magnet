using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animation mainMenuAnimator;
    [SerializeField] AnimationClip fadeInAnimation;
    [SerializeField] AnimationClip fadeOutAnimation;

    public void OnFadeInComplete()
    {
        Debug.Log("FadeIn Completed");
    }

    public void OnFadeOutComplete()
    {
        Debug.Log("FadeOut Completed");
    }

    public void FadeIn()
    {
        mainMenuAnimator.Stop();
        mainMenuAnimator.clip = fadeInAnimation;
        mainMenuAnimator.Play();
    }
    public void FadeOut()
    {
        mainMenuAnimator.Stop();
        mainMenuAnimator.clip = fadeOutAnimation;
        mainMenuAnimator.Play();
    }

}