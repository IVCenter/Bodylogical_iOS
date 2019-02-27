using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoggingVisualizer : Visualizer {
    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return ArchetypeTransform.Find("model").GetChild(0).GetComponent<Animator>(); } }
    public override HealthStatus Status { get; set; }


    public Transform companionTransform;
    public Animator CompanionAnimator { get { return companionTransform.GetComponent<Animator>(); } }
    public Transform leftPoint, rightPoint;
    /// <summary>
    /// This cannot be determined at runtime because Awake() won't be called if
    /// the object is disabled and Pause() would shift the companion's position,
    /// even if the object is disabled.
    /// </summary>
    public Vector3 companionOriginalLocalPos;

    private IEnumerator archetypeMovement;
    private IEnumerator companionMovement;
    private bool archetypeMovingRight = true;
    private bool companionMovingRight = true;
    private float archetypeMovementSpeed;
    private readonly float companionMovementSpeed = 0.003f;

    public override void Initialize() {
        companionTransform.localPosition = companionOriginalLocalPos;
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        if (archetypeMovement == null) {
            ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
            CompanionAnimator.transform.localEulerAngles = new Vector3(0, 90, 0);
            archetypeMovement = ArchetypeJog();
            StartCoroutine(archetypeMovement);
            companionMovement = CompanionJog();
            StartCoroutine(companionMovement);
        }

        if (newStatus != Status) {
            Status = newStatus;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the animation, moves the people back to original position.
    /// </summary>
    public override void Pause() {
        if (archetypeMovement != null) {
            StopCoroutine(archetypeMovement);
            archetypeMovement = null;
            StopCoroutine(companionMovement);
            companionMovement = null;
            CompanionAnimator.Play("Idle");
            ArchetypeAnimator.Play("Idle");
            ArchetypeTransform.localPosition = leftPoint.localPosition;
            ArchetypeTransform.localEulerAngles = new Vector3(0, 0, 0);
            companionTransform.localPosition = companionOriginalLocalPos;
            CompanionAnimator.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    private HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.UseAlt);

        ArchetypeAnimator.SetFloat("JoggingSpeed", score * 0.01f);
        archetypeMovementSpeed = score * 0.00003f;

        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator ArchetypeJog() {
        ArchetypeAnimator.SetTrigger("Jog");

        float stepLength = 0;
        while (true) {
            Vector3 startPos, endPos;
            if (archetypeMovingRight) {
                startPos = leftPoint.localPosition;
                endPos = rightPoint.localPosition;
            } else {
                startPos = rightPoint.localPosition;
                endPos = leftPoint.localPosition;
            }

            while (stepLength < 1.0f) {
                ArchetypeTransform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += archetypeMovementSpeed;
                yield return null;
            }

            ArchetypeTransform.localPosition = endPos;
            archetypeMovingRight = !archetypeMovingRight;
            stepLength = 0.0f;
            if (archetypeMovingRight) {
                ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
            } else {
                ArchetypeTransform.localEulerAngles = new Vector3(0, 90, 0);
            }
            yield return null;
        }
    }

    private IEnumerator CompanionJog() {
        CompanionAnimator.SetTrigger("Jog");

        float stepLength = 0;
        while (true) {
            Vector3 startPos, endPos;
            if (companionMovingRight) {
                startPos = new Vector3(leftPoint.localPosition.x, leftPoint.localPosition.y, companionOriginalLocalPos.z);
                endPos = new Vector3(rightPoint.localPosition.x, rightPoint.localPosition.y, companionOriginalLocalPos.z);
            } else {
                startPos = new Vector3(rightPoint.localPosition.x, rightPoint.localPosition.y, companionOriginalLocalPos.z);
                endPos = new Vector3(leftPoint.localPosition.x, leftPoint.localPosition.y, companionOriginalLocalPos.z);
            }

            while (stepLength < 1.0f) {
                companionTransform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += companionMovementSpeed;
                yield return null;
            }

            companionTransform.localPosition = endPos;
            companionMovingRight = !companionMovingRight;
            stepLength = 0;
            if (companionMovingRight) {
                companionTransform.localEulerAngles = new Vector3(0, 90, 0);
            } else {
                companionTransform.localEulerAngles = new Vector3(0, -90, 0);
            }
            yield return null;
        }
    }
}
