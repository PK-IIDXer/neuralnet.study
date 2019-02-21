using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using WpfApp1.Models;
using WpfApp1.Utils;

using MnistTrain.Models;
using System.Windows.Media.Imaging;

namespace MnistTrain.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region 変更通知プロパティ

        private BitmapSource _MnistImageSource;

        public BitmapSource MnistImageSource
        {
            get => _MnistImageSource;
            set => RaisePropertyChangedIfSet(ref _MnistImageSource, value);
        }

        #endregion

        public void Initialize()
        {
            var allMnists = MnistImage.Load();
            MnistImageSource = Model.Convert(allMnists[0].CreateBitmapImage());
        }
    }
}
