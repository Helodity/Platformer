using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils {
  static public float RoundToPlace (float input, int place) {
    float factor = Mathf.Pow (10, place);

    input *= factor;
    input = Mathf.Round (input);
    return input / factor;

  }

}