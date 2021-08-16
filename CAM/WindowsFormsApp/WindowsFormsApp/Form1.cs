using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Threading;
namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection camera;
        private VideoCaptureDevice cam;
        string InputData = String.Empty;
        delegate void SetTextCallback(string text);


        public Form1()
        {
            InitializeComponent();
            camera = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach(FilterInfo info in camera)
            {
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex=0;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox2.Items.AddRange(ports);
            comboBox2.SelectedIndex = 0;

            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBox3.Items.AddRange(BaudRate);
            comboBox3.SelectedIndex = 3;
            if (cam != null && cam.IsRunning)
            {
                cam.Stop();
            }
            cam = new VideoCaptureDevice(camera[comboBox1.SelectedIndex].MonikerString);
            cam.NewFrame += Cam_NewFrame;
            cam.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {

                label5.Text = ("Chưa kết nối "+comboBox2.Text);
                label5.ForeColor = Color.Red;
            }
            else if (serialPort1.IsOpen)
            {
                label5.Text = ("Đã kết nối "+comboBox2.Text);
                label5.ForeColor = Color.Green;

            }
        }
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            InputData = serialPort1.ReadExisting();
            if (InputData != String.Empty)
            {
                SetText(InputData);
                CapSave();
            }
        }
        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText); 
                this.Invoke(d, new object[] { text });
            }
            else this.textBox1.Text += text;
        }
        //chụp và lưu 
   

        private void CapSave()
        {
            if (InputData == "0") ///nhận 0 
            {
                pictureBox2.Image = pictureBox1.Image;
                Invoke((MethodInvoker)(delegate ()
                {
                    var image = pictureBox2.Image;
                    SaveImageCapture(image);
                }));

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox2.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox3.Text);
                serialPort1.Open();
            }
            catch
            {
                MessageBox.Show("Không thể mở " + comboBox2.Text);
            }
        }
        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (cam != null && cam.IsRunning)
            {
                cam.Stop();
            }
        }
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        public static void SaveImageCapture(System.Drawing.Image image)
        {
            string filename = DateTime.Now.ToString("ddhhss");
            FileStream fileStream = new FileStream(@"D:\DO AN\anh\" + filename + ".png", FileMode.CreateNew);
            image.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
