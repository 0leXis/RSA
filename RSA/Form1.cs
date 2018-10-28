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
            ELGAMALEncryptDecrypt.Log = textBoxLog;
        }

        private void buttonGenerateKeys_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            textBoxLog.Clear();

            if (radioButtonRSA.Checked)
            {
                RSAEncryptDecrypt.GenerateKeys(out int N, out int E, out long D);

                textBoxOpenEY.Text = Convert.ToString(E);
                textBoxOpenNG.Text = Convert.ToString(N);
                textBoxSecretNP.Text = Convert.ToString(N);
                textBoxSecretDX.Text = Convert.ToString(D);
            }
            else
            {
                ELGAMALEncryptDecrypt.GenerateKeys(out int X, out int P, out long G, out long Y);

                textBoxOpenEY.Text = Convert.ToString(Y);
                textBoxOpenNG.Text = Convert.ToString(G);
                textBoxOpenP.Text = Convert.ToString(P);
                textBoxSecretNP.Text = Convert.ToString(P);
                textBoxSecretDX.Text = Convert.ToString(X);
            }

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
            if (radioButtonRSA.Checked)
            {
                if (textBoxOpenEY.Text == "" || textBoxOpenNG.Text == "")
                    MessageBox.Show("Открытый ключ не введён!");
                else
                    if (textBoxMsg.Text == "")
                    MessageBox.Show("Сообщение не введено!");
                else
                {
                    textBoxResult.Text = RSAEncryptDecrypt.Encrypt(Convert.ToInt32(textBoxOpenEY.Text), Convert.ToInt32(textBoxOpenNG.Text), textBoxMsg.Text);
                }
            }
            else
            {
                if (textBoxOpenEY.Text == "" || textBoxOpenNG.Text == "" || textBoxOpenP.Text == "")
                    MessageBox.Show("Открытый ключ не введён!");
                else
                    if (textBoxMsg.Text == "")
                    MessageBox.Show("Сообщение не введено!");
                else
                {
                    textBoxResult.Text = ELGAMALEncryptDecrypt.Encrypt(Convert.ToInt32(textBoxOpenP.Text), Convert.ToInt64(textBoxOpenNG.Text), Convert.ToInt64(textBoxOpenEY.Text), textBoxMsg.Text);
                }
            }
        }

        private void buttonDecr_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
            if (radioButtonRSA.Checked)
            {
                if (textBoxSecretDX.Text == "" || textBoxSecretNP.Text == "")
                    MessageBox.Show("Закрытый ключ не введён!");
                else
                if (textBoxMsg.Text == "")
                    MessageBox.Show("Сообщение не введено!");
                else
                {
                    textBoxResult.Text = RSAEncryptDecrypt.Decrypt(Convert.ToInt32(textBoxSecretDX.Text), Convert.ToInt32(textBoxSecretNP.Text), textBoxMsg.Text);
                }
            }
            else
            {
                if (textBoxSecretDX.Text == "" || textBoxSecretNP.Text == "")
                    MessageBox.Show("Закрытый ключ не введён!");
                else
                if (textBoxMsg.Text == "")
                    MessageBox.Show("Сообщение не введено!");
                else
                {
                    textBoxResult.Text = ELGAMALEncryptDecrypt.Decrypt(Convert.ToInt32(textBoxSecretDX.Text), Convert.ToInt32(textBoxSecretNP.Text), textBoxMsg.Text);
                }
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

    static class ELGAMALEncryptDecrypt
    {
        //--------------------------------
        //   Константы
        //--------------------------------
        public static readonly string alfavit = "||абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";

        //--------------------------------
        //   Лог
        //--------------------------------
        public static TextBox Log;

        //--------------------------------
        //   Генерация простых чисел
        //--------------------------------
        private static List<int> ReshetoEratosfena(out int P)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Генерация простых чисел" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);

            var IntArr = new List<int>();
            for (var i = 2; i <= 70; i++)
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
                if (IntArr[i] < 40)
                    IntArr.Remove(IntArr[i]);
                else
                    break;
            Log.AppendText("Всего простых от 40 до 70: " + Convert.ToString(IntArr.Count) + Environment.NewLine);

            P = IntArr[new Random().Next(0, IntArr.Count - 1)];

            Log.AppendText("Случайное число P из диапазона: " + Convert.ToString(P) + Environment.NewLine);

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
        public static void GenerateKeys(out int X, out int P, out long G, out long Y)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Генерация ключа" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);

            P = 0;
            ELGAMALEncryptDecrypt.ReshetoEratosfena(out P);

            var GTmp = new BigInteger(new Random().Next(10, P-1));
            var M = P - 1;
            Log.AppendText("M = " + Convert.ToString(M) + Environment.NewLine);
            while (true)
            {
                var Exit = true;
                GTmp++;
                if (BigInteger.ModPow(GTmp, M, P) == 1)
                    for (var i = 1; i < P - 1; i++)
                        if (BigInteger.ModPow(GTmp, i, P) == 1)
                            Exit = false;
                if (Exit)
                    break;
            }
            Log.AppendText("G = " + Convert.ToString(GTmp) + Environment.NewLine);
            G = Convert.ToInt64(Convert.ToString(GTmp));

            X = new Random().Next(10, P - 1);
            Log.AppendText("X = " + Convert.ToString(X) + Environment.NewLine);

            var YTmp = BigInteger.ModPow(G, X, P);
            Log.AppendText("Y = " + Convert.ToString(YTmp) + Environment.NewLine);
            Y = Convert.ToInt64(Convert.ToString(YTmp));
        }

        //--------------------------------
        //   Шифрование
        //--------------------------------
        public static string Encrypt(int P, long G, long Y, string msg)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Шифрование" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Исходная строка: " + msg + Environment.NewLine);

            var ResString = "";

            for (var i = 0; i < msg.Length; i++)
            {
                var K = new Random().Next(10, P-1);
                var A = BigInteger.ModPow(G, K, P);

                var B = (BigInteger.Pow(Y, K) * alfavit.IndexOf(msg[i])) % P;

                if (i != 0)
                    ResString += " ";
                ResString += A.ToString() + " " + B.ToString();
            }

            Log.AppendText("Ответ: " + ResString + Environment.NewLine);
            return ResString;
        }

        //--------------------------------
        //   Расшифровка
        //--------------------------------
        public static string Decrypt(int X, int P, string msg)
        {
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Расшифровка" + Environment.NewLine);
            Log.AppendText("--------------------------" + Environment.NewLine);
            Log.AppendText("Исходная строка: " + msg + Environment.NewLine);

            var msgsplit = msg.Split(' ');

            var ResString = "";
            for (var i = 0; i < msgsplit.Length; i+=2)
            {
                var Tmp1 = BigInteger.Pow(Convert.ToInt64(msgsplit[i]), X);
                var Tmp = new BigInteger(Convert.ToInt64(msgsplit[i + 1]));
                var Tmp2 = (Tmp % P) / (Tmp1 % P);
                ResString += alfavit[Convert.ToInt32(Tmp2.ToString())];
            }

            Log.AppendText("Ответ: " + ResString + Environment.NewLine);
            return ResString;
        }
    }
}
