using UnityEngine;

namespace BlockBuilder
{
    [CreateAssetMenu(menuName = "BlockBuilder/BlockBuilderSettings")]
    public class BlockBuilderSettings : ScriptableObject
    {
        public int placementBlockCount = 2;
        public BuildingBlockSet[] blockSets;
    }
}