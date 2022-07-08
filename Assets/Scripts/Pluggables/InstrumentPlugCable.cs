using UnityEngine;

namespace Pluggables
{
    public class InstrumentPlugCable : PlugCable
    {
        public InstrumentSO instrument;
        public Sprite cableSprite;
        
        private InstrumentMB IMB;
        [SerializeField] private Sprite[] cableColorSprites;

        protected new void Awake()
        {
            IMB = GetComponent<InstrumentMB>();
            instrument = IMB.GetIdentifierSO();
            itemColor = IMB.cableColor;
            // TODO: Move these to Renderer.
            // cableSprite = cableColorSprites[(int)itemColor];
        }
        
        protected void Start()
        {
            if (cableSprite != null) return;
            
            // cableSprite = cableColorSprites[0];
        }
    }
}