using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour {

    [SerializeField]
    public Transform target;

    void Start() {

    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MovetoCursor();
        }
    }

    private void MovetoCursor() {
        // ray from camera origin to point where mouse has clicked
        // this ray will help us find points of intersection in our scene
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        // // to see the line from the camera to the mouse click position
        // Debug.DrawRay(ray.origin, ray.direction * 100, Color.white);

        bool hasHit = Physics.Raycast(ray, out hit);

        if (hasHit) {
            // destination for the player's navmesh agent
            // player moves towards the point where the mouse clicked occured
            this.GetComponent<NavMeshAgent>().destination = hit.point;
        }
    }
}
