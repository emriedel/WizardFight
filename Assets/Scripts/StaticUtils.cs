using UnityEngine;
using System;
using System.Collections;

public static class StaticUtils {
    // from http://www.joshondesign.com/2013/03/01/improvedEasingEquations
    public static float easeOutElastic(float t) {
        float p = 0.3f;
        
        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - p / 4f) * (2f * Mathf.PI) / p) + 1;
    }
}
