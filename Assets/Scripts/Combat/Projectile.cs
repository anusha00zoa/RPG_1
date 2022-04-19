using System;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat {

    public class Projectile : MonoBehaviour {

        [SerializeField] bool isHoming = true;
        [SerializeField] float speed = 1.0f;
        [SerializeField] float maxLifetime = 10.0f;
        [SerializeField] float lifeAfterImpact = 1.0f;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit = null;

        float damage = 0.0f;

        Health target = null;

        GameObject instigator = null;

        private void Start() {
            transform.LookAt(GetAimLocation());
        }

        void Update() {
            if (target == null)
                return;

            if (isHoming && !target.IsDead())
                transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        // setter function
        public void SetTarget(Health target, GameObject instigator, float damage) {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            // if projectile doesnt hit anything, destroy after maxLifetime seconds
            Destroy(gameObject, maxLifetime);
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();

            if (targetCapsuleCollider == null)
                return target.transform.position;

            return target.transform.position + (Vector3.up * targetCapsuleCollider.height / 2);
        }

        private void OnTriggerEnter(Collider other) {
            // check that we collided with our target
            if (other.gameObject != target.gameObject)
                return;

            // check if target is dead and let projectile move beyond target
            if (target.IsDead())
                return;

            // do damage and then destroy the projectile
            target.TakeDamage(damage, instigator);

            // on impact, projectile should stop moving
            speed = 0.0f;

            // play hit effect if any
            if (hitEffect != null)
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);


            foreach (GameObject toDestroy in destroyOnHit) {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
