using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;


public enum ModbusFunc : byte
{
    // READ
    ReadCoils = 0x01, // 디지털 출력 (비트) 읽기
    ReadDiscreteInputs = 0x02, // 디지털 입력 (비트) 읽기
    ReadHoldingRegisters = 0x03, // 홀딩 레지스터(16비트) 읽기
    ReadInputRegisters = 0x04, // 입력 레지스터(16비트) 읽기

    // WRITE
    WriteSingleCoil = 0x05, // 단일 Coil ON/OFF
    WriteSingleRegister = 0x06, // 단일 Register 쓰기
    WriteMultipleCoils = 0x0F, // 여러 Coil 쓰기
    WriteMultipleRegisters = 0x10 // 여러 Register 쓰기
}
public enum UpsQueryType
{
    /// <summary>UPS 상태 비트 레지스터</summary>
    BitRegister = 0,

    /// <summary>UPS 남은 동작 시간 (분)</summary>
    RemainingTime = 1,

    /// <summary>UPS 배터리 충전 상태 (%)</summary>
    ChargeState = 2,

    /// <summary>UPS 배터리 전압 (V)</summary>
    BatteryVoltage = 3,

    /// <summary>UPS 내부 온도 (℃)</summary>
    InternalTemperature = 4,

    /// <summary>UPS 출력 전압 (V)</summary>
    OutputVoltage = 5,

    /// <summary>UPS 입력 전압 (V)</summary>
    InputVoltage = 6
}



namespace BCR_Reader_Pro.Model
{

    public class ModBusPdu
    {
        public byte FunctionCode { get; private set; }
        public byte[] Data { get; private set; } = Array.Empty<byte>();
        public ModBusPdu() { }
        public ModBusPdu(byte functionCode, byte[] data)
        {
            FunctionCode = functionCode;
            Data = data ?? Array.Empty<byte>(); // data가 null이면 empty로
        }
        public void Build(byte func, byte[] data)
        {
            FunctionCode = func;
            Data = data ?? Array.Empty<byte>();
        }

        public void BuildReadHoldingRegs(ushort startAddr, ushort count)
        {
            FunctionCode = 0x03;
            Data = new byte[4] 
            {
                (byte)(startAddr >> 8), (byte)(startAddr & 0xFF),
                (byte)(count     >> 8), (byte)(count     & 0xFF)
            };
        }

        public void BuildReadInputRegs(ushort startAddr, ushort count)
        {
            FunctionCode = 0x04;
            Data = new byte[4] {
                (byte)(startAddr >> 8), (byte)(startAddr & 0xFF),
                (byte)(count     >> 8), (byte)(count     & 0xFF)
            };
        }

        public void BuildWriteSingleReg(ushort addr, ushort value)
        {
            FunctionCode = 0x06;
            Data = new byte[4] {
                (byte)(addr  >> 8), (byte)(addr  & 0xFF),
                (byte)(value >> 8), (byte)(value & 0xFF)
            };
        }

        public byte[] ToBytes()
        {
            var buf = new byte[1 + Data.Length];
            buf[0] = FunctionCode;
            Buffer.BlockCopy(Data, 0, buf, 1, Data.Length);
            return buf;
        }

        public static ModBusPdu FromBytes(byte[] pduBytes)
        {
            if (pduBytes is null || pduBytes.Length < 1)
                throw new ArgumentException("Invalid PDU bytes");

            var pdu = new ModBusPdu();
            pdu.Build(pduBytes[0], pduBytes.Skip(1).ToArray());
            return pdu;
        }
        public byte[] BuildReadInputRegisters(ushort startAddr, ushort count)
        {
            return new byte[]
            {
            0x04, // Function Code: Read Input Registers
            (byte)(startAddr >> 8),
            (byte)(startAddr & 0xFF),
            (byte)(count >> 8),
            (byte)(count & 0xFF)
            };
        }

    }
    public abstract class ModbusAduBase
    {
        public byte FunctionCode { get; private set; }
        public byte[] Data { get; private set; } = Array.Empty<byte>();
        public abstract byte[] ExtractPduBytes(byte[] frame);
        public abstract bool VerifyFrame(byte[] frame);
        public abstract byte[] BuildPacket(byte unitId, ModBusPdu pdu);
        public abstract byte[] BuildReadInputRegisters(byte unitId, ushort startAddr, ushort count);

        protected ModBusPdu _pdu = new ModBusPdu();
    }
    public sealed class ModBusTcpAdu : ModbusAduBase
    {
        private static ushort _transactionIdCounter = 0;

