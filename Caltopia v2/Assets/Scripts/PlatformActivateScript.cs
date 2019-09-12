using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class PlatformActivateScript : GameEvent
{
    public GameObject trigger;
    public GameObject platform;
    public GameObject dropship;

    public float movementSpeed = 10;
    public GameObject endPoint;
    int pin2 = 1;
    int pin3 = 1;
    int pin4 = 1;
    int pin5 = 0;

    Animator upAnim; //raise
    Animator downAnim; //lower, follow
    Animator elevatorDescend; //descend
    //Animator irisOpen;

    bool lowerPlatformExectued = false;
    bool raisePlatformExecuted = false;
    bool descendElevatorExecuted = false;

    // Start is called before the first frame update
    void Start()
    {
        upAnim = platform.GetComponent<Animator>();
        downAnim = platform.GetComponent<Animator>();
        elevatorDescend = dropship.GetComponent<Animator>();
        UduinoManager.Instance.pinMode(2, PinMode.Input_pullup);
        UduinoManager.Instance.pinMode(3, PinMode.Input_pullup);
        UduinoManager.Instance.pinMode(4, PinMode.Input_pullup);
        UduinoManager.Instance.pinMode(5, PinMode.Input_pullup);
    }

    void FixedUpdate()
    {

            pin2 = UduinoManager.Instance.digitalRead(2); // descend elevator elevatordescend
            pin3 = UduinoManager.Instance.digitalRead(3); // lower platform downanim
            pin4 = UduinoManager.Instance.digitalRead(4); // raise platform upanim
            pin5 = UduinoManager.Instance.digitalRead(5); // thrusters

        if (Time.time > 10f)
        {
            print("pin2" + pin2);
            print("pin3" + pin3);
            print("pin4" + pin4);
            print("pin5" + pin5);
            //if (buttonValue == 1 && !executed)
            //{
            //    this.Run();
            //    executed = true;
            //}
            if (pin3 == 0 && !lowerPlatformExectued)
            {
                StartCoroutine(PlayAudioClips());
                downAnim.SetBool("lower", true);
                lowerPlatformExectued = true;
            }
            if (pin4 == 0 && !raisePlatformExecuted)
            {
                upAnim.SetBool("raise", true);
                raisePlatformExecuted = true;
            }
            if (pin2 == 0 && !descendElevatorExecuted)
            {
                StartCoroutine(Dropship());
                descendElevatorExecuted = true;
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.Run();
        }
    }

    public override void Run()
    {
        Lower(true);
        StartCoroutine(PlayAudioClips());
        //StartCoroutine(Dropship());
    }
   
    void Lower(bool state)
    {
        downAnim.SetBool("lower", state);
    }

    void DescendAndFollow(bool state)
    {
        elevatorDescend.SetBool("descend", state);
        downAnim.SetBool("follow", state);
    }

    void Follow(bool state)
    {
        downAnim.SetBool("follow", state);
    }

    IEnumerator Dropship()
    {
        if (!this.executed)
        {
            yield return new WaitForSeconds(5f);
        }
        
        //Follow(true);
        DescendAndFollow(true);
    }
}
