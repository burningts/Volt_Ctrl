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
        byte DataLength = 136;
        
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
                pictureBox1.BackgroundImage = Properties.Resources.deg0;
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
                    pictureBox1.BackgroundImage = Properties.Resources.deg0;
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
                    pictureBox1.BackgroundImage = Properties.Resources.deg0;
                    timer1.Start();
                    AutoScanFlag++;
                    break;
                case (2): 
                    WriteByteToSerialPort(AutoReadByteData2, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.deg30;
                    timer1.Start();
                    AutoScanFlag++;
                    break;
                case (3):
                    WriteByteToSerialPort(AutoReadByteData3, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.deg45;
                    timer1.Start();
                    AutoScanFlag++;
                    break;
                case (4):
                    WriteByteToSerialPort(AutoReadByteData4, 0, (DataLength + 1));
                    pictureBox1.BackgroundImage = Properties.Resources.deg60;
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
                pictureBox1.BackgroundImage = Properties.Resources.deg0;
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
                            pictureBox1.BackgroundImage = Properties.Resources.deg0;
                            break;
                        case 30:ReadByteFromFile(ReadByteData, FilePath15_30);
                            pictureBox1.BackgroundImage = Properties.Resources.deg30;
                            break;
                        case 45:ReadByteFromFile(ReadByteData, FilePath15_45);
                            pictureBox1.BackgroundImage = Properties.Resources.deg45;
                            break;
                        case 60:ReadByteFromFile(ReadByteData, FilePath15_60);
                            pictureBox1.BackgroundImage = Properties.Resources.deg60;
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
                            pictureBox1.BackgroundImage = Properties.Resources.deg0;
                            break;
                        case 30:ReadByteFromFile(ReadByteData, FilePath16_30);
                            pictureBox1.BackgroundImage = Properties.Resources.deg30;
                            break;
                        case 45:ReadByteFromFile(ReadByteData, FilePath16_45);
                            pictureBox1.BackgroundImage = Properties.Resources.deg45;
                            break;
                        case 60:ReadByteFromFile(ReadByteData, FilePath16_60);
                            pictureBox1.BackgroundImage = Properties.Resources.deg60;
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
                            pictureBox1.BackgroundImage = Properties.Resources.deg0;
                            break;
                        case 30:ReadByteFromFile(ReadByteData, FilePath17_30);
                            pictureBox1.BackgroundImage = Properties.Resources.deg30;
                            break;
                        case 45:ReadByteFromFile(ReadByteData, FilePath17_45);
                            pictureBox1.BackgroundImage = Properties.Resources.deg45;
                            break;
                        case 60:ReadByteFromFile(ReadByteData, FilePath17_60);
                            pictureBox1.BackgroundImage = Properties.Resources.deg60;
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

            WriteByteData[0] = 0x14;
            WriteByteData[1] = 0x26;
            WriteByteData[2] = 0x22;    //3.78
            WriteByteData[3] = 0x04;
            WriteByteData[4] = 0x33;    //5.78
            WriteByteData[5] = 0x15;
            WriteByteData[6] = 0x44;    //7.78
            WriteByteData[7] = 0x26;

            WriteByteData[8] = 0x10;    //1.78
            WriteByteData[9] = 0xF3;
            WriteByteData[10] = 0x22;   //3.78
            WriteByteData[11] = 0x04;
            WriteByteData[12] = 0x33;   //5.78
            WriteByteData[13] = 0x15;
            WriteByteData[14] = 0x44;   //7.78
            WriteByteData[15] = 0x26;

            WriteByteData[16] = 0x10;   //1.78
            WriteByteData[17] = 0xF3;
            WriteByteData[18] = 0x22;   //3.78
            WriteByteData[19] = 0x04;
            WriteByteData[20] = 0x33;   //5.78
            WriteByteData[21] = 0x15;
            WriteByteData[22] = 0x44;   //7.78
            WriteByteData[23] = 0x26;
            WriteByteData[24] = 0x10;   //1.78
            WriteByteData[25] = 0xF3;
            WriteByteData[26] = 0x22;   //3.78
            WriteByteData[27] = 0x04;
            WriteByteData[28] = 0x33;   //5.78
            WriteByteData[29] = 0x15;
            WriteByteData[30] = 0x44;   //7.78
            WriteByteData[31] = 0x26;
            WriteByteData[32] = 0x10;   //1.78
            WriteByteData[33] = 0xF3;
            WriteByteData[34] = 0x22;   //3.78
            WriteByteData[35] = 0x04;
            WriteByteData[36] = 0x33;   //5.78
            WriteByteData[37] = 0x15;
            WriteByteData[38] = 0x44;   //7.78
            WriteByteData[39] = 0x26;
            WriteByteData[40] = 0x10;   //1.78
            WriteByteData[41] = 0xF3;
            WriteByteData[42] = 0x22;   //3.78
            WriteByteData[43] = 0x04;
            WriteByteData[44] = 0x33;   //5.78
            WriteByteData[45] = 0x15;
            WriteByteData[46] = 0x44;   //7.78
            WriteByteData[47] = 0x26;
            WriteByteData[48] = 0x10;   //1.78
            WriteByteData[49] = 0xF3;
            WriteByteData[50] = 0x22;   //3.78
            WriteByteData[51] = 0x04;
            WriteByteData[52] = 0x33;   //5.78
            WriteByteData[53] = 0x15;
            WriteByteData[54] = 0x44;   //7.78
            WriteByteData[55] = 0x26;
            WriteByteData[56] = 0x10;   //1.78
            WriteByteData[57] = 0xF3;
            WriteByteData[58] = 0x22;   //3.78
            WriteByteData[59] = 0x04;
            WriteByteData[60] = 0x33;   //5.78
            WriteByteData[61] = 0x15;
            WriteByteData[62] = 0x44;   //7.78
            WriteByteData[63] = 0x26;
            WriteByteData[64] = 0x10;   //1.78
            WriteByteData[65] = 0xF3;
            WriteByteData[66] = 0x22;   //3.78
            WriteByteData[67] = 0x04;
            WriteByteData[68] = 0x33;   //5.78
            WriteByteData[69] = 0x15;
            WriteByteData[70] = 0x44;   //7.78
            WriteByteData[71] = 0x26;
            WriteByteData[72] = 0x10;   //1.78
            WriteByteData[73] = 0xF3;
            WriteByteData[74] = 0x22;   //3.78
            WriteByteData[75] = 0x04;
            WriteByteData[76] = 0x33;   //5.78
            WriteByteData[77] = 0x15;
            WriteByteData[78] = 0x44;   //7.78
            WriteByteData[79] = 0x26;
            WriteByteData[80] = 0x10;   //1.78
            WriteByteData[81] = 0xF3;
            WriteByteData[82] = 0x22;   //3.78
            WriteByteData[83] = 0x04;
            WriteByteData[84] = 0x33;   //5.78
            WriteByteData[85] = 0x15;
            WriteByteData[86] = 0x44;   //7.78
            WriteByteData[87] = 0x26;
            WriteByteData[88] = 0x10;   //1.78
            WriteByteData[89] = 0xF3;
            WriteByteData[90] = 0x22;   //3.78
            WriteByteData[91] = 0x04;
            WriteByteData[92] = 0x33;   //5.78
            WriteByteData[93] = 0x15;
            WriteByteData[94] = 0x44;   //7.78
            WriteByteData[95] = 0x26;
            WriteByteData[96] = 0x10;   //1.78
            WriteByteData[97] = 0xF3;
            WriteByteData[98] = 0x22;   //3.78
            WriteByteData[99] = 0x04;
            WriteByteData[100] = 0x33;  //5.78
            WriteByteData[101] = 0x15;
            WriteByteData[102] = 0x44;  //7.78
            WriteByteData[103] = 0x26;
            WriteByteData[104] = 0x10;  //1.78
            WriteByteData[105] = 0xF3;
            WriteByteData[106] = 0x22;  //3.78
            WriteByteData[107] = 0x04;
            WriteByteData[108] = 0x33;  //5.78
            WriteByteData[109] = 0x15;
            WriteByteData[110] = 0x44;  //7.78
            WriteByteData[111] = 0x26;
            WriteByteData[112] = 0x10;  //1.78
            WriteByteData[113] = 0xF3;
            WriteByteData[114] = 0x22;  //3.78
            WriteByteData[115] = 0x04;
            WriteByteData[116] = 0x33;  //5.78
            WriteByteData[117] = 0x15;
            WriteByteData[118] = 0x44;  //7.78
            WriteByteData[119] = 0x26;
            WriteByteData[120] = 0x10;  //1.78
            WriteByteData[121] = 0xF3;
            WriteByteData[122] = 0x22;  //3.78
            WriteByteData[123] = 0x04;
            WriteByteData[124] = 0x33;  //5.78
            WriteByteData[125] = 0x15;
            WriteByteData[126] = 0x44;  //7.78
            WriteByteData[127] = 0x26;
            WriteByteData[128] = 0x10;  //1.78
            WriteByteData[129] = 0xF3;
            WriteByteData[130] = 0x22;  //3.78
            WriteByteData[131] = 0x04;
            WriteByteData[132] = 0x33;  //5.78
            WriteByteData[133] = 0x15;
            WriteByteData[134] = 0x44;  //7.78
            WriteByteData[135] = 0x26;




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
            ZeroData[129] = 0x10;
            ZeroData[130] = 0x00;
            ZeroData[131] = 0x20;
            ZeroData[132] = 0x00;
            ZeroData[133] = 0x30;
            ZeroData[134] = 0x00;
            ZeroData[135] = 0x40;
            ZeroData[136] = 0x00;


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
