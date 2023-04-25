using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    public GameObject firstScene;
    public GameObject secondScene;
    public GameObject thirdScene;

    public void showScene(bool status, int whichScene)
    {
        if ((whichScene == 1) && (status == false))
        {
            firstScene.SetActive(true);
        }
        else if ((whichScene == 1) && (status == true))
        {
            firstScene.SetActive(false);
        }

        if ((whichScene == 2) && (status == false))
        {
            secondScene.SetActive(true);
        }
        else if ((whichScene == 2) && (status == true))
        {
            secondScene.SetActive(false);
        }

        if ((whichScene == 3) && (status == false))
        {
            thirdScene.SetActive(true);
        }
        else if ((whichScene == 3) && (status == true))
        {
            thirdScene.SetActive(false);
        }
    }
}
