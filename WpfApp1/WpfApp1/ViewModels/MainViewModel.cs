using Livet;
using Livet.Commands;

using WpfApp1.Models;
using System.Windows;
using WpfApp1.Utils;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace WpfApp1.ViewModels
{
    public class MainViewModel : ViewModel
    {
        #region コンストラクタ

        public MainViewModel()
        {
            Initialize();
        }

        #endregion

        #region メンバ変数
        private Model _model;
        #endregion

        #region 変更通知プロパティ

        #region パラメータ１

        private double _Param1;
        /// <summary>
        /// パラメータ１
        /// </summary>
        /// <remarks>
        /// 数字用のコントロールを入手したり作ったりするのめんどいので
        /// 数字以外が入ったらメッセージ出すようにしてみた。
        /// </remarks>
        public string Param1
        {
            get
            { return _Param1.ToString(); }
            set
            {
                if (!double.TryParse(value, out _Param1))
                {
                    MessageBox.Show("数字を入力してね", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region パラメータ２

        private double _Param2;
        /// <summary>
        /// パラメータ2
        /// </summary>
        /// <remarks>
        /// 数字用のコントロールを入手したり作ったりするのめんどいので
        /// 数字以外が入ったらメッセージ出すようにしてみた。
        /// </remarks>
        public string Param2
        {
            get
            { return _Param2.ToString(); }
            set
            {
                if (!double.TryParse(value, out _Param2))
                {
                    MessageBox.Show("数字を入力してね", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region 計算結果

        private double _Mult;
        /// <summary>
        /// 計算結果
        /// </summary>
        /// <remarks>
        /// 数字用のコントロールを入手したり作ったりするのめんどいので
        /// 数字以外が入ったらメッセージ出すようにしてみた。
        /// </remarks>
        public string Mult
        {
            get
            { return _Mult.ToString(); }
            set
            {
                if (!double.TryParse(value, out _Mult))
                {
                    MessageBox.Show("数字を入力してね", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region 損失

        private double _Loss;
        /// <summary>
        /// 損失
        /// </summary>
        public double Loss
        {
            get => _Loss;
            set => RaisePropertyChangedIfSet(ref _Loss, value);
        }

        #endregion


        private BitmapSource _Image;

        public BitmapSource Image
        {
            get => _Image;
            set => RaisePropertyChangedIfSet(ref _Image, value);
        }


        #endregion

        #region 初期化処理
        public void Initialize()
        {
            _model = new Model();
            int num = 1;
            var mnists = MnistImage.Load();
            // var cnv = mnists[num].CreateBitmapImage();
            // var lbl = mnists[num].Label;
            // cnv.Save(@"C:\Users\h-saito\Desktop\neuralnet.study\WpfApp1\mnist_dataset\train-images-idx3-ubyte\" + num + "-" + lbl + ".bmp");

            int count = 0;
            foreach (var mn in mnists)
            {
                var cnv = mn.CreateBitmapImage();
                var lbl = mn.Label;
                string path = @"C:\Users\h-saito\Desktop\neuralnet.study\WpfApp1\mnist_dataset\train-images-idx3-ubyte\" + lbl + @"\" + count + ".bmp";
                cnv.Save(path);
                count++;
            }
        }
        #endregion

        #region コマンド

        #region 計算

        private ViewModelCommand _ForwardCommand;

        public ViewModelCommand ForwardCommand
        {
            get
            {
                if (_ForwardCommand == null)
                {
                    _ForwardCommand = new ViewModelCommand(Forward);
                }
                return _ForwardCommand;
            }
        }

        public void Forward()
        {
            Mult = _model.Forward(_Param1, _Param2).ToString();
        }

        #endregion

        #region 損失を求める

        private ViewModelCommand _GetLossCommand;

        public ViewModelCommand GetLossCommand
        {
            get
            {
                if (_GetLossCommand == null)
                {
                    _GetLossCommand = new ViewModelCommand(GetLoss);
                }
                return _GetLossCommand;
            }
        }

        public void GetLoss()
        {
            double loss = 0;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    loss += _model.GetLoss(i, j);
                }
            }
            Loss = loss / 100;
        }

        #endregion

        #region 学習

        private ViewModelCommand _BackCommand;

        public ViewModelCommand BackCommand
        {
            get
            {
                if (_BackCommand == null)
                {
                    _BackCommand = new ViewModelCommand(Back);
                }
                return _BackCommand;
            }
        }

        public void Back()
        {
            _model.Train(_Param1, _Param2, _Mult);
        }

        #endregion

        #region まとめて覚えさせる

        private ViewModelCommand _BatchCommand;

        public ViewModelCommand BatchCommand
        {
            get
            {
                if (_BatchCommand == null)
                {
                    _BatchCommand = new ViewModelCommand(Batch);
                }
                return _BatchCommand;
            }
        }

        public void Batch()
        {
            Random rnd = new System.Random();

            for (var i = 0; i < 100; i++)
            {
                _Param1= rnd.Next(10);
                _Param2 = rnd.Next(10);
                _Mult = _Param1 * _Param2;

                Back();
            }
            Param1 = 0.ToString();
            Param2 = 0.ToString();
            Mult = 0.ToString();
        }

        #endregion

        #region 初期化

        private ViewModelCommand _InitCommand;

        public ViewModelCommand InitCommand
        {
            get
            {
                if (_InitCommand == null)
                {
                    _InitCommand = new ViewModelCommand(Init);
                }
                return _InitCommand;
            }
        }

        public void Init()
        {
            Initialize();
            _Param1 = 0;
            _Param2 = 0;
            _Mult = 0;
            RaisePropertyChanged(nameof(Param1));
            RaisePropertyChanged(nameof(Param2));
            RaisePropertyChanged(nameof(Mult));
        }

        #endregion

        #endregion
    }
}
