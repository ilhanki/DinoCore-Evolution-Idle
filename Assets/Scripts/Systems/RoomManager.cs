using System.Collections.Generic;
using DinoCore.Data;
using DinoCore.UI;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Manages all rooms in the game. Spawns room GameObjects from templates.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        [Header("Room Templates (assign all RoomData SOs)")]
        [SerializeField] private List<RoomData> roomTemplates = new();

        [Header("Containers")]
        [SerializeField] private Transform logicContainer; // Persistent container for Logic (Room.cs)
        [SerializeField] private Transform roomContainer;  // UI Container (ScrollContent)

        [Header("Prefabs")]
        [SerializeField] private GameObject roomLogicPrefab; // Prefab with Room.cs
        [SerializeField] private GameObject roomPanelPrefab; // Prefab with RoomPanel.cs

        private readonly Dictionary<string, Room> rooms = new();
        private readonly List<RoomPanel> roomPanels = new();

        public IReadOnlyDictionary<string, Room> Rooms => rooms;

        /// <summary>
        /// All rooms as a sorted list (by sortOrder). Used by DinoManager for room index lookup.
        /// </summary>
        public List<Room> AllRooms
        {
            get
            {
                var list = new List<Room>(rooms.Values);
                list.Sort((a, b) => a.Data.sortOrder.CompareTo(b.Data.sortOrder));
                return list;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Called by GameManager after save is loaded or on first launch.
        /// </summary>
        public void InitializeRooms()
        {
            // Clear existing
            foreach (Transform child in logicContainer) Destroy(child.gameObject);
            foreach (Transform child in roomContainer) Destroy(child.gameObject);
            rooms.Clear();
            roomPanels.Clear();

            // Sort by sortOrder for consistent display
            roomTemplates.Sort((a, b) => a.sortOrder.CompareTo(b.sortOrder));

            foreach (var template in roomTemplates)
            {
                if (rooms.ContainsKey(template.roomId)) continue;

                // 1. Logic (Persistent)
                GameObject logicObj = Instantiate(roomLogicPrefab, logicContainer);
                logicObj.name = $"RoomLogic_{template.roomId}";
                Room room = logicObj.GetComponent<Room>();
                if (room == null)
                {
                    Debug.LogError($"[RoomManager] Logic prefab missing Room script!");
                    continue;
                }
                room.Initialize(template);
                rooms[template.roomId] = room;

                // 2. UI (Visual)
                if (roomPanelPrefab != null && roomContainer != null)
                {
                    GameObject panelObj = Instantiate(roomPanelPrefab, roomContainer);
                    panelObj.name = $"RoomPanel_{template.roomId}";
                    RoomPanel panel = panelObj.GetComponent<RoomPanel>();
                    if (panel != null)
                    {
                        panel.Initialize(room);
                        roomPanels.Add(panel);
                    }
                }
            }
        }

        public Room GetRoom(string roomId)
        {
            rooms.TryGetValue(roomId, out Room room);
            return room;
        }

        public int UnlockedRoomCount
        {
            get
            {
                int count = 0;
                foreach (var kvp in rooms)
                    if (kvp.Value.IsUnlocked) count++;
                return count;
            }
        }

        /// <summary>
        /// Total income per second across all active rooms (for UI display).
        /// </summary>
        public double TotalIncomePerSecond
        {
            get
            {
                double total = 0;
                foreach (var kvp in rooms)
                    if (kvp.Value.IsUnlocked)
                        total += kvp.Value.IncomePerSecond;
                return total;
            }
        }

        /// <summary>
        /// Simulate offline income for all rooms.
        /// </summary>
        public double SimulateAllOffline(double seconds)
        {
            double totalCollected = 0;
            foreach (var kvp in rooms)
                totalCollected += kvp.Value.SimulateOffline(seconds);
            return totalCollected;
        }

        /// <summary>
        /// Reset all rooms for rebirth.
        /// </summary>
        public void ResetAllForRebirth()
        {
            foreach (var kvp in rooms)
                kvp.Value.ResetForRebirth();
        }

        /// <summary>
        /// Force all RoomPanel UIs to refresh immediately (e.g. after rebirth).
        /// </summary>
        public void RefreshAllPanels()
        {
            foreach (var panel in roomPanels)
            {
                if (panel != null) panel.ForceRefresh();
            }
        }

        // ── Save / Load ────────────────────────────────────────
        public void WriteToSave(SaveData data)
        {
            data.rooms.Clear();
            foreach (var kvp in rooms)
                data.rooms.Add(kvp.Value.ToSaveData());
        }

        public void LoadFromSave(SaveData data)
        {
            foreach (var roomSave in data.rooms)
            {
                if (rooms.TryGetValue(roomSave.roomId, out Room room))
                    room.LoadFromSave(roomSave);
            }
        }
    }
}
