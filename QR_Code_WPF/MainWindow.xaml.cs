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
using System.Windows.Markup;
using QR_Code_WPF.WindowPage;
using QR_Code_WPF.DataBase;
using QR_Code_WPF.Classes;

namespace QR_Code_WPF
{
    public partial class MainWindow : Window
    {
        private System.Windows.Media.Color selectedColor;

        public MainWindow()
        {
            InitializeComponent();
        }

        
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
            MessageBox.Show("Приветствуем вас в приложении QR-Code! Здесь вы сможете создавать разнообразные типы QR-кодов. Мы поддерживаем все - от обычных до более надежных с защитой ECC (Error Correction Capability) и даже возможностью добавления цвета.\r\n\r\nЭто приложение также предоставляет возможность создания QR-кодов с особым ключом - электронной подписью. Это означает, что созданные QR-коды можно считывать только с помощью этого приложения, обеспечивая дополнительный уровень безопасности и конфиденциальности", "Информация QR-Code", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Help_U_ECC_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Когда говорят о \"уровне коррекции ошибок\" (Error Correction Level, ECC) в QR-кодах (от \"L\" до \"H\"), это означает, что QR-код способен восстановить часть информации, даже если часть QR-кода повреждена или искажена.\r\n\r\nУровни коррекции ошибок в QR-кодах представлены четырьмя уровнями: L (низкий), M (средний), Q (высокий) и H (очень высокий). Каждый уровень представляет разную степень коррекции ошибок:\r\n\r\n- Уровень L обладает наименьшей степенью коррекции ошибок, тогда как уровень H имеет самую высокую степень коррекции.\r\n- Уровень H, например, может восстановить большую часть информации, даже если значительная часть QR-кода повреждена, в то время как уровень L может справиться только с небольшими повреждениями.\r\n\r\nПомимо уровней L и H, в QR-кодах также существуют уровни M и Q. Уровень M обеспечивает среднюю степень коррекции ошибок между L и H. Уровень Q является высоким уровнем коррекции, позволяющим исправить больше ошибок, чем L, но меньше, чем H, обеспечивая точное считывание данных при сканировании", "Информация о защите ECC", MessageBoxButton.OK, MessageBoxImage.Information);

        }


        
        // открыть окно - создание QR
        private void CreateQr_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в данном разделе", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // открыть окно - список QR
        private void SpisoDBQR_Click(object sender, RoutedEventArgs e)
        {
            ListBDQr_Window QrWindow = new ListBDQr_Window();
            QrWindow.Show();
            this.Close();
        }



        private bool wrongVerify = false;

        //МЕТОД ДЛЯ РАСШИФРОКИ ПОДПИСИ
        private bool VerifySignature(string data, string signature)
        {
            string publicKeyPath = Path.Combine(Environment.CurrentDirectory, "KeyFolder\\publicKey.xml"); // должен находится, где и исполняемый файл 

            if (!File.Exists(publicKeyPath))
            {
                wrongVerify = true;
                return false;
            }

            using (RSA rsaPublic = RSA.Create())
            {
                rsaPublic.FromXmlString(File.ReadAllText(publicKeyPath));
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] signatureBytes = Convert.FromBase64String(signature);
                return rsaPublic.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }

