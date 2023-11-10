using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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




        private void CreateQr_Click(object sender, RoutedEventArgs e)
        {
            Window main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void SpisoDBQR_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в данном разделе", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
