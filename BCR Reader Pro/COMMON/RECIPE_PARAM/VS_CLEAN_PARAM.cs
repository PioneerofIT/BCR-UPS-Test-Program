using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO;
using VSP.COMMON.BASE_COMPONENT;
using VSP.CONTROLLER;



namespace VSP.COMMON.RECIPE_PARAM
{
    using VCLEANITEM = List<TCleanItem>; // C++의 std::vector<TCleanItem>을 변환

    public struct TCleanItem
    {
        // ============================================================  
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화 (Constructor / Initialization)  
        // ============================================================  
        #region ObjectLifecycle
        public TCleanItem()
        {
            StartVac = 0.0;
            RfPower = 0;
            GasFlow = new int[(int)MfcType.Max]; // 배열 크기 설정  
            StepTime = 0;
        }

        public void Clear()
        {
            StartVac = 0.0;
            RfPower = 0;
            StepTime = 0;
            GasFlow = null;
        }
        public void CopyFrom(TCleanItem arg)
        {
            StartVac = arg.StartVac;
            RfPower = arg.RfPower;
            StepTime = arg.StepTime;

            if (arg.GasFlow != null)
            {
                if (GasFlow == null || GasFlow.Length != arg.GasFlow.Length)
                    GasFlow = new int[arg.GasFlow.Length];

                for (int i = 0; i < GasFlow.Length; i++)
                    GasFlow[i] = arg.GasFlow[i];
            }
        }
        #endregion

        // ============================================================  
        // Description : [2] Properties (속성 및 설정 값)  
        // ============================================================  
        #region Properties
        public double StartVac;
        public int RfPower;
        public int[] GasFlow;
        public int StepTime;
        #endregion

        // ============================================================  
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)  
        // ============================================================  
        #region AccessorsComputation
        public int GetGasSp(int gas)
        {
            if (GasFlow != null && gas >= 0 && gas < GasFlow.Length)
                return GasFlow[gas];
            return 0;
        }
        #endregion

        // ============================================================  
        // Description : [4] Internal Logic / Validation ( 데이터 검증)  
        // ============================================================  
        #region InternalLogicValidation

        #endregion

