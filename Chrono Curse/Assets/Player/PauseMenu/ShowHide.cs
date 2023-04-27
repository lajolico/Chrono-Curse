using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> showingHiding = new List<GameObject>();
    public AppearingButtons appearButts;
    public Pause pause;
    // Start is called before the first frame update
    void OnDisable()
    {
        foreach (GameObject obj in showingHiding)
        {
            obj.SetActive(false);
        }
    }

    public void fadeButtonOut()
    {
        appearButts.disableButton();
    }

    public void resumeGame()
    {
        pause.Resume();
    }

    public void activateObj()
    {
        foreach (GameObject obj in showingHiding)
        {
            obj.SetActive(true);
        }
    }
}
