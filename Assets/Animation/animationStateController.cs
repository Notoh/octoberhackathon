﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    public static Animator animator;
    int point;
    bool WhichAction;
    public Transform[] Coordinates;
    public GameObject Sword;
    private float delay;
    private const float delayAmount = 0.100f;
    public static int position;
    public static bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
   
    }

    // Update is called once per frame
   
    


    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        if (Random.Range(0,2) == 1) {
            WhichAction = true; // jab is done
        }else { WhichAction = false; } // block is done
 

        point = Random.Range(1, 9);
        if (Input.GetKey("m") || WhichAction)
        {
            animator.SetBool("Jab", true);
            isHit = true;
            point = Random.Range(1, 10);
            Sword.transform.position = Coordinates[point-1].position;
            position = point - 1;
            Sword.transform.rotation = Coordinates[point-1].rotation;
 
        }
         delay = delayAmount;
        if (Input.GetKey("b") || !WhichAction)
        {
            animator.SetBool("Block", true);
            isHit = false;
        }
        /*else
        {
            animator.SetBool("Jab", false);
            animator.SetBool("Slash", false);
            animator.SetBool("Block", false);
        }*/
    }
}
