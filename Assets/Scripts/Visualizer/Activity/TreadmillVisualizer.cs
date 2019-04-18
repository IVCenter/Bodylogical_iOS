using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillVisualizer : Visualizer {
    public override string VisualizerName { get { return "Jogging"; } }

    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.HumanAnimator; } }
    public override HealthStatus Status { get; set; }

    /// <summary>
    /// This cannot be determined at runtime because Awake() won't be called if
    /// the object is disabled and Pause() would shift the companion's position,
    /// even if the object is disabled.
    /// </summary>
    public Vector3 companionOriginalLocalPos;

    private IEnumerator archetypeMovement;
    private IEnumerator companionMovement;
    private bool archetypeRunning = true;
    private bool archetypeTriggerSet;

    public override void Initialize() {
        ActivityManager.Instance.CompanionTransform.localPosition = companionOriginalLocalPos;
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        if (archetypeMovement == null) {
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
            ActivityManager.Instance.CompanionAnimator.ResetTrigger("Jog");
            ActivityManager.Instance.CompanionAnimator.Play("Idle");
            ArchetypeAnimator.ResetTrigger("Jog");
            ArchetypeAnimator.ResetTrigger("Walk");
            ArchetypeAnimator.Play("Idle");
        }
    }

    /// <summary>
    /// Generates a new speed for both the archetype and the companion.
    /// Speed is calculated using the health scores AND the age.
    /// 
    /// </summary>
    /// <returns>The new speed.</returns>
    /// <param name="index">Index. Range from 0-4. The larger it is, the older the people are.</param>
    /// <param name="choice">Choice.</param>
    private HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.UseAlt);
        float yearMultiplier = 1 - index * 0.05f;

        ActivityManager.Instance.CompanionAnimator.SetFloat("JoggingSpeed", yearMultiplier);

        float archetypeAnimationSpeed = score * 0.01f * yearMultiplier;
        if (archetypeAnimationSpeed <= 0.5f) { // switch to walking
            if (archetypeRunning) {
                archetypeTriggerSet = false;
            }
            archetypeRunning = false;
            ArchetypeAnimator.SetFloat("WalkingSpeed", score * 0.02f * yearMultiplier);
        } else {
            if (!archetypeRunning) {
                archetypeTriggerSet = false;
            }
            archetypeRunning = true;
            ArchetypeAnimator.SetFloat("JoggingSpeed", score * 0.01f * yearMultiplier);
        }


        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator ArchetypeJog() {
        // Reset trigger so that it would always select an animation when the visualization starts
        archetypeTriggerSet = false;

        while (true) {
            if (archetypeRunning) {
                if (!archetypeTriggerSet) {
                    archetypeTriggerSet = true;
                    ArchetypeAnimator.ResetTrigger("Walk");
                    ArchetypeAnimator.SetTrigger("Jog");
                }
            } else {
                if (!archetypeTriggerSet) {
                    archetypeTriggerSet = true;
                    ArchetypeAnimator.ResetTrigger("Jog");
                    ArchetypeAnimator.SetTrigger("Walk");
                }
            }
            yield return null;
        }
    }

    private IEnumerator CompanionJog() {
        Animator currAnimator = ActivityManager.Instance.CompanionAnimator; 
        currAnimator.SetTrigger("Jog");
        while (true) {
            if (currAnimator != ActivityManager.Instance.CompanionAnimator) {
                currAnimator = ActivityManager.Instance.CompanionAnimator;
                currAnimator.SetTrigger("Jog");
            }
            yield return null;
        }
    }
}
