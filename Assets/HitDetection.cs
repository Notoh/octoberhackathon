using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private bool one;
    private float time = 0;
    private float enemytime = 0;
    void LateUpdate()
    {
        if (HealthEffects.enemyhealth <= 0)
        {
            one = true;
            return;
        }
       
        PlayerController.Action playerAction = PlayerController.action;
        bool isHit = animationStateController.isHit;
        int positionEnemy = animationStateController.position;
        
        if ((playerAction.actionType == PlayerController.Action.ActionType.Jab ||
             playerAction.actionType == PlayerController.Action.ActionType.Swing) && isHit)
        {
            if (enemytime > 0)
            {
                enemytime -= Time.deltaTime;
                return;
            }
            HealthEffects.enemyhealth -= 10;
            enemytime = 1f;
            return;
        }
        else if ((playerAction.actionType == PlayerController.Action.ActionType.Trip ||
                  playerAction.actionType == PlayerController.Action.ActionType.Reset) && isHit)
        {
            
            if (time > 0)
            {
                time -= Time.deltaTime;
                return;
            }
            HealthEffects.Health -= 10;
            time = 1f;
        }
     
    }
    void onGUI()
    {

        if (one)
        {
            GUI.Label(new Rect(0, 0, 100, 100), "you won");
        }
    }
}