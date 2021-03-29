using System;
using System.Collections.Generic;

/// <summary>
/// Data class for storing health numbers.
/// </summary>
public class Health {
    public DateTime date;
    public Dictionary<HealthType, float> values;
}