
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace binaryWrite
{
    public partial class Form1 : Form
    {
        const int DataLength = 5; //写入4个有效数据
        byte[] Writedata = new byte[100]; 
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Writedata[0] = 0x12;
            Writedata[1] = 0x53;
            Writedata[2] = 37;
            Writedata[3] = 148;
            Writedata[4] = 0xdd;
            //if (textBox1.Text == string.Empty)
            //{
            //    MessageBox.Show("要写的文本不能为空");
            //}
            //else
            {
                saveFileDialog1.Filter = "二进制文件(*.dat)|*.dat";  //设置保存文件格式
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) //判断是否选择文件
                {
                   try
                    {
                        //使用另存为
                        FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);//写入文件
                        BinaryWriter myWriter = new BinaryWriter(myStream);
                        for (int i = 0; i < DataLength; i++)
                        {
                            myWriter.Write(Writedata[i]);
                        }
                        myWriter.Close();
                        myStream.Close();
                        textBox1.Text = string.Empty;
                        MessageBox.Show("数据写入成功！", "成功");
                    }
                    catch
                    {
                        MessageBox.Show("数据写入失败！");
                    }
                    
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] str = new string[10];
            openFileDialog1.Filter = "*.dat|*.dat";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = string.Empty;
                FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);//读取数据
                BinaryReader myReader = new BinaryReader(myStream);
                //StreamReader myReader = new StreamReader(myStream);
                //string s =  myReader.ReadLine();
                //textBox1.Text = ;
                //textBox1.AppendText(myReader.ReadLine());

                if (myReader.PeekChar() != -1)
                {
                    textBox1.Text = Convert.ToString(myReader.ReadByte());
                    str[1] = Convert.ToString(myReader.ReadByte());
                    str[2] = Convert.ToString(myReader.ReadByte());
                    textBox2.Text = Convert.ToString(myReader.ReadByte());
                    //textBox1.Text = myReader.ReadString();
                }
                myReader.Close();
                myStream.Close();
            }
        }

    }
}
