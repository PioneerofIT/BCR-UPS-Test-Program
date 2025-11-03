namespace VSP.CONTROLLER.SimulatorCtrl.Objects
{
    public class LinkInput
    {
        public int Index { get; set; } = 0;
        public string WireName { get; set; } = "";
        public bool IsMotionInput { get; set; } = false;
        public bool InputOnWhenOutputOn { get; set; } = false;
        public bool IsNeedInitWithOutput { get; set; } = false;
    }

    public class CDO_SIMULATOR
    {
        public string Name { get; set; } = "";
        public bool Default { get; set; } = false;
        private bool _on = false;

        public List<LinkInput> LinkedInputs { get; } = new List<LinkInput>();

        public void AddLinkInput(LinkInput link)
        {
            LinkedInputs.Add(link);
        }

        public void SetOn(bool value)
        {
            _on = value;
        }

        public bool IsOn()
        {
            return _on;
        }
    }
}