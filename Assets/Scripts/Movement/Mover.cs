using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {

  public class Mover : MonoBehaviour {

    [SerializeField]
    public Transform target;

    void Update() {
      // animate our player character
      UpdateAnimator();
    }

    public void MoveTo(Vector3 dest) {
      this.GetComponent<NavMeshAgent>().destination = dest;
    }

    private void UpdateAnimator() {
      // get global velocity from nav mesh agent
      Vector3 velocity = GetComponent<NavMeshAgent>().velocity;

      // convert this into a local value relative to the character
      Vector3 localVelocity = transform.InverseTransformDirection(velocity);

      // we only care about the z which is forward for the character
      float speed = localVelocity.z;

      GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
    }
  }
}
