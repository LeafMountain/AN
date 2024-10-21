using System;
using DG.Tweening;
using EventManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Serializable]
    public class ProgressBar
    {
        public TMP_Text countText;
        public Image fillImage;
    }

    public ProgressBar energyBar;
    public ProgressBar supplyBar;

    private Tweener energyTweener;
    private Tweener supplyTweener;

    public TMP_Text screenText;

    private void Start()
    {
        screenText.GetComponent<CanvasGroup>().alpha = 0;
        Events.AddListener(Flag.Storeable, OnStorableUpdated);
        Events.AddListener(Flag.DamageRecieved, OnDamageRecieved);
    }

    private void FixedUpdate()
    {
        if(GameManager.Instance.localPlayer)
            OnStorableUpdated(null, null);
    }

    private void OnStorableUpdated(object origin, EventArgs eventargs)
    {
        energyTweener?.Kill();
        energyTweener = energyBar.fillImage.DOFillAmount(GameManager.Instance.localPlayer.energy / 100f, .3f).SetEase(Ease.OutBounce);
        energyBar.countText.text = GameManager.Instance.localPlayer.energy.ToString();

        supplyTweener?.Kill();
        supplyTweener = supplyBar.fillImage.DOFillAmount(GameManager.Instance.localPlayer.supplies / 50f, .3f).SetEase(Ease.OutBounce);
        supplyBar.countText.text = GameManager.Instance.localPlayer.supplies.ToString();
    }
    
    private void OnDamageRecieved(object origin, EventArgs eventargs)
    {
        var damageArgs = eventargs as DamageRecievedArgs;
        if (damageArgs.destroyed && damageArgs.reciever == GameManager.Instance.localPlayer)
        {
            screenText.GetComponent<CanvasGroup>().DOFade(1f, 1f).onComplete = () =>
            {
                screenText.GetComponent<CanvasGroup>().DOFade(0f, 3f).SetDelay(5f);
            };
        }
    }
}
