using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerAnimationVisualizer : Visualizer {
    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.ModelTransform.GetComponent<Animator>(); } }
    public override HealthStatus Status { get; set; }
    
    public Transform companionTransform;
    public Animator CompanionAnimator { get { return companionTransform.GetComponent<Animator>(); } }
    public Vector3 companionOriginalLocalPos;
    // TODO: reconsider if we need animation or simply Lerp
    //public Animator soccerAnimator;
    public GameObject soccer;
    public Transform leftPoint, rightPoint;

    private IEnumerator soccerMovement;
    private bool movingRight = true;
    private float soccerSpeed;

    public override void Initialize() {
        companionTransform.localPosition = companionOriginalLocalPos;
        HumanManager.Instance.ModelTransform.localPosition = new Vector3(0, 0, 0);
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);
        ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
        CompanionAnimator.transform.localEulerAngles = new Vector3(0, -90, 0);

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

    public override void Pause() {
        if (soccerMovement != null) {
            StopCoroutine(soccerMovement);
            soccerMovement = null;
            CompanionAnimator.ResetTrigger("Kick");
            CompanionAnimator.Play("Idle");
            ArchetypeAnimator.ResetTrigger("Kick");
            ArchetypeAnimator.Play("Idle");
        }

        ArchetypeTransform.localEulerAngles = new Vector3(0, 0, 0); 
        CompanionAnimator.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    private HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.UseAlt);

        soccerSpeed = score * 0.001f;

        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator Kick() {
        float stepLength = 0;
        while (true) {
            if (movingRight) {
                ArchetypeAnimator.SetTrigger("KickSoccer");
            } else {
                CompanionAnimator.SetTrigger("KickSoccer");
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

            while (stepLength < 1.0f) {
                soccer.transform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += soccerSpeed;
                if (movingRight) { // kicking animation moves charater. Pin them to the ground whenever the soccer is moving.
                    HumanManager.Instance.ModelTransform.localPosition = new Vector3(0, 0, 0);
                } else {
                    companionTransform.localPosition = companionOriginalLocalPos;
                }
                yield return null;
            }

            soccer.transform.localPosition = endPos;
            movingRight = !movingRight;
            stepLength = 0;
            yield return null;
        }
    }
}
