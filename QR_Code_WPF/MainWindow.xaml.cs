using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using QRCoder;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;

namespace QR_Code_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateQRCode_Click(object sender, RoutedEventArgs e)
        {
            string dataToSign = inputTextBox.Text;
            string privateKeyPath = "D:\\VISUAL_STUDIO_\\Vs_Project\\C#\\OTHER_WORK\\QR_Code_WPF_N\\QR_Code_WPF\\KeyFolder\\privateKey.xml";

            if (!File.Exists(privateKeyPath))
            {
                MessageBox.Show("Приватный ключ не найден!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(File.ReadAllText(privateKeyPath));

                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
                byte[] signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string signatureString = Convert.ToBase64String(signature);

                string qrData = dataToSign + ";" + signatureString;

                // Получаем выбранный ECC уровень
                string selectedEccLevel = ecclevelbox.SelectedValue.ToString();

                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                // Используем выбранный ECC уровень
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, GetEccLevel(selectedEccLevel));
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                qrCodeImage.Save("QRCodeWithSignature.png");

                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Png);
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                qrCodeImageElement.Source = imageSource;
            }
        }

        // Вспомогательная функция для получения ECC уровня
        private QRCodeGenerator.ECCLevel GetEccLevel(string selectedEccLevel)
        {
            switch (selectedEccLevel)
            {
                case "L":
                    return QRCodeGenerator.ECCLevel.L;
                case "M":
                    return QRCodeGenerator.ECCLevel.M;
                case "Q":
                    return QRCodeGenerator.ECCLevel.Q;
                case "H":
                    return QRCodeGenerator.ECCLevel.H;
                default:
                    return QRCodeGenerator.ECCLevel.L; // По умолчанию используется L
            }
        }

        private void OpenQR_Scan_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                Bitmap qrCodeBitmap = new Bitmap(imagePath);
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode(qrCodeBitmap);

                if (result != null)
                {
                    string qrData = result.Text;
                    string[] parts = qrData.Split(';');
                    string data = parts[0];
                    string signature = parts[1];

                    if (VerifySignature(data, signature))
                    {
                        // Открываем URL в браузере
                        Process.Start(data);
                    }
                    else
                    {
                        MessageBox.Show("Недействительная подпись!", "Ошибка", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("QR-код не распознан!", "Ошибка", MessageBoxButton.OK);
                }
            }
        }

        private bool VerifySignature(string data, string signature)
        {
            string publicKeyPath = "D:/VISUAL_STUDIO_/Vs_Project/C#/OTHER_WORK/QR_Code_WPF_N/QR_Code_WPF/KeyFolder/publicKey.xml";

            if (!File.Exists(publicKeyPath))
            {
                MessageBox.Show("Публичный ключ не найден!", "Ошибка", MessageBoxButton.OK);
                return false;
            }

            using (RSA rsaPublic = RSA.Create())
            {
                rsaPublic.FromXmlString(File.ReadAllText(publicKeyPath));
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] signatureBytes = Convert.FromBase64String(signature);
                return rsaPublic.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        private void Video_Scan_Click(object sender, RoutedEventArgs e)
        {
            // Реализуйте сканирование QR-кода с камеры здесь
        }

        private void ChangeQRColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            // Открываем диалоговое окно выбора цвета
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedColor = colorDialog.Color;

                // Преобразуйте System.Drawing.Color в System.Windows.Media.Color
                System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);

                // Создаем новый QR-код с выбранным цветом
                CreateQRCodeWithColor(inputTextBox.Text, mediaColor);
            }
        }

        private void CreateQRCodeWithColor(string dataToSign, System.Windows.Media.Color color)
        {
            string privateKeyPath = "D:\\VISUAL_STUDIO_\\Vs_Project\\C#\\OTHER_WORK\\QR_Code_WPF_N\\QR_Code_WPF\\KeyFolder\\privateKey.xml";

            if (!File.Exists(privateKeyPath))
            {
                MessageBox.Show("Приватный ключ не найден!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(File.ReadAllText(privateKeyPath));

                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
                byte[] signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string signatureString = Convert.ToBase64String(signature);

                string qrData = dataToSign + ";" + signatureString;

                // Получаем выбранный ECC уровень
                string selectedEccLevel = ecclevelbox.SelectedValue.ToString();

                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                // Используем выбранный ECC уровень
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, GetEccLevel(selectedEccLevel));
                QRCode qrCode = new QRCode(qrCodeData);

                // Преобразуем System.Media.Color в System.Drawing.Color
                System.Drawing.Color qrColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

                Bitmap qrCodeImage = qrCode.GetGraphic(20, qrColor, System.Drawing.Color.White, true);

                qrCodeImage.Save("QRCodeWithSignature.png");

                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Png);
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                qrCodeImageElement.Source = imageSource;
            }
        }

    }
}
