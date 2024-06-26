using Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HelperUtilities
{
    public static float Epsilon => 0.11f;

    public static bool Approx(float val1, float val2)
    {
        return Math.Abs(val1 - val2) < Epsilon;
    }

    public static bool Approx(float val1, float val2, float epsilon)
    {
        return Math.Abs(val1 - val2) < epsilon;
    }

    public static void PrintTimesSpeed(float normalDuration, float fastDuration, float fastestDuration)
    {
        Debug.Log(
            $"normal duration {normalDuration}, fast {fastDuration} expected fast: {normalDuration / 2} actual diff: {(normalDuration / 2) - fastDuration} -> {(Approx((normalDuration / 2), fastDuration) ? "PASS" : "NO-PASS")}");
        Debug.Log(
            $"normal duration {normalDuration}, fastest {fastestDuration} expected fastest: {normalDuration / 3} actual diff: {(normalDuration / 3) - fastestDuration} -> {(Approx((normalDuration / 3), fastestDuration) ? "PASS" : "NO-PASS")}");
    }
}