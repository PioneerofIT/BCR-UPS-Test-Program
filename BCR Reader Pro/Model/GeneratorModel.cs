using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCR_Reader_Pro.Model
{
    public enum GeneratorQryType
    {
        QRY_GEN_MODE = 0, 
        QRY_GEN_FWD, 
        QRY_GEN_REF, 
        QRY_GEN_SP, 
        QRY_RON, 
        QRY_GEN_ERR,
        WRITE_GEN_MODE, 
        WRITE_GEN_SP, 
        WRITE_GEN_RON
    };

    internal class GeneratorModel
    {

        public bool IsOutOn { get; set; }
        public bool OutOnSet { get; set; }
        public int ReadMode { get; set; }
       
        public int WirteMode { get; set; }   
        public int ReadPowerSet {  get; set; }
        public int WritePowerSet { get; set; }
        public int ReadPowerFwd { get; set; }
        public int ReadPowerRef { get; set; }

    }
    public class GeneratorPacketModel
    {
        private GeneratorModel _model = new();
        
        public List<char> GetQueryFrame(GeneratorQryType type) 
        {
            List<char> result = new List<char>();

            result.Add('?');

            switch(type)
            {
                case GeneratorQryType.QRY_GEN_MODE:
                    result.Add('m');
                    result.Add('o');
                    break;

                case GeneratorQryType.QRY_GEN_FWD:
                    result.Add('f');
                    result.Add('w');
                    break;

                case GeneratorQryType.QRY_GEN_REF:
                    result.Add('r');
                    result.Add('e');
                    break;

                case GeneratorQryType.QRY_GEN_SP:
                    result.Add('s');
                    result.Add('e');
                    break;

                case GeneratorQryType.QRY_GEN_ERR:
                    result.Add('s');
                    result.Add('f');
                    break;

                case GeneratorQryType.QRY_RON:
                    result.Add('r');
                    result.Add('o');
                    break;
            }
            result.Add('\r');

            return result;
        
        }


    }
}
