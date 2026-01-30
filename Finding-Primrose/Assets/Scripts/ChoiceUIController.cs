using UnityEngine;

public class ChoiceUI : MonoBehaviour
{
    public InteractionController interaction;

    public void OnChoiceA()
    {
        interaction.ChooseOption(0);
    }

    public void OnChoiceB()
    {
        interaction.ChooseOption(1);
    }

    public void OnReturn()
    {
        interaction.ReturnToExploration();
    }
}
