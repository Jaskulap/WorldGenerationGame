using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public WorldGeneration worldGeneration;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            worldGeneration.GetSwamp().CreateSwampBorders();
        }
    }
}
