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

            PlayerStats._Controls[0] =  (KeyCode)state.Controls[0];
            PlayerStats._Controls[1] =  (KeyCode)state.Controls[1];
            PlayerStats._Controls[2] =  (KeyCode)state.Controls[2];
            PlayerStats._Controls[3] =  (KeyCode)state.Controls[3];
            PlayerStats._Controls[4] =  (KeyCode)state.Controls[4];
            PlayerStats._Controls[5] =  (KeyCode)state.Controls[5];
            PlayerStats._Controls[6] =  (KeyCode)state.Controls[6];
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

        Controls[0] = (int)PlayerStats._Controls[0];
        Controls[1] = (int)PlayerStats._Controls[1];
        Controls[2] = (int)PlayerStats._Controls[2];
        Controls[3] = (int)PlayerStats._Controls[3];
        Controls[4] = (int)PlayerStats._Controls[4];
        Controls[5] = (int)PlayerStats._Controls[5];
        Controls[6] = (int)PlayerStats._Controls[6];
    }
}