using System;
using System.Linq;

[Serializable]
public class PlayerData
{
    private static PlayerData _instance;
    public string id;
    public string username;

    private PlayerData()
    {
    }

    public static PlayerData instance
    {
        get { return _instance ??= new PlayerData(); }
    }

    public void Initialize(string newUsername)
    {
        if (username == null)
        {
            id = GenerateRandomId(4);
            username = newUsername;
        }
    }

    private string GenerateRandomId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}