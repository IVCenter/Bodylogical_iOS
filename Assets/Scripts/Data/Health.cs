using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Data class for storing health numbers.
/// </summary>
[Serializable]
public class Health {
    public DateTime date;
    public Dictionary<HealthType, float> values;

    // TODO: format date to MM/YY, and weight to lb (* 2.2)
    
    public static readonly Dictionary<string, HealthType> ValueMap = new Dictionary<string, HealthType> {
        {"body_weight", HealthType.weight},
        {"body_mass_index", HealthType.bmi},
        {"fasting_serum_glucose", HealthType.glucose},
        {"serum_hba1c", HealthType.aic},
        {"systolic_blood_pressure", HealthType.sbp},
        {"diastolic_blood_pressure", HealthType.dbp}
    };

    public static Health FromJson(JObject jObject) {
        Health health = new Health();
        health.date = (DateTime) jObject["date"];
        health.values = (from obj in (JArray) jObject["biomarker_values"]
                select new {
                    key = (string) obj["type"],
                    value = (float) obj["value"]
                })
            .ToDictionary(x => ValueMap[x.key], x => x.value);
        return health;
    }
}