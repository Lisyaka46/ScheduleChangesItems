using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ScheduleChangesItems.Classes.TxtPoint
{
    /// <summary>
    /// Тег файла .txtpoint
    /// </summary>
    internal struct Tag
    {
        /// <summary>
        /// Имя тега
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Значение тега
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Инициализация тега без значения
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <returns>Структура тега без значения</returns>
        public static Tag Taging(string Name)
        {
            Tag result = default;
            result.Name = Name;
            result.Value = string.Empty;
            return result;
        }

        /// <summary>
        /// Инициализация тега с статичным значением
        /// </summary>
        /// <param name="Name">Имя тега</param>
        /// <param name="Value">Значение тега</param>
        /// <returns>Структура тега с значением</returns>
        public static Tag Taging(string Name, object Value)
        {
            Tag result = default;
            result.Name = Name;
            result.Value = Value.ToString();
            return result;
        }

        public sealed class TagNaming
        {
            /// <summary>
            /// Тег инициализации коллекции
            /// </summary>
            public const string TagSeriesInit = "Series_Init";

            /// <summary>
            /// Тег имени коллекции
            /// </summary>
            public const string TagName = "Name";

            /// <summary>
            /// Тег создания позиции тенденции
            /// </summary>
            public const string TagPointTrend = "Point";

            /// <summary>
            /// Тег имени позиции тенденции
            /// </summary>
            public const string TagPointNameTrend = "Point_Name";

            /// <summary>
            /// Тег цвета коллекции
            /// </summary>
            public const string TagHex = "Hex";

            /// <summary>
            /// Тег выделенного цвета коллекции
            /// </summary>
            public const string TagSelectHex = "Select_Hex";

            /// <summary>
            /// Тег деинсталяции коллекции
            /// </summary>
            public const string TagStyleChart = "Style";

            /// <summary>
            /// Тег деинсталяции коллекции
            /// </summary>
            public const string TagUninstallCollection = "~";
        }
    }
}
