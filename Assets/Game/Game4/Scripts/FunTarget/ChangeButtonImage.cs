using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System.Threading;
using System;

public class ChangeButtonImage : MonoBehaviour
{
    [SerializeField] public Sprite newButtonImage; //Selected image (Green)
    [SerializeField] public Sprite oldButtonImage; //Reset image (Red)
    [SerializeField] public Button button;
    public Animator selectionAnimator;
    public CancellationTokenSource cancellationTokenSource;
    private bool switchAnimation;
    

    public async void PlaySelectedAnim(bool state)
    {
        switchAnimation = state;
        if (state)
        {
            cancellationTokenSource = new CancellationTokenSource();
        }
        else
        {
            cancellationTokenSource?.Cancel();
            button.image.sprite = oldButtonImage;
            return;
        }
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            button.image.sprite = oldButtonImage;
            try
            {
                await Awaitable.WaitForSecondsAsync(0.5f, cancellationTokenSource.Token);
                button.image.sprite = newButtonImage;
                await Awaitable.WaitForSecondsAsync(0.5f, cancellationTokenSource.Token);
            }catch(OperationCanceledException)
            {

            }
          
        }
        Debug.Log("Finished");
        button.image.sprite = oldButtonImage;
    }
  
    public void ChangeImage()
    {
        button.image.sprite = newButtonImage;
    }

    public void ResetImage()
    {
        button.image.sprite = oldButtonImage;
    }
}
