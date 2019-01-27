using System;
using System.Security.Cryptography;

using Livet;
using MathNet.Numerics.LinearAlgebra.Double;

namespace WpfApp1.Models
{
    public class NNModel : NotificationObject
    {
        #region 定数
        /// <summary>
        /// 中間層の厚み
        /// </summary>
        private const int NODE_NUMBER = 10;

        /// <summary>
        /// 学習率
        /// </summary>
        private const double LEARNING_RATE = 0.01;
        #endregion

        #region パラメータ
        private DenseMatrix W1 { get; set; }

        private DenseMatrix W2 { get; set; }

        private DenseMatrix B1 { get; set; }

        private DenseMatrix B2 { get; set; }
        #endregion

        #region コンストラクタ
        public NNModel()
        {
            W1 = SetInitialValues(new DenseMatrix(2, NODE_NUMBER));
            B1 = SetInitialValues(new DenseMatrix(1, NODE_NUMBER));

            W2 = SetInitialValues(new DenseMatrix(NODE_NUMBER, 1));
            B2 = SetInitialValues(new DenseMatrix(1, 1));
        }
        #endregion

        #region NNモデル
        /// <summary>
        /// 順伝播
        /// </summary>
        /// <param name="input">入力する二つの値</param>
        /// <returns>乗じた結果</returns>
        public double Forward(DenseMatrix input)
        {
            var v1 = input * W1 + B1;
            var t1 = new DenseMatrix(1, NODE_NUMBER);
            for (var i = 0; i < NODE_NUMBER; i++)
            {
                t1[0, i] = ReLU(v1[0, i]);
            }
            var x = t1 * W2 + B2;

            return x[0, 0];
        }

        /// <summary>
        /// 逆伝播
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public void Train(DenseMatrix input, double teacher)
        {
            #region 必要なデータ
            var x1 = input * W1 + B1;
            var a1 = new DenseMatrix(1, NODE_NUMBER);
            for (var i = 0; i < NODE_NUMBER; i++)
            {
                a1[0, i] = ReLU(x1[0, i]);
            }
            var x2 = a1 * W2 + B2;
            #endregion

            var dLdX2 = new DenseMatrix(1);
            dLdX2[0,0] = Math.Abs(x2[0, 0] - teacher);

            var dLdW2 = a1.Transpose() * dLdX2;
            var dLdB2 = dLdX2;

            var dLdA1 = dLdX2 * W2.Transpose();
            var dLdX1 = new DenseMatrix(1, NODE_NUMBER);
            for (var i = 0; i < NODE_NUMBER; i++)
            {
                dLdX1[0, i] = Math.Max(0, dLdA1[0, i]);
            }
            var dLdB1 = dLdX1;
            var dLdW1 = input.Transpose() * dLdX1;

            W1 -= (DenseMatrix)dLdW1.Multiply(LEARNING_RATE);
            B1 -= (DenseMatrix)dLdB1.Multiply(LEARNING_RATE);
            W2 -= (DenseMatrix)dLdW2.Multiply(LEARNING_RATE);
            B2 -= (DenseMatrix)dLdB2.Multiply(LEARNING_RATE);
        }
        #endregion

        #region 活性化関数
        /// <summary>
        /// ReLU
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double ReLU(double x)
        {
            return Math.Max(0.0, x);
        }

        /// <summary>
        /// 微分されたReLU
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double DiffReLu(double x)
        {
            return x > 0 ? 1.0 : 0.0;
        }
        #endregion

        #region 損失関数
        /// <summary>
        /// 損失関数
        /// </summary>
        /// <param name="x">入力値</param>
        /// <returns>x^2/2</returns>
        private double SquareSum(double x)
        {
            return x * x / 2;
        }

        /// <summary>
        /// 微分された損失関数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double DiffSquareSum(double x)
        {
            return Math.Abs(x);
        }
        #endregion

        #region 初期化
        /// <summary>
        /// Heの初期値を取得します。
        /// </summary>
        /// <param name="denseMatrix"></param>
        /// <returns></returns>
        private DenseMatrix SetInitialValues(DenseMatrix denseMatrix)
        {
            var standardDivertion = Math.Sqrt(2.0 / denseMatrix.RowCount);
            var meanValue = 0.0;

            var result = new DenseMatrix(denseMatrix.RowCount, denseMatrix.ColumnCount);

            for (var i = 0; i < denseMatrix.RowCount; i++)
            {
                for (var j = 0; j < denseMatrix.ColumnCount; j++)
                {
                    result[i, j] = GetNormRandom(meanValue, standardDivertion);
                }
            }

            return result;
        }

        /// <summary>
        /// 一様乱数を取得する
        /// </summary>
        /// <returns>乱数:-1～+1</returns>
        public static double GetRandom()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bs = new byte[sizeof(Int32)];
            rng.GetBytes(bs);
            int iR = BitConverter.ToInt32(bs, 0);
            return ((double)iR / Int32.MaxValue);
        }

        /// <summary>
        /// 標準正規分布に従う乱数
        /// </summary>
        /// <returns>N(0,1)に従う乱数</returns>
        public static double GetNormRandom()
        {
            double dR1 = Math.Abs(GetRandom());
            double dR2 = Math.Abs(GetRandom());
            return (Math.Sqrt(-2 * Math.Log(dR1, Math.E)) * Math.Cos(2 * Math.PI * dR2));
        }

        /// <summary>
        /// 正規分布(μ・σ)に従う乱数
        /// </summary>
        /// <param name="m">平均:μ</param>
        /// <param name="s">標準偏差:σ</param>
        /// <returns>N(m,s)に従う乱数</returns>
        public static double GetNormRandom(double m, double s)
        {
            return (m + s * GetNormRandom());
        }
        #endregion
    }
}
