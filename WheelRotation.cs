using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{

    [SerializeField] GameObject[] wheels;
    float[] _speeds = new float[5];
    [SerializeField] Transform [] BPMControllers;
    [SerializeField] float [] _distances;


    [SerializeField] Transform [] _port;
    [SerializeField] GameObject[] BPMCentralSphere;
    [SerializeField] float distanceToPort;



    // Update is called once per frame
    void Update()
    {

        for (int a = 0; a < wheels.Length; a++)
        {
            _distances[a] = Vector3.Distance(BPMControllers[a].position, transform.position);
            _speeds[a] = Map(_distances[a], 0.6f, 0.7f, 0.01f, 3f);
            wheels[a].transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime * _speeds[a]);
        }

        // distanceToPort = Vector3.Distance(_port, transform.position);




    }

    // Map Function
    float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
