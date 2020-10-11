using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthEffects : MonoBehaviour
{
    public static int Health;
    public static int enemyhealth;
    public ShaderEffect_BleedingColors distort;
    public Text HPText;
    public Text EnemyText;
    // Start is called before the first frame update
    void Start()
    {
        Health = 100;
        HPText.text = "HP: " + Health.ToString ();
        enemyhealth = 100;
        EnemyText.text = "HP: " + Health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Health == 0)
        {
            distort.intensity = distort.intensity + 10;
            distort.shift = distort.intensity + 10;
        }
        else
        {
           distort.intensity = 10f - Health / 10;
            distort.shift = 10f - Health / 10;
        }

        HPText.text = "HP: " + Health.ToString();
        EnemyText.text = "HP: " + enemyhealth.ToString();
    }
}
