using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class AnimScript : MonoBehaviour
{
    public float keySpeed = 10;
    public float mouseSpeed = 1.25f;
    public GameObject eye;

    private Quaternion originalRotation;
    private Vector2 angle = new Vector2(0f, 0f);
    private Vector2 minAngle = new Vector2(-360f, -90f);
    private Vector2 maxAngle = new Vector2(360f, 90f);
    private float limit = 360f;

    void Awake()
    {
        Debug.Log("In Away for Fly Control");
        eye = GameObject.FindWithTag("portalanim");
    }

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime*1);
 
    }

    void Strafe(float dist)
    {
        transform.position += eye.transform.right * dist;
    }

    void Fly(float dist)
    {
        transform.position += eye.transform.forward * dist;
    }

    void Look(Vector2 dist)
    {
        angle += dist;

        angle.x = ClampAngle(angle.x, minAngle.x, maxAngle.x);
        angle.y = ClampAngle(angle.y, minAngle.y, maxAngle.y);

        Quaternion quatX = Quaternion.AngleAxis(angle.x, Vector3.up);
        Quaternion quatY = Quaternion.AngleAxis(angle.y, -Vector3.right);

        transform.localRotation = originalRotation * quatX * quatY;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -limit)
        {
            angle += limit;
        }
        else if (angle > limit)
        {
            angle -= limit;
        }
        return Mathf.Clamp(angle, min, max);
    }
}






