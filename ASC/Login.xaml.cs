using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Security.Cryptography;
namespace ASC
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            if (File.Exists("Login")) {
                var saved = MyAes.FromAes256(File.ReadAllBytes("Login")).Split('\n');
                IP.Text = saved[0];
                login.Text = saved[1];
                Password.Password = saved[2];
            }
        }
        public string GetLoginString()
        {
            ShowDialog();
            return "Server=" + IP.Text + ";Database=basa;port=3306;User Id=" + login.Text + ";password=" + Password.Password;
        }
        private void Close_Click(object sender, RoutedEventArgs e) => ExitLogin();
        private void IP_PreviewTextInput(object sender, TextCompositionEventArgs e) { if (!char.IsDigit(e.Text, 0) && e.Text[0] != '.') e.Handled = true; }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) ExitLogin(); }
        void ExitLogin()
        {
            File.WriteAllBytes("Login", MyAes.ToAes256(IP.Text + "\n" + login.Text + "\n" + Password.Password));
            Close();
        }
    }
    class MyAes
    {
        static byte[] aeskey = { 0x3F, 0x45, 0x28,0x48, 0x2B, 0x4D, 0x62, 0x51, 0x65, 0x54, 0x68, 0x57, 0x6D, 0x5A, 0x71, 0x33, 0x74, 0x36, 0x77, 0x39, 0x7A, 0x24, 0x43, 0x26, 0x46, 0x29, 0x4A, 0x40, 0x4E, 0x63, 0x52, 0x66 };
        public static byte[] ToAes256(string src)
        {
            Aes aes = Aes.Create();
            aes.GenerateIV();//Генерируем соль
            aes.Key = aeskey;//Присваиваем ключ. aeskey - переменная (массив байт)
            byte[] encrypted;
            ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write)) 
                    using (StreamWriter sw = new StreamWriter(cs)) sw.Write(src);
                encrypted = ms.ToArray();
            }
            return encrypted.Concat(aes.IV).ToArray();
        }
        public static string FromAes256(byte[] shifr)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++) bytesIv[j] = shifr[i];//Списываем соль
            for (int i = 0; i < shifr.Length - 16; i++) mess[i] = shifr[i];//Списываем оставшуюся часть сообщения
            Aes aes = Aes.Create();
            aes.Key = aeskey;//Задаем тот же ключ, что и для шифрования
            aes.IV = bytesIv;//Задаем соль
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(mess))
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                        return sr.ReadToEnd();            
        }
    }
}
