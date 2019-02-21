using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WpfApp1.Utils
{
    /// <summary>
    /// MNISTデータセットの画像を表すオブジェクトを生成するクラス
    /// </summary>
    public class MnistImage
    {
        #region 定数
        /// <summary>
        /// MNISTデータセットのパス
        /// </summary>
        private const string MNIST_FILE_PATH = @"C:\Users\h-saito\Desktop\neuralnet.study\WpfApp1\mnist_dataset\train-images-idx3-ubyte\train-images.idx3-ubyte";

        /// <summary>
        /// ラベルファイルのパス
        /// </summary>
        private const string LABEL_FILE_PATH = @"C:\Users\h-saito\Desktop\neuralnet.study\WpfApp1\mnist_dataset\train-labels-idx1-ubyte\train-labels.idx1-ubyte";
        #endregion

        #region プロパティ
        /// <summary>
        /// 画像の高さを取得します。
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 画像の幅を取得します。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// MNIST画像のピクセルを取得します。
        /// </summary>
        public byte[][] Pixels { get; }

        /// <summary>
        /// 0 ~ 9までのラベルを取得します。
        /// </summary>
        public byte Label { get; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// <see cref="MnistImage"/> コンストラクタ
        /// </summary>
        /// <param name="height">画像の高さ</param>
        /// <param name="width">画像の幅</param>
        /// <param name="pixels">画像を構成するピクセル</param>
        /// <param name="label">ラベル</param>
        public MnistImage(int height, int width, byte[][] pixels, byte label)
        {
            Height = height;
            Width = width;
            Label = label;
            Pixels = new byte[height][];
            for (var i = 0; i < height; i++)
            {
                Pixels[i] = new byte[width];
            }
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    Pixels[i][j] = pixels[i][j];
                }
            }
        }
        #endregion

        #region staticメソッド
        /// <summary>
        /// MNIST データセットとラベルをロードします。
        /// </summary>
        /// <param name="pixelFilePath">MNIST データセットのパス</param>
        /// <param name="labelFilePath">ラベルのパス</param>
        /// <returns>MnistImageの配列</returns>
        public static MnistImage[] Load(string pixelFilePath = MNIST_FILE_PATH, string labelFilePath = LABEL_FILE_PATH)
        {
            using (var imageStream = File.OpenRead(pixelFilePath))
            using (var labelStream = File.OpenRead(labelFilePath))
            using (var imageReader = new BinaryReader(imageStream))
            using (var labelReader = new BinaryReader(labelStream))
            {
                int magic1 = imageReader.ReadInt32();
                magic1 = ReverseBytes(magic1);

                // 画像の枚数を取得
                int imageCount = imageReader.ReadInt32();
                imageCount = ReverseBytes(imageCount);

                // 画像の高さを取得
                int imageHeight = imageReader.ReadInt32();
                imageHeight = ReverseBytes(imageHeight);

                // 画像の幅を取得
                int imageWidth = imageReader.ReadInt32();
                imageWidth = ReverseBytes(imageWidth);

                int magic2 = labelReader.ReadInt32();
                magic2 = ReverseBytes(magic2);

                // ラベルの個数を取得
                int labelCount = labelReader.ReadInt32();
                labelCount = ReverseBytes(labelCount);

                // 読み込んだ1枚分の画像データを格納するバッファを作成
                var pixels = new byte[imageHeight][];
                for (var i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new byte[imageWidth];
                }

                // 読み込んだすべての MNIST 画像を格納する配列を作成
                var result = new MnistImage[imageCount];

                for (int di = 0; di < imageCount; di++)
                {
                    for (int i = 0; i < imageHeight; i++) // get 28x28 pixel values
                    {
                        for (int j = 0; j < imageWidth; j++)
                        {
                            byte b = imageReader.ReadByte();
                            pixels[i][j] = b;
                        }
                    }

                    // ラベルを取得
                    byte label = labelReader.ReadByte();

                    var image = new MnistImage(imageHeight, imageWidth, pixels, label);
                    result[di] = image;
                }

                return result;
            }
        }

        #endregion

        #region メソッド
        /// <summary>
        /// ベクトルに変換します
        /// </summary>
        /// <returns>ベクトル</returns>
        public Vector<double> ToVector()
        {
            var flatten = Pixels.SelectMany(row => row)
                .Select(b => Convert.ToDouble(b));
            var vector = Vector<double>.Build.DenseOfEnumerable(flatten);
            return vector;
        }

        /// <summary>
        /// 行列に変換します。
        /// </summary>
        /// <returns></returns>
        public Matrix<double> ToMatrix()
        {
            return Matrix<double>.Build.Dense(Height, Width, (row, col) => Pixels[row][col]);
        }

        /// <summary>
        /// bitmapイメージを作ります。
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateBitmapImage()
        {
            Bitmap canvas = new Bitmap(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int color = (int)Pixels[x][y];
                    canvas.SetPixel(y, x, Color.FromArgb(255 - color, 255 - color, 255 - color));
                }
            }

            return canvas;
        }

        #endregion

        #region privateメソッド
        
        /// <summary>
        /// 整数のビットを逆順にします。
        /// </summary>
        /// <param name="value">整数</param>
        /// <returns>ビットを逆順にした整数</returns>
        private static int ReverseBytes(int value)
        {
            byte[] intAsBytes = BitConverter.GetBytes(value);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }

        #endregion
    }
}
