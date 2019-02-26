using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerAnimationVisualizer : Visualizer {
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get; private set; }

    public Animator companionAnimator;
    // TODO: reconsider if we need animation or simply Lerp
    //public Animator soccerAnimator;
    public GameObject soccer;
    public Transform leftPoint, rightPoint;

    private IEnumerator soccerMovement;
    private bool movingRight = true;

    public override HealthStatus Status { get; set; }
    private float soccerSpeed;

    public void Initialize() {
        ArchetypeAnimator = HumanManager.Instance.SelectedHuman.transform.Find("model").GetChild(0).GetComponent<Animator>();
        //ArchetypeAnimator.transform.localEulerAngles = new Vector3(0, -90, 0);
        MoveBack moveBack = HumanManager.Instance.SelectedHuman.transform.Find("model").GetComponent<MoveBack>();
        moveBack.animator = ArchetypeAnimator;
        moveBack.humanModel = ArchetypeAnimator.gameObject;
        moveBack.moveToPoint = new Vector3(0, 0, 0);
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);
        HumanManager.Instance.SelectedHuman.transform.localEulerAngles = new Vector3(0, -90, 0);
        //ArchetypeAnimator.transform.localEulerAngles = new Vector3(0, -90, 0);
        companionAnimator.transform.localEulerAngles = new Vector3(0, -90, 0);

        if (soccerMovement == null) {
            soccerMovement = Kick();
            StartCoroutine(soccerMovement);
        }

        if (newStatus != Status) {
            Status = newStatus;
            return true;
        }
        return false;
    }

    public void Pause() {
        if (soccerMovement != null) {
            StopCoroutine(soccerMovement);
            soccerMovement = null;
            companionAnimator.Play("Idle");
            ArchetypeAnimator.Play("Idle");
        }

        HumanManager.Instance.SelectedHuman.transform.localEulerAngles = new Vector3(0, 0, 0); 
        companionAnimator.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    public HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.UseAlt);

        soccerSpeed = score * 0.001f;

        return HealthUtil.CalculateStatus(score);
    }

    IEnumerator Kick() {
        float movedDist = 0;
        while (true) {
            if (movingRight) {
                ArchetypeAnimator.SetTrigger("KickSoccer");
            } else {
                companionAnimator.SetTrigger("KickSoccer");
            }
            yield return new WaitForSeconds(1.0f);

            Vector3 startPos, endPos;
            if (movingRight) {
                startPos = leftPoint.localPosition;
                endPos = rightPoint.localPosition;
            } else {
                startPos = rightPoint.localPosition;
                endPos = leftPoint.localPosition;
            }

            float journeyLength = Vector3.Distance(startPos, endPos);
            float passedLength = 0;
            while (movedDist < journeyLength) {
                soccer.transform.localPosition = Vector3.Lerp(startPos, endPos, passedLength);
                passedLength += soccerSpeed;
                movedDist = Vector3.Distance(startPos, soccer.transform.localPosition);
                yield return null;
            }

            soccer.transform.localPosition = endPos;
            movingRight = !movingRight;
            movedDist = 0;
            yield return null;
        }
    }
}
