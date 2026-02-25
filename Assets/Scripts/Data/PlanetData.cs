using UnityEngine;

namespace DinoCore.Data
{
    /// <summary>
    /// ScriptableObject that defines a planet for upper prestige.
    /// Create via Assets → Create → DinoCore → Planet Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPlanet", menuName = "DinoCore/Planet Data")]
    public class PlanetData : ScriptableObject
    {
        [Header("Identity")]
        public string planetId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public Sprite background;

        [Header("Requirements")]
        public int requiredRebirthCount = 5;

        [Header("Bonus")]
        public float multiplierBonus = 0.25f;

        [Header("Visuals")]
        public Color themeColor = Color.cyan;
        public int sortOrder;
    }
}
