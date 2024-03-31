using UnityEngine;

public class BGMAudioManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Optional: Check if another BGM is already playing
        if (FindObjectsOfType<BGMAudioManager>().Length > 1)
        {
            Destroy(gameObject); // Destroy this if another BGM is already playing
        }
    }
}
