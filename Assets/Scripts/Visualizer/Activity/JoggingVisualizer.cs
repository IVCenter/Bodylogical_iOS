using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoggingVisualizer : Visualizer {
    public override string VisualizerKey { get { return "General.ActJog"; } }

    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.HumanAnimator; } }

    public override HealthStatus Status { get; set; }

    public Vector3 leftPoint, rightPoint;
    /// <summary>
    /// This cannot be determined at runtime because Awake() won't be called if
    /// the object is disabled and Pause() would shift the companion's position,
    /// even if the object is disabled.
    /// </summary>
    public Vector3 companionOriginalLocalPos;

    private IEnumerator archetypeMovement;
    private IEnumerator companionMovement;
    private float archetypeMovementSpeed;
    private float companionMovementSpeed = 0.003f;

    public override void Initialize() {
        ActivityManager.Instance.CurrentTransform.localPosition = companionOriginalLocalPos;
    }

    public override bool Visualize(float index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        // Set stars
        ActivityManager.Instance.charHeart.Display(newStatus);

        if (archetypeMovement == null) {
            ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
            ActivityManager.Instance.CurrentTransform.localEulerAngles = new Vector3(0, -90, 0);
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
            ActivityManager.Instance.CurrentAnimator.SetTrigger("Idle");
            ArchetypeAnimator.SetTrigger("Idle");
        }
        ArchetypeTransform.localPosition = leftPoint;
        ArchetypeTransform.localEulerAngles = new Vector3(0, 0, 0);
        ActivityManager.Instance.CurrentTransform.localEulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Generates a new speed for both the archetype and the companion.
    /// Speed is calculated using the health scores AND the age.
    ///
    /// </summary>
    /// <returns>The new speed.</returns>
    /// <param name="index">Index. Range from 0-4. The larger it is, the older the people are.</param>
    /// <param name="choice">Choice.</param>
    private HealthStatus GenerateNewSpeed(float index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.SelectedArchetype.gender);
        // Account for activity ability loss due to aging.
        float yearMultiplier = 1 - index * 0.05f;

        // Companion: always running
        companionMovementSpeed = 0.003f * yearMultiplier;
        ActivityManager.Instance.CurrentAnimator.SetFloat("AnimationSpeed", yearMultiplier);
        ActivityManager.Instance.CurrentAnimator.SetFloat("LerpAmount", yearMultiplier);

        // Archetype: switches among running, walking and wheelchairing.
        archetypeMovementSpeed = score * 0.00003f * yearMultiplier;

        // Blend tree lerping:
        // The walking/jogging animation only plays at a score of 30-100 (not bad).
        // Therefore, we need to convert from a scale of 30-100 to 0-1.
        ArchetypeAnimator.SetFloat("LerpAmount", (score - 30) / 70.0f);

        // Walking and running requires different playback speeds.
        if (HealthUtil.CalculateStatus(score) == HealthStatus.Intermediate) {
            ArchetypeAnimator.SetFloat("AnimationSpeed", score * 0.02f * yearMultiplier);
        } else {
            ArchetypeAnimator.SetFloat("AnimationSpeed", score * 0.01f * yearMultiplier);
        }

        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator ArchetypeJog() {
        ArchetypeAnimator.SetTrigger("Jog");
        float stepLength = 0;
        bool archetypeMovingRight = true;
        while (true) {
            Vector3 startPos, endPos;
            // set start and end
            if (archetypeMovingRight) {
                startPos = leftPoint;
                endPos = rightPoint;
            } else {
                startPos = rightPoint;
                endPos = leftPoint;
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
        Animator currAnimator = ActivityManager.Instance.CurrentAnimator;
        currAnimator.SetTrigger("Jog");

        float stepLength = 0;
        bool companionMovingRight = true;
        while (true) {
            Vector3 startPos, endPos;
            if (companionMovingRight) {
                startPos = new Vector3(leftPoint.x, leftPoint.y, companionOriginalLocalPos.z);
                endPos = new Vector3(rightPoint.x, rightPoint.y, companionOriginalLocalPos.z);
            } else {
                startPos = new Vector3(rightPoint.x, rightPoint.y, companionOriginalLocalPos.z);
                endPos = new Vector3(leftPoint.x, leftPoint.y, companionOriginalLocalPos.z);
            }

            while (stepLength < 1.0f) {
                ActivityManager.Instance.CurrentTransform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += companionMovementSpeed;

                // Since companion may change (from younger to older and vise versa),
                // need to check if the companion has changed.
                if (currAnimator != ActivityManager.Instance.CurrentAnimator) {
                    currAnimator = ActivityManager.Instance.CurrentAnimator;
                    currAnimator.SetTrigger("Jog");
                }

                yield return null;
            }

            ActivityManager.Instance.CurrentTransform.localPosition = endPos;
            companionMovingRight = !companionMovingRight;
            stepLength = 0;
            if (companionMovingRight) {
                ActivityManager.Instance.CurrentTransform.localEulerAngles = new Vector3(0, -90, 0);
            } else {
                ActivityManager.Instance.CurrentTransform.localEulerAngles = new Vector3(0, 90, 0);
            }
            yield return null;
        }
    }
}
