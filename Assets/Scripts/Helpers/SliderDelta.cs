using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SliderDelta : MonoBehaviour
{
    public Image image;
    public Slider slider;
    
    private Tweener tweener;

    public void Start()
    {
        image.fillAmount = slider.value;
    }

    public void SetFill(float fill)
    {
        tweener?.Kill();
        tweener = image.DOFillAmount(fill, .5f).SetDelay(.3f);
    }
}
