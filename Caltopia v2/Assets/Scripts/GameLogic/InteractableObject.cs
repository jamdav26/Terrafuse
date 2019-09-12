using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {

    GameManager gameManager;

    public bool isInteractable;
    //public bool IsInteractable { get; set; }

    public GameEvent[] gameEvents;

    private void Start() 
    {
        gameManager = GameManager.gameManager;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (isInteractable & other.tag == "Player")
        {
            gameManager.RunEvents(this);
        }
    }
}
