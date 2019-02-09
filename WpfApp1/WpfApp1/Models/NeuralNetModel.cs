using Livet;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using WpfApp1.Utils;

namespace WpfApp1.Models
{
    class NeuralNetModel
    {
        #region 定数
        /// <summary>
        /// 学習率
        /// </summary>
        private const double LEARNING_RATE = 0.01;
        #endregion

        #region プロパティ
        /// <summary>
        /// 重み行列のリスト
        /// </summary>
        public List<DenseMatrix> Weights { get; set; }

        /// <summary>
        /// バイアスのリスト
        /// </summary>
        public List<DenseMatrix> Biases { get; set; }

        /// <summary>
        /// 活性化関数
        /// </summary>
        private Functions.ActivationFt _ActivationFt { get; set; }

        /// <summary>
        /// 出力層の活性化関数
        /// </summary>
        private Functions.ActivationFt _LastActivationFt { get; set; }

        /// <summary>
        /// 損失関数
        /// </summary>
        private Functions.LossFunction _LossFunction { get; set; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// モデルを構築します。
        /// </summary>
        /// <param name="vs">ノード数のリスト</param>
        /// <param name="activator">活性化関数</param>
        /// <param name="lastActivator">出力層に用いる活性化関数</param>
        /// <param name="lossFunction">損失関数</param>
        /// <remarks>
        /// vsは長さ2以上の正の整数のリストを指定してください。
        /// 先頭の整数は入力値の次元、末尾の整数は出力値の次元を意味します。
        /// </remarks>
        public NeuralNetModel(List<int> vs,
            Functions.Activators activator = Functions.Activators.ReLU,
            Functions.Activators lastActivator = Functions.Activators.SoftMax,
            Functions.LossFunctions lossFunction = Functions.LossFunctions.CrossEntropyError)
        {
            #region 引数チェック
            if (vs.Count < 2)
                throw new Exception("長さ2未満のリストは指定できません。");

            foreach (int v in vs)
            {
                if (v < 1)
                    throw new Exception("1未満の整数は指定できません。");
            }
            #endregion

            #region モデルの構築
            Weights = new List<DenseMatrix>();
            Biases = new List<DenseMatrix>();

            for (int i = 1; i < vs.Count; i++)
            {
                Weights.Add(Functions.GetInitializedMatrixByHe(vs[i - 1], vs[i]));
                Biases.Add(Functions.GetInitializedMatrixByHe(1, vs[i]));
            }
            #endregion

            #region 活性化関数と損失関数
            _ActivationFt = Functions.GetActivation(activator);

            _LastActivationFt = Functions.GetActivation(lastActivator);

            _LossFunction = Functions.GetLossFunction(lossFunction);
            #endregion
        }
        #endregion

        #region 順伝播
        /// <summary>
        /// 順伝播
        /// </summary>
        /// <param name="input">入力値</param>
        /// <returns>出力値</returns>
        public DenseMatrix Forward(DenseMatrix input)
        {
            #region 入力値チェック
            if (input.RowCount != 1
                || input.ColumnCount != Weights[0].RowCount)
            {
                throw new Exception("入力値の次元が不正です");
            }
            #endregion

            var ipt = new DenseMatrix(1, input.ColumnCount);

            // 一段階ごとの計算結果を保持しておく変数
            var mediumVectors = new List<DenseMatrix>
            {
                ipt * Weights[0] + Biases[0]
            };

            for (int i = 1; i < Weights.Count; i++)
            {
                var previous = _ActivationFt(mediumVectors.Last());
                
                mediumVectors.Add(previous * Weights[i] + Biases[i]);
            }

            return _LastActivationFt(mediumVectors.Last() * Weights.Last() + Biases.Last());
        }

        /// <summary>
        /// 順伝播を計算し、損失を求めます
        /// </summary>
        /// <param name="predict">予測データ</param>
        /// <param name="input">入力データ</param>
        /// <param name="teacher">教師データ</param>
        /// <returns>損失</returns>
        public double Forward(out DenseMatrix predict, DenseMatrix input, DenseMatrix teacher)
        {
            predict = Forward(input);
            var result = _LossFunction(predict, teacher);

            return result;
        }
        #endregion

        #region 逆伝播
        /// <summary>
        /// 逆伝播
        /// </summary>
        /// <param name="input">入力値</param>
        /// <param name="teacher">教師データ</param>
        public void BackPropagation(DenseMatrix input, DenseMatrix teacher)
        {
            #region 入力値チェック
            if (input.RowCount != 1
                || input.ColumnCount != Weights.First().RowCount)
            {
                throw new Exception("入力値の次元が不正です");
            }

            if (teacher.RowCount != 1
                || teacher.ColumnCount != Weights.Last().ColumnCount)
            {
                throw new Exception("教師データの次元が不正です");
            }
            #endregion

            // 微分を保持する変数
            var dLdW = new List<DenseMatrix>();
            var dLdB = new List<DenseMatrix>();

            #region 順伝播
            var ipt = new DenseMatrix(1, input.ColumnCount);

            // 一段階ごとの計算結果を保持しておく変数
            var mediumVectors = new List<DenseMatrix>
            {
                ipt * Weights[0] + Biases[0]
            };

            for (int i = 1; i < Weights.Count; i++)
            {
                var previous = _ActivationFt(mediumVectors.Last());

                mediumVectors.Add(previous * Weights[i] + Biases[i]);
            }

            mediumVectors.Add(mediumVectors.Last() * Weights.Last() + Biases.Last());

            var L = _LossFunction(mediumVectors.Last(), teacher);
            #endregion

            // todo 逆に伝播していく様をえがいていく
        }
        #endregion
    }
}
