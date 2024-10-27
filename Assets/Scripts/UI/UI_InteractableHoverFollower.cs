using System;
using EventManager;
using TMPro;
using UnityEngine;

public class UI_InteractableHoverFollower : MonoBehaviour
{
    [SerializeField]
    private GameObject root;
    [SerializeField]
    private TMP_Text text;
    private IInteractable lookTarget;

    public void OnEnable()
    {
        Events.AddListener(Flag.LookTarget, OnLookTargetUpdated);
    }

    private void OnLookTargetUpdated(object origin, EventArgs eventargs)
    {
        lookTarget = (origin as GameObject)?.GetComponent<IInteractable>();
        root.SetActive(lookTarget != null);
        
        if (lookTarget != null)
        {
            text.SetText(lookTarget.GetPrompt());
        }
    }

    private void LateUpdate()
    {
        var target = GameManager.Instance.localPlayer.interactor.Target;

        if (target)
        {
            transform.position = GameManager.CameraController.camera.WorldToScreenPoint(target.transform.position);
        }
    }
}
