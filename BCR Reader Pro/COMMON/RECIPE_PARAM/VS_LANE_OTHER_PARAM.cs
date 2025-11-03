using System.Collections;
using System.Diagnostics;
using System.IO;



namespace VSP.COMMON.RECIPE_PARAM
{
    /*===========================================================================
    Description	: TLaneSkipInfo struct
    ===========================================================================*/
    public struct TLaneSkipInfo 
    {
        private BitArray laneSkip; // 멤버 변수 네이밍 통일

        public TLaneSkipInfo(int laneCount)
        {
            laneSkip = new BitArray(laneCount);
        }

        public void Reset()
        {
            laneSkip.SetAll(false);
        }

        // ✅ 연산자 오버로딩: ==, !=
        public static bool operator ==(TLaneSkipInfo left, TLaneSkipInfo right)
        {
            return left.laneSkip.Equals(right.laneSkip);
        }

        public static bool operator !=(TLaneSkipInfo left, TLaneSkipInfo right)
        {
            return !left.laneSkip.Equals(right.laneSkip);
        }

        // ✅ 대입 연산자(`operator=`) 역할을 하는 CopyFrom
        public void CopyFrom(TLaneSkipInfo other)
        {
            for (int i = 0; i < laneSkip.Length; i++)
            {
                laneSkip[i] = other.laneSkip[i];
            }
        }

        // ✅ 특정 Lane이 Skip 상태인지 확인
        public bool IsLaneSkipped(int index)
        {
            Debug.Assert(index >= 0 && index < laneSkip.Length, "Index out of range");
            return laneSkip[index];
        }

        // ✅ Lane Skip 설정
        public void SetLaneSkip(int index, bool skip)
        {
            Debug.Assert(index >= 0 && index < laneSkip.Length, "Index out of range");
            laneSkip[index] = skip;
        }

        // ✅ 모든 Lane이 Skip 상태인지 확인
        public bool IsAllLanesSkipped()
        {
            return laneSkip.Cast<bool>().All(bit => bit);
        }

        // ✅ Equals & GetHashCode 오버라이드 (객체 비교 가능하도록)
        public override bool Equals(object obj)
        {
            if (obj is TLaneSkipInfo other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return laneSkip.GetHashCode();
        }
    }
   
    /*===========================================================================
    Description	: TTimerCountInfo struct
    ===========================================================================*/
    public struct TTimerCountInfo
    {
        private int[] timerValues;

        public TTimerCountInfo(int recipeTcMax)
        {
            timerValues = new int[recipeTcMax];
        }

        public void Reset()
        {
            Array.Clear(timerValues, 0, timerValues.Length);
        }

        public static bool operator ==(TTimerCountInfo left, TTimerCountInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TTimerCountInfo left, TTimerCountInfo right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TTimerCountInfo other)
        {
            Array.Copy(other.timerValues, timerValues, timerValues.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is TTimerCountInfo other)
                return timerValues.SequenceEqual(other.timerValues);
            return false;
        }

        public override int GetHashCode()
        {
            return timerValues.Aggregate(17, (hash, val) => hash * 31 + val.GetHashCode());
        }
    }
    
    /*===========================================================================
    Description	: TConvSpeedInfo struct
    ===========================================================================*/
    public struct TConvSpeedInfo
    {
        private int[] convSpeed;

        public TConvSpeedInfo(int velocityMax)
        {
            convSpeed = new int[velocityMax];
        }

        public void Reset()
        {
            Array.Clear(convSpeed, 0, convSpeed.Length);
        }

        public static bool operator ==(TConvSpeedInfo left, TConvSpeedInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TConvSpeedInfo left, TConvSpeedInfo right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TConvSpeedInfo other)
        {
            Array.Copy(other.convSpeed, convSpeed, convSpeed.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is TConvSpeedInfo other)
                return convSpeed.SequenceEqual(other.convSpeed);
            return false;
        }

        public override int GetHashCode()
        {
            return convSpeed.Aggregate(17, (hash, speed) => hash * 31 + speed.GetHashCode());
        }
    }
  
    /*===========================================================================
    Description	: Lane Use Skip & Other Parameter  (.vsr) visionsemicon recipe
    ===========================================================================*/
    public struct TRecipeItems
    {
        private int[] recipeItem;

        public TRecipeItems(int itemMax)
        {
            recipeItem = new int[itemMax];
        }

