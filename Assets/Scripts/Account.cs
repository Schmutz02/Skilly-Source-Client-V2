public static class Account
{
    public static string Username { get; private set; }

    public static void Login(string username)
    {
        Reset();
        Username = username;
    }

    private static void Reset()
    {
        Username = null;
    }
}