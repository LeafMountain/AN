using UnityEditor;
using UnityEngine;

namespace BlockBuilder
{
    public class BuildingBlock : MonoBehaviour
    {
        [System.Serializable]
        public struct BuildingConnection
        {
            public int connection;
            public bool symmetrical;

            public static BuildingConnection Air()
            {
                return new BuildingConnection
                {
                    connection = 1, symmetrical = true,
                };
            }
        }
        
        [Tooltip("0 = air. 1 = ground. 2 = void. 3 = anything but air.")]
        private const int NullConnection = 0;
        private const int AirConnection = 1;
        private const int VoidConnection = 2;

        public bool lockConnections = false;
        public BuildingConnection[] buildingConnections = new BuildingConnection[6];

        public bool gatherAirConnections = false;
        public bool showGizmo = false;

        // [Button("Gather Connections")]
        [ContextMenu("GatherConections")]
        public void GatherConnections()
        {
            if(lockConnections) return;
            lockConnections = true;
            int rotation = (int)(transform.rotation.eulerAngles.y / 90f);
            rotation %= 4;

            void SetupConnection(Directions direction)
            {
                int oppositeDirection = (int)direction.Opposite();
                Vector3 checkDirection = transform.TransformDirection(direction.ToVector3());
                BuildingConnection neighborConnection = GetConnection(checkDirection, (Directions)oppositeDirection, rotation);
                int neighborConnectionID = neighborConnection.connection;
                int connectionID = buildingConnections[(int)direction].connection;
                if (neighborConnectionID != ~connectionID && ~connectionID != VoidConnection)
                {
                    if (buildingConnections[(int)direction].connection == neighborConnectionID && neighborConnectionID != AirConnection)
                    {
                        buildingConnections[(int)direction].symmetrical = true;
                    }
                    else
                    {
                        if (neighborConnection.symmetrical && neighborConnectionID != AirConnection)
                        {
                            buildingConnections[(int)direction].connection = neighborConnection.connection;
                            buildingConnections[(int)direction].symmetrical = true;
                        }
                        else
                        {
                            buildingConnections[(int)direction].connection = ~neighborConnectionID;
                        }
                    }
#if UNITY_EDITOR
                    PrefabUtility.ApplyPrefabInstance(gameObject, InteractionMode.AutomatedAction);
#endif
                }
            }

            SetupConnection(Directions.Right);
            SetupConnection(Directions.Back);
            SetupConnection(Directions.Left);
            SetupConnection(Directions.Forward);
            SetupConnection(Directions.Down);
            SetupConnection(Directions.Up);
        }

        private BuildingConnection GetConnection(Vector3 direction, Directions checkConnection, int rotation = 0)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 2f) == false) return BuildingConnection.Air();
            if (hit.transform.TryGetComponent(out BuildingBlock hitBlock) == false) return BuildingConnection.Air();

            if ((int) checkConnection < 4)
            {
                int otherRotation = (int) (hit.transform.rotation.eulerAngles.y / 90f);
                otherRotation = otherRotation % 4;
                checkConnection = (Directions) (((float) checkConnection + ((rotation - otherRotation) + 4)) % 4);
            }

            if (hitBlock.buildingConnections[(int) checkConnection].connection == NullConnection) return new BuildingConnection() {connection = Random.Range(10, int.MaxValue - 1)};
            return hitBlock.buildingConnections[(int) checkConnection];
        }

        [ContextMenu("Reset Connections")]
        public void ResetConnections()
        {
            buildingConnections = new BuildingConnection [6];
            lockConnections = false;
        }

        public void RebuildConnections()
        {
            ResetConnections();
            GatherConnections();
        }

        private void OnDrawGizmosSelected()
        {
            void ValidConnectionGizmo(Directions direction, int i)
            {
                Directions oppositeDirection = direction.Opposite();
                Vector3 worldDirection = transform.TransformDirection(direction.ToVector3());
                BuildingConnection buildingConnection = buildingConnections[(int) direction];
                BuildingConnection oppositeConnection = GetConnection(worldDirection, oppositeDirection, i);
                if (oppositeConnection.connection != ~buildingConnection.connection 
                    && (oppositeConnection.symmetrical == false || oppositeConnection.connection != buildingConnection.connection))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(worldDirection, Vector3.one * .5f);
                }
            }

            Gizmos.color = new Color(0f, 1f, 1f, .3f);
            Gizmos.matrix = transform.localToWorldMatrix;

            int rotation = (int)(transform.rotation.eulerAngles.y / 90f);
            rotation %= 4;

            ValidConnectionGizmo(Directions.Right, rotation);
            ValidConnectionGizmo(Directions.Back, rotation);
            ValidConnectionGizmo(Directions.Left, rotation);
            ValidConnectionGizmo(Directions.Forward, rotation);
            ValidConnectionGizmo(Directions.Up, rotation);
            ValidConnectionGizmo(Directions.Down, rotation);

            if (showGizmo)
            {
                Gizmos.color = new Color(1f, 1f, 1f, .3f);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }
}
