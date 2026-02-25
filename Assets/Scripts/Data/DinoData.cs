using UnityEngine;

namespace DinoCore.Data
{
    /// <summary>
    /// ScriptableObject that defines a dinosaur template.
    /// Create via Assets → Create → DinoCore → Dino Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDino", menuName = "DinoCore/Dino Data")]
    public class DinoData : ScriptableObject
    {
        [Header("Identity")]
        public string dinoId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public Sprite idleSprite;

        [Header("Bonus Type")]
        public DinoType dinoType;
        [Tooltip("Which room index this dino affects (0 = Room 1, 1 = Room 2...). -1 = global/not room-specific.")]
        public int targetRoomIndex = -1;

        [Header("Progression")]
        [Tooltip("Position in the sequential unlock list. Lower = earlier.")]
        public int sortOrder;

        [Header("Economy")]
        public double baseCost = 100;
        public float costExponent = 1.15f;

        [Header("Bonus Values")]
        [Tooltip("Base bonus when first purchased (e.g. 0.5 = +50% or 0.3 = 30% speed reduction)")]
        public float baseBonus = 0.10f;

        [Header("Rarity")]
        public DinoRarity rarity = DinoRarity.Common;
    }

    public enum DinoRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
