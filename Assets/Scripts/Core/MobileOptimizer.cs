using UnityEngine;

namespace DinoCore.Core
{
    /// <summary>
    /// Sets up mobile-optimized runtime settings on game start.
    /// Attach to the same [GAME_MANAGER] GameObject.
    /// </summary>
    public class MobileOptimizer : MonoBehaviour
    {
        [Header("Frame Rate")]
        [SerializeField] private int targetFrameRate = 60;

        [Header("Sleep")]
        [SerializeField] private bool preventSleep = true;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = 0;

            if (preventSleep)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Reduce physics overhead (not used in this 2D idle game)
            Physics.autoSimulation = false;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }
    }
}
