using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PropAudio : MonoBehaviour
{
    public FMOD.Studio.EventInstance propAudio;

    [SerializeField]
    private List<string> audioAssetNames = new List<string>();
    public string breakEffect;
    int randNumber;

    private void PlayAudio(string stepAsset) 
    {
        propAudio = FMODUnity.RuntimeManager.CreateInstance(stepAsset);

        propAudio.start();
        propAudio.release();
    }

    public void TriggerToPlayAudio(bool notBroken)
    {

        if (notBroken)
        {
            var rand = new System.Random();
            randNumber = rand.Next(audioAssetNames.Count);

            PlayAudio(audioAssetNames[randNumber]);
        }
        else
        {
            PlayAudio(breakEffect);
        }

    }
}
