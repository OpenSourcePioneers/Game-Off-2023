using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("Play_2D_Music", this.gameObject);
        AkSoundEngine.PostEvent("Play_Ext_Amb_Forest", this.gameObject);
    }
}
