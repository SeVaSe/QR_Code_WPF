using QR_Code_WPF.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QR_Code_WPF.Classes
{
    public class DatabaseManager
    {
        private QRdbEntities dbContext;

        public DatabaseManager(QRdbEntities dbContext)
        {
            // Конструктор класса DatabaseManager принимает контекст базы данных.
            this.dbContext = dbContext;
        }

        public void InsertQRType(string typeName)
        {
            // Метод для вставки новой записи в таблицу QRType.
            // Принимает имя типа (например, "обычные" или "цветные").
            dbContext.QRType.Add(new QRType { TypeName = typeName });
            // Создаем новый экземпляр QRType, устанавливаем свойство TypeName и добавляем его в контекст.
            dbContext.SaveChanges();
            // Сохраняем изменения в базе данных, что приводит к вставке новой записи.
        }

        public void InsertErrorCorrectionLevel(string levelName)
        {
            // Метод для вставки новой записи в таблицу ErrorCorrectionLevel.
            // Принимает имя уровня коррекции ошибок (например, "L", "M", "Q" или "H").
            dbContext.ErrorCorrectionLevel.Add(new ErrorCorrectionLevel { LevelName = levelName });
            // Создаем новый экземпляр ErrorCorrectionLevel, устанавливаем свойство LevelName и добавляем его в контекст.
            dbContext.SaveChanges();
            // Сохраняем изменения в базе данных, что приводит к вставке новой записи.
        }

        public void InsertQRCode(int qrTypeID, bool hasSignature, string qrCodeName, DateTime creationDate, string photoPath, int errorCorrectionLevelID)
        {
            // Метод для вставки новой записи в таблицу QRCode.
            // Принимает идентификатор типа QR, наличие электронной подписи, имя QR, дату создания, путь к фото и идентификатор уровня коррекции ошибок.
            var qrCode = new QRCode
            {
                QRTypeID = qrTypeID,
                HasSignature = hasSignature,
                QRCodeName = qrCodeName,
                CreationDate = creationDate,
                PhotoPath = photoPath,
                ErrorCorrectionLevelID = errorCorrectionLevelID
            };
            // Создаем новый экземпляр QRCode, устанавливаем все свойства и добавляем его в контекст.
            dbContext.QRCode.Add(qrCode);
            dbContext.SaveChanges();
            // Сохраняем изменения в базе данных, что приводит к вставке новой записи.
        }




        public class RelayCommand : ICommand
        {
            // Делегат для выполнения команды
            private readonly Action<object> _execute;

            // Делегат для проверки, может ли команда быть выполнена
            private readonly Func<object, bool> _canExecute;

            // Конструктор, принимающий делегат для выполнения команды и делегат для проверки возможности выполнения команды
            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            // Метод для проверки, может ли команда быть выполнена
            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

            // Метод для выполнения команды
            public void Execute(object parameter) => _execute(parameter);

            // Событие, уведомляющее систему о том, что результат вызова метода CanExecute мог измениться,
            // и следует переоценить, может ли команда быть выполнена
            public event EventHandler CanExecuteChanged
            {
                // Добавление обработчика события к CommandManager.RequerySuggested
                add { CommandManager.RequerySuggested += value; }

                // Удаление обработчика события из CommandManager.RequerySuggested
                remove { CommandManager.RequerySuggested -= value; }
            }
        }


    }

}
