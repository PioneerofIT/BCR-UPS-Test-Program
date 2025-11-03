using System;
using System.Collections.Generic;
using System.IO;
using VSP.CONTROLLER.SimulatorCtrl.Objects;

namespace VSP.CONTROLLER.SimulatorCtrl
{
    public class CAnalogParam
    {
        public string type;          // "Vacuum" or "Gas"
        public double maxVoltage;
        public double minValue;
        public double maxValue;
        public int outputTag;        // e.g., Y00A → 10
        public int resolution;
    }

    public class CSimAioCtrl : CIoCtrl
    {
        private Dictionary<int, CAnalogParam> m_AnalogParam = new Dictionary<int, CAnalogParam>();
        private Dictionary<int, double> m_AnalogOutput = new Dictionary<int, double>();

        public CSimAioCtrl()
        {
            LoadAnalogConfigFile(@"..\..\..\..\CONTROLLER\Simulator\Config\Analog.csv");
        }

        public bool WriteAnalogVal(int nIdx, double dVal)
        {
            m_AnalogOutput[nIdx] = dVal;
            return true;
        }

        public override ushort GetWord(int nIdx, ushort wRange = 0x00, bool IsOut = false)
        {
            ushort wVal = GeneratorData(nIdx);
            wVal = (ushort)((wVal >> 4) + 1);
            return wVal;
        }

        public ushort GeneratorData(int nIdx)
        {
            const int shiftBits = 4;

            if (!m_AnalogParam.ContainsKey(nIdx))
                return 0;

            CAnalogParam cfg = m_AnalogParam[nIdx];
            double dValue = 0.0;

            if (cfg.type == "Vacuum")
            {
                if (SimDioCtrl.SIM_DO[cfg.outputTag].IsOn())
                    dValue = 0.190;
                else
                    dValue = cfg.maxVoltage;
            }
            else
            {
                dValue = m_AnalogOutput.ContainsKey(nIdx) ? m_AnalogOutput[nIdx] : 0.0;
            }

            int maxDigital = (1 << cfg.resolution) - 1;
            int val = (int)(dValue * maxDigital / cfg.maxVoltage);
            return (ushort)(val << shiftBits);
        }

        public void LoadAnalogConfigFile(string path)
        {
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) // skip header
            {
                var tokens = lines[i].Split(',');
                if (tokens.Length < 8)
                    continue;

                string idxStr = tokens[0].Trim(); // e.g., AX03
                if (!int.TryParse(idxStr.Substring(2), out int nIdx))
                    continue;

                var cfg = new CAnalogParam
                {
                    type = tokens[2].Trim(),
                    maxVoltage = double.Parse(tokens[3].Trim()),
                    minValue = double.Parse(tokens[4].Trim()),
                    maxValue = double.Parse(tokens[5].Trim()),
                    resolution = int.Parse(tokens[7].Trim())
                };

                string hexStr = tokens[6].Trim().Substring(2); // "Y00A" → "00A"
                cfg.outputTag = Convert.ToInt32(hexStr, 16);   // → 10

                m_AnalogParam[nIdx] = cfg;
            }
        }
    }
}
