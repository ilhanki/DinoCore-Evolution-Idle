using DinoCore.Core;
using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Click button with visual feedback.
    /// </summary>
    public class ClickButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text clickIncomeText;
        [SerializeField] private Transform feedbackParent;
        [SerializeField] private GameObject floatingTextPrefab;

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnEnable()
        {
            GameEvents.OnClick += ShowFeedback;
        }

        private void OnDisable()
        {
            GameEvents.OnClick -= ShowFeedback;
        }

        private void Start()
        {
            UpdateClickText();
        }

        private void OnClick()
        {
            ClickManager.Instance.DoClick();
            UpdateClickText();
        }

        private void UpdateClickText()
        {
            if (clickIncomeText != null && ClickManager.Instance != null)
                clickIncomeText.text = "+" + BigNumberFormatter.Format(ClickManager.Instance.ClickIncome);
        }

        private void ShowFeedback(double amount)
        {
            if (floatingTextPrefab == null || feedbackParent == null) return;

            GameObject go = Instantiate(floatingTextPrefab, feedbackParent);
            var tmp = go.GetComponent<TMP_Text>();
            if (tmp != null)
                tmp.text = "+" + BigNumberFormatter.Format(amount);

            // FloatingText kendi ömrünü yönetiyor, ekstra Destroy'a gerek yok
        }
    }
}
