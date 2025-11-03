using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VSP.CONTROLLER.Din;
using static VSP.CONTROLLER.Dout;

/*      public static CDI I_PumpOvld_Btm => DI[(int)DigitalInput.X000];
        public static CDI I_MainAirPres => DI[(int)DigitalInput.X001];
        public static CDI I_ECoolOn_Btm => DI[(int)DigitalInput.X002];
        public static CDI I_Gas1Pres => DI[(int)DigitalInput.X003];
        public static CDI I_Gas2Pres => DI[(int)DigitalInput.X004];
        public static CDI I_Gas3Pres => DI[(int)DigitalInput.X005];
        public static CDI I_N2Pres => DI[(int)DigitalInput.X006];
        public static CDI I_VacValveOpen_Btm => DI[(int)DigitalInput.X007];
        public static CDI I_ChamberOpen_Btm => DI[(int)DigitalInput.X008];

        public static CDI I_ChamberClose_Btm => DI[(int)DigitalInput.X009];
        public static CDI I_ChamberInStripDetect_Btm => DI[(int)DigitalInput.X00A];
        public static CDI I_ChamberOutStripDetect_Btm => DI[(int)DigitalInput.X00B];

        Dout

        public static CDO O_RfGenPower_Btm => DO[(int)DigitalOutput.Y000];
        public static CDO O_VacPumpPwr_Btm => DO[(int)DigitalOutput.Y001];
        public static CDO O_ECool_Btm => DO[(int)DigitalOutput.Y002];
        public static CDO O_Gas1Open_Btm => DO[(int)DigitalOutput.Y003];
        public static CDO O_Gas2Open_Btm => DO[(int)DigitalOutput.Y004];
        public static CDO O_Gas3Open_Btm => DO[(int)DigitalOutput.Y005];
        public static CDO O_N2Purge_Btm => DO[(int)DigitalOutput.Y006];
        public static CDO O_VacValveOpen_Btm => DO[(int)DigitalOutput.Y007];
        public static CDO O_AirPurge_Btm => DO[(int)DigitalOutput.Y008];
        public static CDO O_GaugeValOpen_Btm => DO[(int)DigitalOutput.Y009];
        public static CDO O_ChamberOpen_Btm => DO[(int)DigitalOutput.Y00A];
        public static CDO O_ChamberClose_Btm => DO[(int)DigitalOutput.Y00B];*/
namespace BCR_Reader_Pro.Model
{
    
    enum FuncType{Do, Query }
    
    internal class uPmModel
    {

      
        public uPmModel()
        {
            
        }
       
        public void VaacValveOpen(bool open)
        {      
            if (open)
                O_VacValveOpen_Btm.On();
            else
                O_VacValveOpen_Btm.Off();
        }
        public bool IsVaacValveOpen()
        {
            if (I_VacValveOpen_Btm.IsOn())
                return true;
            else
                return false;
        }

        public void RfGenPwrOn(bool open)
        {
            if(open)
                O_RfGenPower_Btm.On();
            else
                O_RfGenPower_Btm.Off();
        }
        public bool IsRfGenPwrOn()
        {
            if (O_RfGenPower_Btm.IsOn())
                return true;
            else
                return false;       
        }
        public void LidOpen(bool open)
        {
            if (open)
                O_ChamberClose_Btm.On();
            else
                O_ChamberClose_Btm.Off();

        }
        public bool IsLidOpen()
        {
            if(I_ChamberOpen_Btm.IsOn())
                return true;
            else 
                return false;
        }

        public void VacPumpPowerOn(bool open)
        {
            if (open)
                O_VacPumpPwr_Btm.On();
            else
                O_VacPumpPwr_Btm.Off();
        }
        public bool IsVacPumpPowerOn()
        {
            if (O_VacPumpPwr_Btm.IsOn())
                return true;
            else
                return false;
        }
        public void PurgeOn(bool on)
        {
            if(on)
            {
                O_VacPumpPwr_Btm.On();
                O_N2Purge_Btm.On();
            }
            else 
            {
                O_VacPumpPwr_Btm.Off();
                O_N2Purge_Btm.Off();
            }

        }
        public bool IsIsPurgeOn()
        {
            if(O_N2Purge_Btm.IsOn())
                return true;
            else
                return false;          
            
        }
        public bool IsIsPurgeOff()
        {
            if (O_N2Purge_Btm.IsOff() && O_AirPurge_Btm.IsOff())
                return true;
            else
                return false;

        }

        public void ElecCoolingOn(bool on)
        {
            if (on)
                O_ECool_Btm.On();
            else
                O_ECool_Btm.Off();
        }
        public bool IsElecCoolingOn(bool issol)
        {
            bool ret = false;

            if (issol)
                ret = O_ECool_Btm.IsOn();
            else
                ret = I_ECoolOn_Btm.IsOn();

            return ret;
        }



    }
}
