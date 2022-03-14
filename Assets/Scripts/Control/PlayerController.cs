using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control {

  public class PlayerController : MonoBehaviour {

    void Update() {
      // do player combat actions
      InteractWithCombat();
      // do player movement actions
      InteractWithMovement();
    }

    private void InteractWithCombat() {
      // returns a list of all the hit results
      RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

      foreach(RaycastHit hit in hits) {
        // check if the hit object is a worthy combat component
        if (hit.transform.TryGetComponent(out CombatTarget ct)) {
          // attack on mouse click
          if (Input.GetMouseButtonDown(0)) {
            GetComponent<Fighter>().Attack(ct);
          }
        }
      }
    }

    private void InteractWithMovement() {
      // Input.GetMouseButton returns true as long as a mouse button is clicked
      // use it to allow the player to continuously follow the mouse
      if (Input.GetMouseButton(0)) {
        MovetoCursor();
      }
    }

    private void MovetoCursor() {
      RaycastHit hit;

      bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

      if (hasHit) {
        // destination for the player's nav mesh agent
        // player moves towards the point where the mouse clicked occured
        GetComponent<Mover>().MoveTo(hit.point);
      }
    }

    private static Ray GetMouseRay() {
      // ray from camera origin to point where mouse has clicked
      // this ray will help us find points of intersection in our scene
      return Camera.main.ScreenPointToRay(Input.mousePosition);

      // // to see the line from the camera to the mouse click position
      // Debug.DrawRay(ray.origin, ray.direction * 100, Color.white);
    }
  }
}