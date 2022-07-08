namespace Pluggables
{
    public class InstrumentPlugCable : PlugCable
    {
        public InstrumentSO instrument;
        
        private InstrumentMB IMB;

        protected new void Awake()
        {
            IMB = GetComponent<InstrumentMB>();
            instrument = IMB.GetIdentifierSO();
            color = IMB.color;
        }
    }
}