using ScheduleChangesItems.Classes.TxtPoint;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace ScheduleChangesItems.Classes
{
    /// <summary>
    /// Класс манипуляции над файлом .txtpoint
    /// </summary>
    internal class TxtPointFileManipulate
    {
        /// <summary>
        /// Регулярное выражение поиска имени тега
        /// </summary>
        private static readonly Regex NameTagRegReaded = new Regex(@"\<[^\>]*");

        /// <summary>
        /// Регулярное выражение поиска значения тега
        /// </summary>
        private static readonly Regex ValueTagRegReaded = new Regex(@"\>[^\<]*");

        /// <summary>
        /// Объект коллекций олицитворяющий файл
        /// </summary>
        static List<Series> Series;

        /// <summary>
        /// Объект выделяющихся цветов
        /// </summary>
        static List<Color> Colors;

        /// <summary>
        /// Объект рабочей коллекции
        /// </summary>
        static Series RefSeriesManipulation = null;

        /// <summary>
        /// Прочитать файл .txtpoint
        /// </summary>
        /// <param name="Directory">Директория файла</param>
        /// <returns>Массив коллекций</returns>
        public static (Series[], Color[])? ReadFile(string Directory)
        {
            Series = new List<Series>();
            Colors = new List<Color>();
            Tag tag;
            string[] Lines = File.ReadAllLines(Directory);
            Error CheckErrorTag;
            for (int i = 0; i < Lines.Length; i++)
            {
                tag = ReadTag(Lines[i]);
                CheckErrorTag = Taging(tag);
                if (CheckErrorTag.Num > 0)
                {
                    MessageBox.Show
                        ($"Произошла ошибка при чтении файла {Path.GetFileName(Directory)}" +
                        "\nПроверьте синтаксис тегов." +
                        $"\n\n{CheckErrorTag.Message}\nК ошибке привела строка {i}", $"Ошибка чтения файла {Path.GetFileName(Directory)}");
                    return null;
                }
            };
            if (RefSeriesManipulation == null) return (Series.ToArray(), Colors.ToArray());
            return null;
        }

        /// <summary>
        /// Прочитать текст преобразовав его в тег
        /// </summary>
        /// <param name="Text">Текст синтаксиса тега</param>
        /// <returns>Тег</returns>
        public static Tag ReadTag(string Text)
        {
            if (Text.Length == 0) return Tag.Taging(string.Empty, string.Empty);
            return Tag.Taging(NameTagRegReaded.Match(Text).Value.Remove(0, 1), ValueTagRegReaded.Match(Text).Value.Remove(0, 1));
        }

        /// <summary>
        /// Проверка тега
        /// </summary>
        /// <param name="tag">Структура тега</param>
        private static Error Taging(Tag tag)
        {
            switch (tag.Name)
            {
                case "Series_Init":
                    if (RefSeriesManipulation == null)
                    {
                        Series.Add(new Series(tag.Value, 1));
                        RefSeriesManipulation = Series[Series.Count - 1];
                        Colors.Add(Color.FromArgb(80, 80, 80));
                        break;
                    }
                    return Error.E7();
                case "Name":
                    if (RefSeriesManipulation != null)
                    {
                        RefSeriesManipulation.Name = tag.Value;
                        break;
                    }
                    return Error.E2();
                case "Point":
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            try
                            {
                                RefSeriesManipulation.Points.AddY(Convert.ToInt32(tag.Value));
                            }
                            catch { return Error.E5(); }
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case "Point_Name":
                    if (RefSeriesManipulation != null)
                    {
                        if (RefSeriesManipulation.Points.Count > 0)
                        {
                            RefSeriesManipulation.Points[RefSeriesManipulation.Points.Count - 1].Name = tag.Value;
                            break;
                        }
                        return Error.E3();
                    }
                    return Error.E2();
                case "Hex":
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            RefSeriesManipulation.Color = ColorTranslator.FromHtml(tag.Value);
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case "Select_Hex":
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            Colors[Colors.Count - 1] = ColorTranslator.FromHtml(tag.Value);
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case "~":
                    if (RefSeriesManipulation != null)
                    {
                        RefSeriesManipulation = null;
                        break;
                    }
                    return Error.E2();
                case "!":
                case "":
                    break;
                default:
                    return Error.E1(tag.Name);
            }
            return default;
        }

        private struct Error
        {
            private string _Message;

            private uint _Num;

            public string Message => $"{_Message}\nОшибка #{_Num}";

            public uint Num => _Num;

            private static Error Create(string message, uint number)
            {
                return new Error
                {
                    _Message = message,
                    _Num = number
                };
            }

            public static Error E1(string TagName) => Create($"Тега <{TagName}> не существует.", 1u);

            public static Error E2() => Create("Коллекция не проинициализирована.", 2u);

            public static Error E3() => Create("Не возможно присвоить имя несуществующей позиции тенденции.", 3u);

            public static Error E4(string TagName) => Create($"Тег <{TagName}> не может быть исполнен с пустым значением.", 4u);

            public static Error E5() => Create("Невозможно данную строку обработать как число.", 5u);

            public static Error E6(string TagName) => Create($"Невозможно создать новый элемент по тегу <{TagName}> так как он уже создан.", 6u);

            public static Error E7() => Create("Невозможно инициализировать новую коллекцию не деинсталировав уже инициализированую.", 7u);
        }
    }
}
