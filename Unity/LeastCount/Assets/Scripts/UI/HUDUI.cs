﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickHome()
    {
        GameMode.Instance.SetMode(eMode.E_M_SPLASH);
    }

    public void OnClickBack()
    {

    }
}
