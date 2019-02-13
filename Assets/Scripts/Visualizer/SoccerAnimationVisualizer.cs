using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerAnimationVisualizer : MonoBehaviour {
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get; set; }

    public Animator companionAnimator;
    // TODO: reconsider if we need animation or simply Lerp
    //public Animator soccerAnimator;
    public GameObject soccer;
    public Transform leftPoint, rightPoint;

    private IEnumerator soccerMovement;
    private bool movingRight = true;

    private float soccerSpeed = 0.1f;

    public void Visualize() {
        //ArchetypeAnimator = HumanManager.Instance.SelectedHuman.GetComponent<Animator>();
        HumanManager.Instance.SelectedHuman.transform.localEulerAngles = new Vector3(0, -90, 0);
        //ArchetypeAnimator.transform.localEulerAngles = new Vector3(0, 90, 0);
        companionAnimator.transform.localEulerAngles = new Vector3(0, -90, 0);

        if (soccerMovement == null) {
            soccerMovement = Kick();
            StartCoroutine(soccerMovement);
        }
    }

    public void Pause() {
        //ArchetypeAnimator.transform.localEulerAngles = new Vector3(0, 90, 0);
        HumanManager.Instance.SelectedHuman.transform.localEulerAngles = new Vector3(0, 0, 0); 
        companionAnimator.transform.localEulerAngles = new Vector3(0, 180, 0);

        if (soccerMovement != null) {
            StopCoroutine(soccerMovement);
            soccerMovement = null;
        }
    }

    public void GenerateNewSpeed() {
        float addition;
        do {
            addition = Random.Range(-0.02f, 0.02f);
        } while (soccerSpeed + addition <= 0.0f && soccerSpeed + addition > 0.2f);
        soccerSpeed += addition;
    }

    IEnumerator Kick() {
        float movedDist = 0;
        while (true) {
            Vector3 startpos, endpos;
            if (movingRight) {
                startpos = leftPoint.localPosition;
                endpos = rightPoint.localPosition;
            } else {
                startpos = rightPoint.localPosition;
                endpos = leftPoint.localPosition;
            }
            float journeyLength = Vector3.Distance(startpos, endpos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength * soccerSpeed;
                soccer.transform.localPosition = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime * soccerSpeed;// TODO: fix
                yield return null;
            }

            soccer.transform.localPosition = endpos;
            movingRight = !movingRight;
            movedDist = 0;
            yield return null;
        }
    }
}
