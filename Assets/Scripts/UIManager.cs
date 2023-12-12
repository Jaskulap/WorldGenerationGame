using System;
using System.IO;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class UIManager : MonoBehaviour
{
    public SaveHandler saveHandler;
    public GameObject generatePopUp;
    public GameObject savePopUp;
    public GameObject loadPopUp;
    public GameObject contentObject;
    public Button buttonPrefab;
    private GameObject activePopUp;
    [SerializeField]
    public WorldGeneration worldGeneration;

    public Slider islandRadiusSlider;
    public Slider landformSlider;
    public Slider resourceRateSlider;

    public TMP_InputField seedStringInputField;

    public void Start()
    {
        worldGeneration = FindObjectOfType<WorldGeneration>();
        CloseAllPopUp();
        RestartLoadOptions();
        islandRadiusSlider.value = worldGeneration.IslandRadius;
    }

    public void CloseAllPopUp()
    {
        generatePopUp.SetActive(false);
        savePopUp.SetActive(false);
        loadPopUp.SetActive(false);
    }

    // Change the active pop-up window based on user interaction
    public void ChangePopUp(GameObject newPopUp)
    {
        try
        {
            if (newPopUp == null)
            {
                throw new ArgumentNullException(nameof(newPopUp), "New pop-up object cannot be null.");
            }

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

        // Handle the error related to passing null as newPopUp
        catch (ArgumentNullException ex)
        {
            Debug.LogError($"Argument error: {ex.Message}");
        }

        // Handle the error related to null references
        catch (NullReferenceException ex)
        {
            Debug.LogError($"Null reference error: {ex.Message}");
        }

        // Handle other errors
        catch (Exception ex)
        {
            Debug.LogError($"An unexpected error occurred: {ex.Message}");
        }
    }

    // Reload the load options based on available save folders
    public void RestartLoadOptions()
    {
        loadPopUp.SetActive(true);
        string folderPath = Application.persistentDataPath;
        string[] directories = Directory.GetDirectories(folderPath);
        int offset = 0;

        // Destroy existing buttons in the contentObject
        for (var i = contentObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(contentObject.transform.GetChild(i).gameObject);
        }

        // Instantiate buttons for each save and set up their behavior
        foreach (string folderDirectory in directories)
        {
            string folderName = Path.GetFileName(folderDirectory);
            Debug.Log(folderName);
            Button newButton = Instantiate(buttonPrefab, contentObject.transform);
            newButton.GetComponentInChildren<TMP_Text>().text = folderName;
            newButton.onClick.AddListener(() => saveHandler.OnWorldPick(folderName));

            // Adjust the button position
            newButton.transform.position = new Vector3(newButton.transform.position.x, newButton.transform.position.y + offset, 0);

            offset -= 100;
        }

        // Deactivate the loadPopUp after setting up the buttons
        loadPopUp.SetActive(false);
    }

    // Update the seed string based on user input
    public void OnSeedInputChange()
    {
        worldGeneration.SeedString = seedStringInputField.text;
    }

    // Update WorldGeneration script parameters based on slider values
    public void OnGenerateSlidersValueChanged()
    {
        worldGeneration.IslandRadius = (int)islandRadiusSlider.value;
        worldGeneration.LandformScale = (float)landformSlider.value;
        worldGeneration.ResourceRate = (float)resourceRateSlider.value;
    }

}