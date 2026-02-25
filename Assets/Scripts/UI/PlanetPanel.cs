using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Planet unlock confirmation panel.
    /// </summary>
    public class PlanetPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text currentMultiplierText;
        [SerializeField] private TMP_Text nextPlanetText;
        [SerializeField] private TMP_Text requirementText;
        [SerializeField] private TMP_Text planetCountText;
        [SerializeField] private Image planetIcon;

        private void Awake()
        {
            if (unlockButton != null)
                unlockButton.onClick.AddListener(DoUnlock);
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancel);
        }

        public void Open()
        {
            if (panel == null) return;
            RefreshPreview();
            panel.SetActive(true);
        }

        public void Close()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnCancel()
        {
            if (MainNavigation.Instance != null)
                MainNavigation.Instance.SwitchToMine();
            else
                Close();
        }

        private void RefreshPreview()
        {
            var pm = PlanetManager.Instance;
            if (pm == null) return;

            if (unlockButton != null)
                unlockButton.interactable = pm.CanUnlockNextPlanet;

            if (currentMultiplierText != null)
                currentMultiplierText.text = $"Gezegen Çarpanı: x{pm.PlanetMultiplier:F0}";

            if (planetCountText != null)
                planetCountText.text = $"Açılan Gezegen: {pm.PlanetCount}";

            var next = pm.NextPlanet;
            if (next != null)
            {
                if (nextPlanetText != null)
                    nextPlanetText.text = next.displayName;
                if (planetIcon != null && next.icon != null)
                    planetIcon.sprite = next.icon;
                if (requirementText != null)
                    requirementText.text = $"Gerekli Rebirth: {pm.RequiredRebirthCountForNextPlanet}";
            }
        }

        private void DoUnlock()
        {
            if (PlanetManager.Instance.TryUnlockPlanet())
                Close();
        }
    }
}
