using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slashing : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform[] blocks;
    public Transform Sword;
    public int index;
    public int index2;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        if (index == 0)
        {
            Sword.position = blocks[index2].position;
            Sword.rotation = blocks[index2].rotation;
        }
        else
        {
        Sword.position = points[index].position;
        Sword.rotation = points[index].rotation;
        }
        
    }
}
