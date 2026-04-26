using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CANMessageSimulator
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();

        // 複数CAN ID
        private string[] canIds = { "0x100", "0x120", "0x130", "0x200" };

        private int cycle = 200; // 200msで高速更新
        private int errorCount = 0;

        private List<string> logList = new List<string>();
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = cycle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label6.Text = "設定読み込み完了";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
            label6.Text = "シミュレーション開始";
            // ★ 初期値を表示して「動き始めた感」を出す
            label1.Text = "CAN ID: ----";
            label3.Text = "Data: ----";
            label7.Text = "Speed: ---- km/h";
            label8.Text = "RPM: ----";
            label9.Text = "Temp: ---- °C";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            label6.Text = "シミュレーション停止";
            // ★ 停止中であることをUIに反映
            label1.Text = "CAN ID: (停止中)";
            label3.Text = "Data: ----";
            label7.Text = "Speed: ---- km/h";
            label8.Text = "RPM: ----";
            label9.Text = "Temp: ---- °C";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            errorCount++;
            label5.Text = "Error Count: " + errorCount;

            byte[] errorData = new byte[8];
            for (int i = 0; i < 8; i++) errorData[i] = 0xFF;

            label3.Text = "Data: " + BitConverter.ToString(errorData);
            label6.Text = "エラー注入";

            logList.Add($"{DateTime.Now},ERROR,{BitConverter.ToString(errorData)}");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV Files (*.csv)|*.csv";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(dialog.FileName, logList);
                label6.Text = "ログ保存完了";
            }
        }
        // ★ タイマー：CANデータ送信
        private void timer1_Tick(object sender, EventArgs e)
        {
            // CAN IDをランダムに切り替え
            string id = canIds[rand.Next(canIds.Length)];
            label1.Text = "CAN ID: " + id;

            // 車載っぽいデータ生成
            byte[] data = GenerateCarLikeData();

            // ★ Data を label2 に表示
            label2.Text = "Data: " + BitConverter.ToString(data);

            // 速度・回転数・温度を表示
            int speed = data[0];
            int rpm = data[1] + (data[2] << 8);
            int temp = (sbyte)data[3];

            label7.Text = "Speed: " + speed + " km/h";
            label8.Text = "RPM: " + rpm;
            label9.Text = "Temp: " + temp + " °C";

            // 時刻更新
            label4.Text = "Last Sent: " + DateTime.Now.ToString("HH:mm:ss.fff");

            // ログ追加
            logList.Add($"{DateTime.Now},{id},{BitConverter.ToString(data)}");

        }

        // ★ ランダムCANデータ生成
        private byte[] GenerateRandomData()
        {
            byte[] data = new byte[8];

            // 速度（0〜180 km/h）
            int speed = rand.Next(0, 181);
            data[0] = (byte)(speed & 0xFF);

            // 回転数（0〜6000 rpm）
            int rpm = rand.Next(0, 6001);
            data[1] = (byte)(rpm & 0xFF);
            data[2] = (byte)((rpm >> 8) & 0xFF);

            // 温度（-20〜120℃）
            int temp = rand.Next(-20, 121);
            data[3] = (byte)(temp & 0xFF);

            // 残りはランダム
            for (int i = 4; i < 8; i++)
                data[i] = (byte)rand.Next(0, 256);

            return data;
        }
        private byte[] GenerateCarLikeData()
        {
            byte[] data = new byte[8];

            // 速度（0〜180 km/h）
            int speed = rand.Next(0, 181);
            data[0] = (byte)(speed & 0xFF);

            // 回転数（0〜6000 rpm）
            int rpm = rand.Next(0, 6001);
            data[1] = (byte)(rpm & 0xFF);
            data[2] = (byte)((rpm >> 8) & 0xFF);

            // 温度（-20〜120℃）
            int temp = rand.Next(-20, 121);
            data[3] = (byte)(temp & 0xFF);

            // 残りはランダム
            for (int i = 4; i < 8; i++)
                data[i] = (byte)rand.Next(0, 256);

            return data;
        }
    }
}
