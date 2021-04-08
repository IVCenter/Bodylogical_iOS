using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestNetwork : MonoBehaviour {
    [SerializeField] private Text text;
    
    private void Start() {
        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine() {
        Archetype archetype = new Archetype {
            age = 23,
            gender = Gender.Male,
            height = 180,
            weight = 55
        };
        LongTermHealth health = new LongTermHealth();
        NetworkError error = new NetworkError();
        yield return NetworkUtils.UserMatch(archetype, health, error);
        if (error.status != NetworkStatus.Success) {
            Debug.LogError(error.message);
            yield break;
        }
        
        text.text = health.healths[0].date.ToString("yyyy-MM-dd");
    }
}