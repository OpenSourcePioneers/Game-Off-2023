using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ak_PC_Footsteps_test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        playingID = AkSoundEngine.PostEvent("Play_PC_Footstep", this.gameObject);
    }
        private uint playingID;

    // Update is called once per frame
    void Update()
    {
        
    }
}
