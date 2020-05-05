using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.uwu";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerStatsSaveState savestate = new PlayerStatsSaveState();

        formatter.Serialize(stream, savestate);
        stream.Close();
    }

    public static void Load()
    {
        string path = Application.persistentDataPath + "/player.uwu";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerStatsSaveState state = formatter.Deserialize(stream) as PlayerStatsSaveState;
            stream.Close();

            PlayerStats._SecuredCoins = state.SecuredCoins;

            PlayerStats._UpKey    =  (KeyCode)state.Controls[0];
            PlayerStats._DownKey  =  (KeyCode)state.Controls[1];
            PlayerStats._LeftKey  =  (KeyCode)state.Controls[2];
            PlayerStats._RightKey =  (KeyCode)state.Controls[3];
            PlayerStats._JumpKey  =  (KeyCode)state.Controls[4];
            PlayerStats._DashKey  =  (KeyCode)state.Controls[5];
            PlayerStats._GrabKey  =  (KeyCode)state.Controls[6];
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
        }
    }
}

public class PlayerStatsSaveState
{
    public int[] Controls;
    public int SecuredCoins;

    public PlayerStatsSaveState()
    {
        SecuredCoins = PlayerStats._SecuredCoins;

        Controls[0] = (int)PlayerStats._UpKey;
        Controls[1] = (int)PlayerStats._DownKey;
        Controls[2] = (int)PlayerStats._LeftKey;
        Controls[3] = (int)PlayerStats._RightKey;
        Controls[4] = (int)PlayerStats._JumpKey;
        Controls[5] = (int)PlayerStats._DashKey;
        Controls[6] = (int)PlayerStats._GrabKey;
    }
}