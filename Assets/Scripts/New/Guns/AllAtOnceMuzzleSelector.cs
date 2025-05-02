using System.Collections.Generic;
using UnityEngine;

public class AllAtOnceMuzzleSelector : MonoBehaviour, IMuzzleSelector
{
    [SerializeField] private List<Transform> muzzles;
    public Transform GetNextMuzzle() => null; // Not used
    public IEnumerable<Transform> GetAllMuzzles() => muzzles;
}