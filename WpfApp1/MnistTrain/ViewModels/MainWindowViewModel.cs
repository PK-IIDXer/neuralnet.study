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
using System.Threading.Tasks;
using System.Windows;

namespace MnistTrain.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region インスタンス変数
        private Model _model;
        #endregion

        #region 変更通知プロパティ

        #region Mnist画像
        private BitmapSource _MnistImageSource;
        /// <summary>
        /// Mnist画像
        /// </summary>
        public BitmapSource MnistImageSource
        {
            get => _MnistImageSource;
            set => RaisePropertyChangedIfSet(ref _MnistImageSource, value);
        }
        #endregion

        #region プログレスバーを表示するかどうか

        private Visibility _ProgressBarVisibility;
        /// <summary>
        /// プログレスバーを表示するかどうか
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get => _ProgressBarVisibility;
            set => RaisePropertyChangedIfSet(ref _ProgressBarVisibility, value);
        }

        #endregion

        #region プログレスバーのコメント

        private string _ProgressBarComment;
        /// <summary>
        /// プログレスバーのコメント
        /// </summary>
        public string ProgressBarComment
        {
            get => _ProgressBarComment;
            set => RaisePropertyChangedIfSet(ref _ProgressBarComment, value);
        }

        #endregion

        #endregion

        public async void Initialize()
        {
            ProgressBarVisibility = Visibility.Visible;
            ProgressBarComment = "Modelオブジェクト構成中";

            await Task.Run(() =>
            {
                _model = new Model();
            });

            ProgressBarVisibility = Visibility.Hidden;
        }

        #region コマンド

        private ViewModelCommand _TaskTestCommand;

        public ViewModelCommand TaskTestCommand
        {
            get
            {
                if (_TaskTestCommand == null)
                {
                    _TaskTestCommand = new ViewModelCommand(TaskTest);
                }
                return _TaskTestCommand;
            }
        }

        public void TaskTest()
        {
            ProgressBarVisibility = Visibility.Visible;
            ProgressBarComment = "学習中";

            _model.TrainMnist();

            ProgressBarVisibility = Visibility.Hidden;
        }

        #endregion

        #region メソッド



        #endregion
    }
}