            if (wrongVerify)
            {
                MessageBox.Show("Публичный ключ не найден!", "Ошибка", MessageBoxButton.OK);
            }
        }

        string rrr = "*";
        //СКАНИРОВАНИЕ QR
        private void BorderQRScan_Click(object sender, MouseButtonEventArgs e)
        {
            string imagePath = string.Empty;
            Bitmap qrCodeBitmap = null;
            BarcodeReader reader = new BarcodeReader();
            Result result = null;
            string data = string.Empty;
            string rrr = string.Empty;
            bool wrong1 = false;
            bool wrong2 = false;
            int count = 0;

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                qrCodeBitmap = new Bitmap(imagePath);
                

                for (int i = 0; i < 30; i++) //100
                {
                    result = reader.Decode(qrCodeBitmap);

                    if (result != null)
                    {
                        string qrData = result.Text;
                        string[] parts = qrData.Split(';');
                        data = parts[0];
                        rrr = data;

                        try
                        {
                            if (parts.Length > 1)
                            {
                                string signature = parts[1];
                                if (VerifySignature(data, signature))
                                {
                                    if (count == 0)
                                    {
                                        MessageBox.Show(data);
                                    }
                                    count++;
                                    Process.Start(data);
                                    
                                    
                                    break; // Если подпись верна, завершаем цикл
                                }
                                else
                                {
                                    wrong1 = true;
                                    
                                }
                            }
                            else
                            {
                                if (count == 0)
                                {
                                    MessageBox.Show(data);
                                }
                                count++;
                                Process.Start(data);

                               
                                break; // Если подпись отсутствует, завершаем цикл
                            }
                        }
                        catch
                        {
                            wrong2 = true;
                        }
                    }
                }

                count = 0;


                // ВЫВОД ОШИБОК ПРИ СКАНИРОВАНИИ
                if (result == null)
                {
                    MessageBox.Show("QR-код не распознан!", "Ошибка", MessageBoxButton.OK);
                }
                
                if (wrong1)
                {
                    MessageBox.Show(data);
                    MessageBox.Show("Недействительная подпись!1 ИНТЕРНЕТ", "Ошибка", MessageBoxButton.OK);
                }
                else if (wrong2)
                {
                    MessageBox.Show(data);
                    MessageBox.Show("Недействительная подпись!2 СИСТЕМНАЯ ОШИБКА КЛЮЧА", "Ошибка", MessageBoxButton.OK);
                }
                
            }
        }




        //СОЗДАНИЕ QR-ОБЫКНОВЕННЫЙ
        private void OrdinaryQR_Click(object sender, RoutedEventArgs e)
        {
            string dataToSign = TxtBox_Link_Border1.Text;
            string privateKeyPath = Path.Combine(Environment.CurrentDirectory, "KeyFolder\\privateKey.xml"); // должен находится, где и исполняемый файл 
            bool isChecked = ChckBox_KEY1.IsChecked ?? false;

            MessageBox.Show(privateKeyPath);

            if (TxtBox_Link_Border1.Text != "" && TxtBox_SaveName_Border1.Text != "")
            {
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
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.L);
                        QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);

                        Bitmap qrCodeImage = qrCode.GetGraphic(20);

                        // Сохранение QR-кода
                        string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                        if (!Directory.Exists(saveFolderPath))
                        {
                            Directory.CreateDirectory(saveFolderPath);
                        }
                        string filePath = Path.Combine(saveFolderPath, $"QROrdinaryK_{TxtBox_SaveName_Border1.Text}.png");
                        qrCodeImage.Save(filePath);

                        MemoryStream ms = new MemoryStream();
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        BitmapImage imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        ms.Seek(0, SeekOrigin.Begin);
                        imageSource.StreamSource = ms;
                        imageSource.EndInit();

                        qrCodeImageElement.Source = imageSource;



                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(1, true, $"QROrdinaryK_{TxtBox_SaveName_Border1.Text}", DateTime.Now, filePath, 1);
                        MessageBox.Show($"Был создан QR с подписью - QROrdinaryK_{TxtBox_SaveName_Border1.Text}");
                    }
                }
                else
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.L);
                    QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);

                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    // Сохранение QR-кода
                    string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                    if (!Directory.Exists(saveFolderPath))
                    {
                        Directory.CreateDirectory(saveFolderPath);
                    }
                    string filePath = Path.Combine(saveFolderPath, $"QROrdinaryNK_{TxtBox_SaveName_Border1.Text}.png");
                    qrCodeImage.Save(filePath);

                    MemoryStream ms = new MemoryStream();
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    imageSource.StreamSource = ms;
                    imageSource.EndInit();

                    qrCodeImageElement.Source = imageSource;



                    QRdbEntities db = new QRdbEntities();
                    DatabaseManager dbManager = new DatabaseManager(db);
                    dbManager.InsertQRCode(1, false, $"QROrdinaryNK_{TxtBox_SaveName_Border1.Text}", DateTime.Now, filePath, 1);
                    MessageBox.Show($"Был создан QR без подписи - QROrdinaryNK_{TxtBox_SaveName_Border1.Text}");
                }

                TxtBox_Link_Border1.Clear();
                TxtBox_SaveName_Border1.Clear();
            }
            else
            {
                MessageBox.Show("Заполните все нужные поля, для построения QR-Code!", "Ошибка! Не все поля заполнены", MessageBoxButton.OK, MessageBoxImage.Error);
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
            string dataToSign = TxtBox_Link_Border2.Text;
            string privateKeyPath = Path.Combine(Environment.CurrentDirectory, "KeyFolder\\privateKey.xml"); // должен находится, где и исполняемый файл 
            bool isChecked = ChckBox_KEY2.IsChecked ?? false;

            if (TxtBox_Link_Border2.Text != "" && TxtBox_SaveName_Border2.Text != "")
            {
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

                        // Используем выбранный ECC уровень
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.L);
                        QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);

                        // Преобразуем System.Media.Color в System.Drawing.Color
                        System.Drawing.Color qrColor = System.Drawing.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);

                        Bitmap qrCodeImage = qrCode.GetGraphic(20, qrColor, System.Drawing.Color.White, true);

                        string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                        if (!Directory.Exists(saveFolderPath))
                        {
                            Directory.CreateDirectory(saveFolderPath);
                        }
                        string filePath = Path.Combine(saveFolderPath, $"QRColorK_{TxtBox_SaveName_Border2.Text}.png");
                        qrCodeImage.Save(filePath);

                        MemoryStream ms = new MemoryStream();
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        var imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        ms.Seek(0, SeekOrigin.Begin);
                        imageSource.StreamSource = ms;
                        imageSource.EndInit();

                        qrCodeImageElement.Source = imageSource;



                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(2, true, $"QRColorK_{TxtBox_SaveName_Border2.Text}", DateTime.Now, filePath, 1);

                        MessageBox.Show($"Был создан QR с подписью - QRColorK_{TxtBox_SaveName_Border2.Text}");
                    }
                }
                else
                {
                    string qrData = dataToSign;

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();

                    // Используем выбранный ECC уровень
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.L);
                    QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);

                    // Преобразуем System.Media.Color в System.Drawing.Color
                    System.Drawing.Color qrColor = System.Drawing.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);

                    Bitmap qrCodeImage = qrCode.GetGraphic(20, qrColor, System.Drawing.Color.White, true);

                    string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                    if (!Directory.Exists(saveFolderPath))
                    {
                        Directory.CreateDirectory(saveFolderPath);
                    }
                    string filePath = Path.Combine(saveFolderPath, $"QRColorNK_{TxtBox_SaveName_Border2.Text}.png");
                    qrCodeImage.Save(filePath);

                    MemoryStream ms = new MemoryStream();
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    imageSource.StreamSource = ms;
                    imageSource.EndInit();

                    qrCodeImageElement.Source = imageSource;



                    QRdbEntities db = new QRdbEntities();
                    DatabaseManager dbManager = new DatabaseManager(db);
                    dbManager.InsertQRCode(2, false, $"QRColorNK_{TxtBox_SaveName_Border2.Text}", DateTime.Now, filePath, 1);

                    MessageBox.Show($"Был создан QR без подписи - QRColorNK_{TxtBox_SaveName_Border2.Text}");
                }
                selectedColor = System.Windows.Media.Colors.Black; // Инициализация выбранного цвета черным цветом или любым другим значением по умолчанию

                TxtBox_Link_Border2.Clear();
                TxtBox_SaveName_Border2.Clear();
            }
            else
            {
                MessageBox.Show("Заполните все нужные поля, для построения QR-Code!", "Ошибка! Не все поля заполнены", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }



        // СОЗДАНИЕ QR C ECC НАДЕЖНОСТЬЮ
        private void CreateQRECC_Click(object sender, RoutedEventArgs e)
        {
            string dataToSign = TxtBox_Link_Border3.Text;
            string privateKeyPath = Path.Combine(Environment.CurrentDirectory, "KeyFolder\\privateKey.xml"); // должен находится, где и исполняемый файл 
            bool isChecked = ChckBox_KEY3.IsChecked ?? false;
            int countECC = 0;


            if (TxtBox_Link_Border3.Text != "" && TxtBox_SaveName_Border3.Text != "")
            {
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
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.L);

                        switch ((int)Slider_BorderECC.Value)
                        {
                            case 0:
                                countECC = 1;
                                qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.L);
                                break;
                            case 1:
                                countECC = 2;
                                qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.M);
                                break;
                            case 2:
                                countECC = 3;
                                qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                                break;
                            case 3:
                                countECC = 4;
                                qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.H);
                                break;
                        }
                        QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);

                        Bitmap qrCodeImage = qrCode.GetGraphic(20);

                        // Сохранение QR-кода
                        string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                        if (!Directory.Exists(saveFolderPath))
                        {
                            Directory.CreateDirectory(saveFolderPath);
                        }
                        string filePath = Path.Combine(saveFolderPath, $"QREccK_{TxtBox_SaveName_Border3.Text}.png");
                        qrCodeImage.Save(filePath);

                        MemoryStream ms = new MemoryStream();
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        BitmapImage imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        ms.Seek(0, SeekOrigin.Begin);
                        imageSource.StreamSource = ms;
                        imageSource.EndInit();

                        qrCodeImageElement.Source = imageSource;


                        if (countECC == 1)
                        {
                            QRdbEntities db = new QRdbEntities();
                            DatabaseManager dbManager = new DatabaseManager(db);
                            dbManager.InsertQRCode(3, true, $"QREccK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 1);
                        }
                        else if (countECC == 2) 
                        {
                            QRdbEntities db = new QRdbEntities();
                            DatabaseManager dbManager = new DatabaseManager(db);
                            dbManager.InsertQRCode(3, true, $"QREccK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 2);
                        }
                        else if (countECC == 3)
                        {
                            QRdbEntities db = new QRdbEntities();
                            DatabaseManager dbManager = new DatabaseManager(db);
                            dbManager.InsertQRCode(3, true, $"QREccK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 3);
                        }
                        else if (countECC == 4)
                        {
                            QRdbEntities db = new QRdbEntities();
                            DatabaseManager dbManager = new DatabaseManager(db);
                            dbManager.InsertQRCode(3, true, $"QREccK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 4);
                        }
                        else
                        {
                            QRdbEntities db = new QRdbEntities();
                            DatabaseManager dbManager = new DatabaseManager(db);
                            dbManager.InsertQRCode(3, true, $"QREccK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 1);
                        }

                        MessageBox.Show($"Был создан QR с подписью - QREccK_{TxtBox_SaveName_Border3.Text}");
                    }
                }
                else
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.L);

                    switch ((int)Slider_BorderECC.Value)
                    {
                        case 0:
                            countECC = 1;
                            qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.L);
                            break;
                        case 1:
                            countECC = 2;
                            qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.M);
                            break;
                        case 2:
                            countECC = 3;
                            qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.Q);
                            break;
                        case 3:
                            countECC = 4;
                            qrCodeData = qrGenerator.CreateQrCode(dataToSign, QRCodeGenerator.ECCLevel.H);
                            break;
                    }
                    QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);

                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    // Сохранение QR-кода
                    string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "SaveQR");
                    if (!Directory.Exists(saveFolderPath))
                    {
                        Directory.CreateDirectory(saveFolderPath);
                    }
                    string filePath = Path.Combine(saveFolderPath, $"QREccNK_{TxtBox_SaveName_Border3.Text}.png");
                    qrCodeImage.Save(filePath);

                    MemoryStream ms = new MemoryStream();
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    imageSource.StreamSource = ms;
                    imageSource.EndInit();

                    qrCodeImageElement.Source = imageSource;




                    if (countECC == 1)
                    {
                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(3, false, $"QREccNK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 1);
                    }
                    else if (countECC == 2)
                    {
                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(3, false, $"QREccNK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 2);
                    }
                    else if (countECC == 3)
                    {
                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(3, false, $"QREccNK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 3);
                    }
                    else if (countECC == 4)
                    {
                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(3, false, $"QREccNK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 4);
                    }
                    else
                    {
                        QRdbEntities db = new QRdbEntities();
                        DatabaseManager dbManager = new DatabaseManager(db);
                        dbManager.InsertQRCode(3, false, $"QREccNK_{TxtBox_SaveName_Border3.Text}", DateTime.Now, filePath, 1);
                    }

                    MessageBox.Show($"Был создан QR без подписи - QREccNK_{TxtBox_SaveName_Border3.Text}");
                }

                TxtBox_Link_Border3.Clear();
                TxtBox_SaveName_Border3.Clear();
            }
            else
            {
                MessageBox.Show("Заполните все нужные поля, для построения QR-Code!", "Ошибка! Не все поля заполнены", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
