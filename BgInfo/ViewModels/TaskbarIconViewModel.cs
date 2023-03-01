﻿using System.Windows;
using System.Windows.Input;
using BgInfo.Views;
using Prism.Commands;
using Prism.Mvvm;
using Zodiacon.WPF;

namespace BgInfo.ViewModels
{
  sealed class TaskbarIconViewModel : BindableBase
  {
    readonly BgInfoManager _mgr;
    readonly IUIServices UI;

    public TaskbarIconViewModel(BgInfoManager mgr, IUIServices ui)
    {
      _mgr = mgr;
      UI = ui;
      ExitCommand = new DelegateCommand(() => Application.Current.Shutdown());

      SettingsCommand = new DelegateCommand(() =>
      {
        _mgr.EnableTray(false);
        var vm = UI.DialogService.CreateDialog<SettingsViewModel, SettingsView>(_mgr.Settings);
        if (vm.ShowDialog() == true)
        {
          // apply changes
          mgr.ApplySettings(vm);
        }

        _mgr.EnableTray(true);
      });

      RefreshCommand = new DelegateCommand(() => _mgr.Refresh());

      AboutCommand = new DelegateCommand(() =>
      {
        _mgr.EnableTray(false);
        MessageBox.Show(Application.Current.MainWindow, "BgInfo (WPF Style) by Pavel Yosifovich (C)2016-2018", "About BgInfo");
        _mgr.EnableTray(true);
      });
    }


    public ICommand ExitCommand { get; }
    public ICommand SettingsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AboutCommand { get; }
  }
}
