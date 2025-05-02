using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined {
    public GameObject PlayerPrefab;
    public CameraController cameraController;

    public void PlayerJoined(PlayerRef player) {
        if (player == Runner.LocalPlayer) {
            var spawnedPlayer = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            // cameraController.Character = spawnedPlayer.gameObject;
            // SceneManager.LoadScene("PatriksPlayground", LoadSceneMode.Additive);
        }
    }
}