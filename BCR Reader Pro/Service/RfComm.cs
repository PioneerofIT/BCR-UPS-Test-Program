using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCR_Reader_Pro.Model;

namespace BCR_Reader_Pro.Service
{
    public enum GeneratorCommStep
    {
        SEND_COMMAND,
        WAIT_RESPONSE,
        STEP_DECISION
    }
    
    internal class RfComm : BaseThread
    {
        private GeneratorPacketModel _packetModel;
        private GeneratorModel _generatorModel;
        private Dictionary<GeneratorQryType, List<char>> _comandMap = new();
        private IEnumerator<KeyValuePair<GeneratorQryType, List<char>>>? _iterator;
        private SerialPort? _RfComm;
        private StringBuilder _rxBuffer = new StringBuilder();
        private string[] _packets;
        private bool _doneReading;

        public RfComm(int idx, int delay) : base(idx, delay)
        {
            _packetModel = new GeneratorPacketModel();
            _generatorModel  = new GeneratorModel();
            _RfComm = new SerialPort();
            FillComand();
            SetCommPort();
        }

        private void SetCommPort()
        {
            _RfComm.PortName = "COM1";
            _RfComm.BaudRate = 19200;
            _RfComm.Parity = Parity.None;
            _RfComm.DataBits = 8;
            _RfComm.StopBits = StopBits.One;
            _RfComm.Encoding = Encoding.ASCII;
            _RfComm.NewLine = "\r";
            _RfComm.DataReceived += OnDataReceived;
            _RfComm.DtrEnable = true;
            _RfComm.RtsEnable = true;

            _RfComm.ReadTimeout = 1000;
            _RfComm.WriteTimeout = 1000;
        }


        private void FillComand()
        {
            _comandMap.Clear();
            _comandMap.Add(GeneratorQryType.QRY_GEN_MODE, _packetModel.GetQueryFrame(GeneratorQryType.QRY_GEN_MODE));
            _comandMap.Add(GeneratorQryType.QRY_GEN_FWD, _packetModel.GetQueryFrame(GeneratorQryType.QRY_GEN_FWD));
            _comandMap.Add(GeneratorQryType.QRY_GEN_REF, _packetModel.GetQueryFrame(GeneratorQryType.QRY_GEN_REF));
            _comandMap.Add(GeneratorQryType.QRY_GEN_SP, _packetModel.GetQueryFrame(GeneratorQryType.QRY_GEN_ERR));
            _comandMap.Add(GeneratorQryType.QRY_GEN_ERR, _packetModel.GetQueryFrame(GeneratorQryType.QRY_GEN_ERR));
            _comandMap.Add(GeneratorQryType.QRY_RON, _packetModel.GetQueryFrame(GeneratorQryType.QRY_RON));

            _iterator = _comandMap.GetEnumerator();

        }


        protected override int AutoRun()
        {
            switch (_step)
            {
                case (int)GeneratorCommStep.SEND_COMMAND:
                    if(TxCommand(300))
                    {
                        NextStep((int)GeneratorCommStep.WAIT_RESPONSE);
                    }
                    return 0;

                case (int)GeneratorCommStep.WAIT_RESPONSE:
                    if(_doneReading)
                    {
                        if(DecodeRxData())
                        {
                            if (!_iterator.MoveNext())
                            {
                                // 끝이면 다시 처음으로
                                _iterator = _comandMap.GetEnumerator();
                                _iterator.MoveNext();  // 첫 항목으로 이동
                            }
                            NextStep((int)GeneratorCommStep.STEP_DECISION);
                        }
                    }
                    _doneReading = false;
                    return 0;

                case (int)GeneratorCommStep.STEP_DECISION:
                    NextStep((int)GeneratorCommStep.SEND_COMMAND);
                    return 0;
            }
            return 0;

        }
            

        private bool TxCommand(int timeout)
        {

            _RfComm.DiscardInBuffer();
            _RfComm.DiscardOutBuffer();

            List<char> query = _iterator.Current.Value;
            string queryString = new string(query.ToArray());
            byte[] buffer = Encoding.ASCII.GetBytes(queryString);

            _RfComm.Write(buffer, 0, buffer.Length);
            return true;
        }
        private bool DecodeRxData()
        {
            string response = string.Join("", _packets).Trim();
            var type = _iterator.Current.Key;

            switch(type)
            {
                case GeneratorQryType.QRY_GEN_MODE:
                    if(response.Contains("DSR"))
                    {
                        _generatorModel.ReadMode = 1; //Remote
                    }
                    else
                    {
                        _generatorModel.ReadMode = 0;
                    }
                    break;

                case GeneratorQryType.QRY_GEN_FWD:
                    if (response.StartsWith("FW"))
                    {
                        string valuePart = response.Substring(2);  // "123"
                        if (int.TryParse(valuePart, out int power))
                        {
                            _generatorModel.ReadPowerFwd = power;
                        }
                    }
                    //else return false;
                    break;

                case GeneratorQryType.QRY_GEN_REF:
                    if (response.StartsWith("RE"))
                    {
                        string valuePart = response.Substring(2);  // "123"
                        if (int.TryParse(valuePart, out int power))
                        {
                            _generatorModel.ReadPowerRef = power;
                        }
                    }
                    //else return false;
                    break;

                case GeneratorQryType.QRY_GEN_SP:
                    if (response.StartsWith("SE"))
                    {
                        string valuePart = response.Substring(2);  // "123"
                        if (int.TryParse(valuePart, out int power))
                        {
                            _generatorModel.ReadPowerSet = power;
                        }
                    }
                    //else return false;
                        break;

                case GeneratorQryType.QRY_GEN_ERR:

                    break;

                case GeneratorQryType.QRY_RON:
                    if (response.Contains("RON"))
                    {
                        _generatorModel.IsOutOn = true;
                    }
                    else if (response.Contains("ROF"))
                    {
                        _generatorModel.IsOutOn = false;
                    }
                    //else
                    //{
                    //    return false;
                    //}
                    break;
            }

            return true;
        }
        private bool IsRxTimeOut()
        {
            return false;
        }
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                string chunk = _RfComm.ReadExisting();
             
                _rxBuffer.Append(chunk);

                if (_rxBuffer.ToString().Contains("\r"))
                {
                    _packets = _rxBuffer.ToString().Split('\r');
                    _doneReading = true;
                } 


            }
            catch (TimeoutException)
            {
                Console.WriteLine("TimeOut");
            }
        }

        //private bool IsRxComplete()
        //{

        //}



    }
}
