using System;
using DG.Tweening;
using EventManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour
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

    private void Start()
    {
        Events.AddListener(Flag.Storeable, OnStorableUpdated);
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
}
