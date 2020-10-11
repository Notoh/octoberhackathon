using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anything : MonoBehaviour
{
    Vector3 pos;
    float speed = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
    pos = transform.position;
    }

// Update is called once per frame
void Update()
{
    /*if (Input.GetKeyDown(KeyCode.W))
    {
        pos.x += speed;// * Time.deltaTime;
    }
    if (Input.GetKeyUp(KeyCode.A))
    {
        pos.x -= speed;// * Time.deltaTime;
    }
    if (Input.GetKeyUp(KeyCode.S))
    {
        pos.z += speed;// * Time.deltaTime;
    }
    if (Input.GetKeyUp(KeyCode.D))
    {
        pos.z -= speed;// * Time.deltaTime;
    }*/
    pos += transform.forward * speed * Time.deltaTime;
}
}
