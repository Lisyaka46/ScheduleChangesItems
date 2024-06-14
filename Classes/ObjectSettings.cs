using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;

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
        public ObjectParameter<bool> VisiblyMax_and_Min { get; private set; }

        /// <summary>
        /// Количество видимых позиций графика
        /// </summary>
        public ObjectParameter<int> CountVisiblePosGraph { get; private set; }

        /// <summary>
        /// Инициализировать объект настроек с настройками по умолчанию
        /// </summary>
        public ObjectSettings()
        {
            VisiblyMax_and_Min = new ObjectParameter<bool>(true);
            CountVisiblePosGraph = new ObjectParameter<int>(9);
        }

        /// <summary>
        /// Прочитать файл настроек
        /// </summary>
        /// <param name="LinesSettingParameters">Строки файла настроек</param>
        public void SetParametersSettingFile(string[] LinesSettingParameters)
        {
            Regex regexName = new Regex(@"\b[^:]+");
            Regex regexValue = new Regex(@" [^\r\n]+");
            string Name, Value;
            List<string> LinesSort = new List<string>();
            for (int i = 0; i < LinesSettingParameters.Length; i++)
            {
                Name = regexName.Match(LinesSettingParameters[i]).Value;
                Value = regexValue.Match(LinesSettingParameters[i]).Value.Trim();
                if (Name.Length > 0 && Value.Length > 0)
                {
                    PropertyInfo PropParameter = GetType().GetProperty(Name);
                    if (PropParameter != null)
                    {
                        Type type = PropParameter.GetValue(this, null).GetType().GetTypeInfo().GenericTypeArguments[0];
                        if (type == typeof(int)) PropParameter.SetValue(this,
                            new ObjectParameter<int>(Value, type));
                        else if (type == typeof(string)) PropParameter.SetValue(this,
                            new ObjectParameter<string>(Value, type));
                        else if (type == typeof(bool)) PropParameter.SetValue(this,
                            new ObjectParameter<bool>(Value.Equals("1"), type));
                        else continue;
                    }
                    else continue;
                }
                else continue;
                LinesSort.Add(LinesSettingParameters[i]);
            }
            File.WriteAllLines("Settings.txt", App.Setting.ConvertSettingToLineStrings(), Encoding.UTF8);
        }

        /// <summary>
        /// Конвертировать настройки в строковые значения тектового файла настроек
        /// </summary>
        /// <returns>Массив строк текста для файла настроек</returns>
        public string[] ConvertSettingToLineStrings()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            string[] MassLines = new string[properties.Length];
            Type type;
            for (int i = 0; i < MassLines.Length; i++)
            {
                type = properties[i].GetValue(this, null).GetType().GetTypeInfo().GenericTypeArguments[0];
                MassLines[i] = $"{properties[i].Name}: ";
                if (type == typeof(int))
                    MassLines[i] += $"{((ObjectParameter<int>)properties[i].GetValue(this)).Value}";
                else if (type == typeof(string))
                    MassLines[i] += $"{((ObjectParameter<string>)properties[i].GetValue(this)).Value}";
                else if (type == typeof(bool))
                    MassLines[i] += $"{(((ObjectParameter<bool>)properties[i].GetValue(this)).Value ? 1 : 0)}";
                else MassLines[i] = string.Empty;
            }
            return MassLines;
        }
    }

    /// <summary>
    /// Класс параметра настроек
    /// </summary>
    /// <typeparam name="T">Тип который хранится в параметре</typeparam>
    public class ObjectParameter<T>
    {
        /// <summary>
        /// Значение параметра
        /// </summary>
        public T Value { get; private set; }

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
        /// Инициализировать объект параметра настроек
        /// </summary>
        /// <param name="Value">Значение по умолчанию</param>
        public ObjectParameter(T Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Инициализировать объект параметра настроек через привидение типов
        /// </summary>
        /// <param name="Value">Значение по умолчанию</param>
        /// <param name="type">Тип к которому приводится object</param>
        public ObjectParameter(object Value, Type type)
        {
            this.Value = (T)Convert.ChangeType(Value, type);
        }

        /// <summary>
        /// Преобразование параметра в его зачение
        /// </summary>
        /// <param name="Param">Параметр настроек</param>
        public static implicit operator T(ObjectParameter<T> Param) => Param.Value;
    }
}
