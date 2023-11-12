using QR_Code_WPF.DataBase;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZXing;
using static QR_Code_WPF.Classes.DatabaseManager;

namespace QR_Code_WPF.WindowPage
{
    /// <summary>
    /// Логика взаимодействия для ListBDQr_Window.xaml
    /// </summary>
    public partial class ListBDQr_Window : Window
    {

        public ListBDQr_Window()
        {
            InitializeComponent();
            LoadQRData();
            DataContext = this;

            ScanCommand = new RelayCommand(ScanExecute, CanScanExecute);

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
            Window QRWind = new ListBDQr_Window();
            this.Close();
            QRWind.Show();
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



        // открыть окно по созданию QR
        private void CreateQr_Click(object sender, RoutedEventArgs e)
        {
            Window main = new MainWindow();
            main.Show();
            this.Close();
        }

        // открыть окно по списку QR
        private void SpisoDBQR_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в данном разделе", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }





        //модель базы
        public class QRViewModel
        {
            public int QRCodeID { get; set; }
            public string QRName { get; set; }
            public string QRType { get; set; }
            public string ECCLevel { get; set; }
            public DateTime CreationDate { get; set; }
            public string HasSignature { get; set; }
            public string ImagePath { get; set; }
        }

        // методы данных из БД (в виде объектов QRViewModel) будут отображены в пользовательском интерфейсе ScrollVieverQRDB 
        private void LoadQRData()
        {
            using (var dbContext = new QRdbEntities())
            {
                var qrViewModels = dbContext.QRCode
                    .Select(q => new QRViewModel
                    {
                        QRCodeID = q.QRCodeID,
                        QRName = q.QRCodeName,
                        QRType = q.QRType.TypeName,
                        ECCLevel = q.ErrorCorrectionLevel.LevelName,
                        CreationDate = q.CreationDate,
                        HasSignature = q.HasSignature ? "Да" : "Нет",
                        ImagePath = q.PhotoPath
                    })
                    .ToList();

                ScrollVieverQRDB.DataContext = qrViewModels;
            }
        }


        // КОМАНДА ДЛЯ УДАЛЕНИЯ QR
        private RelayCommand _deleteCommand;

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        parameter => DeleteCommandExecute(parameter),
                        parameter => CanDeleteCommandExecute(parameter)
                    );
                }
                return _deleteCommand;
            }
        }

        // Метод, определяющий, может ли команда быть выполнена
        private bool CanDeleteCommandExecute(object parameter)
        {
            return true; 
        }

        // Метод, выполняющий действие при выполнении команды удаления
        private void DeleteCommandExecute(object parameter)
        {
            if (parameter is QRViewModel qrViewModel)
            {
                DeleteQRCode(qrViewModel.QRCodeID);
                LoadQRData(); // Обновляем данные после удаления
                MessageBox.Show($"Удален QR - {qrViewModel.QRName}", "Удаление QR", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Метод для удаления QR-кода из базы данных
        public void DeleteQRCode(int qrCodeId)
        {
            using (var dbContext = new QRdbEntities())
            {
                var qrCode = dbContext.QRCode.FirstOrDefault(q => q.QRCodeID == qrCodeId);
                if (qrCode != null)
                {
                    dbContext.QRCode.Remove(qrCode);
                    dbContext.SaveChanges();
                }
            }
        }


        // СКАНИРОВАНИЕ QR
        bool wrongVerify = false;

        // Метод для проверки подписи QR-кода
        private bool VerifySignature(string data, string signature)
        {
            string publicKeyPath = System.IO.Path.Combine(Environment.CurrentDirectory, "KeyFolder\\publicKey.xml");

            if (!File.Exists(publicKeyPath))
            {
                wrongVerify = true;
                MessageBox.Show("Публичный ключ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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


        // КОМАНДА ДЛЯ СКАНИРОВАНИЯ QR
        public ICommand ScanCommand { get; }

        // Обработчик события нажатия на Border (визуальный элемент, представляющий QR-код)
        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var qrCodeViewModel = (QRViewModel)border.DataContext;
            ScanExecute(qrCodeViewModel.ImagePath);
        }

        // Метод для выполнения сканирования QR-кода
        private void ScanExecute(object parameter)
        {
            if (parameter is string imagePath)
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    MessageBox.Show("Путь к изображению не указан!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Сканирование QR-кода на изображении: {imagePath}", "Идет сканирование...", MessageBoxButton.OK, MessageBoxImage.Information);

                try
                {
                    Bitmap qrCodeBitmap = new Bitmap(imagePath);
                    BarcodeReader reader = new BarcodeReader();
                    Result result = reader.Decode(qrCodeBitmap);

                    if (result != null)
                    {
                        string qrData = result.Text;

                        string[] parts = qrData.Split(';');
                        string data = parts[0];

                        if (parts.Length > 1)
                        {
                            string signature = parts[1];
                            if (VerifySignature(data, signature))
                            {
                                Process.Start(data);
                                MessageBox.Show($"QR-код успешно обработан. Будет открыта ссылка: {data}", "Успешный скан", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Проверьте свое подключение к интернету!", "Ошибка с подключением", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            Process.Start(data);
                            MessageBox.Show($"QR-код успешно обработан. Будет открыта ссылка: {data}", "Успешный скан", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("QR-код не распознан!", "Ошибка сканирования", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Исключение при обработке QR-кода: {ex.Message}", "Не предвиденное исключение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Метод, определяющий, может ли быть выполнена команда сканирования
        private bool CanScanExecute(object parameter)
        {
            return true;
        }









    }
}
