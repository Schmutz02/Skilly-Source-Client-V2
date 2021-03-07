using UnityEngine;

public static class Account
{
    public const string USERNAME_KEY = "username";
    public static string Username { get; private set; }

    public static void Login(string username, bool rememberUsername)
    {
        Reset();
        Username = username;

        if (rememberUsername)
        {
            PlayerPrefs.SetString(USERNAME_KEY, username);
        }
    }

    private static void Reset()
    {
        Username = null;
        PlayerPrefs.DeleteKey(USERNAME_KEY);
    }
}