using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Loads all data from the csv files.
/// </summary>
public static class DataLoader {
    private const string UserMatchUrl = "https://orchestrator-dot-pg-us-e-app-165685.appspot.com/subjects";

    private static string ForecastURL(string subjectId) =>
        $"https://forecast-service-dot-pg-us-e-app-165685.appspot.com/subjects/{subjectId}/forecast";

    private static byte[] ToBytes(string json) => Encoding.UTF8.GetBytes(
        JObject.Parse(json).ToString(Formatting.None));


    /// <summary>
    /// Matches the user against an existing archetype, and retrieves the baseline health.
    /// </summary>
    /// <param name="archetype">The user's basic information.</param>
    /// <param name="health">A reference to the baseline health. It will be populated after the coroutine ends.</param>
    public static IEnumerator UserMatch(Archetype archetype, LongTermHealth health) {
        /*
         * Notice that UnityWebRequest.POST() will escape characters, meaning that the request body will be corrupted.
         * Therefore, we need to convert the json into a byte array (which won't be escaped) and construct the request
         * from scratch.
         */
        using (UnityWebRequest www = new UnityWebRequest(UserMatchUrl)) {
            www.method = "POST";
            www.uploadHandler = new UploadHandlerRaw(ToBytes(archetype.ToJson()));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("org-id", "11");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
            } else {
                JObject jObject = JObject.Parse(www.downloadHandler.text);
                string subjectId = (string) jObject["subject_id"];
                archetype.subjectId = subjectId;
                Lifestyle baseline = new Lifestyle {
                    archetype = archetype
                };

                yield return Forecast(subjectId, baseline, health);
            }
        }
    }

    public static IEnumerator Forecast(string subjectId, Lifestyle lifestyle, LongTermHealth health) {
        using (UnityWebRequest www = new UnityWebRequest(ForecastURL(subjectId))) {
            www.method = "POST";
            www.uploadHandler = new UploadHandlerRaw(ToBytes(lifestyle.ToJson()));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("org-id", "11");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
            } else {
                JObject jObject = JObject.Parse(www.downloadHandler.text);
                health.PopulateFromJson((JArray) jObject["biomarker_forecasts"]);
            }
        }
    }

    public static List<HealthRange> LoadRanges() {
        TextAsset rangeAsset = Resources.Load<TextAsset>("Data/Ranges");
        return CSVParser.LoadCsv<HealthRange>(rangeAsset.text);
    }
}