using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Security.Cryptography;
namespace ASC
{
    public partial class Login : Window
    {
        string server = "127.0.0.1";
        string user = "root";
        string pass = "12345";
        public Login()
        {
            InitializeComponent();
            Aes aes = Aes.Create();
            aes.GenerateIV();
            if (File.Exists("Login"))
            {
                var decoded = MyAes.FromAes256(File.ReadAllBytes("Login"));
                var saved = decoded.Split('\n');
                IP.Text = saved[0];
                login.Text = saved[1];
                Password.Password = saved[2];
            }
        }
        public string GetLoginString()
        {
            ShowDialog();
            return "Server=" + server +
                   ";Database=basa;" +
                   "port=3306;" +
                   "User Id=" + user +
                   ";password=" + pass;
        }
        private void Close_Click(object sender, RoutedEventArgs e) => ExitLogin();

        private void IP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0) && e.Text[0] != '.') e.Handled = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ExitLogin();
        }

        void ExitLogin()
        {
            server = IP.Text;
            user = login.Text;
            pass = Password.Password;
            File.Delete("Login");
            File.WriteAllBytes("Login", MyAes.ToAes256(server + "\n" + user + "\n" + pass));
            Close();
        }
    }

    class MyAes
    {
        static byte[] aeskey = { 0x3F, 0x45, 0x28,0x48, 0x2B, 0x4D, 0x62, 0x51, 0x65, 0x54, 0x68, 0x57, 0x6D, 0x5A, 0x71, 0x33, 0x74, 0x36, 0x77, 0x39, 0x7A, 0x24, 0x43, 0x26, 0x46, 0x29, 0x4A, 0x40, 0x4E, 0x63, 0x52, 0x66 };
        public static byte[] ToAes256(string src)
        {
            //Объявляем объект класса AES
            Aes aes = Aes.Create();
            //Генерируем соль
            aes.GenerateIV();
            //Присваиваем ключ. aeskey - переменная (массив байт), сгенерированная методом GenerateKey() класса AES
            aes.Key = aeskey;
            byte[] encrypted;
            ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(src);
                    }
                }
                //Записываем в переменную encrypted зашиврованный поток байтов
                encrypted = ms.ToArray();
            }
            //Возвращаем поток байт + крепим соль
            return encrypted.Concat(aes.IV).ToArray();
        }

        public static string FromAes256(byte[] shifr)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];
            //Списываем соль
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++) bytesIv[j] = shifr[i];
            //Списываем оставшуюся часть сообщения
            for (int i = 0; i < shifr.Length - 16; i++) mess[i] = shifr[i];
            //Объект класса Aes
            Aes aes = Aes.Create();
            //Задаем тот же ключ, что и для шифрования
            aes.Key = aeskey;
            //Задаем соль
            aes.IV = bytesIv;
            //Строковая переменная для результата
            string text = "";
            byte[] data = mess;
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        //Результат записываем в переменную text в вие исходной строки
                        text = sr.ReadToEnd();
                    }
                }
            }
            return text;
        }
    }
}
