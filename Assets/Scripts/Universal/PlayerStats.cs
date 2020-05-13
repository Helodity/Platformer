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

  //Coins that were collected in another level
  public static int _SecuredCoins;
}