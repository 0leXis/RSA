using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using System.IO;

namespace RSA_digital_signature
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                var FS = File.OpenRead(openFileDialog1.FileName);
                RSADigitalSignature.Document = new byte[FS.Length];
                FS.Read(RSADigitalSignature.Document, 0, RSADigitalSignature.Document.Length);
                FS.Close();
            }
        }

        private void buttonHesh_Click(object sender, EventArgs e)
        {
            if (RSADigitalSignature.Document == null)
                MessageBox.Show("Документ не загружен!");
            else
            {
                int N, E;
                long D;
                textBoxHesh.Text = RSADigitalSignature.GetDigitalSignature(out N, out E, out D);
                textBoxOpenD.Text = Convert.ToString(D);
                textBoxSecretE.Text = Convert.ToString(E);
                textBoxOpenN.Text = Convert.ToString(N);
                textBoxSecretN.Text = Convert.ToString(N);
            }
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (textBoxOpenD.Text == "" || textBoxOpenN.Text == "")
                MessageBox.Show("Открытый ключ не введён!");
            else
                if (textBoxHesh.Text == "")
                MessageBox.Show("Хеш-функция не введена!");
            else
            {
                if(RSADigitalSignature.CheckDigitalSignature(Convert.ToInt32(textBoxOpenN.Text), Convert.ToInt64(textBoxOpenD.Text), textBoxHesh.Text))
                {
                    labelDS.ForeColor = Color.Green;
                    labelDS.Text = "Функции совпадают";
                }
                else
                {
                    labelDS.ForeColor = Color.Red;
                    labelDS.Text = "Функции не совпадают";
                }
            }
        }
    }

    static class RSADigitalSignature
    {
        public static byte[] Document;

        private static string GetHesh(int N)
        {

            var H = new BigInteger(0);
            for(long i=0; i<Document.Length; i++)
                H = BigInteger.ModPow(H + Document[i], 2, N);
            return H.ToString();
        }

        public static string GetDigitalSignature(out int N, out int E, out long D)
        {
            RSAEncryptDecrypt.GenerateKeys(out N, out E, out D);
            var Hesh = GetHesh(N);

            return RSAEncryptDecrypt.Encrypt(E, N, Hesh);
        }

        public static bool CheckDigitalSignature(int N, long D, string HeshE)
        {
            var Hesh = RSAEncryptDecrypt.Decrypt(D, N, HeshE);
            var Hesh2 = GetHesh(N);

            if (Hesh == Hesh2)
                return true;
            else
                return false;
        }

    }

    static class RSAEncryptDecrypt
    {
        //--------------------------------
        //   Константы
        //--------------------------------
        public static readonly string alfavit = "||абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ0123456789.,:;!() abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //--------------------------------
        //   Генерация простых чисел
        //--------------------------------
        private static List<int> ReshetoEratosfena(out int P, out int Q)
        {
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

            P = IntArr[new Random().Next(0, IntArr.Count - 1)];
            do
            {
                Q = IntArr[new Random().Next(0, IntArr.Count - 1)];
            }
            while (Q == P);

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

            var P = 0;
            var Q = 0;
            var ProstList = RSAEncryptDecrypt.ReshetoEratosfena(out P, out Q);

            N = P * Q;

            var M = (P - 1) * (Q - 1);

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

            D = 1;
            while (D * E % M != 1)
            {
                D++;
            }
        }

        //--------------------------------
        //   Шифрование
        //--------------------------------
        public static string Encrypt(int E, int N, string msg)
        {

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

            return ResString;
        }

        //--------------------------------
        //   Расшифровка
        //--------------------------------
        public static string Decrypt(long D, int N, string msg)
        {

            var tmpN = new BigInteger(N);
            var tmpD = new BigInteger(D);

            var msgsplit = msg.Split(' ');

            var ResString = "";
            for (var i = 0; i < msgsplit.Length; i++)
            {
                var Rem = BigInteger.ModPow(new BigInteger(Convert.ToInt32(msgsplit[i])), tmpD, tmpN);
                if (Convert.ToInt32(Rem.ToString()) > alfavit.Length)
                    ResString += "0";
                else
                    ResString += alfavit[Convert.ToInt32(Rem.ToString())];
            }

            return ResString;
        }
    }

}
