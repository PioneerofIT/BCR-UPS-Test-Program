using System.Runtime.InteropServices;


namespace VSP.CONTROLLER.AJIN
{

    public class CAxtCtrl
    {
        // Axt 라이브러리 초기화 및 종료
        [DllImport("AxtLib.dll")]
        public static extern int AxtInitialize(IntPtr hWnd, short nIrqNo);

        [DllImport("AxtLib.dll")]
        public static extern short AxtOpenDeviceAuto(short BusType);

        [DllImport("AxtLib.dll")]
        public static extern void AxtCloseDeviceAll();

        [DllImport("AxtLib.dll")]
        public static extern void AxtClose();

        // Axt 라이브러리 초기화 및 장치 열기
        public static bool OpenAxtLib()
        {
            if (AxtInitialize(IntPtr.Zero, -1) != 0) // 초기화 실패 확인
            {
                //LogHelper.LOG_PRINTF("AXT", "AAA");
                //UtilExtern.ShowInitialMessage("AxtInitialize() Fail");
                //QueryMsgDlg.ShowMsg("AxtInitialize() Fail");w
                //util.ShowMsg();
                return false;
            }

            if (AxtOpenDeviceAuto(1) != 0) // 장치 오픈 실패 확인
            {
                //QueryMsgDlg.ShowMsg("AxtOpenDeviceAuto() Fail");
                return false;
            }

            return true;
        }

        // Axt 라이브러리 닫기 및 장치 해제
        public static void CloseAxtLib()
        {
            AxtCloseDeviceAll();
            AxtClose();
        }
    }
}
