using StatefulUI.Runtime.Core;
using UnityEngine;

namespace UI {
    public class UI : MonoBehaviour {
        public UIPlayerInventory PlayerInventory = new();

        [SerializeField] StatefulComponent view;

        public void Init() {
            PlayerInventory.Init(view);


            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = GameManager.CameraController.camera;
            canvas.planeDistance = 1f;
        }
    }
}