using System.Collections.Generic;
using UnityEngine;

public class SequentialMuzzleSelector : MonoBehaviour, IMuzzleSelector
{
    [SerializeField] private List<Transform> muzzles;
    private int index = 0;

    public Transform GetNextMuzzle()
    {
        var muzzle = muzzles[index];
        index = (index + 1) % muzzles.Count;
        return muzzle;
    }

    public IEnumerable<Transform> GetAllMuzzles() => muzzles;
}