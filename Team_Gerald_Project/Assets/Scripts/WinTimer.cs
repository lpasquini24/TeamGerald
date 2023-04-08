using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTimer : MonoBehaviour
{
    public float WinTime;
    public SceneManagerBehavior sceneManager;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > WinTime) sceneManager.LoadScene(4);
    }
}
