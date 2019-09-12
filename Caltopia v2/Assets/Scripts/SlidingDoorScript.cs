using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class SlidingDoorScript : GameEvent
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject scannerScreen;

    Animator leftAnim;
    Animator rightAnim;
    //Animator scanAnim;

    // Start is called before the first frame update
    void Start()
    {
        leftAnim = leftDoor.GetComponent<Animator>();
        rightAnim = rightDoor.GetComponent<Animator>();
        //scanAnim = scannerScreen.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" & this.executed == false)
        {
            this.executed = true;
            this.Run();
        }
    }

    public override void Run()
    {
        var glowgreen = new Color(0.06f, 0.36f, 0.26f);

        var material = scannerScreen.GetComponent<Renderer>().material;
        material.EnableKeyword("_EMISSION");
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        material.SetColor("_EmissionColor", glowgreen);

        StartCoroutine(SlideDoorsWithDelay(true, 4.6f));
        if (audioSources.Length != 0)
        {
            this.PlayAudio();
        }

        Debug.Log("collide");
    }

    IEnumerator SlideDoorsWithDelay(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        SlideDoors(state);
        StartCoroutine(PlayAudioClips());
    }

    void SlideDoors(bool state)
    {
        leftAnim.SetBool("slide", state);
        rightAnim.SetBool("slide", state);
    }

    /*void HandScan(bool state)
    {
        scanAnim.SetBool("scan", state);
    }*/
}
