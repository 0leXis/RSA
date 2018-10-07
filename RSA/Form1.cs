using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace RSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            RSAEncryptDecrypt.Log = textBoxLog;
        }

        private void buttonGenerateKeys_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            textBoxLog.Clear();

            RSAEncryptDecrypt.GenerateKeys(out int N, out int E, out long D);

            textBoxOpenE.Text = Convert.ToString(E);
            textBoxOpenN.Text = Convert.ToString(N);
            textBoxSecretN.Text = Convert.ToString(N);
            textBoxSecretD.Text = Convert.ToString(D);

            this.Cursor = Cursors.Default;
        }

        private void textBoxMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!RSAEncryptDecrypt.alfavit.Contains(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ' ')
                e.Handled = true;
        }

        private void buttonEncr_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
            if (textBoxOpenE.Text == "" || textBoxOpenN.Text == "")
                MessageBox.Show("Открытый ключ не введён!");
            else
                if (textBoxMsg.Text == "")
                    MessageBox.Show("Сообщение не введено!");
                else
                {
                    textBoxResult.Text = RSAEncryptDecrypt.Encrypt(Convert.ToInt32(textBoxOpenE.Text), Convert.ToInt32(textBoxOpenN.Text), textBoxMsg.Text);
                }
        }

        private void buttonDecr_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
            if (textBoxSecretD.Text == "" || textBoxSecretN.Text == "")
                MessageBox.Show("Закрытый ключ не введён!");
            else
                if (textBoxMsg.Text == "")
                MessageBox.Show("Сообщение не введено!");
            else
            {
                textBoxResult.Text = RSAEncryptDecrypt.Decrypt(Convert.ToInt32(textBoxSecretD.Text), Convert.ToInt32(textBoxSecretN.Text), textBoxMsg.Text);
            }
        }
    }

    static class RSAEncryptDecrypt
    {
        //--------------------------------
        //   Константы
        //--------------------------------
        public static readonly string alfavit = "||абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ0123456789.,:;!() abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //--------------------------------
        //   Лог
        //--------------------------------
        public static TextBox Log;

        //--------------------------------
        //   Генерация простых чисел
        //--------------------------------
        private static List<int> ReshetoEratosfena(out int P, out int Q)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Генерация простых чисел"+Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);

            var IntArr = new List<int>();
            for (var i = 2; i <= 20000; i++)
                IntArr.Add(i);

            for (var i = 0; i < IntArr.Count; i++)
            {
                for (var j = i; j < IntArr.Count; j++)
                {
                    if ((IntArr[i] != IntArr[j]) && (IntArr[j] % IntArr[i] == 0))
                    {
                        IntArr.RemoveAt(j);
                        j--;
                    }
                }
            }

            var ResList = new List<int>(IntArr);

            for (var i = 0; i < IntArr.Count;)
                if (IntArr[i] < 10000)
                    IntArr.Remove(IntArr[i]);
                else
                    break;
            Log.AppendText("Всего простых от 10000 до 20000: "+Convert.ToString(IntArr.Count) + Environment.NewLine);

            P = IntArr[new Random().Next(0, IntArr.Count - 1)];
            do
            {
                Q = IntArr[new Random().Next(0, IntArr.Count - 1)];
            }
            while (Q == P);

            Log.AppendText("Случайное число P из диапазона: " + Convert.ToString(P) + Environment.NewLine);
            Log.AppendText("Случайное число Q из диапазона: " + Convert.ToString(Q) + Environment.NewLine);

            Log.AppendText("--------------------------" + Environment.NewLine);

            return ResList;
        }
        
        //--------------------------------
        //   Алгоритм Евклида
        //--------------------------------
        private static bool EuclidIsNOD1(int a, int b)
        {

            while (a != 0 && b != 0)
            {
                if (a > b)
                    a -= b;
                else
                    b -= a;
            }
            if (a == 1 || b == 1)
                return false;
            else
                return true;
        }

        //--------------------------------
        //   Генерация ключей
        //--------------------------------
        public static void GenerateKeys(out int N, out int E, out long D)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Генерация ключа" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);

            var P = 0;
            var Q = 0;
            var ProstList = RSAEncryptDecrypt.ReshetoEratosfena(out P, out Q);

            N = P * Q;
            Log.AppendText("N = "+ Convert.ToString(N) + Environment.NewLine);

            var M = (P - 1) * (Q - 1);
            Log.AppendText("M = "+ Convert.ToString(M) + Environment.NewLine);

            E = 0;
            while (true)
            {
                var rnd = ProstList[new Random().Next(0, ProstList.Count - 1)];
                if (!EuclidIsNOD1(rnd, M))
                {
                    E = rnd;
                    break;
                }
            }
            Log.AppendText("E = " + Convert.ToString(E) + Environment.NewLine);

            D = 1;
            while (D * E % M != 1)
            {
                D++;
            }
            Log.AppendText("D = " + Convert.ToString(D) + Environment.NewLine);
        }

        //--------------------------------
        //   Шифрование
        //--------------------------------
        public static string Encrypt(int E, int N, string msg)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Шифрование" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Исходная строка: "+ msg + Environment.NewLine);

            var ResString = "";
            var tmpN = new BigInteger(N);
            var tmpE = new BigInteger(E);

            for (var i = 0; i < msg.Length; i++)
            {
                var tmpIndex = new BigInteger(alfavit.IndexOf(msg[i]));
                var Rem = BigInteger.ModPow(tmpIndex, tmpE, tmpN);
                if (i != 0)
                    ResString += " ";
                ResString += Rem.ToString();
            }

            Log.AppendText("Ответ: " + ResString + Environment.NewLine);
            return ResString;
        }

        //--------------------------------
        //   Расшифровка
        //--------------------------------
        public static string Decrypt(int D, int N, string msg)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Расшифровка" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Исходная строка: " + msg + Environment.NewLine);

            var tmpN = new BigInteger(N);
            var tmpD = new BigInteger(D);

            var msgsplit = msg.Split(' ');

            var ResString = "";
            for (var i = 0; i < msgsplit.Length; i++)
            {
                var Rem = BigInteger.ModPow(new BigInteger(Convert.ToInt32(msgsplit[i])), tmpD, tmpN);
                ResString += alfavit[Convert.ToInt32(Rem.ToString())];
            }

            Log.AppendText("Ответ: " + ResString + Environment.NewLine);
            return ResString;
        }
    }
}
