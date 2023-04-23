using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSaves : MonoBehaviour
{
    public GameManager Instance;
    public GameObject deleteSaveButton;

    void Start()
    {
        deleteSaveButton.GetComponent<Button>().onClick.AddListener(delegate { DeleteFiles(); });
    }

    public void DeleteFiles()
    {
        SaveManager.Instance.DeleteDungeonSave();
        SaveManager.Instance.DeleteEnemySave();
        SaveManager.Instance.DeletePlayerSave();
        Debug.Log("Save data deleted");
    }
}
