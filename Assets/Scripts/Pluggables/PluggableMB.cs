using UnityEngine;

namespace Pluggables
{
    [ExecuteInEditMode]
    public class PluggableMB : MonoBehaviour
    {
        [SerializeField] protected PluggablesSO identifierSO;
        protected SpriteRenderer spriteRenderer;
        protected BoxCollider2D boxCol;

        public ColorEnum color;

        public void Init()
        {
            // Should be called when the identifier is set. Not sure how to do this at current ;(
            //print(name + " initialising");
            if (identifierSO != null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = identifierSO.coloredSprites[(int)color];
                boxCol = GetComponent<BoxCollider2D>();
                boxCol.size = identifierSO.colliderDimensions;
                boxCol.offset = identifierSO.colliderOffset;
                boxCol.isTrigger = true;
            }
            else
                print("ERROR: " + name + " pluggable scriptable object is missing");
        }

        void Awake()
        {
            //spriteRenderer = GetComponent<SpriteRenderer>();
            //boxCol = GetComponent<BoxCollider2D>();
            //boxCol.isTrigger = true;
        }

        private void Start()
        {
            //Init(); // would love this to be somewhere else
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "CableHead")
            {
                // Check to see if baxter is carrying a cable, if he is then call
                //identifier.OnConnect();
            }
        }

        public PluggablesSO GetIdentifierSO() { return identifierSO; }
    }
}