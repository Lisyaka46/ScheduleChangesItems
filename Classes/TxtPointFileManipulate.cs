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
        /// Массив визуализационных объектов коллекции
        /// </summary>
        static List<VisualizationSeries> VisObjects;

        /// <summary>
        /// Объект рабочей коллекции
        /// </summary>
        static Series RefSeriesManipulation = null;

        /// <summary>
        /// Прочитать файл .txtpoint
        /// </summary>
        /// <param name="Directory">Директория файла</param>
        /// <returns>Массив коллекций</returns>
        public static (Series[], VisualizationSeries[])? ReadFile(string Directory)
        {
            Series = new List<Series>();
            VisObjects = new List<VisualizationSeries>();
            Tag tag;
            string[] Lines = File.ReadAllLines(Directory);
            Error CheckErrorTag;
            for (int i = 0; i < Lines.Length; i++)
            {
                tag = ReadTag(Lines[i]);
                CheckErrorTag = Taging(tag);
                if (CheckErrorTag.Num > 0)
                {
                    RefSeriesManipulation = null;
                    MessageBox.Show
                        ($"Произошла ошибка при чтении файла {Path.GetFileName(Directory)}" +
                        "\nПроверьте синтаксис тегов." +
                        $"\n{Lines[i]}\n{CheckErrorTag.Message}\nК ошибке привела строка {i + 1}", $"Ошибка чтения файла {Path.GetFileName(Directory)}");
                    return null;
                }
            };
            if (RefSeriesManipulation == null) return (Series.ToArray(), VisObjects.ToArray());
            else MessageBox.Show
                        ($"Произошла ошибка при чтении файла {Path.GetFileName(Directory)}" +
                        "\nНе хватает конечной деинсталляции колекции." +
                        "\n\nОшибка E0", $"Ошибка чтения файла {Path.GetFileName(Directory)}");
            RefSeriesManipulation = null;
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
                case Tag.TagNaming.TagSeriesInit:
                    if (RefSeriesManipulation == null)
                    {
                        Series.Add(new Series(tag.Value, 1));
                        RefSeriesManipulation = Series[Series.Count - 1];
                        VisObjects.Add(new VisualizationSeries(Color.Black, Color.FromArgb(80, 80, 80)));
                        break;
                    }
                    return Error.E7();
                case Tag.TagNaming.TagName:
                    if (RefSeriesManipulation != null)
                    {
                        RefSeriesManipulation.Name = tag.Value;
                        break;
                    }
                    return Error.E2();
                case Tag.TagNaming.TagPointTrend:
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            try
                            {
                                RefSeriesManipulation.Points.Add(new DataPoint()
                                {
                                    YValues = new double[1] { Convert.ToInt32(tag.Value) },
                                    BorderWidth = 8
                                });
                            }
                            catch { return Error.E5(); }
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case Tag.TagNaming.TagPointNameTrend:
                    if (RefSeriesManipulation != null)
                    {
                        if (RefSeriesManipulation.Points.Count > 0)
                        {
                            RefSeriesManipulation.Points[RefSeriesManipulation.Points.Count - 1].AxisLabel = tag.Value;
                            break;
                        }
                        return Error.E3();
                    }
                    return Error.E2();
                case Tag.TagNaming.TagHex:
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            Color color = ColorTranslator.FromHtml(tag.Value);
                            RefSeriesManipulation.Color = color;
                            RefSeriesManipulation.BorderColor = color;
                            VisObjects[VisObjects.Count - 1].ColorDefault = color;
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case Tag.TagNaming.TagSelectHex:
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            Color color = ColorTranslator.FromHtml(tag.Value);
                            VisObjects[VisObjects.Count - 1].ColorSelect = color;
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case Tag.TagNaming.TagStyleChart:
                    if (RefSeriesManipulation != null)
                    {
                        if (tag.Value.Length > 0)
                        {
                            try
                            {
                                RefSeriesManipulation.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), tag.Value);
                            }
                            catch { return Error.E8(); }
                            break;
                        }
                        return Error.E4(tag.Name);
                    }
                    return Error.E2();
                case Tag.TagNaming.TagUninstallCollection:
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

            public string Message => $"{_Message}\nОшибка E{_Num}";

            public uint Num => _Num;

            private static Error Create(string message, uint number)
            {
                return new Error
                {
                    _Message = message,
                    _Num = number
                };
            }

            /// <summary>
            /// Несуществующий тег
            /// </summary>
            public static Error E1(string TagName) => Create($"Тега <{TagName}> не существует.", 1u);

            /// <summary>
            /// Не проинициализированная коллекция
            /// </summary>
            public static Error E2() => Create("Коллекция не проинициализирована.", 2u);

            /// <summary>
            /// Присвоение имени не существующей позиции
            /// </summary>
            public static Error E3() => Create("Не возможно присвоить имя несуществующей позиции тенденции.", 3u);

            /// <summary>
            /// Недопустимо пустое значение
            /// </summary>
            public static Error E4(string TagName) => Create($"Тег <{TagName}> не может быть исполнен с пустым значением.", 4u);

            /// <summary>
            /// Строка не является числом
            /// </summary>
            public static Error E5() => Create("Невозможно данную строку обработать как число.", 5u);

            /// <summary>
            /// Невозможно создать тот же элемент по тегу
            /// </summary>
            public static Error E6(string TagName) => Create($"Невозможно создать новый элемент по тегу <{TagName}> так как он уже создан.", 6u);

            /// <summary>
            /// Невозможно инициализипровать коллекцию
            /// </summary>
            public static Error E7() => Create("Невозможно инициализировать новую коллекцию не деинсталлировав уже инициализированую.", 7u);

            /// <summary>
            /// Невозможно создать стиль
            /// </summary>
            public static Error E8() => Create("Невозможно создать введёный стиль графика тенденции.", 8u);
        }
    }
}
