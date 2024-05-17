using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleChangesItems.Classes
{
    /// <summary>
    /// Визуализационный объект коллекции
    /// </summary>
    public class VisualizationSeries
    {
        /// <summary>
        /// Цвет обычного состояния позиции
        /// </summary>
        public Color ColorDefault { get; set; }

        /// <summary>
        /// Цвет выделенного состояния тенденции
        /// </summary>
        public Color ColorSelect { get; set; }

        /// <summary>
        /// Инициализация объекта визуализации коллекции
        /// </summary>
        /// <param name="Default">Обычный цвет</param>
        /// <param name="Select">Выделенный цвет</param>
        public VisualizationSeries(Color Default, Color Select)
        {
            ColorDefault = Default;
            ColorSelect = Select;
        }
    }
}
