using System.Collections.Generic;
using UnityEngine;

public class RandomMuzzleSelector : MonoBehaviour, IMuzzleSelector
{
    [SerializeField] private List<Transform> muzzles;

    public Transform GetNextMuzzle() => muzzles[Random.Range(0, muzzles.Count)];
    public IEnumerable<Transform> GetAllMuzzles() => muzzles;
}