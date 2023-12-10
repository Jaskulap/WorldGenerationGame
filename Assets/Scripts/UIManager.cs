using System.IO;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    public SaveHandler saveHandler;
    public GameObject generatePopUp;
    public GameObject savePopUp;
    public GameObject loadPopUp;
    public GameObject contentObject;
    public Button buttonPrefab;
    private GameObject activePopUp;

    public void Start()
    {
        CloseAllPopUp();
        RestartLoadOptions();
    }

    public void CloseAllPopUp()
    {
        generatePopUp.SetActive(false);
        savePopUp.SetActive(false);
        loadPopUp.SetActive(false);
    }



    public void ChangePopUp(GameObject newPopUp)
    {
        if (activePopUp == null)
        {
            activePopUp = newPopUp;
            activePopUp.SetActive(true);
        }
        else if (activePopUp == newPopUp)
        {
            activePopUp.SetActive(false);
            activePopUp = null;
        }
        else
        {
            activePopUp.SetActive(false);
            activePopUp = newPopUp;
            activePopUp.SetActive(true);
        }
        if (activePopUp == loadPopUp)
        {
            loadPopUp.SetActive(true);
            RestartLoadOptions();

            loadPopUp.SetActive(true);
        }
    }

    public void RestartLoadOptions()
    {
        loadPopUp.SetActive(true);
        string folderPath = Application.persistentDataPath;
        string[] directories = Directory.GetDirectories(folderPath);
        int offset = 0;
        for (var i = contentObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(contentObject.transform.GetChild(i).gameObject);

        }


        foreach (string folderDirectory in directories)
        {

            string folderName = Path.GetFileName(folderDirectory);
            Debug.Log(folderName);
            Button newButton = Instantiate(buttonPrefab, contentObject.transform);
            newButton.GetComponentInChildren<TMP_Text>().text = folderName;
            newButton.onClick.AddListener(() => saveHandler.OnWorldPick(folderName));
            newButton.transform.position = new Vector3(newButton.transform.position.x, newButton.transform.position.y + offset, 0);

            offset -= 100;


        }
        loadPopUp.SetActive(false);
    }

}