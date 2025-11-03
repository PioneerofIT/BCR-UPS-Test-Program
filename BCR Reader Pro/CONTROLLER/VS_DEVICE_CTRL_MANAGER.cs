
using VSP.CONTROLLER.AJIN;


namespace VSP.CONTROLLER
{

    public sealed class CVSDeviceCtrlManager
    {
        // ============================================================  
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화 (Constructor / Initialization)  
        // ============================================================  
        private CVSDeviceCtrlManager()
        {
            AxtLibInit = false;
            AxlLibInit = false;
            InitCtrlLibrary();
            CreateIoObjects();
            CreateIoCtrl();
            CreateMotionManger();

            //UtilExtern.ShowInitialMessage("Device Controller Manager 생성됨!");

           // InitializeValues();
        }

        ~CVSDeviceCtrlManager()
        {

            DeleteMotionManger();

            DeleteIoObjects();
            DeleteIoCtrl();

            CloseCtrlLibrary();
            //UtilExtern.ShowInitialMessage("Device Controller Manager 제거됨!");
            
        }

      

        public void Initialize()
        {
            if (!isInitialized)
            {
                //UtilExtern.ShowInitialMessage("CVSDeviceCtrlManager 초기화 완료!");
                isInitialized = true;
            }
        }

        //private void InitializeValues()
        //{
        //    DioVal = new TVsIoData();
        //    AioVal = new TVsIoData();
        //    MDioVal = new TVsIoData();
        //    NetVal = new TVsIoData();

        //    DioCtrl = new CIoCtrl();
        //    MDioCtrl = new CIoCtrl();
        //    DNetIoCtrl = new CIoCtrl();
        //    AiCtrl = new CIoCtrl();
        //    AoCtrl = new CIoCtrl();
        //}

        // ============================================================  
        // Description : [2] Properties (속성 및 설정 값)  
        // ============================================================  
        public static CVSDeviceCtrlManager Instance => instance ??= new CVSDeviceCtrlManager();
        private static CVSDeviceCtrlManager instance = null!; // null! null일리 없다고 컴파일러에 알려줌
        private static bool isInitialized = false; // 객체가 한 번만 생성되었는지 확인하는 변수

        public static CVS_IO_OBJ_MANAGER IoObjManager { get; private set; }

        private CMotionManager CMotionManager = new CMotionManager();
        public CMotionManager MotionManager => CMotionManager;

        private bool AxtLibInit = false;
        private bool AxlLibInit = false;
        private bool AdlinkInit = false;
        private bool CifInit = false;
        private bool CifXInit = false;

        private TVsIoData DioVal = new TVsIoData();
        private TVsIoData AioVal = new TVsIoData();
        private TVsIoData MDioVal = new TVsIoData();
        private TVsIoData NetVal = new TVsIoData();

        public CIoCtrl DiCtrl { get; set; } = new CIoCtrl();
        public CIoCtrl DoCtrl { get; set; } = new CIoCtrl();
        public CIoCtrl MDiCtrl { get; set; } = new CIoCtrl();
        public CIoCtrl DNetIoCtrl { get; set; } = new CIoCtrl();
        public CIoCtrl AiCtrl { get; set; } = new CIoCtrl();
        public CIoCtrl AoCtrl { get; set; } = new CIoCtrl();


        public bool IsAxtLibInit => AxtLibInit;
        public bool IsAxlLibInit => AxlLibInit;
        public bool IsAdlinkLibInit => AdlinkInit;
        public bool IsCifLibInit => CifInit;
        public bool IsCifXLibInit => CifXInit;


        // ============================================================  
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)  
        // ============================================================  

        public byte GetDioByte(bool isOut, int idx) { /* 구현 필요 */ return 0; }
        public void SetDioByte(bool isOut, int idx, byte val) { /* 구현 필요 */ }

        public ushort GetDioWord(bool isOut, int idx) { /* 구현 필요 */ return 0; }
        public void SetDioWord(bool isOut, int idx, ushort val) { /* 구현 필요 */ }


        // ============================================================  
        // Description : [4] Internal Logic / Validation ( 데이터 검증)  
        // ============================================================  
        #region InternalLogicValidation
        private void InitCtrlLibrary() 
        {
            AxtLibInit = CAxtCtrl.OpenAxtLib();
        }
        private void CloseCtrlLibrary()
        {
            if (AxtLibInit)
                CAxtCtrl.CloseAxtLib();
        }

      
        public static void CreateIoObjects()
        {
            IoObjManager = new CVS_IO_OBJ_MANAGER();
        }

        private void CreateIoCtrl()
        {
            DiCtrl = new CAxtDioCtrl();              // ① 실제 컨트롤러 생성
            CDI.SetIoController(DiCtrl);             // ② CDI 전체에 컨트롤러 바인딩
            Din.Initialize(DiCtrl);

            DoCtrl = new CAxtDioCtrl();              // ① 실제 컨트롤러 생성
            CDO.SetIoController(DoCtrl);             // ② CDI 전체에 컨트롤러 바인딩
            Dout.Initialize(DoCtrl);


            //MDiCtrl = new CAxtMotionDioCtrl();              // ① 실제 컨트롤러 생성
            //CDO.SetIoController(DoCtrl);             // ② CDI 전체에 컨트롤러 바인딩

            //AiCtrl = new CAdlinkIoCtrl();              // ① 실제 컨트롤러 생성
            //CAnalogIn.SetIoController(AiCtrl);             // ② CDI 전체에 컨트롤러 바인딩

            //AoCtrl = new CAxtAioCtrl();
            //CAnalogOut.SetIoController(AoCtrl);

            //DiCtrl = new SimDioCtrl();              // ① 실제 컨트롤러 생성
            CDI.SetIoController(DiCtrl);             // ② CDI 전체에 컨트롤러 바인딩

            DoCtrl = DiCtrl;              // ① 실제 컨트롤러 생성
            CDO.SetIoController(DoCtrl);             // ② CDI 전체에 컨트롤러 바인딩

            //MDiCtrl = new SimMotionDioCtrl(); ;              // ① 실제 컨트롤러 생성
            CDI_M.SetIoController(MDiCtrl);             // ② CDI 전체에 컨트롤러 바인딩

            //AiCtrl = new CSimAioCtrl();              // ① 실제 컨트롤러 생성
            CAnalogIn.SetIoController(AiCtrl);             // ② CDI 전체에 컨트롤러 바인딩

            AoCtrl = AiCtrl;
            CAnalogOut.SetIoController(AoCtrl);

            //SimIOLinker obj = new SimIOLinker();
            //obj.ReadConfigFileAndLink();

            // 이후부터 CDI 객체들이 내부적으로 DiCtrl을 사용 가능!
        }

        private void DeleteIoObjects() { /* IO 객체 삭제 */ }
        private void DeleteIoCtrl() { /* IO 컨트롤 삭제 */ }

        private void SetAnalogCalData() { /* 아날로그 데이터 설정 */ }
        #endregion

        // ============================================================  
        // Description : [5] Unclassified (추후 정리 예정)  
        // ============================================================  
        #region Unclassified
        private void CreateMotionManger() { /* 모션 매니저 생성 */ }
        private void DeleteMotionManger() { /* 모션 매니저 삭제 */ }
        #endregion
    }

}