        public override byte[] BuildPacket(byte unitId, ModBusPdu pdu)
        {
            // PDU 바이트 생성
            byte[] pduBytes = pdu.ToBytes();

            // Transaction ID (2바이트)
            ushort transactionId = ++_transactionIdCounter;
            byte tidHi = (byte)(transactionId >> 8);
            byte tidLo = (byte)(transactionId & 0xFF);

            // Protocol ID (2바이트) — 항상 0x0000
            byte protoHi = 0x00;
            byte protoLo = 0x00;

            // Length (2바이트) = UnitID(1) + PDU 길이
            ushort length = (ushort)(1 + pduBytes.Length);
            byte lenHi = (byte)(length >> 8);
            byte lenLo = (byte)(length & 0xFF);

            // Unit ID (1바이트)
            byte unit = unitId;

            // MBAP header 7바이트 + PDU
            byte[] frame = new byte[7 + pduBytes.Length];
            frame[0] = tidHi;
            frame[1] = tidLo;
            frame[2] = protoHi;
            frame[3] = protoLo;
            frame[4] = lenHi;
            frame[5] = lenLo;
            frame[6] = unit;

            Array.Copy(pduBytes, 0, frame, 7, pduBytes.Length);

            return frame;
        }

        //정해진 길이만큼 데이터가 들어왔는지
        public override bool VerifyFrame(byte[] frame)
        {
            if (frame == null || frame.Length < 8) return false;
           
            ushort len = (ushort)((frame[4] << 8) | frame[5]);
            return frame.Length >= 6 + len;
        }

        //실제 PDU(FunctionCode + Data) 부분만 추출
        public override byte[] ExtractPduBytes(byte[] frame)
        {
            if (!VerifyFrame(frame)) throw new InvalidOperationException("Invalid TCP MODBUS frame");

            // PDU는 MBAP 헤더 7바이트 이후부터 시작
            return frame.Skip(7).ToArray();
        }
        public override byte[] BuildReadInputRegisters(byte unitId, ushort startAddr, ushort count)
        {
            var pduBytes = _pdu.BuildReadInputRegisters(startAddr, count);

            ushort length = (ushort)(pduBytes.Length + 1); // +UnitID
            ushort tid = ++_transactionIdCounter;

            var frame = new byte[7 + pduBytes.Length];
            frame[0] = (byte)(tid >> 8);
            frame[1] = (byte)(tid & 0xFF);
            frame[2] = 0x00; frame[3] = 0x00;        // Protocol ID
            frame[4] = (byte)(length >> 8);
            frame[5] = (byte)(length & 0xFF);
            frame[6] = unitId;

            Array.Copy(pduBytes, 0, frame, 7, pduBytes.Length);
            return frame;
        }


    }

    public abstract class UpsPacketBase
    {
        protected readonly ModbusAduBase _adu;

        protected TcpClient? _client;
        protected NetworkStream? _stream;

        protected UpsPacketBase(ModbusAduBase adu)
        {
            _adu = adu;
        }

        public async Task<bool> ConnectAsync(string ip, int port, int timeoutMs = 3000)
        {
            _client = new TcpClient();
            var connect = _client.ConnectAsync(ip, port);
            var timeout = Task.Delay(timeoutMs);

            if (await Task.WhenAny(connect, timeout) == timeout)
                throw new TimeoutException("UPS 연결 시간 초과");

            _stream = _client.GetStream();
            _stream.ReadTimeout = timeoutMs;
            _stream.WriteTimeout = timeoutMs;
            return true;
        }

        public void Disconnect()
        {
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }
        public byte[] BuildReadCommand(UpsQueryType query)
        {
            ushort startAddr = 0;
            ushort count = 0;

            switch (query)
            {
                case UpsQueryType.BitRegister: startAddr = 0x0000; count = 0x0005; break;
                case UpsQueryType.RemainingTime: startAddr = 0x0080; count = 0x0002; break;
                case UpsQueryType.ChargeState: startAddr = 0x0082; count = 0x0001; break;
                case UpsQueryType.BatteryVoltage: startAddr = 0x0083; count = 0x0002; break;
                case UpsQueryType.InternalTemperature: startAddr = 0x0087; count = 0x0001; break;
                case UpsQueryType.OutputVoltage: startAddr = 0x008E; count = 0x0002; break;
                case UpsQueryType.InputVoltage: startAddr = 0x0097; count = 0x0002; break;
                default: throw new ArgumentOutOfRangeException(nameof(query));
            }

            // 🔹 Modbus ADU로 프레임 생성 후 그대로 반환
            return _adu.BuildReadInputRegisters(0x01, startAddr, count);
        }
        protected async Task<byte[]?> SendAndReceiveAsync(byte[] request, int rxBufSize = 256)
        {
            if (_stream == null) throw new InvalidOperationException("미연결 상태");

            await _stream.WriteAsync(request, 0, request.Length);

            var buf = new byte[rxBufSize];
            int read = await _stream.ReadAsync(buf, 0, buf.Length);
            if (read <= 0) return null;

            return buf.Take(read).ToArray();
        }
    }

    public sealed class UpsApcPacketR2 : UpsPacketBase
    {
        private string _ip;
        private int _port;

        public UpsApcPacketR2( string ip, string port, ModbusAduBase adu) : base(adu)
        {
            _ip = ip;
            _port = int.Parse(port);
        }


    }


}
