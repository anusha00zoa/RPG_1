using UnityEngine;

namespace RPG.UI {
    public class DamageTextSpawner : MonoBehaviour {

        [SerializeField] DamageText damageTextPrefab = null;


        public void Spawn(float damageAmount) {
            DamageText instance = Instantiate<DamageText>(damageTextPrefab, transform);

            // set the text
            instance.GetComponent<DamageText>().SetValue(damageAmount);
        }

    }
}
