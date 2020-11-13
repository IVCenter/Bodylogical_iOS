using System.Collections;
using UnityEngine;

public class BackwardsProps : PropAnimation {
    [SerializeField] private Transform[] props;
    
    public override IEnumerator Animate() {
        Vector3[] propPositions = new Vector3[props.Length];
        for (int i = 0; i < propPositions.Length; i++) {
            propPositions[i] = props[i].localPosition;
        }

        while (true) {
            for (int i = 0; i < propPositions.Length; i++) {
                propPositions[i].z += speed * Time.deltaTime;
                if (propPositions[i].z > 1) {
                    propPositions[i].z = -1;
                }

                props[i].transform.localPosition = propPositions[i];
            }

            yield return null;
        }
    }
}