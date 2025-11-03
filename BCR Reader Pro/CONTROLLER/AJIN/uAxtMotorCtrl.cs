using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VS_MTR; 

namespace VSP.CONTROLLER.AJIN
{

    public class CAxtMotorCtrl : CMotorCtrl 
    {
        public CAxtMotorCtrl(MotorType nCard) : base(nCard)
        { 
        
        }

        public static CMotorCtrl Create(MotorType nCard)
        {
            return new CAxtMotorCtrl(nCard);
        }

        public override bool OpenDevice()
        {
            if (CAxtCAMCFS20.CFS20IsInitialized() == -1)
            {
                MotorCtrlInit = false;
                return false;
            }
            else
            {
                CAxtCAMCFS20.CFS20KeSetMainClk(16384000);
                MotorCtrlInit = true;
                return true;
            }
        }
        public override void CloseDevice()
        { 
        
        }

        // Initial Setting Methods
        public virtual bool SetDecelPulseMode(int nAxis, uint decelstart, uint pulseout, uint detectsignal)
        { 
            if(MotorCtrlInit)
            {
                switch(CardType) //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_drive_mode1((short)nAxis, (byte)decelstart, (byte)pulseout, (byte)detectsignal);
                        break;
                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_drive_mode1((short)nAxis, (byte)decelstart, (byte)pulseout, (byte)detectsignal);
                        break;
                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_drive_mode1((short)nAxis, (byte)decelstart, (byte)pulseout, (byte)detectsignal);
                        break;
                    default:
                        break;
                }
            }
            return true; 
        }
        public virtual bool SetDriveMode(int nAxis, uint enc_method, uint dwStopMode) 
        {
            return true; 
        }
        public virtual bool SetInSignals(int nAxis, uint enc_method, uint dwInpos, uint dwAlarm,
            uint dwNSLmt, uint dwPSLmt, uint dwNLmt, uint dwPLmt, bool bEncReverse)
        {
            Inposlevel = dwInpos;
            Alarmlevel = dwAlarm;
            Nlimitlevel = dwNSLmt;
            PlimitLevel = dwPLmt;
            int DirIncrese = bEncReverse ? 1 : 0;

            if (MotorCtrlInit)
            {
                switch (CardType)
                {
                    case MotorType.CAM5: //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                        CAxtCAMC5M.C5Mset_drive_mode2((short)nAxis, (byte)enc_method, (byte)dwInpos, (byte)dwAlarm,
                            (byte)dwNSLmt, (byte)dwPSLmt, (byte)dwNLmt, (byte)dwPLmt);
                        break;
                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_drive_mode2((short)nAxis, (byte)enc_method, (byte)dwInpos, (byte)dwAlarm,
                            (byte)dwNSLmt, (byte)dwPSLmt, (byte)dwNLmt, (byte)dwPLmt);
                        break;
                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_drive_mode2((short)nAxis, (byte)enc_method, (byte)dwInpos, (byte)dwAlarm,
                            (byte)dwNSLmt, (byte)dwPSLmt, (byte)dwNLmt, (byte)dwPLmt);
                        CAxtCAMCFS20.CFS20set_enc_reverse((short)nAxis, (byte)DirIncrese);
                        break;
                    default: break;
                }
            }
            
