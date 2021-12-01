using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestOscReceiver : MonoBehaviour
{
    public OSC osc;
    public float x = 2;
   
    // Start is called before the first frame update
    void Start()
    {
        osc.SetAddressHandler("/1/faderA", OnReceiveData);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnReceiveData(OscMessage message)
    {
        x = message.GetFloat(0);
        
    }
}
