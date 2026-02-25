using System;
using DinoCore.Data;

namespace DinoCore.Systems
{
    /// <summary>
    /// Runtime data for a single dinosaur instance.
    /// One-time purchase only, no leveling.
    /// </summary>
    [Serializable]
    public class Dino
    {
        public DinoData data;
        public bool isOwned;

        public string DinoId => data.dinoId;
        public DinoType Type => data.dinoType;

        /// <summary>
        /// Bonus value. Returns baseBonus when owned, 0 when not.
        /// </summary>
        public float EffectiveBonus => isOwned ? data.baseBonus : 0f;

        /// <summary>
        /// Cost to purchase this dino (one-time buy).
        /// </summary>
        public double PurchaseCost => data.baseCost;

        public DinoSaveData ToSaveData()
        {
            return new DinoSaveData
            {
                dinoId = DinoId,
                isOwned = isOwned
            };
        }

        public void LoadFromSave(DinoSaveData save)
        {
            isOwned = save.isOwned;
        }
    }
}
