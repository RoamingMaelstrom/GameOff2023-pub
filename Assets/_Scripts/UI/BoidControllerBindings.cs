using UnityEngine;
using UnityEngine.UI;

public class BoidControllerBindings : MonoBehaviour
{
    [SerializeField] BoidController boidController;
    [SerializeField] GameObject boidControlsPanel;

    [SerializeField] Slider alignmentSlider;
    [SerializeField] Slider cohesionSlider;
    [SerializeField] Slider separationSlider;

    [SerializeField] Slider aggressionSlider;
    [SerializeField] Toggle collapseOnCentreToggle;

    [SerializeField] Toggle circularFormationToggle;
    [SerializeField] Toggle lowHealthDronesNearCentreToggle;

    [SerializeField] Toggle followObjectToggle;
    [SerializeField] Toggle followPlayerByDefaultToggle;

    private void FixedUpdate() 
    {
        SetBoidControllerValues();
    }

    // public void SetPanelActive(bool value) => boidControlsPanel.SetActive(value);
    public void SetPanelActive(bool value) {}

    public void SetBoidControllerValues()
    {
        boidController.alignment = alignmentSlider.value;
        boidController.cohesion = cohesionSlider.value;
        boidController.separation = separationSlider.value;

        boidController.aggression = aggressionSlider.value;
        boidController.collapseOnCentre = collapseOnCentreToggle.isOn;

        boidController.useCircularFormation = circularFormationToggle.isOn;
        boidController.lowHealthDronesNearCentre = lowHealthDronesNearCentreToggle.isOn;

        boidController.followSelectedObject = followObjectToggle.isOn;
        boidController.followPlayerByDefault = followPlayerByDefaultToggle.isOn;
    }

    public void SetValuesOnBoidSwitch()
    {
        alignmentSlider.SetValueWithoutNotify(boidController.alignment);
        cohesionSlider.SetValueWithoutNotify(boidController.cohesion);
        separationSlider.SetValueWithoutNotify(boidController.separation);

        aggressionSlider.SetValueWithoutNotify(boidController.aggression);
        collapseOnCentreToggle.SetIsOnWithoutNotify(boidController.collapseOnCentre);

        circularFormationToggle.SetIsOnWithoutNotify(boidController.useCircularFormation);
        lowHealthDronesNearCentreToggle.SetIsOnWithoutNotify(boidController.lowHealthDronesNearCentre);

        followObjectToggle.SetIsOnWithoutNotify(boidController.followSelectedObject);
        followPlayerByDefaultToggle.SetIsOnWithoutNotify(boidController.followPlayerByDefault);
    }
}