            return true; 
        }

        public virtual bool SetServoOnLevel(int nAxis, uint dwLevel)
        {
            if (!MotorCtrlInit) { return false; }

            //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
            switch (CardType) 
            {
                case MotorType.CAM5:
                    CAxtCAMC5M.C5Mset_servo_level((short)nAxis, (byte)dwLevel);
                    break;

                case MotorType.CFS:
                    CAxtCAMCFS.CFSset_servo_level((short)nAxis, (byte)dwLevel);
                    break;

                case MotorType.CFS2:
                    CAxtCAMCFS20.CFS20set_servo_level((short)nAxis, (byte)dwLevel);
                    break;

                default :
                    CAxtCAMCFS20.CFS20set_servo_level((short)nAxis, (byte)dwLevel);
                    break;
            }
            return true;
        }
        public virtual bool SetAlarmOnLevel(int nAxis, uint dwLevel)
        {
            if (!MotorCtrlInit ) { return false; }

            //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
            switch (CardType)
            {
                case MotorType.CAM5:
                    CAxtCAMC5M.C5Mset_alarm_level((short)nAxis, (byte)dwLevel);
                    break;

                case MotorType.CFS:
                    CAxtCAMCFS.CFSset_alarm_level((short)nAxis, (byte)dwLevel);
                    break;

                case MotorType.CFS2:
                    CAxtCAMCFS20.CFS20set_alarm_level((short)nAxis, (byte)dwLevel);
                    break;

                default:
                    CAxtCAMCFS20.CFS20set_alarm_level((short)nAxis, (byte)dwLevel);
                    break;
            }
            return true;
        }
        public virtual bool SetAlarmResetLevel(int nAxis, uint dwLevel) { return true; }
        public virtual bool SetInPosLevel(int nAxis, uint dwLevel) 
        {
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType)
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_inposition_level((short)nAxis, (byte)dwLevel);
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_inposition_level((short)nAxis, (byte)dwLevel);
                        break;

                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_inposition_level((short)nAxis, (byte)dwLevel);
                        break;

                    default:
                        CAxtCAMCFS20.CFS20set_inposition_level((short)nAxis, (byte)dwLevel);
                        break;
                }
            }
            return true; 
        
        }
        public virtual bool SetEmgInputLevel(int nAxis, uint dwLevel)
        {
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType) // CAxtCAMC5M emg level 없다.
                {
                    case MotorType.CAM5:
                        //CAxtCAMC5M.CFSset_emg_signal_enable((short)nAxis, (byte)dwLevel); 
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_emg_signal_enable((short)nAxis, (byte)dwLevel);
                        break;

                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_emg_signal_enable((short)nAxis, (byte)dwLevel);
                        break;

                    default:
                        CAxtCAMCFS20.CFS20set_emg_signal_enable((short)nAxis, (byte)dwLevel);
                        break;
                }
            }
            return true; 
        }

        public virtual bool SetMovePulsePerUnit(int nAxis, double dbPulse)
        {
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType) 
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_movepulse_perunit((short)nAxis, (Int32)dbPulse);
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_movepulse_perunit((short)nAxis, dbPulse);
                        break;

                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_movepulse_perunit((short)nAxis, dbPulse);
                        break;

                    default:
                        CAxtCAMCFS20.CFS20set_movepulse_perunit((short)nAxis, dbPulse);
                        break;
                }
            }
            return true;
        }
        public virtual bool SetStartStopSpeed(int nAxis, double dSpeed)
        {
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType)
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_startstop_speed((short)nAxis, dSpeed);
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_startstop_speed((short)nAxis, dSpeed);
                        break;

                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_startstop_speed((short)nAxis, dSpeed);
                        break;

                    default:
                        break;
                }
            }
            return true; 
        }
        public virtual bool SetMaxSpeed(int nAxis, double dbMax)
        {
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType)
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_max_speed((short)nAxis, dbMax);
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_max_speed((short)nAxis, dbMax);
                        break;

                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_max_speed((short)nAxis, dbMax);
                        break;

                    default:
                        CAxtCAMCFS20.CFS20set_max_speed((short)nAxis, dbMax);
                        break;
                }
            }
            return true; 
        }

        // I/O Functions
        public virtual void OutOnOff(int nAxis, byte bitNo, byte bStatus)
        {
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType)
                {
                    case MotorType.CAM5:
                        if(bStatus != 0)
                            CAxtCAMC5M.C5Mchange_output_bit((short)nAxis, bitNo, 1);
                        else
                            CAxtCAMC5M.C5Mreset_output_bit((short)nAxis, bitNo);
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_output_bit((short)nAxis, bitNo);
                        break;

                    case MotorType.CFS2:
                        if(bStatus != 0)
                            CAxtCAMCFS20.CFS20change_output_bit((short)nAxis, bitNo, 1);
                        else
                            CAxtCAMCFS20.CFS20change_output_bit((short)nAxis, bitNo, 0);
                        break;

                    default:
                         break;
                }
            }
        }
        public virtual void ServoEnable(int nAxis, bool bStatus)
        {
            byte OnOff =(byte)(bStatus ? 1 : 0);
            if (MotorCtrlInit)
            {
                //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
                switch (CardType)
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_servo_enable((short)nAxis, OnOff);
                        break;

                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_servo_enable((short)nAxis, OnOff);
                        break;

                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_servo_enable((short)nAxis, OnOff);
                        break;

                    default:
                        break;
                }
            }
        }
        public virtual bool IsServoEnabled(int nAxis)
        {
            //0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
            if (MotorCtrlInit)
            {
                if (CardType == MotorType.CAM5)
                    return CAxtCAMC5M.C5Mget_servo_enable((short)nAxis) == 1;
                else if (CardType == MotorType.CFS)
                    return CAxtCAMCFS.CFSget_servo_enable((short)nAxis) == 1;
                else if (CardType == MotorType.CFS2)
                    return CAxtCAMCFS20.CFS20get_servo_enable((short)nAxis) == 1;
                else
                    return CAxtCAMCFS.CFSget_servo_enable((short)nAxis) == 1;
            }

            return false;
          
        }
        public virtual void AlarmReset(int nAxis)
        {
            // COM_SVRON = 0,COM_ALMCLR = 1, COM_OUT2 = 2, COM_OUT3 = 3

            OutOnOff(nAxis, 1, 1);
            Thread.Sleep(200);
            OutOnOff(nAxis, 1, 0);
        }

        public virtual bool IsReady(int nAxis)
        { 
            return false; 
        }
        public virtual bool IsEmergencyOn(int nAxis)
        {
            // LOW(0x00): N.C, HIGH(0x01): N.O, UNUSED(0x02), USED(0x03): 현 상태 유지
            if (!MotorCtrlInit)
                return true;

            bool bRet = true;

            if (CardType == MotorType.CAM5)
                bRet = (CAxtCAMC5M.C5Mget_stop_sel((short)nAxis) == 0x00);
            //	else if (m_nCardType == CFS)
            //		bRet = (CFSget_stop_sel(nAxis) == LOW);
            else if (CardType == MotorType.CFS2)
                bRet = (CAxtCAMCFS20.CFS20get_stop_sel((short)nAxis) == 0x00);
            else
                bRet = (CAxtCAMCFS20.CFS20get_stop_sel((short)nAxis) == 0x00);

            return bRet;
        }
        public virtual bool IsAlarmOn(int nAxis)
        {
            // LOW(0x00): N.C, HIGH(0x01): N.O, UNUSED(0x02), USED(0x03): 현 상태 유지
            if (!MotorCtrlInit)
                return true;

            uint level, state;

            if (CardType == MotorType.CAM5)
            {
                state = CAxtCAMC5M.C5Mget_alarm_switch((short)nAxis);
                level = CAxtCAMC5M.C5Mget_alarm_level((short)nAxis);
            }
            else if (CardType == MotorType.CFS)
            {
                state = CAxtCAMCFS.CFSget_alarm_switch((short)nAxis);
                level = CAxtCAMCFS.CFSget_alarm_level((short)nAxis);
            }
            else if (CardType == MotorType.CFS2)
            {
                state = CAxtCAMCFS20.CFS20get_alarm_switch((short)nAxis);
                level = CAxtCAMCFS20.CFS20get_alarm_level((short)nAxis);
            }
            else
            {
                state = CAxtCAMCFS20.CFS20get_alarm_switch((short)nAxis);
                level = CAxtCAMCFS20.CFS20get_alarm_level((short)nAxis);
            }
            bool bRet = (state == 0x01);//level);
            
            return bRet;
          
        }
        public virtual bool IsInpos(int nAxis) 
        {
            if (!MotorCtrlInit)
                return false;

            bool bRet = true;

            if (CardType == MotorType.CAM5)
                bRet = (CAxtCAMC5M.C5Mget_inposition_switch((short)nAxis) == 0x01);
            else if (CardType == MotorType.CFS)
                bRet = (CAxtCAMCFS.CFSget_inposition_switch((short)nAxis) == 0x01);
            else if (CardType == MotorType.CFS2)
                bRet = (CAxtCAMCFS20.CFS20get_inposition_switch((short)nAxis) == 0x01);
            else
                bRet = (CAxtCAMCFS20.CFS20get_inposition_switch((short)nAxis) == 0x01);

            return true; 
        }
        public virtual void SetInposWait(int nAxis, byte bWait) 
        {
            if (!MotorCtrlInit)
                return;

            switch (CardType)
            {
                case MotorType.CAM5:
                    CAxtCAMC5M.C5Mset_inposition_enable((short)nAxis, bWait);
                    break;
                case MotorType.CFS:
                    CAxtCAMCFS.CFSset_inposition_enable((short)nAxis, bWait);
                    break;
                case MotorType.CFS2:
                    CAxtCAMCFS20.CFS20set_inposition_enable((short)nAxis, bWait);
                    break;
                default:
                    CAxtCAMCFS20.CFS20set_inposition_enable((short)nAxis, bWait);
                    break;
            }
        }

        public virtual bool IsPELM(int nAxis)
        {
            if (!MotorCtrlInit)
                return true;

            bool bRet = true;

            if (CardType == MotorType.CAM5)
                bRet = (CAxtCAMC5M.C5Mget_pend_limit_switch((short)nAxis) == 0x01);
            else if (CardType == MotorType.CFS)
                bRet = (CAxtCAMCFS.CFSget_pend_limit_switch((short)nAxis) == 0x01);
            else if (CardType == MotorType.CFS2)
                bRet = (CAxtCAMCFS20.CFS20get_pend_limit_switch((short)nAxis) == 0x01);
            else
                bRet = (CAxtCAMCFS20.CFS20get_pend_limit_switch((short)nAxis) == 0x01);

            return bRet;

        }
        public virtual bool IsNELM(int nAxis)
        {
            if (!MotorCtrlInit)
                return true;

            bool bRet = true;

            if (CardType == MotorType.CAM5)
                bRet = (CAxtCAMC5M.C5Mget_nend_limit_switch((short)nAxis) == 0x01);
            else if (CardType == MotorType.CFS)
                bRet = (CAxtCAMCFS.CFSget_nend_limit_switch((short)nAxis) == 0x01);
            else if (CardType == MotorType.CFS2)
                bRet = (CAxtCAMCFS20.CFS20get_nend_limit_switch((short)nAxis) == 0x01);
            else
                bRet = (CAxtCAMCFS20.CFS20get_nend_limit_switch((short)nAxis) == 0x01);

            return bRet; 
        }
        public virtual bool IsORG(int nAxis)
        {
            if (!MotorCtrlInit)
                return false;

            bool bRet = GetInputBit(nAxis, (int)SignalType.COM_ORG);

            return bRet;
        }
        public virtual bool IsEdgeOn(int nAxis, uint dwInPort) 
        {
            if (!MotorCtrlInit)
                return false;

            bool bRet = GetInputBit(nAxis, (int)dwInPort);

            return bRet;

        }

        public virtual void ToggleIO(int nAxis, byte byOutputBit)
        {
            if (!MotorCtrlInit)
                return;

            bool bRead = (GetOutputBit(nAxis, byOutputBit) == true );
            byte byState;

            if (bRead)
                byState = 0; // Toggle -> Io On 이면 끄고 Off면 키고
            else
                byState = 1;

            switch (CardType)
            {
                case MotorType.CAM5:
                    CAxtCAMC5M.C5Mchange_output_bit((short)nAxis, byOutputBit, byState);
                    break;
                case MotorType.CFS:
                    CAxtCAMCFS.CFSchange_output_bit((short)nAxis, byOutputBit, byState);
                    break;
                case MotorType.CFS2:
                    CAxtCAMCFS20.CFS20change_output_bit((short)nAxis, byOutputBit, byState);
                    break;
                default:
                    CAxtCAMCFS20.CFS20change_output_bit((short)nAxis, byOutputBit, byState);
                    break;
            }
        }
        public virtual uint GetMechSignal(int nAxis)
        {
            if (MotorCtrlInit)
            {
                if (CardType == MotorType.CAM5)
                    return CAxtCAMC5M.C5Mget_mechanical_signal((short)nAxis);
                else if (CardType == MotorType.CFS)
                    return CAxtCAMCFS.CFSget_mechanical_signal((short)nAxis);
                else if (CardType == MotorType.CFS2)
                    return CAxtCAMCFS20.CFS20get_mechanical_signal((short)nAxis);
                else
                    return CAxtCAMCFS20.CFS20get_mechanical_signal((short)nAxis);
            }

            return (uint)DETECT_DESTINATION_SIGNAL.PElmNegativeEdge;
        }

        // Motion Functions
        public virtual bool InMotion(int nAxis)
        {
            if (!MotorCtrlInit)
                return false;

            bool bRet = false;

            if (CardType == MotorType.CAM5)
                bRet = (CAxtCAMC5M.C5Min_motion((short)nAxis) == 1 ) ? true : false;
            else if (CardType == MotorType.CFS)
                bRet =(CAxtCAMCFS.CFSin_motion((short)nAxis) == 1 ) ? true : false;
            else if (CardType == MotorType.CFS2)
                bRet = (CAxtCAMCFS20.CFS20in_motion((short)nAxis) == 1 ) ? true : false;
            else
                bRet = (CAxtCAMCFS20.CFS20in_motion((short)nAxis) == 1 ) ? true : false;

            return bRet;

        }
        public virtual uint GetEndStatus(int nAxis)
        {
            uint byRet = 0x00;

            if (MotorCtrlInit)
            {
                if (CardType == MotorType.CAM5)
                    byRet = CAxtCAMC5M.C5Mget_end_status((short)nAxis);
                else if (CardType == MotorType.CFS)
                    byRet = CAxtCAMCFS.CFSget_end_status((short)nAxis);
                else if (CardType == MotorType.CFS2)
                    byRet = CAxtCAMCFS20.CFS20get_end_status((short)nAxis);
                else
                    byRet = CAxtCAMCFS20.CFS20get_end_status((short)nAxis);
            }

            //	if (byRet == 0x4400)	// 14bit: Limit(PELM, NELM, PSLM, NSLM, Soft)에 의한 종료
            //		byRet = 0x00;		// 10bit: 신호 검출에 의한 종료(Signal Search-1/2 drive종료)
            //	else if (byRet == 0x0800)
            //		byRet = 0x00;		// 11bit: Preset pulse drive에 의한 종료(지정한 위치/거리만큼 움직이는 함수군)
            uint dwVal = byRet;

            var bits = new BitArray(new int[] { (int)dwVal });

            if (bits.Get(1))                    // FSEND_STATUS_SLM Bit 0, limit 감속 정지 신호 입력에 의한 종료
            {
                
                byRet = 0x00;
            }
            if (bits.Get(1))            // FSEND_STATUS_ELM Bit 1, limit 급 정지 신호 입력에 의한 종료
            {
                
                byRet = 0x00;
            }
            if (bits.Get(9))    // FSEND_STATUS_ORIGIN_DETECT Bit 9, 원점 검출에 의한 종료
            {
                
                byRet = 0x00;
            }
            if (bits.Get(10))   // FSEND_STATUS_SIGNAL_DETECT Bit 10, 신호 검출에 의한 종료 (Signal search-1/2 drive 종료) (V2.0 이상)
            {
                
                byRet = 0x00;
            }
            if (bits.Get(11))   // FSEND_STATUS_PRESET_PULSE_DRIVE Bit 11, Preset pulse drive 종료 (V2.0 이상)
            {
               
                byRet = 0x00;
            }
            if (bits.Get(12))   // FSEND_STATUS_SENSOR_PULSE_DRIVE Bit 12, Sensor pulse drive 종료 (V2.0 이상)
            {
           
                byRet = 0x00;
            }
            if (bits.Get(13))           // FSEND_STATUS_LIMIT Bit 13, Limit 완전 정지에 의한 종료 (V2.0 이상)
            {
             
                byRet = 0x00;
            }
            if (bits.Get(14))       // FSEND_STATUS_SOFTLIMIT Bit 14, Soft limit에 의한 종료 (V2.0 이상)
            {
               
                byRet = 0x00;
            }

            //#ifdef _DEBUG
            //	if(dwVal != 0x00)
            //	{
            //		LOG_PRINTF(L"AXT", L"Axis %d %s - GetEndStatus():%04X", nAxis, strReason.c_str(), dwVal);
            //	}
            //#endif

            return byRet;
        }

        public virtual bool SignalSearch1(int nAxis, double dVel, double dAccel, byte detect_signal, byte byEdge)
        {
            if (!MotorCtrlInit)
                return false;

            byte signal = GetSearchSignalValue(detect_signal, byEdge);

            if (CardType == MotorType.CAM5)
                return (CAxtCAMC5M.C5Mstart_signal_search1((short)nAxis, dVel, dAccel, signal) == 1) ? true : false;
            else if (CardType == MotorType.CFS)
                return (CAxtCAMCFS.CFSstart_signal_search1((short)nAxis, dVel, dAccel, signal) == 1) ? true : false;
            else if (CardType == MotorType.CFS2)
                return (CAxtCAMCFS20.CFS20start_signal_search1((short)nAxis, dVel, dAccel, signal) == 1) ? true : false;
            else
                return (CAxtCAMCFS20.CFS20start_signal_search1((short)nAxis, dVel, dAccel, signal) == 1) ? true : false;
        }
        public virtual bool SignalSearch2(int nAxis, double dVel, byte detect_signal, byte byEdge)
        {
            if (!MotorCtrlInit)
                return false;

            byte signal = GetSearchSignalValue(detect_signal, byEdge);

            if (CardType == MotorType.CAM5)
                return (CAxtCAMC5M.C5Mstart_signal_search2((short)nAxis, dVel,  signal) == 1) ? true : false;
            else if (CardType == MotorType.CFS)
                return (CAxtCAMCFS.CFSstart_signal_search2((short)nAxis, dVel, signal) == 1) ? true : false;
            else if (CardType == MotorType.CFS2)
                return (CAxtCAMCFS20.CFS20start_signal_search2((short)nAxis, dVel, signal) == 1) ? true : false;
            else
                return (CAxtCAMCFS20.CFS20start_signal_search2((short)nAxis, dVel, signal) == 1) ? true : false;
        }
        public virtual bool StartMotor(int nAxis, double dPos, double dVel, double dAcc)
        {
            bool bRet = false;

            if (!MotorCtrlInit)
                return bRet;

            if (CardType == MotorType.CAM5)
                return (CAxtCAMC5M.C5Mstart_move((short)nAxis, dPos, dVel, dAcc) == 1) ? true : false;
            else if (CardType == MotorType.CFS)
                return (CAxtCAMCFS.CFSstart_s_move((short)nAxis, dPos, dVel, dAcc) == 1) ? true : false;
            else if (CardType == MotorType.CFS2)
                return (CAxtCAMCFS20.CFS20start_s_move((short)nAxis, dPos, dVel, dAcc) == 1) ? true : false;
            else
                return (CAxtCAMCFS20.CFS20start_s_move((short)nAxis, dPos, dVel, dAcc) == 1) ? true : false;
        }
        public virtual bool JogMove(int nAxis, double dVel, double dAcc) 
        {
            if (!MotorCtrlInit)
                return false;

            if (CardType == MotorType.CAM5)
                return (CAxtCAMC5M.C5Mv_move((short)nAxis, dVel, dAcc) == 1) ? true : false;
            else if (CardType == MotorType.CFS)
                return (CAxtCAMCFS.CFSv_move((short)nAxis, dVel, dAcc) == 1) ? true : false;
            else if (CardType == MotorType.CFS2)
                return (CAxtCAMCFS20.CFS20v_move((short)nAxis, dVel, dAcc) == 1) ? true : false;
            else
                return (CAxtCAMCFS20.CFS20v_move((short)nAxis, dVel, dAcc) == 1) ? true : false;
        }

        // Position Methods
        public virtual void SetActPos(int nAxis, double pos)
        {
            if (MotorCtrlInit)
            {
                switch (CardType)
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_actual_position((short)nAxis, pos);
                        break;
                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_actual_position((short)nAxis, pos);
                        break;
                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_actual_position((short)nAxis, pos);
                        break;
                    default:
                        CAxtCAMCFS20.CFS20set_actual_position((short)nAxis, pos);
                        break;
                }
            }
        }
        public virtual void SetCmdPos(int nAxis, double pos)
        {
            if (MotorCtrlInit)
            {
                switch (CardType)
                {
                    case MotorType.CAM5:
                        CAxtCAMC5M.C5Mset_command_position((short)nAxis, pos);
                        break;
                    case MotorType.CFS:
                        CAxtCAMCFS.CFSset_command_position((short)nAxis, pos);
                        break;
                    case MotorType.CFS2:
                        CAxtCAMCFS20.CFS20set_command_position((short)nAxis, pos);
                        break;
                    default:
                        CAxtCAMCFS20.CFS20set_command_position((short)nAxis, pos);
                        break;
                }
            }
        }
        public virtual double GetActPos(int nAxis)
        {
            double pos = -10.0f;

            if (MotorCtrlInit)
            {
                switch (CardType)
                {
                    case MotorType.CAM5:
                        pos = CAxtCAMC5M.C5Mget_actual_position((short)nAxis);
                        break;
                    case MotorType.CFS:
                        pos = CAxtCAMCFS.CFSget_actual_position((short)nAxis);
                        break;
                    case MotorType.CFS2:
                        pos = CAxtCAMCFS20.CFS20get_actual_position((short)nAxis);
                        break;
                    default:
                        pos = CAxtCAMCFS20.CFS20get_actual_position((short)nAxis);
                        break;
                }
            }

            return pos;
        }
        public virtual double GetCmdPos(int nAxis) 
        {
            double pos = -10.0f;

            if (MotorCtrlInit)
            {
                switch (CardType)
                {
                    case MotorType.CAM5:
                        pos = CAxtCAMC5M.C5Mget_command_position((short)nAxis);
                        break;
                    case MotorType.CFS:
                        pos = CAxtCAMCFS.CFSget_command_position((short)nAxis);
                        break;
                    case MotorType.CFS2:
                        pos = CAxtCAMCFS20.CFS20get_command_position((short)nAxis);
                        break;
                    default:
                        pos = CAxtCAMCFS20.CFS20get_command_position((short)nAxis);
                        break;
                }
            }

            return pos;
        }

        // Universal DIO Methods
        public static bool GetAxtMotionLibInit()
        {
            return MotorCtrlInit;
        }
        public static short GetTotalAxisCount()
        {
            short nTotAxis = 0;

            if (MotorCtrlInit)
            {
                //		switch (m_nCardType)
                //		{
                //		case CAM5:
                //			nTotAxis = C5Mget_total_numof_axis();
                //			break;
                //		case CFS:
                //			nTotAxis = CFSget_total_numof_axis();
                //			break;
                //		case CFS2:
                //			nTotAxis = CFS20get_total_numof_axis();
                //			break;
                //		default:
                nTotAxis = CAxtCAMCFS20.CFS20get_total_numof_axis();
                //			break;
                //		}
            }

            return nTotAxis;
        }

        public static byte GetOutputByte(int nAxis)
        {
            byte byRet = 0x00;

            if (MotorCtrlInit)
            {
                //		switch (m_nCardType)
                //		{
                //		case CAM5:
                //			byRet = C5Mget_output(nAxis);
                //			break;
                //		case CFS:
                //			byRet = CFSget_output(nAxis);
                //			break;
                //		case CFS2:
                //			byRet = CFS20get_output(nAxis);
                //			break;
                //		default:
                byRet = CAxtCAMCFS20.CFS20get_output((short)nAxis);
                //			break;
                //		}
            }
            return byRet;

        }
        public static bool GetOutputBit(int nAxis, int nOffset)
        {
            bool bRet = false;

            if (MotorCtrlInit)
            {
                //		switch (m_nCardType)
                //		{
                //		case CAM5:
                //			bRet = C5Moutput_bit_on(nAxis, nOffset);
                //			break;
                //		case CFS:
                //			bRet = CFSoutput_bit_on(nAxis, nOffset);
                //			break;
                //		case CFS2:
                //			bRet = CFS20output_bit_on(nAxis, nOffset);
                //			break;
                //		default:
                bRet = CAxtCAMCFS20.CFS20output_bit_on((short)nAxis, (byte)nOffset) == 1 ? true : false;
                //			break;
                //		}
            }

            return bRet;
        }
        public static bool SetOutputBit(int nAxis, int nOffset) 
        {
            bool bRet = false;

            if (MotorCtrlInit)
            {
                //		switch (m_nCardType)
                //		{
                //		case CAM5:
                //			bRet = C5Mset_output_bit(nAxis, nOffset);
                //			break;
                //		case CFS:
                //			bRet = CFSset_output_bit(nAxis, nOffset);
                //			break;
                //		case CFS2:
                //			bRet = CFS20set_output_bit(nAxis, nOffset);
                //			break;
                //		default:
                bRet = CAxtCAMCFS20.CFS20set_output_bit((short)nAxis, (byte)nOffset) == 1 ? true : false;
                //			break;
                //		}
            }

            return bRet;
        }
        public static bool ResetOutputBit(int nAxis, int nOffset) 
        {
            bool bRet = false;

            if (MotorCtrlInit)
            {
                //		switch (m_nCardType)
                //		{
                //		case CAM5:
                //			bRet = C5Mreset_output_bit(nAxis, nOffset);
                //			break;
                //		case CFS:
                //			bRet = CFSreset_output_bit(nAxis, nOffset);
                //			break;
                //		case CFS2:
                //			bRet = CFS20reset_output_bit(nAxis, nOffset);
                //			break;
                //		default:
                bRet = CAxtCAMCFS20.CFS20reset_output_bit((short)nAxis, (byte)nOffset) == 1 ? true : false;
                //			break;
                //		}
            }

            return bRet;
        }

        public static byte GetInputByte(int nAxis) 
        { 
            return 0; 
        }
        public static bool GetInputBit(int nAxis, int nOffset)
        {
            bool bRet = false;

            if (MotorCtrlInit)
            {
                //		switch (m_nCardType)
                //		{
                //		case CAM5:
                //			bRet = C5Minput_bit_on(nAxis, nOffset);
                //			break;
                //		case CFS:
                //			bRet = CFSinput_bit_on(nAxis, nOffset);
                //			break;
                //		case CFS2:
                //			bRet = CFS20input_bit_on(nAxis, nOffset);
                //			break;
                //		default:
                bRet = CAxtCAMCFS20.CFS20input_bit_on((short)nAxis, (byte)nOffset) == 1 ? true : false;
                //			break;
                //		}
            }

            return bRet;
        }
    }
}
