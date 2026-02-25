using UnityEngine;
using TMPro;

namespace DinoCore.UI
{
    /// <summary>
    /// Simple floating text that moves up and fades out.
    /// Attach to a prefab with TMP_Text.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 100f;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private float lifetime = 1f;

        private TMP_Text tmpText;
        private Color originalColor;
        private float elapsed;
        private RectTransform rectTransform;

        private void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            originalColor = tmpText.color;
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            // Move up
            rectTransform.anchoredPosition += Vector2.up * (moveSpeed * Time.deltaTime);

            // Fade out
            float alpha = Mathf.Lerp(1f, 0f, elapsed * fadeSpeed / lifetime);
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            if (elapsed >= lifetime)
                Destroy(gameObject);
        }
    }
}
