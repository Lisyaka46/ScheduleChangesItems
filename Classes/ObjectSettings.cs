using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleChangesItems.Classes
{
    /// <summary>
    /// Класс настроек программы
    /// </summary>
    public class ObjectSettings
    {
        /// <summary>
        /// Видимость состояния позиций
        /// </summary>
        public bool VisiblyMax_and_Min;

        /// <summary>
        /// Инициализировать объект настроек с настройками по умолчанию
        /// </summary>
        public ObjectSettings()
        {
            VisiblyMax_and_Min = true;
        }
    }
}
