using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;
using System.IO;

namespace Volt_Control
{
    public partial class Form1 : Form
    {
        byte DataLength = 128;
        
        bool FileReadOver = true;
        bool IsSerialPortOpen = false;
        bool AutoScan = false;
        int AutoScanFlag = 2;

        const string FilePath15_0 = "../../volt_data/15_0.dat";
        const string FilePath15_30 = "../../volt_data/15_30.dat";
        const string FilePath15_45 = "../../volt_data/15_45.dat";
        const string FilePath15_60 = "../../volt_data/15_60.dat";
        
        const string FilePath16_0 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/16_0.dat";
        const string FilePath16_30 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/16_30.dat";
        const string FilePath16_45 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/16_45.dat";
        const string FilePath16_60 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/16_60.dat";
        
        const string FilePath17_0 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/17_0.dat";
        const string FilePath17_30 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/17_30.dat";
        const string FilePath17_45 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/17_45.dat";
        const string FilePath17_60 = "E:/master_WHU/voltage_control/03Host_Computer/Volt_Control/volt_data/17_60.dat";

        byte[] ReadByteData = new byte[200];
        byte[] AutoReadByteData1 = new byte[200];
        byte[] AutoReadByteData2 = new byte[200];
        byte[] AutoReadByteData3 = new byte[200];
        byte[] AutoReadByteData4 = new byte[200];


        byte[] WriteByteData = new byte[200];
        byte[] ZeroData = new byte[200];
        
        string[] StrData = new string[100];        
        string[] WriteStringData = new string[20];
        string[] FileInfo = new string[20];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SearchAndAddSerialToComboBox(serialPort1, comboBox1);
            comboBox2.Text = "15";
            comboBox3.Text = "0";
            textBox2.Text = "1";
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            ZeroDataArr();
            try
            {
                serialPort1.Open();                
            }
            catch {  }
            ZeroData[0] = DataLength;
            WriteByteToSerialPort(ZeroData, 0, (DataLength + 1));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FileReadOver)
            {
                if (serialPort1.IsOpen)                                     
                {
                    try
                    {
                        serialPort1.Close();
                        IsSerialPortOpen = false;
                    }
                    catch { }                                               
                    button1.Text = "打开串口";
                    //button1.BackgroundImage = Properties.Resources.KEY_OFF;  
                }
                else
                {
                    try
                    {
                        serialPort1.PortName = comboBox1.Text;              
                        serialPort1.Open();                                 
                        button1.Text = "关闭串口";
                        IsSerialPortOpen = true;
                        //button1.BackgroundImage = Properties.Resources.KEY_ON;

                    }
                    catch
                    {
                        MessageBox.Show("串口打开失败", "错误");
                    }
                }
            }
            else MessageBox.Show("请先读取文件");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SearchAndAddSerialToComboBox(serialPort1, comboBox1);       
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AutoScan = false;
            try
            {
                timer1.Stop();
                AutoScanFlag = 2;
            }
            catch { }
            ZeroDataArr();
            if (IsSerialPortOpen)
            {
                ZeroData[0] = DataLength;
                WriteByteToSerialPort(ZeroData, 0, (DataLength + 1));
                pictureBox1.BackgroundImage = Properties.Resources.WHU1;
            }
            else
            {
                MessageBox.Show("请先打开串口");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            if (IsSerialPortOpen)
            {
                AutoScan = true;      
            }
            else
            {
                MessageBox.Show("请先打开串口");
            }
        }


