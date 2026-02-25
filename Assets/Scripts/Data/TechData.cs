using UnityEngine;

namespace DinoCore.Data
{
    public enum TechCategory
    {
        Income,
        Speed,
        Storage,
        RebirthBoost,
        ClickPower,
        DinoEfficiency
    }

    /// <summary>
    /// ScriptableObject that defines a technology upgrade.
    /// Create via Assets → Create → DinoCore → Tech Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTech", menuName = "DinoCore/Tech Data")]
    public class TechData : ScriptableObject
    {
        [Header("Identity")]
        public string techId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public TechCategory category;

        [Header("Economy")]
        public double baseCost = 10;
        public float costExponent = 1.25f;
        public int maxLevel = 50;

        [Header("Effect")]
        public float bonusPerLevel = 0.05f;

        [Header("Unlock")]
        public int requiredRebirthCount;
    }
}
