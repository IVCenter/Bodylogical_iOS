using System.Collections;
using UnityEngine;

public class BackwardsProps : PropAnimation {
    // TODO: random props
    // [System.Serializable]
    // private class Prop {
    //     public GameObject obj;
    //     public float rotateY;
    // }

    [SerializeField] private Transform[] props;
    [SerializeField] private float minBound = -1.5f;
    [SerializeField] private float maxBound = 1.5f;
    
    public override IEnumerator Animate() {
        Vector3[] propPositions = new Vector3[props.Length];
        for (int i = 0; i < propPositions.Length; i++) {
            propPositions[i] = props[i].localPosition;
        }

        while (true) {
            for (int i = 0; i < propPositions.Length; i++) {
                propPositions[i].z += Speed * Time.deltaTime;
                if (propPositions[i].z > maxBound) {
                    propPositions[i].z = minBound;
                }

                props[i].transform.localPosition = propPositions[i];
            }

            yield return null;
        }
    }
}