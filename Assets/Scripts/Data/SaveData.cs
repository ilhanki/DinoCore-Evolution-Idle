using System;
using System.Collections.Generic;

namespace DinoCore.Data
{
    /// <summary>
    /// Root save container. Serialised to JSON via SaveManager.
    /// All fields are public for JsonUtility compatibility.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public long lastSaveTimestamp;
        public double money;
        public double totalMoneyEarned;
        public double techCurrency;
        public int rebirthCount;
        public int planetCount;
        public double clickIncome;
        public int totalClicks;
        public List<RoomSaveData> rooms = new();
        public List<DinoSaveData> dinos = new();
        public List<TechSaveData> techs = new();
        public List<string> unlockedPlanets = new();
    }

    [Serializable]
    public class RoomSaveData
    {
        public string roomId;
        public int level;
        public bool isUnlocked;
        public bool hasAutoCollector;
        public double currentStored;
        public float productionTimer;
    }

    [Serializable]
    public class DinoSaveData
    {
        public string dinoId;
        public bool isOwned;
    }

    [Serializable]
    public class TechSaveData
    {
        public string techId;
        public int level;
    }
}
