using UnityEngine;
using UnityEngine.UI;

public class HUDHappinessDisplay : MonoBehaviour
{

    //[SerializeField] private Sprite[][] sprites;//this is so sad Unity Editor does not support this

    [SerializeField] private SpriteGroup[] spriteGroups;

    private int frame = 0;
    [SerializeField] private float frameRate = 24f;
    private float frameDuration;
    private int state;
    private float timeSinceFrameChange;

    private Image image;

    private void OnEnable()
    {
        UIEvents.onHappinessChanged += OnHappinessChanged;
    }

    private void OnDisable()
    {
        UIEvents.onHappinessChanged -= OnHappinessChanged;
    }

    private void OnHappinessChanged(float happiness)
    {
        SetHappiness(happiness);
    }

    void Start()
    {
        frameDuration = frameRate / 60f;
        image = GetComponent<Image>();
    }

    private void Update()
    {
        timeSinceFrameChange += Time.deltaTime;
        if (timeSinceFrameChange > frameDuration)
        {
            frame = (frame + 1) % spriteGroups[state].sprites.Length;
            image.sprite = spriteGroups[state].sprites[frame];
            timeSinceFrameChange -= frameDuration;
        }
    }

    private void SetHappiness(float happiness)
    {
        // assuming happiness [0,100]
        state = Mathf.RoundToInt(happiness / 100f * 11f);
    }

    [System.Serializable]
    private class SpriteGroup
    {
        public Sprite[] sprites;
    }
}
