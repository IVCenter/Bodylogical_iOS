using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSelectionHandler : MonoBehaviour {

    [Header("Drop Down Prefabs")]
    public GameObject ProximityDisappear;
    public GameObject ProximityAppear;
    public GameObject GazeDisappear;
    public GameObject GazeAppear;
    public GameObject Billboarding;
    public GameObject LoopSound;
    public GameObject ProximitySound;
    public GameObject ProximitySilence;
    public GameObject Translation;

    #region Helper

    /// <summary>
    /// Gets a list of all the direct children of the canvas
    /// which have the tag "InteractionsPicker"
    /// </summary>
    /// <returns>direct children</returns>
    GameObject[] getChildren()
    {
        Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>(true);
        ArrayList kids = new ArrayList(); //new GameObject[childTransforms.Length];
        for (int i = 0; i < childTransforms.Length; i++)
        {
            if (childTransforms[i].tag == "InteractionsPicker")
                kids.Add(childTransforms[i].gameObject);
        }
        GameObject[] children = new GameObject[kids.Count];// = children.ToArray();
        for (int i=0; i<kids.Count; i++)
        {
            children[i] = (GameObject) kids[i];
        }

        //print(string.Format("Children:"));
        //foreach (GameObject child in children) { print(child.name); }
        return children;
    }

    void lowerLaterChildren(string childName, float distance)
    {
        GameObject[] children = getChildren();

        bool isAfter = false;
        for (int i = 0; i < children.Length; i++)
        {
            if (isAfter) {
                RectTransform trans = children[i].GetComponent<RectTransform>();
                trans.localPosition = trans.localPosition + new Vector3(0, -distance, 0);
            }
            if (childName.Equals(children[i].name)) { isAfter = true; }
        }
    }

    public void Toggle(GameObject item, string toggleName)
    {
        float size = item.GetComponent<RectTransform>().rect.height;
        if (item.activeInHierarchy) { size = -size; }
        // Create gap and add settings
        lowerLaterChildren(toggleName, size);
        item.SetActive(!item.activeInHierarchy);
    }

    #endregion Helper

    #region Toggle Selections
    
    public void ToggleProximityDisappear() { Toggle(ProximityDisappear, "Proximity Disappear"); }

    public void ToggleProximityAppear() { Toggle(ProximityAppear, "Proximity Appear"); }

    public void ToggleGazeDisappear() { Toggle(GazeDisappear, "Gaze Disappear"); }

    public void ToggleGazeAppear() { Toggle(GazeAppear, "Gaze Appear"); }

    public void ToggleBillboard() { Toggle(Billboarding, "Billboard"); }

    public void ToggleLoopSound() { Toggle(LoopSound, "Sound Loop"); }

    public void ToggleProximitySound() { Toggle(ProximitySound, "Proximity Sound"); }

    public void TogglePorximitySilence() { Toggle(ProximitySilence, "Proximity Silence"); }

    public void ToggleTranslation() { Toggle(Translation, "Translation"); }

    #endregion Toggle Selections

    public void onDone()
    {
        gameObject.SetActive(false);

        // Get all the values for everything.
        if (ProximityDisappear.activeInHierarchy)
        {
            //GameObject.FindGameObjectWithTag("SketchManager").ImageGenerator.ProximityDisappear(ProximityDisappear.slider.value);
        }
    }
}
