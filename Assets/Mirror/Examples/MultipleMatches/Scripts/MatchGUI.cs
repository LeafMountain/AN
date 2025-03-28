using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleMatch
{
    public class MatchGUI : MonoBehaviour
    {
        Guid matchId;

        [Header("GUI Elements")]
        public Image image;
        public Toggle toggleButton;
        public Text matchName;
        public Text playerCount;

        [Header("Diagnostics")]
        [ReadOnly, SerializeField] internal CanvasController canvasController;

        public void Awake()
        {
#if UNITY_2022_2_OR_NEWER
            canvasController = FindAnyObjectByType<CanvasController>();
#else
            // Deprecated in Unity 2023.1
            canvasController = GameObject.FindObjectOfType<CanvasController>();
#endif
        }

        [ClientCallback]
        public void OnToggleClicked(bool isOn)
        {
            canvasController.SelectMatch(isOn ? matchId : Guid.Empty);
            image.color = isOn ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.2f);
        }

        [ClientCallback]
        public Guid GetMatchId() => matchId;

        [ClientCallback]
        public void SetMatchInfo(MatchInfo infos)
        {
            matchId = infos.matchId;
            matchName.text = $"Match {infos.matchId.ToString().Substring(0, 8)}";
            playerCount.text = $"{infos.players} / {infos.maxPlayers}";
        }
    }
}
