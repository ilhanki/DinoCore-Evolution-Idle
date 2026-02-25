using DinoCore.Core;
using DinoCore.Managers;
using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Rebirth confirmation panel with preview of gains.
    /// </summary>
    public class RebirthPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button rebirthButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text currentMultiplierText;
        [SerializeField] private TMP_Text gainPreviewText;
        [SerializeField] private TMP_Text newMultiplierText;
        [SerializeField] private TMP_Text rebirthCountText;

        private void Awake()
        {
            if (rebirthButton != null)
                rebirthButton.onClick.AddListener(DoRebirth);
            
            // Fix: Use OnCancel instead of Close directly to prevent infinite loop
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
            // Switch back to Mine tab explicitly
            if (MainNavigation.Instance != null)
                MainNavigation.Instance.SwitchToMine();
            else
                Close(); // Fallback if navigation missing
        }

        private void RefreshPreview()
        {
            var rm = RebirthManager.Instance;
            if (rm == null) return;

            if (rebirthButton != null)
                rebirthButton.interactable = rm.CanRebirth;

            if (currentMultiplierText != null)
                currentMultiplierText.text = $"Mevcut Çarpan: x{rm.RebirthMultiplier:F0}";

            double potential = rm.PotentialRebirthCurrency;
            if (gainPreviewText != null)
            {
                if (!rm.CanRebirth && CurrencyManager.Instance != null)
                {
                    double required = rm.RequiredTotalIncome;
                    double current = CurrencyManager.Instance.TotalMoneyEarned;
                    gainPreviewText.text = $"Gerekli: {BigNumberFormatter.Format(required)} (Şu an: {BigNumberFormatter.Format(current)})";
                }
                else
                {
                    gainPreviewText.text = $"Kazanılacak: {BigNumberFormatter.Format(potential)} Tech";
                }
            }

            if (newMultiplierText != null)
            {
                double newMult = rm.RebirthMultiplier + (rm.CanRebirth ? rm.RebirthCountIncrement : 0);
                newMultiplierText.text = $"Yeni Çarpan: x{newMult:F0}";
            }

            if (rebirthCountText != null)
                rebirthCountText.text = $"Toplam Rebirth: {rm.RebirthCount}";
        }

        private void DoRebirth()
        {
            if (RebirthManager.Instance.TryRebirth())
                Close();
        }
    }
}
