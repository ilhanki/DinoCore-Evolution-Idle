using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// UI panel for a single room in the scroll view.
    /// Displays room info, production progress, and action buttons.
    /// </summary>
    public class RoomPanel : MonoBehaviour
    {
        [Header("Room Reference")]
        [SerializeField] private Room room;

        [Header("Display")]
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text incomeText;
        [SerializeField] private TMP_Text storedText;
        [SerializeField] private TMP_Text upgradeCostText;
        [SerializeField] private TMP_Text productionTimeText;
        [SerializeField] private Image progressBar;
        [SerializeField] private Image roomIcon;

        [Header("Buttons")]
        [SerializeField] private Button collectButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button autoCollectorButton;
        [SerializeField] private Button unlockButton;


        [Header("Panels")]
        [SerializeField] private GameObject lockedPanel;
        [SerializeField] private GameObject unlockedPanel;
        [SerializeField] private TMP_Text unlockCostText;
        [SerializeField] private TMP_Text autoCollectorCostText;



        [Header("Production Overlay")]
        [SerializeField] private Image productionOverlay;

        private float refreshTimer;
        private const float RefreshInterval = 0.05f; // 20 FPS for smoother progress

        public void Initialize(Room targetRoom)
        {
            room = targetRoom;
            SetupButtons();
            RefreshDisplay();
        }

        /// <summary>
        /// Called externally (e.g. after rebirth) to force an immediate UI refresh.
        /// Works even when the GameObject is inactive.
        /// </summary>
        public void ForceRefresh()
        {
            RefreshDisplay();
        }


        private void SetupButtons()
        {
            if (collectButton != null)
                collectButton.onClick.AddListener(() => { if (room != null) room.Collect(); });

            if (upgradeButton != null)
                upgradeButton.onClick.AddListener(() => { if (room != null) { room.TryUpgrade(); RefreshDisplay(); } });

            if (autoCollectorButton != null)
                autoCollectorButton.onClick.AddListener(() => { if (room != null) { room.TryBuyAutoCollector(); RefreshDisplay(); } });

            if (unlockButton != null)
                unlockButton.onClick.AddListener(() => { if (room != null) { room.TryUnlock(); RefreshDisplay(); } });


        }

        // Lightweight Update – only runs timer
        private void Update()
        {
            if (room == null) return;

            refreshTimer += Time.deltaTime;
            if (refreshTimer < RefreshInterval) return;
            refreshTimer = 0f;

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (room == null || room.Data == null) return;

            bool unlocked = room.IsUnlocked;

            if (lockedPanel != null) lockedPanel.SetActive(!unlocked);
            if (unlockedPanel != null) unlockedPanel.SetActive(unlocked);

            if (!unlocked)
            {
                if (unlockCostText != null)
                    unlockCostText.text = BigNumberFormatter.Format(room.Data.unlockCost);
                return;
            }

            if (roomNameText != null) roomNameText.text = room.Data.displayName;
            if (levelText != null) levelText.text = $"Lv. {room.Level}";
            if (incomeText != null) incomeText.text = BigNumberFormatter.Format(room.IncomePerCycle) + "/cycle";

            // Show stored amount or production status
            if (storedText != null)
            {
                if (room.IsProductionReady)
                    storedText.text = "+" + BigNumberFormatter.Format(room.CurrentStored);
                else
                    storedText.text = "Üretiliyor...";
            }

            if (upgradeCostText != null) upgradeCostText.text = BigNumberFormatter.Format(room.UpgradeCost);
            if (productionTimeText != null) productionTimeText.text = BigNumberFormatter.FormatTime(room.EffectiveProductionTime);

            if (progressBar != null)
                progressBar.fillAmount = room.ProductionProgress;

            // Production timer overlay: dark overlay shrinks as production progresses
            if (productionOverlay != null)
                productionOverlay.fillAmount = 1f - room.ProductionProgress;

            if (roomIcon != null && room.Data.icon != null)
                roomIcon.sprite = room.Data.icon;

            if (autoCollectorButton != null)
                autoCollectorButton.gameObject.SetActive(!room.HasAutoCollector);

            if (autoCollectorCostText != null)
                autoCollectorCostText.text = BigNumberFormatter.Format(room.AutoCollectorCost);



            // Button interactability
            var cm = Managers.CurrencyManager.Instance;
            if (upgradeButton != null)
                upgradeButton.interactable = cm != null && room.UpgradeCost > 0 && cm.CanAfford(room.UpgradeCost);

            if (collectButton != null)
            {
                // User request: Hide collect button if auto collector is purchased
                if (room.HasAutoCollector)
                {
                    collectButton.gameObject.SetActive(false);
                }
                else
                {
                    collectButton.gameObject.SetActive(true);
                    collectButton.interactable = room.IsProductionReady && room.CurrentStored > 0;
                }
            }
        }
    }
}
