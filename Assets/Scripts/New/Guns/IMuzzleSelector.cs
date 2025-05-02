using System.Collections.Generic;
using UnityEngine;

public interface IMuzzleSelector
{
    Transform GetNextMuzzle();               // Used for sequential/random
    IEnumerable<Transform> GetAllMuzzles();  // Used for simultaneous fire
}