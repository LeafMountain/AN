using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class EnergyPylon : Actor
{
    public UnityEvent<float> OnFill;

    public int maxCharge = 100;
    public int currentCharge;

    public float absorbRange = 5f;
    public float rechargeRange = 5f;
    // public Light light;
    public Tweener tweener;

    protected override void Start()
    {
        base.Start();
        StartTweener(2f);
        EnergyAbsorbTick();
        EnergyTick();
    }

    private async void EnergyTick()
    {
        while (true)
        {
            await Task.Delay(3000);
            AddEnergy(-1);
        }
    }

    private void StartTweener(float duration)
    {
        tweener?.Kill();
        tweener = transform.DORotate(Vector3.up * 180f, duration).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private async void EnergyAbsorbTick()
    {
        while (true)
        {
            await Task.Delay(500);
            if (GameManager.Instance.localPlayer == null) continue;
            var playerDistance = Vector3.Distance(transform.position, GameManager.Instance.localPlayer.transform.position);

            if (GameManager.Instance.localPlayer.supplies == 0)
            {
                StartTweener(2f);
            }
            else
            {
                if (playerDistance < absorbRange)
                {
                    StartTweener(.5f);
                    GameManager.Instance.localPlayer.supplies -= 1;
                    transform.DOPunchScale(Vector3.one * .2f, .3f);
                    currentCharge++;
                    OnFill?.Invoke((float) currentCharge / maxCharge);
                }
            }

            if (currentCharge > 0 && playerDistance < rechargeRange)
            {
                GameManager.Instance.localPlayer.AddEnergy(1);
            }
        }
    }

    public void AddEnergy(int value)
    {
        currentCharge += value;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, absorbRange);
    }
}