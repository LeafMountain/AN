using StatefulUI.Runtime.Core;
using UnityEngine;

namespace UI {
    public class UI : MonoBehaviour {
        [SerializeField] private StatefulComponent view;
        public UIPlayerInventory PlayerInventory = new();

        public void Init() {
            // PlayerInventory.Init(view);


            // Canvas canvas = GetComponent<Canvas>();
            // canvas.worldCamera = GameManager.CameraController.camera;
            // canvas.planeDistance = 1f;
        }
    }
}