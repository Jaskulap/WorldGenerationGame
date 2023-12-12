using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public WorldGeneration worldGeneration;
    void Update()
    {
        // Check if the '1' key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Generate swamp borders
            worldGeneration.GetSwamp().CreateSwampBorders();
        }
    }
}
