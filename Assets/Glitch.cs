using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class Glitch : MonoBehaviour
{
    public ShaderEffect_CorruptedVram cooldown;
    public bool coolingDown;
    public float waitTime = 30.0f;

    // Update is called once per frame
    void Update()
    {
        if (coolingDown == true)
        {
            //Reduce fill amount over 30 seconds
            cooldown.shift += 1000.0f / waitTime * Time.deltaTime;
        }
    }
}