using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Variables that need to be saved between sessions, like levels beaten.
[System.Serializable]
public static class PlayerStats {
  public enum PlayerControls { Up, Down, Left, Right, Jump, Dash, Grab, SIZE }

  public static KeyCode[] _Controls = new KeyCode[(int) PlayerControls.SIZE] {
    KeyCode.W,
    KeyCode.S,
    KeyCode.A,
    KeyCode.D,
    KeyCode.Space,
    KeyCode.J,
    KeyCode.K
  };

  public static bool IsKeyPressed (int control, bool getDown) {
    if (getDown) {
      return Input.GetKeyDown (_Controls[control]);
    } else {
      return Input.GetKey (_Controls[control]);
    }
  }

  //Coins that were collected in another level
  public static int _SecuredCoins;
}