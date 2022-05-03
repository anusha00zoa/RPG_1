using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes {

    public class HealthBar : MonoBehaviour {

        [SerializeField] Health health = null;
        [SerializeField] RectTransform foregroundImage = null;
        [SerializeField] Canvas healthBarRootCanvas = null;

        void Update() {
            // disable health bar if dead
            if (Mathf.Approximately(health.GetHealthFraction(), 0) || Mathf.Approximately(health.GetHealthFraction(), 1)) {
                healthBarRootCanvas.enabled = false;
                return;
            }

            healthBarRootCanvas.enabled = true;
            foregroundImage.localScale = new Vector3(health.GetHealthFraction(), 1, 1);
        }
    }
}