        private void button8_Click(object sender, EventArgs e)
        {
            if (!AutoScan)
            {
                try
                {
                    CheckFreqAndDirection();
                }
                catch
                {
                    MessageBox.Show("读取文件失败");
                }
                if (IsSerialPortOpen)
                {
                    ReadByteData[0] = DataLength;
                    WriteByteToSerialPort(ReadByteData, 0, (DataLength + 1));
                    //MessageBox.Show("手动扫描已启动！");
                }
                else
                {
                    MessageBox.Show("启动失败，请先打开串口");
                }
            }
            else
            {
                try
                {
                    int Time = System.Convert.ToInt32(textBox2.Text);
                    timer1.Interval = Time * 1000;
                }
                catch
                {
                    MessageBox.Show("停留时间输入格式错误！");
                }
                
                try
                {
                    AutoScanCheckFreq();
                }
                catch
                {
                    MessageBox.Show("读取文件失败");
                }
                if (IsSerialPortOpen)
                {
                    AutoReadByteData1[0] = DataLength;
                    AutoReadByteData2[0] = DataLength;
                    AutoReadByteData3[0] = DataLength;
                    AutoReadByteData4[0] = DataLength;
                    WriteByteToSerialPort(AutoReadByteData1, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.WHU1;
                    timer1.Start();
                    //MessageBox.Show("自动扫描已启动！");
                }
                else
                {
                    MessageBox.Show("启动失败，请先打开串口");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            switch(AutoScanFlag)
            {
                case (1): 
                    WriteByteToSerialPort(AutoReadByteData1, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.WHU1;
                    timer1.Start();
                    AutoScanFlag++;
                    break;
                case (2): 
                    WriteByteToSerialPort(AutoReadByteData2, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.WHU2;
                    timer1.Start();
                    AutoScanFlag++;
                    break;
                case (3):
                    WriteByteToSerialPort(AutoReadByteData3, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.WHU3;
                    timer1.Start();
                    AutoScanFlag++;
                    break;
                case (4):
                    WriteByteToSerialPort(AutoReadByteData4, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.WHU4;
                    timer1.Start();
                    AutoScanFlag = 1;
                    break;
            }
        
        }


        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                AutoScanFlag = 2;
            }
            catch { }
            ZeroDataArr();
            if (IsSerialPortOpen)
            {
                ZeroData[0] = DataLength;
                WriteByteToSerialPort(ZeroData, 0, (DataLength + 1));
                pictureBox1.BackgroundImage = Properties.Resources.WHU1;
            }
            else
            {
                MessageBox.Show("停止失败，请打开串口");
            }
        }

        private void ReadByteFromFile(byte[] DataBuffer,string Path)
        {
            int BufferCnt = 1;
            FileStream myStream = new FileStream(@Path, FileMode.Open, FileAccess.Read);
            BinaryReader myReader = new BinaryReader(myStream);
            //while (FileInfoCnt < 10)
            //{
            //    FileInfo[FileInfoCnt] = myReader.ReadString();
            //    FileInfoCnt++;
            //}
            while (BufferCnt < DataLength + 1)
            {
                DataBuffer[BufferCnt] = myReader.ReadByte();
                BufferCnt++;
            }
            myReader.Close();
            myStream.Close();
        }




        private void button4_Click(object sender, EventArgs e)
        {
            ////FileInfo
            //WriteStringData[0] = "17"; //Freq
            //WriteStringData[1] = "0";  //Direction
            //WriteStringData[2] = "0";
            //WriteStringData[3] = "0";
            //WriteStringData[4] = "0";
            //WriteStringData[5] = "0";
            //WriteStringData[6] = "0";
            //WriteStringData[7] = "0";
            //WriteStringData[8] = "0";
            //WriteStringData[9] = "0";

            //Data
            DataWriteArr();

            saveFileDialog1.Filter = "二进制文件(*.dat)|*.dat";  //设置保存文件格式
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) //判断是否选择文件
            {
                try
                {
                    //使用另存为
                    FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);//写入文件
                    BinaryWriter myWriter = new BinaryWriter(myStream);
                    //for(int i = 0; i < 10; i++)
                    //{
                    //    myWriter.Write(WriteStringData[i]);
                    //}
                    for (int i = 0; i < DataLength; i++)
                    {
                        myWriter.Write(WriteByteData[i]);
                    }
                    myWriter.Close();
                    myStream.Close();
                    //textBox1.Text = string.Empty;
                    MessageBox.Show("数据写入成功！", "成功");
                }
                catch
                {
                    MessageBox.Show("数据写入失败！");
                }

            }
           
        }

        //将可用端口号添加到ComboBox
        private void SearchAndAddSerialToComboBox(SerialPort MyPort, ComboBox MyBox)
        {                                                               
            string Buffer;                                              
            string[] COMAbleToUse = {""};
            int cnt = 0;
            MyBox.Items.Clear();                                        
            for (int i = 1; i < 21; i++)                                //串口最多检查到COM20
            {
                try                                                     
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();                                      
                    MyBox.Items.Add(Buffer);                            
                    MyPort.Close();                                     
                    COMAbleToUse[cnt] = Buffer;
                    cnt++;
                }
                catch
                {

                }
            }
            MyBox.Text = COMAbleToUse[0];                                   //初始化
        }


        private void CheckFreqAndDirection()
        {
            int Freq = System.Convert.ToInt32(comboBox2.Text);
            int Direction = System.Convert.ToInt32(comboBox3.Text);
            switch(Freq)
            {
                case (15):
                    switch(Direction)
                    {
                        case 0: ReadByteFromFile(ReadByteData, FilePath15_0);
                            pictureBox1.BackgroundImage = Properties.Resources.WHU1;
                            break;
                        case 30:ReadByteFromFile(ReadByteData, FilePath15_30);
                            pictureBox1.BackgroundImage = Properties.Resources.WHU2;
                            break;
                        case 45:ReadByteFromFile(ReadByteData, FilePath15_45);
                            pictureBox1.BackgroundImage = Properties.Resources.WHU3;
                            break;
                        case 60:ReadByteFromFile(ReadByteData, FilePath15_60);
                            pictureBox1.BackgroundImage = Properties.Resources.WHU4;
                            break;
                        default:
                            MessageBox.Show("未找到相应文件！");
                            break;
                    }
                    break;
                case (16):
                    switch (Direction)
                    {
                        case 0:ReadByteFromFile(ReadByteData, FilePath16_0);
                            break;
                        case 30:ReadByteFromFile(ReadByteData, FilePath16_30);
                            break;
                        case 45:ReadByteFromFile(ReadByteData, FilePath16_45);
                            break;
                        case 60:ReadByteFromFile(ReadByteData, FilePath16_60);
                            break;
                        default:
                            MessageBox.Show("未找到相应文件！");
                            break;
                    }
                    break;
                case (17):
                    switch (Direction)
                    {
                        case 0:ReadByteFromFile(ReadByteData, FilePath17_0);
                            pictureBox1.BackgroundImage = Properties.Resources.WHU2;
                            break;
                        case 30:ReadByteFromFile(ReadByteData, FilePath17_30);
                            break;
                        case 45:ReadByteFromFile(ReadByteData, FilePath17_45);
                            break;
                        case 60:ReadByteFromFile(ReadByteData, FilePath17_60);
                            break;
                        default:
                            MessageBox.Show("未找到相应文件！");
                            break;
                    }
                    break;
                default: MessageBox.Show("未找到相应文件！");
                    break;

            }

        }

        private void AutoScanCheckFreq()
        {
            int Freq = System.Convert.ToInt32(comboBox2.Text);            
            switch (Freq)
            {
                case (15):

                    ReadByteFromFile(AutoReadByteData1, FilePath15_0);

                    ReadByteFromFile(AutoReadByteData2, FilePath15_30);
                    
                    ReadByteFromFile(AutoReadByteData3, FilePath15_45);

                    ReadByteFromFile(AutoReadByteData4, FilePath15_60);

                    break;
                case (16):
                   
                    ReadByteFromFile(AutoReadByteData1, FilePath16_0);

                    ReadByteFromFile(AutoReadByteData2, FilePath16_30);
   
                    ReadByteFromFile(AutoReadByteData3, FilePath16_45);
           
                    ReadByteFromFile(AutoReadByteData4, FilePath16_60);
       
                    break;

                case (17):
                  
                    ReadByteFromFile(AutoReadByteData1, FilePath17_0);
                                            
                    ReadByteFromFile(AutoReadByteData2, FilePath17_30);
                          
                    ReadByteFromFile(AutoReadByteData3, FilePath17_45);
                            
                    ReadByteFromFile(AutoReadByteData4, FilePath17_60);
                           
                    break;
             
                default:MessageBox.Show("未找到相应文件！");
                    break;

            }
        }


        private void DataWriteArr()
        {
            //17_0
            //WriteByteData[0] = 0x13;//7.26
            //WriteByteData[1] = 0xDF;
            //WriteByteData[2] = 0x21;//3.62
            //WriteByteData[3] = 0xEE;
            //WriteByteData[4] = 0x3A;//18.86
            //WriteByteData[5] = 0x0F;
            //WriteByteData[6] = 0x4E;//26.48
            //WriteByteData[7] = 0x1F;

            //WriteByteData[8] = 0x13;//7.26
            //WriteByteData[9] = 0xDF;
            //WriteByteData[10] = 0x21;//3.62
            //WriteByteData[11] = 0xEE;
            //WriteByteData[12] = 0x32;//4.52
            //WriteByteData[13] = 0x69;
            //WriteByteData[14] = 0x43;//6.66
            //WriteByteData[15] = 0x8D;

            //WriteByteData[16] = 0x13;//7.26
            //WriteByteData[17] = 0xDF;
            //WriteByteData[18] = 0x21;//3.62
            //WriteByteData[19] = 0xEE;
            //WriteByteData[20] = 0x3A;//18.86
            //WriteByteData[21] = 0x0F;
            //WriteByteData[22] = 0x4E;//26.48
            //WriteByteData[23] = 0x1F;

            //WriteByteData[24] = 0x13;//7.26
            //WriteByteData[25] = 0xDF;
            //WriteByteData[26] = 0x21;//3.62
            //WriteByteData[27] = 0xEE;
            //WriteByteData[28] = 0x3A;//18.86
            //WriteByteData[29] = 0x0F;
            //WriteByteData[30] = 0x4E;//26.48
            //WriteByteData[31] = 0x1F;

            //WriteByteData[32] = 0x13;//7.26
            //WriteByteData[33] = 0xDF;
            //WriteByteData[34] = 0x21;//3.62
            //WriteByteData[35] = 0xEE;
            //WriteByteData[36] = 0x3A;//18.86
            //WriteByteData[37] = 0x0F;
            //WriteByteData[38] = 0x4E;//26.48
            //WriteByteData[39] = 0x1F;

            //WriteByteData[40] = 0x13;//7.26
            //WriteByteData[41] = 0xDF;
            //WriteByteData[42] = 0x21;//3.62
            //WriteByteData[43] = 0xEE;
            //WriteByteData[44] = 0x3A;//18.86
            //WriteByteData[45] = 0x0F;
            //WriteByteData[46] = 0x4E;//26.48
            //WriteByteData[47] = 0x1F;

            //WriteByteData[48] = 0x13;//7.26
            //WriteByteData[49] = 0xDF;
            //WriteByteData[50] = 0x21;//3.62
            //WriteByteData[51] = 0xEE;
            //WriteByteData[52] = 0x3A;//18.86
            //WriteByteData[53] = 0x0F;
            //WriteByteData[54] = 0x4E;//26.48
            //WriteByteData[55] = 0x1F;

            //WriteByteData[56] = 0x13;//7.26
            //WriteByteData[57] = 0xDF;
            //WriteByteData[58] = 0x21;//3.62
            //WriteByteData[59] = 0xEE;
            //WriteByteData[60] = 0x3A;//18.86
            //WriteByteData[61] = 0x0F;
            //WriteByteData[62] = 0x4E;//26.48
            //WriteByteData[63] = 0x1F;

            //WriteByteData[64] = 0x13;//7.26
            //WriteByteData[65] = 0xDF;
            //WriteByteData[66] = 0x21;//3.62
            //WriteByteData[67] = 0xEE;
            //WriteByteData[68] = 0x3A;//18.86
            //WriteByteData[69] = 0x0F;
            //WriteByteData[70] = 0x4E;//26.48
            //WriteByteData[71] = 0x1F;

            //WriteByteData[72] = 0x13;//7.26
            //WriteByteData[73] = 0xDF;
            //WriteByteData[74] = 0x21;//3.62
            //WriteByteData[75] = 0xEE;
            //WriteByteData[76] = 0x3A;//18.86
            //WriteByteData[77] = 0x0F;
            //WriteByteData[78] = 0x4E;//26.48
            //WriteByteData[79] = 0x1F;

            //WriteByteData[80] = 0x13;//7.26
            //WriteByteData[81] = 0xDF;
            //WriteByteData[82] = 0x21;//3.62
            //WriteByteData[83] = 0xEE;
            //WriteByteData[84] = 0x3A;//18.86
            //WriteByteData[85] = 0x0F;
            //WriteByteData[86] = 0x4E;//26.48
            //WriteByteData[87] = 0x1F;

            //WriteByteData[88] = 0x13;//7.26
            //WriteByteData[89] = 0xDF;
            //WriteByteData[90] = 0x21;//3.62
            //WriteByteData[91] = 0xEE;
            //WriteByteData[92] = 0x3A;//18.86
            //WriteByteData[93] = 0x0F;
            //WriteByteData[94] = 0x4E;//26.48
            //WriteByteData[95] = 0x1F;

            //WriteByteData[96] = 0x13;//7.26
            //WriteByteData[97] = 0xDF;
            //WriteByteData[98] = 0x21;//3.62
            //WriteByteData[99] = 0xEE;
            //WriteByteData[100] = 0x3A;//18.86
            //WriteByteData[101] = 0x0F;
            //WriteByteData[102] = 0x4E;//26.48
            //WriteByteData[103] = 0x1F;

            //WriteByteData[104] = 0x13;//7.26
            //WriteByteData[105] = 0xDF;
            //WriteByteData[106] = 0x21;//3.62
            //WriteByteData[107] = 0xEE;
            //WriteByteData[108] = 0x3A;//18.86
            //WriteByteData[109] = 0x0F;
            //WriteByteData[110] = 0x4E;//26.48
            //WriteByteData[111] = 0x1F;

            //WriteByteData[112] = 0x13;//7.26
            //WriteByteData[113] = 0xDF;
            //WriteByteData[114] = 0x21;//3.62
            //WriteByteData[115] = 0xEE;
            //WriteByteData[116] = 0x3A;//18.86
            //WriteByteData[117] = 0x0F;
            //WriteByteData[118] = 0x4E;//26.48
            //WriteByteData[119] = 0x1F;

            //WriteByteData[120] = 0x13;//7.26
            //WriteByteData[121] = 0xDF;
            //WriteByteData[122] = 0x21;//3.62
            //WriteByteData[123] = 0xEE;
            //WriteByteData[124] = 0x3A;//18.86
            //WriteByteData[125] = 0x0F;
            //WriteByteData[126] = 0x4E;//26.48
            //WriteByteData[127] = 0x1F;

            //15_30
            WriteByteData[0] = 0x13;//7.26
            WriteByteData[1] = 0xDF;
            WriteByteData[2] = 0x21;//3.62
            WriteByteData[3] = 0xEE;
            WriteByteData[4] = 0x32;//4.52
            WriteByteData[5] = 0x69;
            WriteByteData[6] = 0x43;//6.66
            WriteByteData[7] = 0x8D;

            WriteByteData[8] = 0x10;//1.18
            WriteByteData[9] = 0xA1;
            WriteByteData[10] = 0x21;//3.06
            WriteByteData[11] = 0xA2;
            WriteByteData[12] = 0x32;//4.28
            WriteByteData[13] = 0x48;
            WriteByteData[14] = 0x43;//5.92
            WriteByteData[15] = 0x28;

            WriteByteData[16] = 0x13;//7.26
            WriteByteData[17] = 0xDF;
            WriteByteData[18] = 0x21;//3.62
            WriteByteData[19] = 0xEE;
            WriteByteData[20] = 0x32;//4.52
            WriteByteData[21] = 0x69;
            WriteByteData[22] = 0x43;//6.66
            WriteByteData[23] = 0x8D;
            WriteByteData[24] = 0x11;//2.18
            WriteByteData[25] = 0x2A;
            WriteByteData[26] = 0x22;//5.54
            WriteByteData[27] = 0xF4;
            WriteByteData[28] = 0x34;//8.28
            WriteByteData[29] = 0x6A;
            WriteByteData[30] = 0x42;//3.92
            WriteByteData[31] = 0x17;
            WriteByteData[32] = 0x13;//7.26
            WriteByteData[33] = 0xDF;
            WriteByteData[34] = 0x21;//3.62
            WriteByteData[35] = 0xEE;
            WriteByteData[36] = 0x32;//4.52
            WriteByteData[37] = 0x69;
            WriteByteData[38] = 0x43;//6.66
            WriteByteData[39] = 0x8D;
            WriteByteData[40] = 0x11;//2.18
            WriteByteData[41] = 0x2A;
            WriteByteData[42] = 0x22;//5.54
            WriteByteData[43] = 0xF4;
            WriteByteData[44] = 0x34;//8.28
            WriteByteData[45] = 0x6A;
            WriteByteData[46] = 0x42;//3.92
            WriteByteData[47] = 0x17;
            WriteByteData[48] = 0x13;//7.26
            WriteByteData[49] = 0xDF;
            WriteByteData[50] = 0x21;//3.62
            WriteByteData[51] = 0xEE;
            WriteByteData[52] = 0x32;//4.52
            WriteByteData[53] = 0x69;
            WriteByteData[54] = 0x43;//6.66
            WriteByteData[55] = 0x8D;
            WriteByteData[56] = 0x11;//2.18
            WriteByteData[57] = 0x2A;
            WriteByteData[58] = 0x22;//5.54
            WriteByteData[59] = 0xF4;
            WriteByteData[60] = 0x34;//8.28
            WriteByteData[61] = 0x6A;
            WriteByteData[62] = 0x42;//3.92
            WriteByteData[63] = 0x17;
            WriteByteData[64] = 0x13;//7.26
            WriteByteData[65] = 0xDF;
            WriteByteData[66] = 0x21;//3.62
            WriteByteData[67] = 0xEE;
            WriteByteData[68] = 0x32;//4.52
            WriteByteData[69] = 0x69;
            WriteByteData[70] = 0x43;//6.66
            WriteByteData[71] = 0x8D;
            WriteByteData[72] = 0x11;//2.18
            WriteByteData[73] = 0x2A;
            WriteByteData[74] = 0x22;//5.54
            WriteByteData[75] = 0xF4;
            WriteByteData[76] = 0x34;//8.28
            WriteByteData[77] = 0x6A;
            WriteByteData[78] = 0x42;//3.92
            WriteByteData[79] = 0x17;
            WriteByteData[80] = 0x13;//7.26
            WriteByteData[81] = 0xDF;
            WriteByteData[82] = 0x21;//3.62
            WriteByteData[83] = 0xEE;
            WriteByteData[84] = 0x32;//4.52
            WriteByteData[85] = 0x69;
            WriteByteData[86] = 0x43;//6.66
            WriteByteData[87] = 0x8D;
            WriteByteData[88] = 0x11;//2.18
            WriteByteData[89] = 0x2A;
            WriteByteData[90] = 0x22;//5.54
            WriteByteData[91] = 0xF4;
            WriteByteData[92] = 0x34;//8.28
            WriteByteData[93] = 0x6A;
            WriteByteData[94] = 0x42;//3.92
            WriteByteData[95] = 0x17;
            WriteByteData[96] = 0x13;//7.26
            WriteByteData[97] = 0xDF;
            WriteByteData[98] = 0x21;//3.62
            WriteByteData[99] = 0xEE;
            WriteByteData[100] = 0x32;//4.52
            WriteByteData[101] = 0x69;
            WriteByteData[102] = 0x43;//6.66
            WriteByteData[103] = 0x8D;
            WriteByteData[104] = 0x11;//2.18
            WriteByteData[105] = 0x2A;
            WriteByteData[106] = 0x22;//5.54
            WriteByteData[107] = 0xF4;
            WriteByteData[108] = 0x34;//8.28
            WriteByteData[109] = 0x6A;
            WriteByteData[110] = 0x42;//3.92
            WriteByteData[111] = 0x17;
            WriteByteData[112] = 0x13;//7.26
            WriteByteData[113] = 0xDF;
            WriteByteData[114] = 0x21;//3.62
            WriteByteData[115] = 0xEE;
            WriteByteData[116] = 0x32;//4.52
            WriteByteData[117] = 0x69;
            WriteByteData[118] = 0x43;//6.66
            WriteByteData[119] = 0x8D;
            //WriteByteData[120] = 0x11;//2.18
            //WriteByteData[121] = 0x2A;
            //WriteByteData[122] = 0x22;//5.54
            //WriteByteData[123] = 0xF4;
            //WriteByteData[124] = 0x34;//8.28
            //WriteByteData[125] = 0x6A;
            //WriteByteData[126] = 0x42;//3.92
            //WriteByteData[127] = 0x17;
            WriteByteData[120] = 0x13;
            WriteByteData[121] = 0x9E;
            WriteByteData[122] = 0x22;
            WriteByteData[123] = 0x04;
            WriteByteData[124] = 0x32;
            WriteByteData[125] = 0x8D;
            WriteByteData[126] = 0x43;
            WriteByteData[127] = 0x15;



        }

        private void ZeroDataArr()
        {
            ZeroData[1] = 0x10;
            ZeroData[2] = 0x00;
            ZeroData[3] = 0x20;
            ZeroData[4] = 0x00;
            ZeroData[5] = 0x30;
            ZeroData[6] = 0x00;
            ZeroData[7] = 0x40;
            ZeroData[8] = 0x00;
            ZeroData[9] = 0x10;
            ZeroData[10] = 0x00;
            ZeroData[11] = 0x20;
            ZeroData[12] = 0x00;
            ZeroData[13] = 0x30;
            ZeroData[14] = 0x00;
            ZeroData[15] = 0x40;
            ZeroData[16] = 0x00;
            ZeroData[17] = 0x10;
            ZeroData[18] = 0x00;
            ZeroData[19] = 0x20;
            ZeroData[20] = 0x00;
            ZeroData[21] = 0x30;
            ZeroData[22] = 0x00;
            ZeroData[23] = 0x40;
            ZeroData[24] = 0x00;
            ZeroData[25] = 0x10;
            ZeroData[26] = 0x00;
            ZeroData[27] = 0x20;
            ZeroData[28] = 0x00;
            ZeroData[29] = 0x30;
            ZeroData[30] = 0x00;
            ZeroData[31] = 0x40;
            ZeroData[32] = 0x00;
            ZeroData[33] = 0x10;
            ZeroData[34] = 0x00;
            ZeroData[35] = 0x20;
            ZeroData[36] = 0x00;
            ZeroData[37] = 0x30;
            ZeroData[38] = 0x00;
            ZeroData[39] = 0x40;
            ZeroData[40] = 0x00;
            ZeroData[41] = 0x10;
            ZeroData[42] = 0x00;
            ZeroData[43] = 0x20;
            ZeroData[44] = 0x00;
            ZeroData[45] = 0x30;
            ZeroData[46] = 0x00;
            ZeroData[47] = 0x40;
            ZeroData[48] = 0x00;
            ZeroData[49] = 0x10;
            ZeroData[50] = 0x00;
            ZeroData[51] = 0x20;
            ZeroData[52] = 0x00;
            ZeroData[53] = 0x30;
            ZeroData[54] = 0x00;
            ZeroData[55] = 0x40;
            ZeroData[56] = 0x00;
            ZeroData[57] = 0x10;
            ZeroData[58] = 0x00;
            ZeroData[59] = 0x20;
            ZeroData[60] = 0x00;
            ZeroData[61] = 0x30;
            ZeroData[62] = 0x00;
            ZeroData[63] = 0x40;
            ZeroData[64] = 0x00;
            ZeroData[65] = 0x10;
            ZeroData[66] = 0x00;
            ZeroData[67] = 0x20;
            ZeroData[68] = 0x00;
            ZeroData[69] = 0x30;
            ZeroData[70] = 0x00;
            ZeroData[71] = 0x40;
            ZeroData[72] = 0x00;
            ZeroData[73] = 0x10;
            ZeroData[74] = 0x00;
            ZeroData[75] = 0x20;
            ZeroData[76] = 0x00;
            ZeroData[77] = 0x30;
            ZeroData[78] = 0x00;
            ZeroData[79] = 0x40;
            ZeroData[80] = 0x00;
            ZeroData[81] = 0x10;
            ZeroData[82] = 0x00;
            ZeroData[83] = 0x20;
            ZeroData[84] = 0x00;
            ZeroData[85] = 0x30;
            ZeroData[86] = 0x00;
            ZeroData[87] = 0x40;
            ZeroData[88] = 0x00;
            ZeroData[89] = 0x10;
            ZeroData[90] = 0x00;
            ZeroData[91] = 0x20;
            ZeroData[92] = 0x00;
            ZeroData[93] = 0x30;
            ZeroData[94] = 0x00;
            ZeroData[95] = 0x40;
            ZeroData[96] = 0x00;
            ZeroData[97] = 0x10;
            ZeroData[98] = 0x00;
            ZeroData[99] = 0x20;
            ZeroData[100] = 0x00;
            ZeroData[101] = 0x30;
            ZeroData[102] = 0x00;
            ZeroData[103] = 0x40;
            ZeroData[104] = 0x00;
            ZeroData[105] = 0x10;
            ZeroData[106] = 0x00;
            ZeroData[107] = 0x20;
            ZeroData[108] = 0x00;
            ZeroData[109] = 0x30;
            ZeroData[110] = 0x00;
            ZeroData[111] = 0x40;
            ZeroData[112] = 0x00;
            ZeroData[113] = 0x10;
            ZeroData[114] = 0x00;
            ZeroData[115] = 0x20;
            ZeroData[116] = 0x00;
            ZeroData[117] = 0x30;
            ZeroData[118] = 0x00;
            ZeroData[119] = 0x40;
            ZeroData[120] = 0x00;
            ZeroData[121] = 0x10;
            ZeroData[122] = 0x00;
            ZeroData[123] = 0x20;
            ZeroData[124] = 0x00;
            ZeroData[125] = 0x30;
            ZeroData[126] = 0x00;
            ZeroData[127] = 0x40;
            ZeroData[128] = 0x00;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ReadByteFromFile(FileInfo, ReadByteData);
                FileReadOver = true;
                //button3.Enabled = false;
            }
            catch
            {
                MessageBox.Show("文件读取失败");
            }

        }

        private void ReadByteFromFile(string[] FileInfo, byte[] DataBuffer)
        {
            int BufferCnt = 1;
            //int FileInfoCnt = 0;
            openFileDialog1.Filter = "*.dat|*.dat";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //textBox1.Text = string.Empty;
                FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                BinaryReader myReader = new BinaryReader(myStream);
                //while (FileInfoCnt < 10)
                //{
                //    FileInfo[FileInfoCnt] = myReader.ReadString();
                //    FileInfoCnt++;
                //}
                while (BufferCnt < DataLength + 1)
                {
                    DataBuffer[BufferCnt] = myReader.ReadByte();
                    BufferCnt++;
                }
                myReader.Close();
                myStream.Close();
            }

        }

        //多字节写入串口
        private void WriteByteToSerialPort(byte[] Buffer, int offset, int length)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write(Buffer, offset, length);
                }
                catch
                {
                    MessageBox.Show("串口数据发送出错，请检查.", "错误");
                }
            }
        }

        //单字节写入串口
        private void WriteByteToSerialPort(byte data)
        {
            byte[] Buffer = new byte[1] { data };
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write(Buffer, 0, 1);
                }
                catch
                {
                    MessageBox.Show("串口数据发送出错，请检查.", "错误");
                }
            }
        }

        //Buffer:从文件里读出数据缓存
        //offset:文件读出位置偏移量
        //length:读出字节长度
        private void ReadByteFromFile(byte[] Buffer, int offset, int length)
        {
            FileStream TxtFile = new FileStream(FilePath17_0, FileMode.Open);
            TxtFile.Seek(offset, SeekOrigin.Begin);
            TxtFile.Read(Buffer, 1, length);
            //TxtFile.Seek(6, SeekOrigin.Current);
            //TxtFile.Write(Buffer, 0, length);
        }

       
    }
}
