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
        Debug.Log(GameManager.currentGameState);
        if (GameManager.currentGameState == GameManager.GameState.playing)
        {
            timerText.text = Mathf.Ceil(GameManager.timer).ToString();
        }
    }
}
