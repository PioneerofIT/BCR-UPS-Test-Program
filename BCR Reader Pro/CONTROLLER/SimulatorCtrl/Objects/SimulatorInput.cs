namespace VSP.CONTROLLER.SimulatorCtrl.Objects
{
    public class CDI_SIMULATOR
    {
	    string Name;
        bool Default;
        bool On;

        public CDI_SIMULATOR()
        {
            Name = "";            
            Default = false;
            On = false;
        }

        public void SetOn(bool value)
        {
            On = value;
        }

        public bool IsOn()
        {
            return On;
        }
    }
}
