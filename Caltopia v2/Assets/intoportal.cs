using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class intoportal : MonoBehaviour
{
    public Camera portalanimcam, insideportalcam;

    void Start()
    {
        insideportalcam.enabled = false;
        portalanimcam.enabled = true;

    }

    void OnTriggerEnter(Collider other)
    {
        insideportalcam.enabled = true;
        portalanimcam.enabled = false;
        
    }
}

