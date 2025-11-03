using System.Diagnostics;
using VSP.COMMON;
using VSP.COMMON.RECIPE_PARAM;


namespace VSP.COMMON
{
    /* ==========================================================================
    Description	: Recipe Manipulation public static hsjangstatic
    ========================================================================== */
    public class RecipeExtern
    {
        public static bool IsCorrectStep(TCleanItem cleanItem)
        {
            int useGasFlowSum = 0;

            if (!UtilExtern.InRange(cleanItem.StartVac, 0.05, 0.5))
            {
                Debug.WriteLine($"Wrong vacuum value [{cleanItem.StartVac:F3}]\nInput value within 0.05 ~ 0.5");
                return false;
            }

            if (!UtilExtern.InRange(cleanItem.RfPower, 1, CVS_SYS_OPTION.Instance.RfGenPwrLmt))
            {
                Debug.WriteLine($"Wrong rf value [{cleanItem.RfPower}]\nInput value within 1 ~ {SysOption.Manager.RfGenCap}");
                return false;
            }

            for (int i = 0; i < cleanItem.GasFlow.Length; i++)
            {
                if (SysOption.Manager.MfcOpt[i].GasUse && !UtilExtern.InRange(cleanItem.GasFlow[i], 0, SysOption.Manager.MfcOpt[i].MfcCap))
                {
                    Debug.WriteLine($"Wrong gas{i} value [{cleanItem.GasFlow[i]}]\nInput value within 0 ~ {SysOption.Manager.MfcOpt[i].MfcCap}");
                    return false;
                }
                else if(SysOption.Manager.MfcOpt[i].GasUse)
                {
                    useGasFlowSum += cleanItem.GasFlow[i];
                }
            }

            if (useGasFlowSum == 0)
            {
                Debug.WriteLine("All gas flow is '0'");
                return false;
            }

            if (cleanItem.StepTime <= 0)
            {
                Debug.WriteLine($"Wrong time value [{cleanItem.StepTime}]");
                return false;
            }

            return true;
        }
    }
    /* ==========================================================================
    Description	: Recipe Class(Servo + Cleaning + Lane & Other paramters .rcp)
    ========================================================================== */
    public class CvsRecipe
    {
        private bool[] _loaded = new bool[(int)RecipeType.Max];
        private string _logHead;

        private TMotionParam _motionVal = new TMotionParam();
        private TCleanParam _cleanVal = new TCleanParam();
        private TLaneOtherParam _laneOtherVal = new TLaneOtherParam();

        public void Clear()
        {
            _motionVal = new TMotionParam();
            _cleanVal = new TCleanParam();
            _laneOtherVal = new TLaneOtherParam();

            for (int i = 0; i < _loaded.Length; i++)
                _loaded[i] = false;

            _logHead = string.Empty;
        }

        public void SaveRemoteRecipe(string filePath)
        {
            // TODO: 파일 저장 로직 구현
            Console.WriteLine($"[Save] Remote recipe to {filePath}");
        }

        public TMotionParam GetMotionRcp() => _motionVal;
        public void SetMotionRcp(TMotionParam mtrVal) => _motionVal = mtrVal;

        public TCleanParam GetCleanRcp() => _cleanVal;
        public void SetCleanRcp(TCleanParam clnVal) => _cleanVal = clnVal;

        public TLaneOtherParam GetLaneOtherRcp() => _laneOtherVal;

        // ===== Motion Interfaces =====
        public double GetMaxLimit(int motor) => _motionVal.GetMaxLimit(motor);
        public double GetMinLimit(int motor) => _motionVal.GetMinLimit(motor);
        public double GetPosition(int motor, int posId) => _motionVal.GetPosition(motor, posId);
        public double GetVelocity(int motor, int posId) => _motionVal.GetVelocity(motor, posId);
        public double GetAccel(int motor, int posId) => _motionVal.GetAccel(motor, posId);

        // ===== Cleaning Interfaces =====
        public double GetStartVac(int step) => _cleanVal.CleanItems[step].StartVac;
        public int GetGasSp(int step, int gas) => _cleanVal.CleanItems[step].GetGasSp(gas);
        public int GetRfPower(int step) => _cleanVal.CleanItems[step].RfPower;
        public int GetStepTimeSet(int step) => _cleanVal.CleanItems[step].StepTime;
        public double GetOverPressVal() => _cleanVal.OverPress;
        public int GetOverPressTime() => _cleanVal.OverPressTime;
        public int GetTotalCleanTime() => _cleanVal.GetTotalTime();

        // ===== Load / Reset =====
        public bool IsLoadedAll() => _loaded.All(l => l);
        public bool IsLoaded(int type) => type >= 0 && type < _loaded.Length && _loaded[type];

