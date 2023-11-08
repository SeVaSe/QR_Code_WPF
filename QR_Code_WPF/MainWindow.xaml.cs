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
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Code_WPF
{
    public partial class MainWindow : Window
    {
        private System.Windows.Media.Color selectedColor;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*private void GenerateQRCode_Click(object sender, RoutedEventArgs e)
        {
            string dataToSign = inputTextBox.Text;
            string privateKeyPath = "C:\\VISUAL_STUDIO_\\Vs_Project\\C#\\OTHER_WORK\\QR_Code_WPF\\QR_Code_WPF\\KeyFolder\\privateKey.xml";

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

                //************************ПРИДУМАТЬ СОХРАНЕНИЕ В НУЖНУЮ ПАПКУ*************************
                string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                if (!Directory.Exists(saveFolderPath))
                {
                    Directory.CreateDirectory(saveFolderPath);
                }
                string filePath = Path.Combine(saveFolderPath, $"QR_{saveTextBox.Text}.png");
                qrCodeImage.Save(filePath);
                //*************************************************

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


                    try
                    {
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
                    catch 
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
            string publicKeyPath = "C:\\VISUAL_STUDIO_\\Vs_Project\\C#\\OTHER_WORK\\QR_Code_WPF\\QR_Code_WPF\\KeyFolder\\publicKey.xml";

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



        // распознование через цвет, дает сбои
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
            string privateKeyPath = "C:\\VISUAL_STUDIO_\\Vs_Project\\C#\\OTHER_WORK\\QR_Code_WPF\\QR_Code_WPF\\KeyFolder\\privateKey.xml";

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

                //************************ПРИДУМАТЬ СОХРАНЕНИЕ В НУЖНУЮ ПАПКУ*************************
                string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                if (!Directory.Exists(saveFolderPath))
                {
                    Directory.CreateDirectory(saveFolderPath);
                }
                string filePath = Path.Combine(saveFolderPath, $"QR_{saveTextBox.Text}.png");
                qrCodeImage.Save(filePath);
                //*************************************************

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
*/
        private void Visit_QR_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно с показром всех QR
        }

        //Свернуть
        private void Svernut_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //Закрыть
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //передвижение экрана
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();

        }

        //перезагрузка
        private void Perezapusk_Click(object sender, RoutedEventArgs e)
        {
            Window wind = new MainWindow();
            this.Close();
            wind.Show();
        }


        // ВЬЮВЕРЫ 1 и 2
        //1
        private void ScrollViewer_PreviewMouseWheel1(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            double amountSpeed = 30;

            if (e.Delta > 0)
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset + amountSpeed);
            }
            else
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - amountSpeed);
            }
            e.Handled = true;
        }

        //2
        private void ScrollViewer_PreviewMouseWheel2(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            double amountSpeed = 30;

            if (e.Delta > 0)
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset + amountSpeed);
            }
            else
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - amountSpeed);
            }
            e.Handled = true;
        }


        // помощь
        private void Help_U_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Приветствуем вас в приложении QR-Code! Здесь вы сможете создавать разнообразные типы QR-кодов. Мы поддерживаем все - от обычных до более надежных с защитой ECC (Error Correction Capability) и даже возможностью добавления цвета.\r\n\r\nЭто приложение также предоставляет возможность создания QR-кодов с особым ключом - электронной подписью. Это означает, что созданные QR-коды можно считывать только с помощью этого приложения, обеспечивая дополнительный уровень безопасности и конфиденциальности.", "Информация QR-Code", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        // прокрутка внутри ричбоксов
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement is RichTextBox richTextBox)
            {
                if (e.Key == Key.Up)
                {
                    richTextBox.LineUp();
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    richTextBox.LineDown();
                    e.Handled = true;
                }
            }
        }

        private void CreateQr_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в данном разделе", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SpisoDBQR_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли в СПИСОК-QR");
        }






        //СОЗДАНИЕ QR-ОБЫКНОВЕННЫЙ
        private void OrdinaryQR_Click(object sender, RoutedEventArgs e)
        {
            string dataToSign = TxtBox_Link_Border1.Text;
            string privateKeyPath = "D:\\VISUAL_STUDIO_\\Vs_Project\\C#\\OTHER_WORK\\QR_Code_WPF_N\\QR_Code_WPF\\KeyFolder\\privateKey.xml";
            bool isChecked = ChckBox_KEY.IsChecked ?? false;

            if (!File.Exists(privateKeyPath))
            {
                MessageBox.Show("Приватный ключ не найден!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            if (isChecked)
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(File.ReadAllText(privateKeyPath));

                    byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
                    byte[] signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    string signatureString = Convert.ToBase64String(signature);

                    string qrData = dataToSign + ";" + signatureString;

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);

                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    // Сохранение QR-кода
                    string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                    if (!Directory.Exists(saveFolderPath))
                    {
                        Directory.CreateDirectory(saveFolderPath);
                    }
                    string filePath = Path.Combine(saveFolderPath, $"QROrdinaryP_{TxtBox_SaveName_Border1.Text}.png");
                    qrCodeImage.Save(filePath);

                    MemoryStream ms = new MemoryStream();
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    imageSource.StreamSource = ms;
                    imageSource.EndInit();

                    qrCodeImageElement.Source = imageSource;
                }
            }
            else
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                // Сохранение QR-кода
                string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                if (!Directory.Exists(saveFolderPath))
                {
                    Directory.CreateDirectory(saveFolderPath);
                }
                string filePath = Path.Combine(saveFolderPath, $"QROrdinaryNp_{TxtBox_SaveName_Border1.Text}.png");
                qrCodeImage.Save(filePath);

                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Png);
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                qrCodeImageElement.Source = imageSource;
            }
        }



        // СОЗДАНИЕ QR-с ЦВЕТОМ
        // Метод для отображения окна выбора цвета
        private void ChooseColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно выбора цвета
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedDrawingColor = colorDialog.Color;
                selectedColor = System.Windows.Media.Color.FromArgb(selectedDrawingColor.A, selectedDrawingColor.R, selectedDrawingColor.G, selectedDrawingColor.B);
            }
        }

        private void CreateQRCodeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(selectedColor.ToString());
            selectedColor = System.Windows.Media.Colors.Black; // Инициализация выбранного цвета черным цветом или любым другим значением по умолчанию


        }

        
    }
}
