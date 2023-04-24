using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PropAudio : MonoBehaviour
{
    public FMOD.Studio.EventInstance footsteps;
    public string soundAsset1;
    public string soundAsset2;
    public string soundAsset3;
    bool playerIsMoving;

    List<string> audioAssetNames;
    int randNumber;

    [SerializeField]
    float footStepSpeed = 0.5f;

    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioAssetNames = new List<string> { soundAsset1, soundAsset2, soundAsset3 };
    }

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

    private void PlayFootstep(string stepAsset) 
    {
        footsteps = FMODUnity.RuntimeManager.CreateInstance(stepAsset);

        footsteps.start();
        footsteps.release();
    }

    public void CallFootSteps()
    {
        if (timer > footStepSpeed)
        {
            var rand = new System.Random();
            randNumber = rand.Next(3);

            PlayFootstep(audioAssetNames[randNumber]);
            timer = 0.0f;
        }
        timer += Time.deltaTime;
    }
}
