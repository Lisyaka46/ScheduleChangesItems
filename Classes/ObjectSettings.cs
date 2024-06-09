using System;
using System.CodeDom;
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
        public ObjectParameter<bool> VisiblyMax_and_Min;

        /// <summary>
        /// Количество видимых позиций графика
        /// </summary>
        public ObjectParameter<int> CountVisiblePosGraph;

        /// <summary>
        /// Инициализировать объект настроек с настройками по умолчанию
        /// </summary>
        public ObjectSettings()
        {
            VisiblyMax_and_Min = new ObjectParameter<bool>(true);
            CountVisiblePosGraph = new ObjectParameter<int>(9);
        }
    }

    /// <summary>
    /// Класс параметра настроек
    /// </summary>
    /// <typeparam name="T">Тип который хранится в параметре</typeparam>
    public class ObjectParameter<T>
    {
        /// <summary>
        /// Объект значения параметра
        /// </summary>
        private T Value;

        /// <summary>
        /// Делегат события управления значением параметра
        /// </summary>
        public delegate void EventValue();

        /// <summary>
        /// Событие изменения состояния параметра
        /// </summary>
        /// <remarks>
        /// При создании объекта параметра <b>событие не вызывается!!</b>
        /// </remarks>
        public EventValue ValueChange;

        /// <summary>
        /// Присвоить новое значение параметру
        /// </summary>
        /// <param name="NewValue">Новое значение</param>
        public void SetValue(T NewValue)
        {
            Value = NewValue;
            ValueChange?.Invoke();
        }

        /// <summary>
        /// Узнать текущее значение
        /// </summary>
        /// <returns>Текущее значение</returns>
        public T GetValue() => Value;

        /// <summary>
        /// Инициализировать объект параметра настроек
        /// </summary>
        /// <param name="Value">Значение по умолчанию</param>
        public ObjectParameter(T Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Преобразование параметра в его зачение
        /// </summary>
        /// <param name="Param"></param>
        public static implicit operator T(ObjectParameter<T> Param) => Param.GetValue();
    }
}
