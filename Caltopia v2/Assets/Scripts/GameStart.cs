using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : GameEvent
{
    // Start is called before the first frame update
    void Start()
    {
        this.Run();
    }

    public override void Run()
    {
        StartCoroutine(PlayAudioClips());
    }
}
