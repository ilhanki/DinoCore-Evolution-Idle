using DinoCore.Data;
using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Full-panel single-card dino purchase UI.
    /// Shows one dino at a time. Buy → next appears. No levels, no owned list.
    /// </summary>
    public class DinoPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;

        [Header("Dino Card")]
        [SerializeField] private Image dinoIcon;
        [SerializeField] private TMP_Text dinoName;
        [SerializeField] private TMP_Text dinoBonusDesc;
        [SerializeField] private TMP_Text dinoCost;
        [SerializeField] private TMP_Text dinoRarity;
        [SerializeField] private Image rarityBorder;
        [SerializeField] private Button buyButton;
        [SerializeField] private TMP_Text buyButtonText;

        [Header("Progress")]
        [SerializeField] private TMP_Text progressText;

        [Header("All Done")]
        [SerializeField] private GameObject allDonePanel;
        [SerializeField] private GameObject cardPanel;

        // Rarity colors
        private static readonly Color CommonColor = new Color(0.7f, 0.7f, 0.7f);
        private static readonly Color UncommonColor = new Color(0.2f, 0.8f, 0.2f);
        private static readonly Color RareColor = new Color(0.2f, 0.5f, 1f);
        private static readonly Color EpicColor = new Color(0.6f, 0.2f, 0.9f);
        private static readonly Color LegendaryColor = new Color(1f, 0.65f, 0f);

        public void Open()
        {
            if (panel == null) return;
            panel.SetActive(true);
            Refresh();
        }

        public void Close()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        public void Refresh()
        {
            if (DinoManager.Instance == null) return;
            RefreshCard();
            RefreshProgress();
        }

        private void Update()
        {
            // Live-update buy button interactability
            if (panel == null || !panel.activeSelf) return;
            if (DinoManager.Instance == null) return;

            var current = DinoManager.Instance.CurrentDino;
            if (current != null && buyButton != null)
            {
                bool canAfford = Managers.CurrencyManager.Instance != null &&
                                 Managers.CurrencyManager.Instance.CanAfford(current.PurchaseCost);
                buyButton.interactable = canAfford;
            }
        }

        private void RefreshCard()
        {
            var current = DinoManager.Instance.CurrentDino;

            if (current == null)
            {
                // All dinos purchased!
                if (cardPanel != null) cardPanel.SetActive(false);
                if (allDonePanel != null) allDonePanel.SetActive(true);
                return;
            }

            if (cardPanel != null) cardPanel.SetActive(true);
            if (allDonePanel != null) allDonePanel.SetActive(false);

            // Icon
            if (dinoIcon != null)
            {
                if (current.data.icon != null)
                    dinoIcon.sprite = current.data.icon;
            }

            // Name
            if (dinoName != null)
                dinoName.text = current.data.displayName;

            // Bonus description (what this dino does)
            if (dinoBonusDesc != null)
                dinoBonusDesc.text = GetBonusDescription(current);

            // Cost
            if (dinoCost != null)
                dinoCost.text = BigNumberFormatter.Format(current.PurchaseCost);

            // Rarity
            if (dinoRarity != null)
                dinoRarity.text = GetRarityDisplayName(current.data.rarity);

            if (rarityBorder != null)
                rarityBorder.color = GetRarityColor(current.data.rarity);

            // Buy button
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                string id = current.DinoId;
                buyButton.onClick.AddListener(() =>
                {
                    DinoManager.Instance.TryPurchaseDino(id);
                    Refresh();
                });

                bool canAfford = Managers.CurrencyManager.Instance != null &&
                                 Managers.CurrencyManager.Instance.CanAfford(current.PurchaseCost);
                buyButton.interactable = canAfford;
            }

            if (buyButtonText != null)
                buyButtonText.text = $"Satın Al: {BigNumberFormatter.Format(current.PurchaseCost)}";
        }

        private void RefreshProgress()
        {
            if (progressText == null) return;
            int owned = DinoManager.Instance.OwnedCount;
            int total = DinoManager.Instance.AllDinos.Count;
            progressText.text = $"Dino İlerlemesi: {owned}/{total}";
        }

        // ── Helpers ───────────────────────────────────────────
        private string GetBonusDescription(Dino dino)
        {
            string bonusPercent = $"+{dino.data.baseBonus:P0}";
            string roomName = GetRoomName(dino.data.targetRoomIndex);

            switch (dino.Type)
            {
                case DinoType.RoomIncome:
                    return $"{roomName} Gelir Çarpanı {bonusPercent}";
                case DinoType.RoomSpeed:
                    return $"{roomName} Hız Çarpanı {bonusPercent}";
                case DinoType.TechIncome:
                    return $"Tech Çarpanı {bonusPercent}";
                case DinoType.GlobalIncome:
                    return $"Genel Gelir Çarpanı {bonusPercent}";
                default:
                    return $"Bonus: {bonusPercent}";
            }
        }

        private string GetRoomName(int roomIndex)
        {
            if (roomIndex < 0) return "Global";
            if (RoomManager.Instance != null)
            {
                var rooms = RoomManager.Instance.AllRooms;
                if (roomIndex < rooms.Count && rooms[roomIndex].Data != null)
                    return rooms[roomIndex].Data.displayName;
            }
            return $"Oda {roomIndex + 1}";
        }

        private string GetRarityDisplayName(DinoRarity rarity)
        {
            switch (rarity)
            {
                case DinoRarity.Common: return "Sıradan";
                case DinoRarity.Uncommon: return "Nadir Değil";
                case DinoRarity.Rare: return "Nadir";
                case DinoRarity.Epic: return "Epik";
                case DinoRarity.Legendary: return "Efsanevi";
                default: return rarity.ToString();
            }
        }

        private Color GetRarityColor(DinoRarity rarity)
        {
            switch (rarity)
            {
                case DinoRarity.Uncommon: return UncommonColor;
                case DinoRarity.Rare: return RareColor;
                case DinoRarity.Epic: return EpicColor;
                case DinoRarity.Legendary: return LegendaryColor;
                default: return CommonColor;
            }
        }
    }
}
