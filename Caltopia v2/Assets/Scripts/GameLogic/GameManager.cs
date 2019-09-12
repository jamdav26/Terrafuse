using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }
    public GameObject[] keyObjects;

    void Awake() {
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
        }

        gameManager = this;

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {

    }

    public void RunEvents(InteractableObject interactableObject) 
    {
        StartCoroutine(RunEventsInOrder(interactableObject));
    }

    public IEnumerator RunEventsInOrder(InteractableObject interactableObject) {
        foreach (GameEvent gameEvent in interactableObject.gameEvents)
        {
            yield return new WaitForSeconds(gameEvent.runAfterSeconds);
            gameEvent.Run();
            yield return null;
        }
    }

    public void PlayAudioClip(AudioSource audioSource) 
    {
        audioSource.Play();
    }
}
