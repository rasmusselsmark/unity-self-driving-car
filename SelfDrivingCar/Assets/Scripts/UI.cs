using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text SpeedometerText;

    public void CarControllerOnSpeedChanged(object sender, float speed)
    {
        SpeedometerText.text = $"Speed: {(int)speed}";
    }
}
