using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement {
    public class Fader : MonoBehaviour {

        CanvasGroup canvasGroup;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() {
            canvasGroup.alpha = 1.0f;
        }

        IEnumerator FadeOutIn() {
            yield return FadeOut(3.0f);
            yield return FadeIn(3.0f);
        }

        public IEnumerator FadeOut(float time) {
            while (canvasGroup.alpha < 1.0f) {
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time) {
            while (canvasGroup.alpha > 0.0f) {
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
