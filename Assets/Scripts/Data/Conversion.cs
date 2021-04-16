using System.Collections.Generic;
using UnityEngine;

public static class Conversion {
    public static int KgToLb(int kg) => Mathf.RoundToInt(kg * 2.20462f);
    public static int LbToKg(int lb) => Mathf.RoundToInt(lb / 2.20462f);

    public static KeyValuePair<int, int> CmtoFtInch(int cm) {
        int totalInches = Mathf.RoundToInt(cm / 2.54f);
        return new KeyValuePair<int, int>(totalInches / 12, totalInches % 12);
    }

    public static int FtInchToCm(int ft, int inch) => Mathf.RoundToInt((ft * 12 + inch) * 2.54f);
    public static int FtInchToCm(KeyValuePair<int, int> arg) => FtInchToCm(arg.Key, arg.Value);
}