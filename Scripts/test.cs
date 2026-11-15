
using UnityEngine;

public class KeyboardTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("UNITY DETECTED THE E KEY!");
        }
    }
}