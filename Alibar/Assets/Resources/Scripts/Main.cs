using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenUI(UIType.MainMenu);
        UIManager.Instance.OpenUI(UIType.Settings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
