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
        yield return NetworkUtils.UserMatch(archetype, health);
        text.text = health.healths[0].date.ToString("yyyy-MM-dd");
    }
}