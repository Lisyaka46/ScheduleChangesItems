﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace ScheduleChangesItems.GUI
{
    /// <summary>
    /// Логика взаимодействия для ImageButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        private Brush _Background;
        /// <summary>
        /// Отображаемый цвет кнопки
        /// </summary>
        public new Brush Background
        {
            get
            {
                return _Background;
            }
            private set
            {
                _Background = value;
            }
        }

        private Color _ButtonBackground;
        /// <summary>
        /// Основной цвет кнопки
        /// </summary>
        public Color ButtonBackground
        {
            get
            {
                return _ButtonBackground;
            }
            set
            {
                ButtonBorder.Background = new SolidColorBrush(value);
                _ButtonBackground = value;
            }
        }

        private Color _MouseBackground;
        /// <summary>
        /// Цвет который отображается при наведении на кнопку курсором
        /// </summary>
        public Color MouseBackground
        {
            get
            {
                return _MouseBackground;
            }
            set
            {
                _MouseBackground = value;
            }
        }

        private Color _DisabledBackground;
        /// <summary>
        /// Цвет который отображается при отключённом состоянии элемента
        /// </summary>
        public Color DisabledBackground
        {
            get
            {
                return _DisabledBackground;
            }
            set
            {
                _DisabledBackground = value;
            }
        }

        private Color _ClickedBackground;
        /// <summary>
        /// Цвет который отображается при зажатой клавиши мыши
        /// </summary>
        public Color ClickedBackground
        {
            get
            {
                return _ClickedBackground;
            }
            set
            {
                _ClickedBackground = value;
            }
        }

        private ImageSource _Imaging;
        /// <summary>
        /// Изображение которое отображается в кнопке
        /// </summary>
        public ImageSource Imaging
        {
            get
            {
                return _Imaging;
            }
            set
            {
                ButtonImage.Source = value;
                _Imaging = value;
            }
        }

        private Thickness _BorderThickness;
        /// <summary>
        /// Толщина границ кнопки
        /// </summary>
        public new Thickness BorderThickness
        {
            get
            {
                return _BorderThickness;
            }
            set
            {
                ButtonBorder.BorderThickness = value;
                _BorderThickness = value;
            }
        }

        private CornerRadius _CornerRadius;
        /// <summary>
        /// Скруглённость границ кнопки
        /// </summary>
        public CornerRadius CornerRadius
        {
            get
            {
                return _CornerRadius;
            }
            set
            {
                ButtonBorder.CornerRadius = value;
                _CornerRadius = value;
            }
        }

        private double _MillisecondsAnimation;
        /// <summary>
        /// Количество миллисекунд используемые в анимации
        /// </summary>
        public double MillisecondsAnimation
        {
            get
            {
                return _MillisecondsAnimation;
            }
            set
            {
                if (value >= 0d) _MillisecondsAnimation = value;
                else throw new InvalidOperationException("Значение должно быть больше или равно нулю");
            }
        }

        /// <summary>
        /// Позиционирование картинки в кнопке
        /// </summary>
        public Thickness ImageMargin
        {
            get
            {
                return ButtonImage.Margin;
            }
            set
            {
                ButtonImage.Margin = value;
            }
        }


        /// <summary>
        /// Инициализировать объект кнопки с изображением
        /// </summary>
        public ImageButton()
        {
            InitializeComponent();
            ButtonImage.Margin = new Thickness(10, 10, 10, 10);
            ClickedBackground = Colors.LightGray;
            DisabledBackground = Colors.Gray;
            MillisecondsAnimation = 0d;
            ButtonBackground = Colors.Black;
            MouseBackground = Colors.Gray;
            ButtonBorder.Background = new SolidColorBrush(ButtonBackground);
            ButtonBorder.MouseEnter += (sender, e) =>
            {
                if (IsEnabled) MouseEnterDetect();
            };
            ButtonBorder.MouseLeave += (sender, e) =>
            {
                if (IsEnabled) MouseLeaveDetect();
            };
            IsEnabledChanged += (sender, e) =>
            {
                if ((bool)e.OldValue)
                {
                    ButtonBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, null);
                    ButtonBorder.Background = new SolidColorBrush(DisabledBackground);
                }
                else
                {
                    ButtonBorder.Background = new SolidColorBrush(ButtonBackground);
                }
            };
            ButtonBorder.MouseDown += (sender, e) =>
            {
                ButtonBorder.Background = new SolidColorBrush(ClickedBackground);
            };
            ButtonBorder.MouseUp += (sender, e) =>
            {
                if (IsEnabled) MouseEnterDetect();
            };
        }

        /// <summary>
        /// Событие прихода курсора в видимую область кнопки
        /// </summary>
        private void MouseEnterDetect()
        {
            if (!ButtonBorder.Background.IsFrozen && !ButtonBorder.Background.IsSealed)
            {
                ColorAnimation anim = new ColorAnimation(MouseBackground, TimeSpan.FromMilliseconds(MillisecondsAnimation));
                ButtonBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
        }

        /// <summary>
        /// Событие ухода курсора из видимой области кнопки
        /// </summary>
        private void MouseLeaveDetect()
        {
            if (!ButtonBorder.Background.IsFrozen && !ButtonBorder.Background.IsSealed)
            {
                ColorAnimation anim = new ColorAnimation(MouseBackground, ButtonBackground, TimeSpan.FromMilliseconds(MillisecondsAnimation));
                ButtonBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
        }
    }
}
