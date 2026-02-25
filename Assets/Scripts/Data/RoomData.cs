using UnityEngine;

namespace DinoCore.Data
{
    public enum DinoType
    {
        RoomIncome,
        RoomSpeed,
        TechIncome,
        GlobalIncome
    }

    /// <summary>
    /// ScriptableObject that defines a room template.
    /// Create via Assets → Create → DinoCore → Room Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRoom", menuName = "DinoCore/Room Data")]
    public class RoomData : ScriptableObject
    {
        [Header("Identity")]
        public string roomId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public Sprite background;

        [Header("Economy")]
        public double baseIncome = 1;
        public double baseCost = 50;
        public double unlockCost = 100;
        public float costExponent = 1.18f;
        public float levelMultiplierStep = 0.15f;

        [Header("Production")]
        public float baseProductionTime = 10f;
        public double baseStorageCapacity = 100;
        public float storageGrowthPerLevel = 0.10f;

        [Header("Visuals")]
        public Color themeColor = Color.white;
        public int sortOrder;
    }
}
