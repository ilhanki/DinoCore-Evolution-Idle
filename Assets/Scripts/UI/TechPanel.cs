using System.Collections.Generic;
using DinoCore.Data;
using DinoCore.Systems;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Tech tree panel showing available upgrades.
    /// </summary>
    public class TechPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject techItemPrefab;

        private readonly List<TechItemUI> items = new();

        public void Open()
        {
            if (panel == null) return;
            RefreshList();
            panel.SetActive(true);
        }

        public void Close()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void RefreshList()
        {
            // Clear old items by destroying children directly
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            items.Clear();

            var allTechs = TechManager.Instance.GetAllTechs();
            foreach (var tech in allTechs)
            {
                if (techItemPrefab == null) break;
                GameObject go = Instantiate(techItemPrefab, contentParent);
                var ui = go.GetComponent<TechItemUI>();
                if (ui != null)
                {
                    ui.Setup(tech);
                    items.Add(ui);
                }
            }
        }
    }
}
