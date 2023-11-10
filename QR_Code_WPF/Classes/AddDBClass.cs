using QR_Code_WPF.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
