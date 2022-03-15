using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Core {

    public class ActionScheduler : MonoBehaviour {

        IAction currentAction = null;

        public void StartAction(IAction action) {
            // if new action is same as the current action, do nothing
            if (currentAction == action)
                return;

            // if there is no current action to cancel, do nothing
            // else cancel the current action
            if (currentAction !=  null) {
                //Debug.Log("Cancelling action: " + action);
                // call the correct cancel action
                currentAction.Cancel();
            }

            // update the current action
            currentAction = action;

        }
    }
}