        public void ResetCleanParam() => _cleanVal = new TCleanParam();

        public bool LoadCleanRecipe(string recipeName)
        {
            // TODO: 로딩 로직
            Console.WriteLine($"Loading Clean Recipe: {recipeName}");
            return true;
        }

        public bool LoadDeviceRecipe(string recipeName)
        {
            Console.WriteLine($"Loading Device Recipe: {recipeName}");
            return true;
        }

        public bool LoadLaneOtherRecipe(string recipeName)
        {
            Console.WriteLine($"Loading LaneOther Recipe: {recipeName}");
            return true;
        }
    }

    public static class RecipeUtils
    {
        public static bool IsCorrectStep(TCleanItem cleanItem)
        {
            int totalGasFlow = 0;

            if (!UtilExtern.InRange(cleanItem.StartVac, 0.05, 0.5))
            {
                UtilExtern.ShowMsg($"Wrong vacuum value [{cleanItem.StartVac:F3}]\nInput value within 0.05 ~ 0.5");
                return false;
            }

            int maxRfPower = SysOption.Manager.RfGenCap;
            if (!UtilExtern.InRange(cleanItem.RfPower, 1, maxRfPower))
                UtilExtern.ShowMsg($"Wrong RF value [{cleanItem.RfPower}]\nInput value within 1 ~ {maxRfPower}");
            return false;


            for (int i = (int)MfcType.MFC_1; i < (int)MfcType.Max; i++)
            {
                int gasCap = SysOption.Manager.MfcOpt[i].MfcCap;
                if (SysOption.Manager.MfcOpt[i].GasUse && cleanItem.GasFlow[i] > gasCap)
                {
                    UtilExtern.ShowMsg($"Wrong gas{i} value [{cleanItem.GasFlow[i]}]\nInput value within 1 ~ {gasCap}");
                    return false;
                }
                totalGasFlow += cleanItem.GasFlow[i];
            }

            if (totalGasFlow == 0)
            {
                UtilExtern.ShowMsg("All gas flow is '0'");
                return false;
            }

            if (cleanItem.StepTime <= 0)
            {
                UtilExtern.ShowMsg($"Wrong time value [{cleanItem.StepTime}]");
                return false;
            }

            return true;
        }

        public static int SeparateRcpParam(string path, string recipe)
        {
            // TODO: 파일 분리 로직 구현
            Console.WriteLine($"Separating: {path}, {recipe}");
            return 0; // 예시 결과
        }

        public static CvsRecipe MergeIntoRecipe(string path, string recipe, bool remote = false)
        {
            //// TODO: 병합 로직 구현
            //// ==================== [1] File Paths ====================
            //string strRcpPath, strParamName;

            //// ==================== [2] Clean Parameter Load ====================
            //TCleanParam CleanVal = new TCleanParam();
            //strParamName = GetCleanRcpName(strRecipe);
            //strRcpPath = $"{Global.GetDataDir()}{strParamName}.pls";
            //CleanVal.Load(strRcpPath);

            //// ==================== [3] Motion Parameter Load ====================
            //TMotionParam MotionVal = new TMotionParam();
            //strParamName = GetMotionRcpName(strRecipe);
            //strRcpPath = $"{Global.GetDataDir()}{strParamName}.svr";
            //MotionVal.Load(strRcpPath);
            //MotionVal.Save(strPath, bRemote);

            //// ==================== [4] Lane Other Parameter Load ====================
            //TLaneOtherParam LaneOtherVal = new TLaneOtherParam();
            //strRcpPath = $"{Global.GetDataDir()}{strRecipe}.vsr";
            //LaneOtherVal.Load(strRcpPath);
            //LaneOtherVal.Save(strPath, bRemote);

            // ==================== [5] Merge Process ====================
            CvsRecipe Rcp = new CvsRecipe();
            //Rcp.SetCleanRcp(CleanVal);
            //Rcp.SetMotionRcp(MotionVal);
            //Rcp.SaveRemoteRecipe(strPath);

            return Rcp;
        }

        public static bool MakeRecipeName(out string recipeName, string clean, string motion)
        {
            recipeName = $"{clean}_{motion}";
            return true;
        }

        public static bool GetCleanRcpName(out string cleanRcp, string recipeName)
        {
            var parts = recipeName.Split('_');
            cleanRcp = parts.Length >= 1 ? parts[0] : "";
            return !string.IsNullOrEmpty(cleanRcp);
        }

        public static bool GetMotionRcpName(out string motionRcp, string recipeName)
        {
            var parts = recipeName.Split('_');
            motionRcp = parts.Length >= 2 ? parts[1] : "";
            return !string.IsNullOrEmpty(motionRcp);
        }
    }

}
