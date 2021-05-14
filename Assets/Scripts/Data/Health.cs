using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class for storing health numbers.
/// </summary>
public class Health {
    public DateTime date;
    public Dictionary<HealthType, float> values;

    public float this[HealthType type] => values[type];

    public Health() { }

    public Health(Health other) {
        date = other.date; // DateTime is a struct
        values = new Dictionary<HealthType, float>(other.values);
    }

    public static Health Interpolate(Health h1, Health h2, float i) {
        Health hi = new Health {
            values = new Dictionary<HealthType, float>()
        };

        float numDays = (float) (h2.date - h1.date).TotalDays;
        hi.date = h1.date.AddDays(Mathf.Lerp(0, numDays, i));

        foreach (HealthType type in h1.values.Keys) {
            hi.values[type] = Mathf.Lerp(h1[type], h2[type], i);
        }

        return hi;
    }
}