using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour {
  int _Position;
  string[] _DisplayText;
  bool _WantToBind;

  [SerializeField] Text _Text;

  private void Awake () {
    SaveSystem.Load ();
    _Position = 0;
    _DisplayText = new string[(int) PlayerStats.PlayerControls.SIZE];

    for (int i = 0; i < (int) PlayerStats.PlayerControls.SIZE; i++) {
      _DisplayText[i] = System.Enum.GetName (typeof (PlayerStats.PlayerControls), i);
    }

  }

  private void Update () {
    if (!_WantToBind) {
      _Text.text = _DisplayText[_Position] + ": " + System.Enum.GetName (typeof (KeyCode), PlayerStats._Controls[_Position]);
    } else {
      _Text.text = "Press a key";

      KeyCode targetkey = FetchKey ();
      if (targetkey != KeyCode.None) {
        PlayerStats._Controls[_Position] = targetkey;
        _WantToBind = false;
        SaveSystem.Save ();
      }
    }
  }

  public void ChangePosition (int positionChange) {
    _Position += positionChange;
    if (_Position < 0) {
      _Position += (int) PlayerStats.PlayerControls.SIZE;
    }
    if (_Position >= (int) PlayerStats.PlayerControls.SIZE) {
      _Position -= (int) PlayerStats.PlayerControls.SIZE;
    }

  }

  public void StartBind () {
    _WantToBind = true;
  }

  KeyCode FetchKey () {
    var e = System.Enum.GetNames (typeof (KeyCode)).Length;
    for (int i = 0; i < e; i++) {
      if (Input.GetKey ((KeyCode) i)) {
        return (KeyCode) i;
      }
    }

    return KeyCode.None;
  }
}