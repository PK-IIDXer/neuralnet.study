using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utils
{
    class MnistUtils
    {
        #region 定数
        /// <summary>
        /// MNISTデータセットのパス
        /// </summary>
        private const string MNIST_FILE_PATH = @"C:\Users\hytch\Desktop\neuralnet.study\WpfApp1\mnist_dataset\train-images-idx3-ubyte\train-images.idx3-ubyte";

        /// <summary>
        /// ラベルファイルのパス
        /// </summary>
        private const string LABEL_FILE_PATH = @"C:\Users\hytch\Desktop\neuralnet.study\WpfApp1\mnist_dataset\train-labels-idx1-ubyte\train-labels.idx1-ubyte";
        #endregion

        private static MnistImage[] _MnistImages { get; set; }

        public static void LoadMnist()
        {
            _MnistImages = MnistImage.Load(MNIST_FILE_PATH, LABEL_FILE_PATH);

        }
    }
}
