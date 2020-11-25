using BingLibrary.hjb.tools;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace BingLibrary.hjb.PLC
{
    //Update by Bing.2017/10/12
    public class DeltaPLC
    {
        #region 台达PLC操作
        public bool SetM(string coilAddress, bool coilValue)
        {
            return coilValue ? PLCWrite("M", coilAddress, "FF00") : PLCWrite("M", coilAddress, "0000");
        }

        public bool ReadM(string coilAddress)
        {
            string temps = PLCRead("M", coilAddress);
            int tempi = Convert.ToInt32(temps, 16);
            tempi = tempi & 1;
            return tempi == 1 ? true : false;
        }

        public double ReadD(string coilAddress)
        {
            string strD = PLCRead("D", coilAddress);
            double dD = Convert.ToInt32(strD, 16);
            return dD;
        }

        public bool WriteD(string coilAddress, string coilValue)
        {
            try
            {
                PLCWrite("D", coilAddress, Convert.ToInt32(coilValue).ToString("X4"));
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region Modbus Lib

        private string mRecStr;
        private ManualResetEvent Pause_Event = new ManualResetEvent(false);
        private object SerialLock = new object();

        private bool PLCWrite(string coilType, string coilAddress, string coilData, string deviceID = "01")
        {
            if (!curSerialPort.IsOpen)
            {
                ModbusConnect();
            }
            if (coilAddress == "")
                return false;
            if (coilData == "")
                return false;

            string mStr;
            string functionCode, coilStartlAddress, coliValue;
            int m;

            switch (coilType)
            {
                case "M":
                    functionCode = "05";
                    m = Convert.ToInt32(coilAddress);
                    if (m > 1535)
                        m = m + 45056;
                    else
                        m = m + 2048;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "X":
                    functionCode = "05";
                    m = Convert.ToInt32(coilAddress, 8);
                    m += 1024;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "Y":
                    functionCode = "05";
                    m = Convert.ToInt32(coilAddress, 8);
                    m += 1280;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "S":
                    functionCode = "05";
                    m = Convert.ToInt32(coilAddress);
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "D"://32位
                    functionCode = "10";
                    m = Convert.ToInt32(coilAddress);
                    if (m > 4095)
                        m = m + 32768;
                    else
                        m = m + 4096;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "W"://16位
                    functionCode = "06";
                    m = Convert.ToInt32(coilAddress);
                    if (m > 4095)
                        m = m + 32768;
                    else
                        m = m + 4096;
                    coilStartlAddress = m.ToString("X4");
                    break;

                default:
                    return false;
            }
            m = Convert.ToInt32(coilData, 16);
            if (coilType == "D")
            {
                coliValue = m.ToString("X8");
                string SubStr;
                SubStr = coliValue.Substring(0, 4);
                coliValue = coliValue.Insert(8, SubStr);
                coliValue = coliValue.Remove(0, 4);
                mStr = deviceID + functionCode + coilStartlAddress + "000204" + coliValue;
            }
            else
            {
                coliValue = m.ToString("X4");
                mStr = deviceID + functionCode + coilStartlAddress + coliValue;
            }

            mStr = ":" + LRC(mStr) + Environment.NewLine;

            string mRecStr = "";
            lock (SerialLock)
            {
                curSerialPort.ReadExisting();
                curSerialPort.Write(mStr);
                mRecStr = curSerialPort.ReadLine();
            }

            if (mRecStr == "")
            {
                Tool.DebugInfo("写入失败!");
                return false;
            }
            if (mStr.Contains(mRecStr.Remove(13)))
                return true;
            else
            {
                Tool.DebugInfo("写入失败!");
                return false;
            }
        }

        private string PLCRead(string coilType, string coilAddress, string deviceID = "01", string coilCount = "0001", bool mHex = false)
        {
            if (!curSerialPort.IsOpen)
            {
                ModbusConnect();
            }
            string functionCode, coilStartlAddress, coliValue;
            string modbusStr;
            int m;
            switch (coilType)
            {
                case "M":
                    functionCode = "01";
                    m = Convert.ToInt32(coilAddress);
                    if (m > 1535)
                        m += 45056;
                    else
                        m += 2048;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "X":
                    functionCode = "02";
                    m = Convert.ToInt32(coilAddress, 8);
                    m += 1024;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "Y":
                    functionCode = "01";
                    m = Convert.ToInt32(coilAddress, 8);
                    m += 1280;
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "S":
                    functionCode = "01";
                    m = Convert.ToInt32(coilAddress);
                    coilStartlAddress = m.ToString("X4");
                    break;

                case "D"://32位
                    functionCode = "03";
                    m = Convert.ToInt32(coilAddress);
                    if (m > 4095)
                        m += 32768;
                    else
                        m += 4096;
                    coilStartlAddress = m.ToString("X4");
                    coilCount = (Convert.ToInt32(coilCount, 16) * 2).ToString("X4");
                    break;

                case "W"://16位
                    functionCode = "03";
                    m = Convert.ToInt32(coilAddress);
                    if (m > 4095)
                        m += 32768;
                    else
                        m += 4096;
                    coilStartlAddress = m.ToString("X4");
                    break;

                default:
                    return "";
            }

            modbusStr = deviceID + functionCode + coilStartlAddress + coilCount;
            modbusStr = ":" + LRC(modbusStr) + Environment.NewLine;

            string mRecStr = "";
            lock (SerialLock)
            {
                curSerialPort.ReadExisting();
                curSerialPort.Write(modbusStr);
                mRecStr = curSerialPort.ReadLine();
            }

            mRecStr = mRecStr.Substring(1);
            if (mRecStr.Contains(deviceID + functionCode))
            {
                m = 2 * Convert.ToInt32(mRecStr.Substring(4, 2), 16);
                coliValue = mRecStr.Substring(6, m);
                if (coilType == "D")
                {
                    for (int n = 1; n < m / 8 + 1; n++)
                    {
                        string SubStr;
                        SubStr = coliValue.Substring((n - 1) * 8, 4);
                        coliValue = coliValue.Insert(8 * n, SubStr);
                        coliValue = coliValue.Remove((n - 1) * 8, 4);
                    }
                }
                if (mHex)
                {
                    coliValue = "0x" + coliValue;
                }

                return coliValue;
            }
            Debug.Print("ReadBad");
            return "";
        }

        public SerialPort curSerialPort;

        public void ModbusInit(String PortName, int BaudRate, Parity Parity, int DataBits, StopBits StopBits)
        {
            curSerialPort = new SerialPort();
            curSerialPort.PortName = PortName;
            curSerialPort.BaudRate = BaudRate;
            curSerialPort.Parity = Parity;
            curSerialPort.DataBits = DataBits;
            curSerialPort.StopBits = StopBits;
        }

        public bool ModbusConnect()
        {
            bool isConnected = false;
            int connectCounts = 0;
            while (curSerialPort?.PortName != "" && curSerialPort?.IsOpen == false && isConnected == false && connectCounts < 10)
            {
                try
                {
                    curSerialPort.Open();
                    isConnected = true;
                }
                catch
                {
                    Thread.Sleep(500);
                    connectCounts++;
                    isConnected = false;
                }
            }
            return isConnected;
        }

        public void ModbusDisConnect()
        {
            try
            {
                curSerialPort?.Close();
            }
            catch { }
        }

        private byte[] StrToByte(string mStr)
        {
            int wCrc = 0xFFFF;
            int wPolynom = 0xA001;
            mStr = mStr.Trim();
            int count = mStr.Length / 2;
            byte[] b = new byte[count + 2];
            for (int ctick = 0; ctick < count; ctick++)
            {
                string temp = mStr.Substring(ctick * 2, 2);
                b[ctick] = Convert.ToByte(temp, 16);
                wCrc ^= b[ctick];
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) != 0)
                    { wCrc = (wCrc >> 1) ^ wPolynom; }
                    else { wCrc = wCrc >> 1; }
                }
            }
            string strCrc = wCrc.ToString("X4");
            b[count] = Convert.ToByte(strCrc.Substring(2, 2), 16);
            b[count + 1] = Convert.ToByte(strCrc.Substring(0, 2), 16);

            return b;
        }

        private string LRC(string str)
        {
            int mlength = str.Length;
            string c_data, h_lrc;
            long d_lrc = 0;
            for (int cTick = 0; cTick < mlength; cTick++)
            {
                c_data = str.Substring(cTick, 2);
                d_lrc += Convert.ToInt32(c_data, 16);
                cTick++;
            }
            d_lrc = d_lrc & 0xFF;
            if (d_lrc == 0)
                return str + "00";
            d_lrc = 0xFF - d_lrc + 1;
            h_lrc = d_lrc.ToString("X2");
            return str + h_lrc;
        }

        #endregion Modbus Lib
    }
}