        public void Reset()
        {
            Array.Clear(recipeItem, 0, recipeItem.Length);
        }

        public static bool operator ==(TRecipeItems left, TRecipeItems right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TRecipeItems left, TRecipeItems right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TRecipeItems other)
        {
            Array.Copy(other.recipeItem, recipeItem, recipeItem.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is TRecipeItems other)
                return recipeItem.SequenceEqual(other.recipeItem);
            return false;
        }

        public override int GetHashCode()
        {
            return recipeItem.Aggregate(17, (hash, item) => hash * 31 + item.GetHashCode());
        }
    }

    /*===========================================================================
    Description	: Lane Use Skip & Other Parameter  (.vsr) visionsemicon recipe
    ===========================================================================*/
    public struct TLaneOtherParam
    {
        public string LogHead;
        public List<TLaneSkipInfo> LaneSkipOpt;
        public TTimerCountInfo TimerCount;
        public TRecipeItems OtherItems;
        public TConvSpeedInfo ConvSpeed;

        public TLaneOtherParam(int layerMax)
        {
            LogHead = string.Empty;
            LaneSkipOpt = new List<TLaneSkipInfo>(Enumerable.Repeat(new TLaneSkipInfo(), layerMax));
            TimerCount = new TTimerCountInfo();
            OtherItems = new TRecipeItems();
            ConvSpeed = new TConvSpeedInfo();
        }

        public void Clear()
        {
            LogHead = string.Empty;
            LaneSkipOpt.Clear();
            TimerCount = new TTimerCountInfo();
            OtherItems = new TRecipeItems();
            ConvSpeed = new TConvSpeedInfo();
        }

        public static bool operator ==(TLaneOtherParam left, TLaneOtherParam right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TLaneOtherParam left, TLaneOtherParam right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TLaneOtherParam other)
        {
            LogHead = other.LogHead;
            LaneSkipOpt = new List<TLaneSkipInfo>(other.LaneSkipOpt);
            TimerCount = other.TimerCount;
            OtherItems = other.OtherItems;
            ConvSpeed = other.ConvSpeed;
        }

        public override bool Equals(object obj)
        {
            if (obj is TLaneOtherParam other)
                return LogHead == other.LogHead &&
                       LaneSkipOpt.SequenceEqual(other.LaneSkipOpt) &&
                       TimerCount.Equals(other.TimerCount) &&
                       OtherItems.Equals(other.OtherItems) &&
                       ConvSpeed.Equals(other.ConvSpeed);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LogHead, LaneSkipOpt, TimerCount, OtherItems, ConvSpeed);
        }

        public bool Load(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            // 파일 읽기 로직 추가 (예제: 텍스트 파일로 저장된 값 불러오기)
            LogHead = File.ReadAllText(filePath);
            return true;
        }

        public void Save(string filePath, bool isRemote = false)
        {
            File.WriteAllText(filePath, LogHead);
        }

        public void MakeDefault()
        {
            LogHead = "Default";
        }

        public int GetLayerSkipValue(int layer)
        {
            return layer >= 0 && layer < LaneSkipOpt.Count ? LaneSkipOpt[layer].GetHashCode() : 0;
        }

        public void SetLayerSkipValue(int layer, int val)
        {
            if (layer >= 0 && layer < LaneSkipOpt.Count)
                LaneSkipOpt[layer] = new TLaneSkipInfo(val);
        }

        public bool IsLayerLaneSkip(int layer, int lane)
        {
            return layer >= 0 && layer < LaneSkipOpt.Count && LaneSkipOpt[layer].IsLaneSkipped(lane);
        }

        public void SetLayerLaneSkip(int layer, int lane, bool skip)
        {
            if (layer >= 0 && layer < LaneSkipOpt.Count)
                LaneSkipOpt[layer].SetLaneSkip(lane, skip);
        }

        public bool IsLayerLaneSkipAll(int layer)
        {
            return layer >= 0 && layer < LaneSkipOpt.Count && LaneSkipOpt[layer].IsAllLanesSkipped();
        }

        public bool IsLaneSkipAll()
        {
            return LaneSkipOpt.All(lane => lane.IsAllLanesSkipped());
        }

        public int GetTotalSkipLaneCnt()
        {
            return LaneSkipOpt.Sum(lane => lane.GetHashCode());
        }
    }

}
