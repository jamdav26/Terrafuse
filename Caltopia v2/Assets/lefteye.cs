using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lefteye : MonoBehaviour
{
    public float distanceFromEye = .4f;
    public bool showInFrontOfLeftEye = true;
    Transform smallSphere;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        smallSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        smallSphere.localScale = new Vector3(distanceFromEye, distanceFromEye, distanceFromEye) / 10f;
    }
    void Update()
    {
        Vector3 left = Quaternion.Inverse(UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftEye)) * UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye);
        Vector3 right = Quaternion.Inverse(UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightEye)) * UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye);
        Vector3 leftWorld, rightWorld;
        Vector3 offset = (left - right) * 3f;
        Vector3 offsetright = right * 3f;
        Vector3 leftvect = left * 1f;
        Vector3 rightvect = right * 1f;


        Matrix4x4 m = cam.cameraToWorldMatrix;
        leftWorld = m.MultiplyPoint(offsetright);
        rightWorld = m.MultiplyPoint(right);
        smallSphere.position = (showInFrontOfLeftEye ? leftWorld : rightWorld) + cam.transform.forward * distanceFromEye;
    }
}
