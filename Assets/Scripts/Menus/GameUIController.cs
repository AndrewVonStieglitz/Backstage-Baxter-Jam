using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMPro.TextMeshProUGUI timerText;
    // Update is called once per frame
    void Update()
    {
        timerText.text = GameManager.currentGameState + ": " + Mathf.Ceil(GameManager.timer).ToString() + ", " + "happiness: " + GameManager.happiness.ToString();
    }
}