        // ============================================================  
        // Description : [5] Unclassified (추후 정리 예정)  
        // ============================================================  
        #region Unclassified
        // 현재는 정리되지 않은 부분 (추후 확장 가능)
        #endregion
    }
    /* ==========================================================================  
       Description : Cleaning Parameter (.pls) plasma  
       ========================================================================== */
    public class TCleanParam
    {
        // ============================================================  
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화 (Constructor / Initialization)  
        // ============================================================  
        #region ObjectLifecycle
        public TCleanParam()
        {
            LogHead = "CLEAN PARAM";
            OverPress = 0.0;
            OverPressTime = 0;

            CleanItems = new VCLEANITEM();
            CleanItems.Clear();
            //for (int i = 0; i < (int)CleanStepType.Max; i++)
            //    CleanItems.Add(new TCleanItem());
        }

        public void Clear()
        {
            foreach (var item in CleanItems)
                item.Clear(); // TCleanItem에 Clear 메서드가 정의돼 있다고 가정  
        }

        public void CopyFrom(TCleanParam arg)
        {
            if (arg == null || arg.CleanItems == null) return;

            OverPress = arg.OverPress;
            OverPressTime = arg.OverPressTime;

            CleanItems.Clear();
            foreach (var argItem in arg.CleanItems)
            {
                var newItem = new TCleanItem();
                newItem.CopyFrom(argItem);
                CleanItems.Add(newItem);
            }
        }
        #endregion

        // ============================================================  
        // Description : [2] Properties (속성 및 설정 값)  
        // ============================================================  
        #region Properties
        public bool IsLoaded { get; set; } = false;
        public string LogHead { get; set; }
        public double OverPress { get; set; }       // 세정 중 압력 상승값  
        public int OverPressTime { get; set; }      // 세정 시간 제한  
        public VCLEANITEM CleanItems { get; set; }  // C++의 VCLEANITEM(vector) 대응  
        #endregion

        // ============================================================  
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)  
        // ============================================================  
        #region AccessorsComputation
        // 세정의 토탈 시간  
        public int GetTotalTime()
        {
            int totalTime = 0;

            foreach (var item in CleanItems)
            {
                totalTime += item.StepTime; // C++의 `nStepTime`을 `StepTime`으로 변환  
            }

            return totalTime;
        }

        // 세정 단계 개수 반환  
        public int GetStepCount()
        {
            return CleanItems.Count; // `size()` → `Count`로 변환  
        }
        #endregion

        // ============================================================  
        // Description : [4] Internal Logic / Validation ( 데이터 검증)  
        // ============================================================  
        #region InternalLogicValidation
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

                // CLEAN PARAMETER
                if (ini.SectionExists("CLEAN PARAMETER"))
                {
                    for (int i = 0; i < (int)CleanStepType.Max; ++i)
                    {
                        string key = $"Step_{i:D2}";
                        string val = ini.ReadString("CLEAN PARAMETER", key, string.Empty);

                        if (string.IsNullOrWhiteSpace(val))
                            continue;

                        var values = val.Split(',');
                        if (values.Length < 7)
                            return false;

                        var cleanStep = new TCleanItem
                        {
                            StartVac = double.TryParse(values[0], out var startVac) ? startVac : throw new Exception("Invalid StartVac"),
                            RfPower = int.TryParse(values[1], out var rfPwr) ? rfPwr : 0,
                            GasFlow = new int[(int)MfcType.Max],
                            StepTime = int.TryParse(values[6], out var stepTime) ? stepTime : 0
                        };

                        for (int j = 0; j < (int)MfcType.Max; j++)
                        {
                            cleanStep.GasFlow[j] = int.TryParse(values[j + 2], out var gas) ? gas : 0;
                        }

                        if (RecipeExtern.IsCorrectStep(cleanStep))
                        {
                            //CleanItems[i].CopyFrom(cleanStep);
                            CleanItems.Add(cleanStep);
                        }
                        else
                        {
                            Console.WriteLine($"Wrong Clean Parameter Step_{i}, [{filePath}]");
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Section 'CLEAN PARAMETER' not found in {filePath}");
                    return false;
                }

                // ERROR PARAMETER
                if (ini.SectionExists("ERROR PARAMETER"))
                {
                    OverPress = double.TryParse(ini.ReadString("ERROR PARAMETER", "Over Pressure", "0.0"), out var dOverPress) ? dOverPress : 0.0;
                    OverPressTime = int.TryParse(ini.ReadString("ERROR PARAMETER", "Over Pressure Time", "0"), out var nOverPressTime) ? nOverPressTime : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            IsLoaded = true;
            return true;
        }

        public bool Save(string filePath, bool isRemote = false)
        {
            bool bRet = false;
            try
            {
                var ini = new VSIniFile(filePath);

                // SECTION: CLEAN PARAMETER
                for (int i = 0; i < CleanItems.Count; i++)
                {
                    var item = CleanItems[i];
                    string key = $"Step_{i:D2}";

                    // 기본 구성: StartVac, RfPower, GasFlow[0~4], StepTime
                    string value = $"{item.StartVac},{item.RfPower}";

                    for (int j = 0; j < (int)MfcType.Max; j++)
                    {
                        value += $",{item.GasFlow[j]}";
                    }

                    value += $",{item.StepTime}";

                    ini.WriteString("CLEAN PARAMETER", key, value);
                }

                // SECTION: ERROR PARAMETER
                ini.WriteString("ERROR PARAMETER", "Over Pressure", OverPress.ToString());
                ini.WriteString("ERROR PARAMETER", "Over Pressure Time", OverPressTime.ToString());

                Debug.WriteLine($"✅ Saved file: {filePath}, Remote: {isRemote}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error during saving: {ex.Message}");
                return bRet;
            }

            return true;
        }
        #endregion

        // ============================================================  
        // Description : [5] Unclassified (추후 정리 예정)  
        // ============================================================  
        #region Unclassified
        public void MakeDefault()
        {
            LogHead = "Default";
            OverPress = 0.0;
            OverPressTime = 0;
            CleanItems.Clear();
        }
        #endregion
    }

    // Interfaces for Clean =========
    //public double GetStartVac(int step) => CleanItems[step].StartVac;
    //public int GetRfPower(int step) => CleanItems[step].RfPower;
    //public int GetStepTimeSet(int step) => CleanItems[step].StepTime;
    //public int GetGasSp(int step, int gas) => CleanItems[step].GetGasSp(gas);
    //public int GetTotalCleanTime() => CleanItems.Sum(item => item.StepTime);


}
