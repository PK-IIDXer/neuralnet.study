using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utils
{
    /// <summary>
    /// 共通的な関数つめあわせ
    /// </summary>
    class Functions
    {
        #region 活性化関数

        #region デリゲート
        /// <summary>
        /// 活性化関数
        /// </summary>
        public delegate DenseMatrix Activation(DenseMatrix denseMatrix);

        /// <summary>
        /// 活性化関数の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="fromRight">右からの入力</param>
        /// <returns>左への出力</returns>
        public delegate DenseMatrix BackActivation(DenseMatrix fromLeft, DenseMatrix fromRight);

        /// <summary>
        /// 引数に指定した活性化関数を取得する
        /// </summary>
        /// <param name="activation">必要な活性化関数を指定する</param>
        /// <returns>活性化関数</returns>
        public static Activation GetActivation(Activations activation)
        {
            switch (activation)
            {
                case Activations.Identity:
                    return Identity;
                case Activations.SoftMax:
                    return SoftMax;
                case Activations.ReLU:
                    return ReLU;
                case Activations.Sigmoid:
                    return Sigmoid;
                default:
                    throw new Exception("不明な活性化関数の指定");
            }
        }

        public static BackActivation GetBackActivation(Activations activation)
        {
            switch (activation)
            {
                case Activations.Identity:
                    return BackIdentity;
                case Activations.SoftMax:
                    return BackSoftMax;
                case Activations.ReLU:
                    return BackReLU;
                case Activations.Sigmoid:
                    return BackSigmoid;
                default:
                    throw new Exception("不明な活性化関数の指定");
            }
        }
        #endregion

        #region 列挙
        /// <summary>
        /// 活性化関数の列挙
        /// </summary>
        public enum Activations
        {
            /// <summary>
            /// ReLU関数
            /// </summary>
            ReLU,
            /// <summary>
            /// 恒等関数
            /// </summary>
            Identity,
            /// <summary>
            /// シグモイド関数
            /// </summary>
            Sigmoid,
            /// <summary>
            /// ソフトマックス関数
            /// </summary>
            SoftMax
        }
        #endregion

        #region ReLU関数
        /// <summary>
        /// ReLU関数
        /// </summary>
        /// <param name="w">ベクトル</param>
        /// <returns>各成分のmax{0,x}をとったベクトル</returns>
        public static DenseMatrix ReLU(DenseMatrix w)
        {
            #region 入力チェック
            if (w.RowCount != 1)
                throw new Exception("活性化関数にはベクトルを入力してください。");
            #endregion

            var result = new DenseMatrix(1, w.ColumnCount);
            for (var i = 0; i < w.ColumnCount; i++)
            {
                result[0, i] = ReLU(w[0, i]);
            }

            return result;
        }

        /// <summary>
        /// ReLU関数
        /// </summary>
        public static double ReLU(double x)
        {
            return Math.Max(0, x);
        }

        /// <summary>
        /// ReLU関数の逆伝播
        /// </summary>
        /// <param name="fromLeft"></param>
        /// <param name="fromRight"></param>
        /// <returns></returns>
        public static DenseMatrix BackReLU(DenseMatrix fromLeft, DenseMatrix fromRight)
        {
            #region 入力チェック
            if (fromLeft.RowCount != 1 || fromRight.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (fromLeft.ColumnCount != fromRight.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            var diff = new DenseMatrix(fromLeft.ColumnCount);
            for (var i = 0; i < fromLeft.ColumnCount; i++)
            {
                diff[0, i] = DReLU(fromLeft[0, i]);
            }

            return fromRight * diff;
        }

        /// <summary>
        /// ReLU関数の微分
        /// </summary>
        public static double DReLU(double x)
        {
            return x > 1 ? 1 : 0;
        }
        #endregion

        #region シグモイド関数
        /// <summary>
        /// シグモイド関数
        /// </summary>
        public static DenseMatrix Sigmoid(DenseMatrix denseMatrix)
        {
            #region 入力チェック
            if (denseMatrix.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");
            #endregion

            var result = new DenseMatrix(1, denseMatrix.ColumnCount);
            for (int i = 0; i < denseMatrix.ColumnCount; i++)
            {
                result[0, i] = Sigmoid(denseMatrix[0, i]);
            }

            return result;
        }

        /// <summary>
        /// シグモイド関数
        /// </summary>
        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        /// <summary>
        /// シグモイド関数の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="fromRight">右からの入力</param>
        /// <returns>左への出力</returns>
        public static DenseMatrix BackSigmoid(DenseMatrix fromLeft, DenseMatrix fromRight)
        {
            #region 入力チェック
            if (fromLeft.RowCount != 1 || fromRight.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (fromLeft.ColumnCount != fromRight.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion
            
            var diff = new DenseMatrix(fromLeft.ColumnCount);
            for (int i = 0; i < fromLeft.ColumnCount; i++)
            {
                diff[i, i] = DSigmoid(fromLeft[0, i]);
            }

            return fromRight * diff;
        }

        /// <summary>
        /// シグモイド関数の微分
        /// </summary>
        public static double DSigmoid(double x)
        {
            return Math.Exp(-x) / ((1 + Math.Exp(-x)) * (1 + Math.Exp(-x)));
        }
        #endregion

        // 以下、出力層向け
        #region 恒等関数
        /// <summary>
        /// 恒等関数
        /// </summary>
        public static DenseMatrix Identity(DenseMatrix denseMatrix)
        {
            #region 入力チェック
            if (denseMatrix.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");
            #endregion

            return denseMatrix;
        }

        /// <summary>
        /// 恒等関数
        /// </summary>
        public static double Identity(double x)
        {
            return x;
        }

        /// <summary>
        /// 恒等関数の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="fromRight">右からの入力</param>
        /// <returns>左への出力</returns>
        public static DenseMatrix BackIdentity(DenseMatrix fromLeft, DenseMatrix fromRight)
        {
            #region 入力チェック
            if (fromLeft.RowCount != 1 || fromRight.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (fromLeft.ColumnCount != fromRight.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            return fromRight;
        }
        #endregion

        #region SoftMax
        /// <summary>
        /// ソフトマックス関数
        /// </summary>
        public static DenseMatrix SoftMax(DenseMatrix denseMatrix)
        {
            #region 入力チェック
            if (denseMatrix.RowCount != 1)
                throw new Exception("活性化関数にはベクトルを入力してください。");
            #endregion

            var listArg = DenseMatrixToList(denseMatrix);

            var c = listArg.Max();
            var exp = listArg.Select(e => e - c);
            var sumExp = exp.Sum();

            var result = exp.Select(e => e / sumExp).ToList();

            return ListToDenseMatrix(result);
        }

        /// <summary>
        /// ソフトマックス関数の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="fromRight">右からの入力</param>
        /// <returns>左への出力</returns>
        public static DenseMatrix BackSoftMax(DenseMatrix fromLeft, DenseMatrix fromRight)
        {
            #region 入力チェック
            if (fromLeft.RowCount != 1 || fromRight.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (fromLeft.ColumnCount != fromRight.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            var output = SoftMax(fromLeft);
            var jacobian = new DenseMatrix(fromLeft.ColumnCount);

            for (var i = 0; i < fromLeft.ColumnCount; i++)
            {
                for (var j = 0; j < fromLeft.ColumnCount; j++)
                {
                    double dnm = 0.0;
                    for (int k = 0; k < fromLeft.ColumnCount; k++)
                    {
                        dnm += Math.Exp(fromLeft[0, k]);
                    }

                    double mlc = 0.0;
                    mlc = Delta(i, j) * Math.Exp(fromLeft[0, i])
                        - output[0, i] * Math.Exp(fromLeft[0, j]);

                    jacobian[i, j] = mlc / dnm;
                }
            }

            return fromRight * jacobian;
        }
        #endregion

        #endregion

        #region 損失関数

        #region デリゲート関連
        /// <summary>
        /// 損失関数のデリゲート
        /// </summary>
        public delegate double LossFunction(DenseMatrix predict, DenseMatrix teacher);

        /// <summary>
        /// 損失関数の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="teacher">教師データ</param>
        /// <returns>左への出力</returns>
        public delegate DenseMatrix BackLossFunction(DenseMatrix fromLeft, DenseMatrix teacher);

        /// <summary>
        /// 引数で指定した損失関数を取得します。
        /// </summary>
        /// <param name="lossFunction">必要な損失関数を指定</param>
        /// <returns>損失関数</returns>
        public static LossFunction GetLossFunction(LossFunctions lossFunction)
        {
            switch (lossFunction)
            {
                case LossFunctions.CrossEntropyError:
                    return CrossEntropyError;
                case LossFunctions.MeanSquaredError:
                    return MeanSquaredError;
                default:
                    throw new Exception("不明な損失関数の指定");
            }
        }

        /// <summary>
        /// 引数で指定しあ尊師つ関数の逆伝播を取得します。
        /// </summary>
        /// <param name="lossFunctions">損失関数</param>
        /// <returns>損失関数の逆伝播</returns>
        public static BackLossFunction GetBackLossFunction(LossFunctions lossFunctions)
        {
            switch (lossFunctions)
            {
                case LossFunctions.CrossEntropyError:
                    return BackCrossEntropyError;
                case LossFunctions.MeanSquaredError:
                    return BackMeanSquaredError;
                default:
                    throw new Exception("不明な損失関数の指定");
            }
        }
        #endregion

        #region 列挙
        /// <summary>
        /// 損失関数
        /// </summary>
        public enum LossFunctions
        {
            /// <summary>
            /// 二乗和誤差
            /// </summary>
            MeanSquaredError,
            /// <summary>
            /// クロスエントロピー誤差
            /// </summary>
            CrossEntropyError
        }
        #endregion

        #region 二乗和誤差
        /// <summary>
        /// 二乗和誤差
        /// </summary>
        /// <param name="predict">予測データ</param>
        /// <param name="teacher">教師データ</param>
        public static double MeanSquaredError(DenseMatrix predict, DenseMatrix teacher)
        {
            #region 入力チェック
            if (predict.RowCount != 1 || teacher.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (predict.ColumnCount != teacher.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            double result = 0.0;

            for (int i = 0; i < predict.ColumnCount; i++)
            {
                result += (predict[0, i] - teacher[0, i]) * (predict[0, i] - teacher[0, i]) / 2;
            }

            return result;
        }

        /// <summary>
        /// 二乗和誤差の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="teacher">教師データ</param>
        /// <returns>左への出力</returns>
        public static DenseMatrix BackMeanSquaredError(DenseMatrix fromLeft, DenseMatrix teacher)
        {
            #region 入力チェック
            if (fromLeft.RowCount != 1 || teacher.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (fromLeft.ColumnCount != teacher.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            return fromLeft - teacher;
        }
        #endregion

        #region クロスエントロピー誤差
        /// <summary>
        /// クロスエントロピー誤差
        /// </summary>
        /// <param name="predict">予測データ</param>
        /// <param name="teacher">教師データ</param>
        /// <returns></returns>
        public static double CrossEntropyError(DenseMatrix predict, DenseMatrix teacher)
        {
            #region 入力チェック
            if (predict.RowCount != 1 || teacher.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (predict.ColumnCount != teacher.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            double result = 0.0;

            for (var i = 0; i < predict.ColumnCount; i++)
            {
                result -= teacher[0, i] * Math.Log(predict[0, i]);
            }

            return result;
        }

        /// <summary>
        /// クロスエントロピー誤差の逆伝播
        /// </summary>
        /// <param name="fromLeft">左からの入力</param>
        /// <param name="teacher">教師データ</param>
        /// <returns>左への出力</returns>
        public static DenseMatrix BackCrossEntropyError(DenseMatrix fromLeft, DenseMatrix teacher)
        {
            #region 入力チェック
            if (fromLeft.RowCount != 1 || teacher.RowCount != 1)
                throw new Exception("ベクトルを入力してください。");

            if (fromLeft.ColumnCount != teacher.ColumnCount)
                throw new Exception("入力値エラー");
            #endregion

            var result = new DenseMatrix(1, fromLeft.ColumnCount);
            for (int i = 0; i < fromLeft.ColumnCount; i++)
            {
                result[0, i] = -teacher[0, 1] / fromLeft[0, i];
            }
            return result;
        }
        #endregion

        #endregion

        #region アフィン層の逆伝播
        /// <summary>
        /// 入力への逆伝播
        /// </summary>
        /// <param name="weight">重み</param>
        /// <param name="fromRight">出力</param>
        /// <returns>入力への逆伝播</returns>
        public static DenseMatrix BackAffineToInput(
            DenseMatrix weight,
            DenseMatrix fromRight)
        {
            return fromRight * (DenseMatrix)weight.Transpose();
        }

        /// <summary>
        /// 重み行列への逆伝播
        /// </summary>
        /// <param name="input">入力</param>
        /// <param name="fromRight">出力</param>
        /// <returns>重み行列への逆伝播</returns>
        public static DenseMatrix BackAffineToWeight(
            DenseMatrix input,
            DenseMatrix fromRight)
        {
            return (DenseMatrix)input.Transpose() * fromRight;
        }

        /// <summary>
        /// バイアスへの逆伝播
        /// </summary>
        /// <param name="fromRight">出力</param>
        /// <returns>バイアスへの逆伝播</returns>
        public static DenseMatrix BackAffineToBias(
            DenseMatrix fromRight)
        {
            return fromRight;
        }
        #endregion

        #region 初期値関数
        /// <summary>
        /// Heの初期値で初期化された行列を返します
        /// </summary>
        /// <param name="i">Row Count</param>
        /// <param name="j">Column Count</param>
        /// <returns>DenseMatrix</returns>
        public static DenseMatrix GetInitializedMatrixByHe(int i, int j)
        {
            #region 引数チェック
            if (i < 1 || j < 1)
                throw new Exception("初期値には1以上の整数を入力してください。");
            #endregion

            var result = new DenseMatrix(i, j);

            var standardDivertion = Math.Sqrt(2.0 / i);
            var meanValue = 0.0;

            for (var p = 0; p < i; p++)
            {
                for (var q = 0; q < j; q++)
                {
                    result[p, q] = GetNormRandom(meanValue, standardDivertion);
                }
            }

            return result;
        }
        #endregion

        #region その他便利関数

        #region 乱数関係
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

        #region List⇔DenseMatrix
        /// <summary>
        /// DenseMatrix(ベクトル)からリストへ
        /// </summary>
        public static List<double> DenseMatrixToList(DenseMatrix denseMatrix)
        {
            #region 入力チェック
            if (denseMatrix.RowCount != 1)
                throw new Exception("リスト型にキャストできないDenseMatrixです");
            #endregion

            var result = new List<double>();
            for (var i = 0; i < denseMatrix.ColumnCount; i++)
            {
                result.Add(denseMatrix[0, i]);
            }

            return result;
        }

        /// <summary>
        /// リストからDenseMatrix(ベクトル)へ
        /// </summary>
        public static DenseMatrix ListToDenseMatrix(List<double> vs)
        {
            var result = new DenseMatrix(1, vs.Count);
            for (int i = 0; i < vs.Count; i++)
            {
                result[0, i] = vs[i];
            }

            return result;
        }
        #endregion

        #region デルタ関数
        /// <summary>
        /// i == jなら1, i != jなら0を返す関数
        /// </summary>
        public static double Delta(int i, int j)
        {
            return i == j ? 1 : 0;
        }
        #endregion

        #endregion
    }
}
