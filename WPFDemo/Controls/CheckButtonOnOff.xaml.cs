﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WPFDemo.Controls
{
    /// <summary>
    /// CheckButtonOnOff.xaml 的交互逻辑
    /// </summary>
    public partial class CheckButtonOnOff : UserControl
    {
        private bool isOn = true;
        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
                SetStatus(value);
            }
        }

        public CheckButtonOnOff()
        {
            InitializeComponent();
            this.Loaded += CheckButtonOnOff_Loaded;
        }

        private void CheckButtonOnOff_Loaded(object sender, RoutedEventArgs e)
        {
            SetStatus(IsOn, 0);
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsOn = !IsOn;
        }

        private void SetStatus(bool isOn, double sec=0.2)
        {
            double startX, endX;

            if (isOn)
            {
                startX = 0;
                endX = 0;

            }
            else
            {
                startX = 1 - this.ActualWidth / 2 + 1;
                endX = this.ActualWidth / 2;
            }
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = border1.Margin;             //起始值
            ta.To = new Thickness(startX, border1.Margin.Top, endX, border1.Margin.Bottom);        //结束值

            ta.Duration = TimeSpan.FromSeconds(sec);         //动画持续时间
            this.border1.BeginAnimation(Border.MarginProperty, ta);//开始动画
        }
    }
}
