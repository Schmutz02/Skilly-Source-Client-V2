using Models;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    private void Awake()
    {
        Settings.Load();
        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        Settings.Save();
    }
}