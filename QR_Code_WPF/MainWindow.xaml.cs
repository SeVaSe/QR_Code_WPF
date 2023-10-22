using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using QRCoder;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;


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
            string dataToEncode = inputTextBox.Text;

            using (RSA rsa = RSA.Create())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncode);
                byte[] signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string signatureString = Convert.ToBase64String(signature);

                string qrData = dataToEncode;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                // Load your image for the center
                Bitmap centerImage = new Bitmap("C:/Users/kotas/Downloads/kip2.png");

                // Calculate the position to center the image on the QR code
                int xPos = (qrCodeImage.Width - centerImage.Width) / 2;
                int yPos = (qrCodeImage.Height - centerImage.Height) / 2;

                // Merge the QR code and the center image
                using (Graphics graphics = Graphics.FromImage(qrCodeImage))
                {
                    graphics.DrawImage(centerImage, new System.Drawing.Point(xPos, yPos));
                }

                qrCodeImage.Save("QRCodeWithSignature.png");

                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                qrCodeImage.Dispose();

                qrCodeImageElement.Source = imageSource;
            }
        }


    }
}


