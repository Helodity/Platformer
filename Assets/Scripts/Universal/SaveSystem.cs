using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
  public static void Save () {
    BinaryFormatter formatter = new BinaryFormatter ();
    string path = Application.persistentDataPath + "/player.uwu";
    FileStream stream = new FileStream (path, FileMode.Create);

    PlayerStatsSaveState savestate = new PlayerStatsSaveState ();

    formatter.Serialize (stream, savestate);
    stream.Close ();
  }

  public static void Load () {
    string path = Application.persistentDataPath + "/player.uwu";
    try {
      if (File.Exists (path)) {
        BinaryFormatter formatter = new BinaryFormatter ();
        FileStream stream = new FileStream (path, FileMode.Open);
        PlayerStatsSaveState state = formatter.Deserialize (stream) as PlayerStatsSaveState;
        stream.Close ();

        PlayerStats._SecuredCoins = state.SecuredCoins;

        for (int i = 0; i < (int)PlayerStats.PlayerControls.SIZE; i++){
          PlayerStats._Controls[i] = (KeyCode)state.Controls[i];
        }

      } else {
        // No file exists, so create a new one with the current stats
        Save ();
      }
    } catch {
      Debug.LogError ("Error encountered");
    }
  }
}

[System.Serializable]
public class PlayerStatsSaveState {
  public int[] Controls = new int[7];
  public int SecuredCoins;

  public PlayerStatsSaveState () {
    SecuredCoins = PlayerStats._SecuredCoins;

    for(int i = 0; i < (int)PlayerStats.PlayerControls.SIZE; i++){
      Controls[i] = (int)PlayerStats._Controls[i];
    }
  }
}