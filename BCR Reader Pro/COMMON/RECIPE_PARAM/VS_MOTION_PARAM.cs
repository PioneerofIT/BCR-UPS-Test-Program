
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP.CONTROLLER;
using VSP.COMMON.BASE_COMPONENT;
using System.IO; 

namespace VSP.COMMON.RECIPE_PARAM
{
    public struct TMotionItem
    {
        public double MinLimit { get; set; }
        public double MaxLimit { get; set; }

        public TMotionUnit[] MotionUnit { get; private set; }

        

        public TMotionItem()
        {
            MinLimit = 0;
            MaxLimit = 0;

            MotionUnit = new TMotionUnit[ServoConstants.MAX_SERVO_POS];
            
            for (int i = 0; i < MotionUnit.Length; i++)
            {
                MotionUnit[i] = new TMotionUnit();
            }
           
        }

        public void CopyFrom(in TMotionItem arg)
        {
            MinLimit = arg.MinLimit;
            MaxLimit = arg.MaxLimit;

            if (MotionUnit == null || MotionUnit.Length != ServoConstants.MAX_SERVO_POS)
                MotionUnit = new TMotionUnit[ServoConstants.MAX_SERVO_POS];

            for (int i = 0; i < MotionUnit.Length; i++)
            {
                if (MotionUnit[i] == null)
                    MotionUnit[i] = new TMotionUnit();

                MotionUnit[i].CopyFrom(arg.MotionUnit[i]);
            }
        }

        public double GetPosition(int posId) => MotionUnit[posId].Position;
        public double GetVelocity(int posId) => MotionUnit[posId].Velocity;
        public double GetAccel(int posId) => MotionUnit[posId].Acceleration;

    }

    public struct TMotionParam
    {
        public string LogHead { get; private set; } = "MOTION PARAM";
        public TMotionItem[] MotParam { get; private set; }
        public double[] TimerCount { get; private set; }

        public TMotionParam()
        {
            LogHead = string.Empty;
            MotParam = new TMotionItem[(int)ServoMotor.MAX_SERVO_AXIS];
            TimerCount = new double[ServoConstants.MAX_CNT_TMR];

            for (int i = 0; i < MotParam.Length; i++)
            {
                MotParam[i] = new TMotionItem();
            }
        }

        public void Clear()
        {
            for (int i = 0; i < MotParam.Length; i++)
            {
                MotParam[i] = new TMotionItem();
            }
        }

        public void CopyFrom(in TMotionParam arg)
        {
            LogHead = arg.LogHead;

            if (MotParam == null || MotParam.Length != arg.MotParam.Length)
                MotParam = new TMotionItem[(int)ServoMotor.MAX_SERVO_AXIS];

            for (int i = 0; i < MotParam.Length; i++)
            {
                if (MotParam[i].MotionUnit == null)
                    MotParam[i] = new TMotionItem();

                MotParam[i].CopyFrom(arg.MotParam[i]);
            }
        }

