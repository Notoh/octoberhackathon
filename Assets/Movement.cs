using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Vector3 pos;
    float speed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(transform.forward * speed * Time.deltaTime);
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
        }
    }
}
