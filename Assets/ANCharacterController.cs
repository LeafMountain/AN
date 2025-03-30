using Core;
using StarterAssets;
using UnityEngine;

public class ANCharacterController : Actor {
    public float speed = 1f;
    public CharacterController characterController;
    public StarterAssetsInputs playerInputs;

    private CameraController cameraController;

    protected override void Start() {
        base.Start();
        cameraController = GameManager.CameraController;
    }

    protected override void Update() {
        base.Update();

        var x = playerInputs.move.x;
        var y = playerInputs.move.y;

        Debug.Log(playerInputs.move);
        Debug.Log(playerInputs.look);

        var moveDirection = new Vector3(x, 0, y);

        Move(moveDirection, Time.deltaTime);

        characterController.Move(Vector3.down * -Physics.gravity.y);
    }

    private void Move(Vector3 direction, float deltaTime) {
        var cameraForward = cameraController.camera.transform.forward;
        var cameraRight = cameraController.camera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // direction = cameraController.camera.transform.TransformDirection(direction);
        direction = cameraForward * direction.z + cameraRight * direction.x;

        characterController.Move(direction.normalized * (speed * deltaTime));
    }
}