using DinoCore.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Main navigation controller for bottom tab bar.
    /// </summary>
    public class MainNavigation : MonoBehaviour
    {
        [Header("Tab Buttons")]
        [SerializeField] private Button mineTabButton;
        [SerializeField] private Button dinoTabButton;
        [SerializeField] private Button techTabButton;
        [SerializeField] private Button rebirthTabButton;
        [SerializeField] private Button planetTabButton;

        [Header("Panels")]
        [SerializeField] private GameObject minePanel;
        [SerializeField] private DinoPanel dinoPanel;
        [SerializeField] private TechPanel techPanel;
        [SerializeField] private RebirthPanel rebirthPanel;
        [SerializeField] private PlanetPanel planetPanel;

        public static MainNavigation Instance { get; private set; }

        /// <summary>
        /// Opens the Dino panel. (Kept for backward compat with any callers.)
        /// </summary>
        public void OpenDinoPanelForRoom(string roomId)
        {
            SwitchTo(Tab.Dino);
            dinoPanel?.Open();
        }

        private void Awake()
        {
            Instance = this;

            if (mineTabButton != null)
                mineTabButton.onClick.AddListener(() => SwitchTo(Tab.Mine));
            if (dinoTabButton != null)
                dinoTabButton.onClick.AddListener(() => SwitchTo(Tab.Dino));
            if (techTabButton != null)
                techTabButton.onClick.AddListener(() => SwitchTo(Tab.Tech));
            if (rebirthTabButton != null)
                rebirthTabButton.onClick.AddListener(() => SwitchTo(Tab.Rebirth));
            if (planetTabButton != null)
                planetTabButton.onClick.AddListener(() => SwitchTo(Tab.Planet));

            // Auto-switch to Mine tab after rebirth so rooms are visible immediately
            GameEvents.OnRebirth += OnRebirthHandler;
            // Same for planet unlock
            GameEvents.OnPlanetUnlocked += OnPlanetUnlockedHandler;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            GameEvents.OnRebirth -= OnRebirthHandler;
            GameEvents.OnPlanetUnlocked -= OnPlanetUnlockedHandler;
        }

        private void OnRebirthHandler(double _) => SwitchTo(Tab.Mine);
        private void OnPlanetUnlockedHandler(int _) => SwitchTo(Tab.Mine);

        private void Start()
        {
            SwitchTo(Tab.Mine);
        }

        public void SwitchToMine()
        {
            SwitchTo(Tab.Mine);
        }

        private enum Tab { Mine, Dino, Tech, Rebirth, Planet }

        private void SwitchTo(Tab tab)
        {
            if (minePanel != null) minePanel.SetActive(tab == Tab.Mine);
            if (dinoPanel != null) { if (tab == Tab.Dino) dinoPanel.Open(); else dinoPanel.Close(); }
            if (techPanel != null) { if (tab == Tab.Tech) techPanel.Open(); else techPanel.Close(); }
            if (rebirthPanel != null) { if (tab == Tab.Rebirth) rebirthPanel.Open(); else rebirthPanel.Close(); }
            if (planetPanel != null) { if (tab == Tab.Planet) planetPanel.Open(); else planetPanel.Close(); }
        }
    }
}
