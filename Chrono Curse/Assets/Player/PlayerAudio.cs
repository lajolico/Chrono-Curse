using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAudio : MonoBehaviour
{
    public FMOD.Studio.EventInstance playerAudio;

    [SerializeField]
    private List<string> footAudioAssets = new List<string>();

    // [SerializeField]
    // private List<string> attackAudioAssets = new List<string>();

    bool playerIsMoving;
    int randNumber;
    float timer = 0.0f;

    [SerializeField]
    float footStepSpeed = 0.5f;

    public float returnStepSpeed()
    {
        return footStepSpeed;
    }

    public void setStepSpeed(float speed)
    {
        footStepSpeed = speed;
    }

    public void PlayerMovementStatus(bool status)
    {
        playerIsMoving = status;
        if (playerIsMoving)
        {
            CallFootSteps();
        }
    }

    // public void PlayerAttack()
    // {
    //     var rand = new System.Random();
    //     randNumber = rand.Next(attackAudioAssets.Count);

    //     PlayAudio(attackAudioAssets[randNumber]);
    // }

    private void PlayAudio(string stepAsset) 
    {
        playerAudio = FMODUnity.RuntimeManager.CreateInstance(stepAsset);

        playerAudio.start();
        playerAudio.release();
    }

    public void CallFootSteps()
    {
        if (timer > footStepSpeed)
        {
            var rand = new System.Random();
            randNumber = rand.Next(footAudioAssets.Count);

            PlayAudio(footAudioAssets[randNumber]);
            timer = 0.0f;
        }
        timer += Time.deltaTime;
    }
}
