using DinoCore.Data;
using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Individual tech upgrade item in the tech panel.
    /// Attach this to the tech item prefab.
    /// </summary>
    public class TechItemUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text bonusText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Image icon;

        private TechRuntime tech;

        public void Setup(TechRuntime techRuntime)
        {
            tech = techRuntime;

            if (nameText != null) nameText.text = tech.data.displayName;
            if (icon != null && tech.data.icon != null) icon.sprite = tech.data.icon;

            if (upgradeButton != null)
                upgradeButton.onClick.AddListener(OnUpgrade);

            Refresh();
        }

        private void OnUpgrade()
        {
            TechManager.Instance.TryUpgrade(tech.data.techId);
            Refresh();
        }

        private void Refresh()
        {
            if (tech == null) return;

            if (levelText != null)
                levelText.text = tech.IsMaxed ? "MAX" : $"Lv. {tech.level}";

            if (costText != null)
                costText.text = tech.IsMaxed ? "-" : BigNumberFormatter.Format(tech.UpgradeCost);

            if (bonusText != null)
                bonusText.text = $"+{tech.CurrentBonus:P0}";

            if (upgradeButton != null)
                upgradeButton.interactable = !tech.IsMaxed &&
                    Managers.CurrencyManager.Instance.CanAffordTech(tech.UpgradeCost);
        }
    }
}
