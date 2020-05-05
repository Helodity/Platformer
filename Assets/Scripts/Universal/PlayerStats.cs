using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class PlayerStats
{
    //Controls 
    public static KeyCode _UpKey = KeyCode.W;
    public static KeyCode _DownKey = KeyCode.S;
    public static KeyCode _LeftKey = KeyCode.A;
    public static KeyCode _RightKey = KeyCode.D;
    public static KeyCode _JumpKey = KeyCode.Space;
    public static KeyCode _DashKey = KeyCode.J;
    public static KeyCode _GrabKey = KeyCode.K;

    //Coins that were collected in another level
    public static int _SecuredCoins;
}
