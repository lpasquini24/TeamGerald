using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SafeZoneTutorialManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Generator gen;
    public GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gen.power == 80)
        {
            text.SetText("The generator's power will decrease over time. Keep it charged by replenishing it with batteries often.");
            gen.lossRate = 0.5f;
            Destroy(door);
        }
    }
}
