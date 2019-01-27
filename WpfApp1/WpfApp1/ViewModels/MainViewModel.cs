using Livet;
using Livet.Commands;

using WpfApp1.Models;
using System.Windows;

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

        #endregion

        #region 初期化処理
        public void Initialize()
        {
            _model = new Model();
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

        #endregion
    }
}
