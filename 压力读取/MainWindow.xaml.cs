using BingLibrary.hjb.file;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 压力读取
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        #region 变量
        DateTime LastInput;
        private string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        string com1, com2;
        public static DXH.Modbus.DXHModbusRTU COM1;//yali;
        public static DXH.Modbus.DXHModbusRTU COM2;//yali;
        int[] Yali1 = new int[20];
        int[] Yali2 = new int[20];
        double[] Yali_1 = new double[10];
        double[] Yali_2 = new double[10];
        double[] Yali_3 = new double[20];       
        public SeriesCollection SeriesCollection1 { get; set; }
        public ChartValues<double>[] CVVSWR1Up = new ChartValues<double>[1] { new ChartValues<double>() };
        public LineSeries[] lsVSWR1Up = new LineSeries[1] { new LineSeries() { Values = new ChartValues<double>(), PointGeometrySize = 1, PointGeometry = null, Fill = System.Windows.Media.Brushes.Transparent, Stroke = System.Windows.Media.Brushes.Red } };
        #endregion

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadParameter();
            COM1 = new DXH.Modbus.DXHModbusRTU(com1, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            COM2 = new DXH.Modbus.DXHModbusRTU(com2, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);         
            LastInput = DateTime.Now;
            //COM1.StartConnect(); COM2.StartConnect();
            await Task.Run(()=> { COM1.StartConnect(); COM2.StartConnect();});
            run();
            AddMessage("软件加载成功");
        }
        void LoadParameter()
        {
            Pressurevalue1.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue1", "0.00");
            Pressurevalue2.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue2", "0.00");
            Pressurevalue3.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue3", "0.00");
            Pressurevalue4.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue4", "0.00");
            Pressurevalue5.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue5", "0.00");
            Pressurevalue6.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue6", "0.00");
            Pressurevalue7.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue7", "0.00");
            Pressurevalue8.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue8", "0.00");
            Pressurevalue9.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue9", "0.00");
            Pressurevalue10.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue10", "0.00");
            Pressurevalue11.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue11", "0.00");
            Pressurevalue12.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue12", "0.00");
            Pressurevalue13.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue13", "0.00");
            Pressurevalue14.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue14", "0.00");
            Pressurevalue15.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue15", "0.00");
            Pressurevalue16.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue16", "0.00");
            Pressurevalue17.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue17", "0.00");
            Pressurevalue18.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue18", "0.00");
            Pressurevalue19.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue19", "0.00");
            Pressurevalue20.Text = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "Pressurevalue", "Pressurevalue20", "0.00");
            com1 = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "COM", "COM1", "COM1");
            com2 = BingLibrary.hjb.file.Inifile.INIGetStringValue(iniParameterPath, "COM", "COM2", "COM3");
            AddMessage("参数加载完成");
        }
        bool StartGetYALI = false;
        async void run()
        {
            if (!StartGetYALI)
                StartGetYALI = true;
            else
                return;
            await Task.Delay(100);
            while (StartGetYALI)
            {
                await Task.Delay(5000);
                Task func = Task.Run(()=> 
                {
                    Yali1 = COM1.ModbusRead(1, 3, 0, 20);
                    if (Yali1 != null)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Yali_3[i] = Yali1[2 * i + 1];
                        }
                    }
                    Yali2 = COM2.ModbusRead(1, 3, 0, 14);
                    if (Yali2 != null)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Yali_3[i + 10] = Yali2[i + 1];
                        }
                    }
                });
                await func;
                Display();
                TestRun();
                SaveCSVfileYali();
            }
        }
       public  void AddMessage(string str)
        {
            string[] s = MsgTextBox.Text.Split('\n');
            if (s.Length > 1000)
            {
                MsgTextBox.Text = "";
            }
            if (MsgTextBox.Text != "")
            {
                MsgTextBox.Text += "\r\n";
            }
            MsgTextBox.Text += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }

       
        private async void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Task func = Task.Run(() =>
            {
                Yali1 = COM1.ModbusRead(1, 3, 0, 20) ;
                if (Yali1 != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Yali_3[i] = Yali1[2*i + 1];
                    }                  
                }
                Yali2 = COM2.ModbusRead(1, 3, 0, 14);
                if (Yali2 != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Yali_3[i + 10] = Yali2[i + 1];
                    }
                }
            });
            await func;
            Display();
            TestRun();
            SaveCSVfileYali();
         }
        public void Display()
        {
            Pressurevalue1.Text = Yali_3[0].ToString();
            Pressurevalue2.Text = Yali_3[1].ToString();
            Pressurevalue3.Text = Yali_3[2].ToString();
            Pressurevalue4.Text = Yali_3[3].ToString();
            Pressurevalue5.Text = Yali_3[4].ToString();
            Pressurevalue6.Text = Yali_3[5].ToString();
            Pressurevalue7.Text = Yali_3[6].ToString();
            Pressurevalue8.Text = Yali_3[7].ToString();
            Pressurevalue9.Text = Yali_3[8].ToString();
            Pressurevalue10.Text = Yali_3[9].ToString();
            Pressurevalue11.Text = Yali_3[10].ToString();
            Pressurevalue12.Text = Yali_3[11].ToString();
            Pressurevalue13.Text = Yali_3[12].ToString();
            Pressurevalue14.Text = Yali_3[13].ToString();
            Pressurevalue15.Text = Yali_3[14].ToString();
            Pressurevalue16.Text = Yali_3[15].ToString();
            Pressurevalue17.Text = Yali_3[16].ToString();
            Pressurevalue18.Text = Yali_3[17].ToString();
            Pressurevalue19.Text = Yali_3[18].ToString(); ;
            Pressurevalue20.Text = Yali_3[19].ToString();
        }
        public void TestRun()
        {
            try
            {
                CVVSWR1Up = new ChartValues<double>[1] { new ChartValues<double>() };
                lsVSWR1Up = new LineSeries[1] { new LineSeries() { Values = new ChartValues<double>(), PointGeometrySize = 1, PointGeometry = null, Fill = System.Windows.Media.Brushes.Transparent, Stroke = System.Windows.Media.Brushes.Red } };
                for (int i = 0; i < 20; i++)
                {
                    CVVSWR1Up[0].Add(Yali_3[i]);
                }
                lsVSWR1Up[0].Values = CVVSWR1Up[0];
                SeriesCollection1 = new SeriesCollection()
                        {
                          lsVSWR1Up[0]
                          };
                chart1.Series = SeriesCollection1;
                AddMessage("数据读取完成");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void SaveCSVfileYali()
        {
            string filepath = "D:\\压力记录\\压力记录" + GetBanci() + ".csv";
            if (!System.IO.Directory.Exists("D:\\压力记录"))
            {
                Directory.CreateDirectory("D:\\压力记录");
            }
            try
            {
                if (!File.Exists(filepath))
                {
                    string[] heads = { "Time", "Pressurevalue1", "Pressurevalue2", "Pressurevalue3", "Pressurevalue4", "Pressurevalue5", "Pressurevalue6", "Pressurevalue7", "Pressurevalue8", "Pressurevalue9", "Pressurevalue10", "Pressurevalue11", "Pressurevalue12", "Pressurevalue13", "Pressurevalue14", "Pressurevalue15", "Pressurevalue16", "Pressurevalue17", "Pressurevalue18", "Pressurevalue19", "Pressurevalue20" };
                    Csvfile.AddNewLine(filepath, heads);
                }
                string[] conte = { System.DateTime.Now.ToString(), Yali_3[0].ToString(), Yali_3[1].ToString(), Yali_3[2].ToString(), Yali_3[3].ToString(), Yali_3[4].ToString(), Yali_3[5].ToString(), Yali_3[6].ToString(), Yali_3[7].ToString(), Yali_3[8].ToString(), Yali_3[9].ToString(), Yali_3[10].ToString(), Yali_3[11].ToString(), Yali_3[12].ToString(), Yali_3[13].ToString(), Yali_3[14].ToString(), Yali_3[15].ToString(), Yali_3[16].ToString(), Yali_3[17].ToString(), Yali_3[18].ToString(), Yali_3[19].ToString()};
                Csvfile.AddNewLine(filepath, conte);               
            }
            catch (Exception ex)
            {
             AddMessage(ex.Message);
            }
        }
        private string GetBanci()
        {
            string rs = "";
            if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 20)
            {
                rs += DateTime.Now.ToString("yyyyMMdd") + "_D";
            }
            else
            {
                if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 8)
                {
                    rs += DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "_N";
                }
                else
                {
                    rs += DateTime.Now.ToString("yyyyMMdd") + "_N";
                }
            }
            return rs;
        }
        private void MsgTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MsgTextBox.ScrollToEnd();
        }
    }
 }
