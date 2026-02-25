using DinoCore.Managers;
using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;

namespace DinoCore.UI
{
    /// <summary>
    /// Displays offline income popup when the player returns.
    /// </summary>
    public class OfflineIncomePopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private TMP_Text timeAwayText;
        [SerializeField] private TMP_Text incomeText;

        private void Awake()
        {
            // Subscribe in Awake (not OnEnable) so it works even when the GO starts inactive.
            if (OfflineIncomeSystem.Instance != null)
                OfflineIncomeSystem.Instance.OnOfflineIncomeReady += ShowPopup;
        }

        private void OnDestroy()
        {
            if (OfflineIncomeSystem.Instance != null)
                OfflineIncomeSystem.Instance.OnOfflineIncomeReady -= ShowPopup;
        }

        private void Start()
        {
            if (popupPanel != null)
                popupPanel.SetActive(false);

            // Fallback: if offline income was calculated before Awake ran (rare ordering edge case),
            // catch it here using the cached values on OfflineIncomeSystem.
            if (OfflineIncomeSystem.Instance != null &&
                OfflineIncomeSystem.Instance.LastOfflineIncome > 0)
            {
                ShowPopup(OfflineIncomeSystem.Instance.LastOfflineSeconds,
                          OfflineIncomeSystem.Instance.LastOfflineIncome);
            }
        }

        private void ShowPopup(double seconds, double income)
        {
            if (income <= 0 || popupPanel == null) return;

            double maxSeconds = 0;
            if (Managers.GameManager.Instance != null)
                maxSeconds = Managers.GameManager.Instance.Config.maxOfflineHours * 3600;

            if (maxSeconds > 0)
            {
                timeAwayText.text = $"Uzakta geçen süre: {BigNumberFormatter.FormatTime(seconds)} / {BigNumberFormatter.FormatTime(maxSeconds)}";
            }
            else
            {
                timeAwayText.text = $"Uzakta geçen süre: {BigNumberFormatter.FormatTime(seconds)}";
            }
            incomeText.text = $"Kazanılan: {BigNumberFormatter.Format(income)}";
            popupPanel.SetActive(true);
        }

        public void ClosePopup()
        {
            if (popupPanel != null)
                popupPanel.SetActive(false);
        }
    }
}
