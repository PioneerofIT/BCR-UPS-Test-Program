using System.IO;
using System.Windows;
using VSP.CONTROLLER.SimulatorCtrl.Objects;

namespace VSP.CONTROLLER.SimulatorCtrl
{
    public class SimIOLinker
    {
        public SimIOLinker() { }

        public void ReadConfigFileAndLink()
        {            
            string basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            basePath += "\\DATA\\SimulatorConfig";

            LoadInputConfigFile(Path.Combine(basePath, "Input.csv"));
            LoadOutputConfigFile(Path.Combine(basePath, "Output.csv"));
            LoadMotionInputConfigFile(Path.Combine(basePath, "MotionInput.csv"));
            LoadMotionOutputConfigFile(Path.Combine(basePath, "MotionOutput.csv"));
            LoadInOutLinkConfigFile(Path.Combine(basePath, "InputOutputLink.csv"));
        }

        public void LoadInputConfigFile(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);

            int i = 0;
            foreach (var line in lines)
            {
                var parts = line.Trim().Split(',');

                string defaultValue = parts[2].Trim();
                bool on = defaultValue.Equals("ON", StringComparison.OrdinalIgnoreCase);
                SimDioCtrl.SIM_DI[i].SetOn(on);
                i++;
            }
        }

        public void LoadOutputConfigFile(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);

            int i = 0;
            foreach (var line in lines)
            {
                var parts = line.Trim().Split(',');
                if (parts.Length != 3) continue;

                string defaultValue = parts[2].Trim();
                bool on = defaultValue.Equals("ON", StringComparison.OrdinalIgnoreCase);
                SimDioCtrl.SIM_DO[i].SetOn(on);
                i++;
            }
        }

        public void LoadMotionInputConfigFile(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);

            int i = 0;
            foreach (var line in lines)
            {
                var parts = line.Trim().Split(',');

                string defaultValue = parts[2].Trim();
                bool on = defaultValue.Equals("ON", StringComparison.OrdinalIgnoreCase);
                SimDioCtrl.SIM_DI_M[i].SetOn(on);
                i++;
            }
        }

        public void LoadMotionOutputConfigFile(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);

            int i = 0;
            foreach (var line in lines)
            {
                var parts = line.Trim().Split(',');
                if (parts.Length != 3) continue;

                string defaultValue = parts[2].Trim();
                bool on = defaultValue.Equals("ON", StringComparison.OrdinalIgnoreCase);
                SimDioCtrl.SIM_DO_M[i].SetOn(on);
                i++;
            }
        }

        public void LoadInOutLinkConfigFile(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(',');

                string outputStr = parts[0].Trim();
                bool isMotionOutput = outputStr.Contains("MY");

                string outputHex = isMotionOutput
                    ? outputStr.Substring(2)
                    : outputStr.Substring(2);

                int outputIndex = Convert.ToInt32(outputHex, 16);

                for (int col = 3; col < parts.Length; col += 3)
                {
                    string inputStr = parts[col].Trim();
                    if (string.IsNullOrEmpty(inputStr)) continue;

                    bool isMotionInput = inputStr.Contains("MX");
                    string inputHex = isMotionInput
                        ? inputStr.Substring(2)
                        : inputStr.Substring(2);

                    int inputIndex = Convert.ToInt32(inputHex, 16);

                    var link = new LinkInput
                    {
                        Index = inputIndex,
                        WireName = inputStr,
                        IsMotionInput = isMotionInput,
                        InputOnWhenOutputOn = parts[col + 2].Trim().Equals("ON", StringComparison.OrdinalIgnoreCase),
                        IsNeedInitWithOutput = parts[2].Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                    };

                    // 연결
                    if (isMotionOutput)
                        SimDioCtrl.SIM_DO_M[outputIndex].AddLinkInput(link);
                    else
                        SimDioCtrl.SIM_DO[outputIndex].AddLinkInput(link);

                    // 초기화 필요 시 입력 상태도 설정
                    if (link.IsNeedInitWithOutput)
                    {
                        bool isOutOn = isMotionOutput
                            ? SimDioCtrl.SIM_DO_M[outputIndex].IsOn()
                            : SimDioCtrl.SIM_DO[outputIndex].IsOn();

                        bool inputInit = isOutOn ? link.InputOnWhenOutputOn : !link.InputOnWhenOutputOn;

                        if (link.IsMotionInput)
                            SimDioCtrl.SIM_DI_M[inputIndex].SetOn(inputInit);
                        else
                            SimDioCtrl.SIM_DI[inputIndex].SetOn(inputInit);
                    }
                }
            }
        }
    }
}
