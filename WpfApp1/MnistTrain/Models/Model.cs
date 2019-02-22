using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Livet;
using MathNet.Numerics.LinearAlgebra.Double;
using WpfApp1.Models;
using WpfApp1.Utils;

namespace MnistTrain.Models
{
    public class Model : NotificationObject
    {
        #region インスタンス変数
        /// <summary>
        /// ニューラルネットワークモデル
        /// </summary>
        private NeuralNetModel _neuralNetModel;

        /// <summary>
        /// MNISTデータセット
        /// </summary>
        private MnistImage[] _mnistImages;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Model()
        {
            var vs = new List<int> { 28 * 28, 100, 50, 10};
            _neuralNetModel = new NeuralNetModel(vs);

            _mnistImages = MnistImage.Load();
        }
        #endregion

        #region 学習
        public void TrainMnist()
        {
            for (var m = 0; m < _mnistImages.Count(); m++)
            {
                var teacher = new DenseMatrix(1, 10);
                for (var i = 0; i < 10; i++)
                {
                    if (_mnistImages[m].Label == i)
                        teacher[0, i] = 1;
                }
                var vec = _mnistImages[m].ToVector();
                _neuralNetModel.BackPropagation(vec, teacher);
            }
        }
        #endregion

        #region 便利メソッド
        /// <summary>
        /// System.Drawing.BitmapオブジェクトをSystem.Windows.Media.Imaging.BitmapSourceオブジェクトに変換します。
        /// </summary>
        /// <param name="bitmap">Bitmapオブジェクト</param>
        /// <returns>BitmapSourceオブジェクト</returns>
        public static BitmapSource Convert(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
        #endregion
    }
}