        public bool Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"There is no file [{filePath}]");
                return false;
            }

            try
            {
                var ini = new VSIniFile(filePath);
                Clear();

                for (int i = 0; i < (int)ServoMotor.MAX_SERVO_AXIS; i++)
                {
                    string section = $"MOTOR_{i:D2}";
                    if (!ini.SectionExists(section))
                    {
                        Console.WriteLine($"Section '{section}' not found in {filePath}");
                        return false;
                    }

                    // 위치 데이터 읽기
                    for (int j = 0; j < ServoConstants.MAX_SERVO_POS; j++)
                    {
                        string key = $"POS_{j:D2}";
                        string val = ini.ReadString(section, key, string.Empty);

                        if (string.IsNullOrWhiteSpace(val))
                            continue;

                        var values = val.Split(',');
                        if (values.Length < 3)
                        {
                            Console.WriteLine($"Invalid values.Length {values.Length}");
                            return false;
                        }



                        if (double.TryParse(values[0], out double pos) &&
                            double.TryParse(values[1], out double vel) &&
                            double.TryParse(values[2], out double acc))
                        {
                            MotParam[i].MotionUnit[j].Position = pos;
                            MotParam[i].MotionUnit[j].Velocity = vel;
                            MotParam[i].MotionUnit[j].Acceleration = acc;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid Motion parameters at Mtr_{i}, Pos_{j}, [{filePath}]");
                            return false;
                        }
                    }

                    // 리미트 값 읽기
                    string limitKey = $"SERVO LIMIT_{i:D2}";
                    string limitVal = ini.ReadString(section, limitKey, string.Empty);

                    if (!string.IsNullOrWhiteSpace(limitVal))
                    {
                        var limits = limitVal.Split(',');
                        if (limits.Length == 2 &&
                            double.TryParse(limits[0], out double min) &&
                            double.TryParse(limits[1], out double max))
                        {
                            MotParam[i].MinLimit = min;
                            MotParam[i].MaxLimit = max;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid Limits at Motor {i}, [{filePath}]");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Missing limit key: {limitKey}, [{filePath}]");
                        return false;
                    }
                }

               
                for (int i = 0; i < (int)ServoConstants.MAX_CNT_TMR; i++)
                {
                    string key = $"TIMER_CNT_{i:D2}";
                    string val = ini.ReadString("TIMER COUNTER", key, string.Empty);

                    if (string.IsNullOrWhiteSpace(val))
                        continue;

                    if(double.TryParse(val, out double counter))
                    {
                        TimerCount[i] = counter;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid TimerCount parameters at CntTmr_{i}, [{filePath}]");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        public bool Save(string filePath, bool isRemote = false)
        {
            bool bRet;
            if (File.Exists(filePath) && !isRemote)
                File.Delete(filePath); // 기존 파일 삭제

            for (int i = 0; i < (int)ServoMotor.MAX_SERVO_AXIS; i++)
            {
                SavePosValue(filePath, i); // 각 모터 데이터를 즉시 파일에 저장
            }

            

            return true;
        }

        public void SavePosValue(string filePath, int motorIndex)
        {
            var ini = new VSIniFile(filePath);
            string section = $"MOTOR_{motorIndex:D2}";

            for (int i = 0; i < ServoConstants.MAX_SERVO_POS; i++)
            {
                string key = $"POS_{i:D2}";
                string value = string.Format("{0:F3},{1:0},{2:0}",
                    MotParam[motorIndex].MotionUnit[i].Position,
                    MotParam[motorIndex].MotionUnit[i].Velocity,
                    MotParam[motorIndex].MotionUnit[i].Acceleration);

                ini.WriteString(section, key, value);
            }

            string limitKey = $"SERVO LIMIT_{motorIndex:D2}";
            string limitVal = string.Format("{0:F3},{1:F3}",
                MotParam[motorIndex].MinLimit,
                MotParam[motorIndex].MaxLimit);

       
            ini.WriteString(section, limitKey, limitVal);
            ini.UpdateFile(); // 파일에 저장

            for (int i = 0; i < ServoConstants.MAX_CNT_TMR; i++)
            {
                section = "TIMER COUNTER";
                string key = $"TIMER_CNT_{i:D2}";
                string value = Convert.ToString(TimerCount[i]);
                ini.WriteString(section, key, value);
                ini.UpdateFile(); 
            }
        }

        //public void RemoveFile(string filePath)
        //{

        //}

        // 미사용 메서드 (주석 유지 가능)
        // public void SaveTimerCount(string filePath);
        // public void SaveOffsetValue(string filePath, int portIndex);
        // public void SaveSlotKind(string filePath);
        // public void SaveLoadType(string filePath);
        // public void SaveStripIdReadingValue(string filePath);

        public void MakeDefault()
        {

        }

        // Interfaces for Motion =========
        public double GetMaxLimit(int nMotor) => MotParam[nMotor].MaxLimit;
        public double GetMinLimit(int nMotor) => MotParam[nMotor].MinLimit;
        public double GetPosition(int nMotor, int nPosId) => MotParam[nMotor].GetPosition(nPosId);
        public double GetVelocity(int nMotor, int nPosId) => MotParam[nMotor].GetVelocity(nPosId);
        public double GetAccel(int nMotor, int nPosId) => MotParam[nMotor].GetAccel(nPosId);

        public double GetTimerCount(int Index) => TimerCount[Index];
       

    }
}
