﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BgInfo.Models;
using BgInfo.ViewModels;
using BgInfo.Views;
using Hardcodet.Wpf.TaskbarNotification;
using Zodiacon.WPF;
using static BgInfo.NativeMethods;

namespace BgInfo
{
  class BgInfoManager
  {
    TaskbarIcon _tray;
    ObservableCollection<BgViewModel> _screens = new ObservableCollection<BgViewModel>();
    DispatcherTimer _timer;

    public Settings Settings { get; } = new Settings();

    public BgInfoManager()
    {
      _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };
      _timer.Tick += _timer_Tick;
      _timer.Start();
    }

    public void ApplySettings(SettingsViewModel settings)
    {
      Settings.FontFamily = settings.SelectedFont.Source;
      Settings.FontSize = settings.SelectedFontSize;
      Settings.TextColor = new SolidColorBrush(settings.TextColor);
      _timer.Interval = settings.SelectedInterval;
      Settings.IntervalSeconds = (int)settings.SelectedInterval.TotalSeconds;
    }

    private void _timer_Tick(object sender, EventArgs e)
    {
      Refresh();
    }

    public int CreateWindows()
    {
      var windows = 0;

      EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr data) =>
      {
        Debug.WriteLine($"monitor: {hMonitor}");

        var info = new MonitorInfo();
        info.Init();
        GetMonitorInfo(hMonitor, ref info);

        var vm = new BgViewModel(info, Settings);
        var win = new MainView
        {
          Left = info.rcWork.Left,
          Top = info.rcWork.Top,
          Width = info.rcWork.Width,
          Height = info.rcWork.Height,
          DataContext = vm
        };
        _screens.Add(vm);

        win.Show();
        windows++;
        return true;
      }, IntPtr.Zero);

      return windows;
    }

    public void EnableTray(bool enable)
    {
      _tray.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
    }

    public void InitTray()
    {
      var ui = new UIServicesDefaults();
      _tray = Application.Current.FindResource("TrayIcon") as TaskbarIcon;
      var vm = new TaskbarIconViewModel(this, ui);

      _tray.DataContext = vm;

      TaskbarIcon.SetParentTaskbarIcon(Application.Current.MainWindow, _tray);
    }

    public void Refresh()
    {
      foreach (var screen in _screens)
      {
        screen.Refresh();
      }
    }
  }
}
