using Livet;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;
using WpfApp1.Utils;

namespace WpfApp1.Models
{
    public class Model : NotificationObject
    {
        #region メンバ変数
        private readonly NeuralNetModel _neuralNetModel;
        #endregion

        #region コンストラクタ
        public Model()
        {
            List<int> vs = new List<int> { 2, 100, 50, 20, 119, 1 };
            _neuralNetModel = new NeuralNetModel(
                vs,
                Functions.Activations.ReLU,
                Functions.Activations.Identity,
                Functions.LossFunctions.MeanSquaredError);
        }
        #endregion

        #region メソッド
        public double Forward(double x, double y)
        {
            var input = new DenseMatrix(1, 2);
            input[0, 0] = x / 10;
            input[0, 1] = y / 10;

            return _neuralNetModel.Forward(input)[0, 0] * 100;
        }

        public double GetLoss(int x, int y)
        {
            var input = new DenseMatrix(1, 2);
            input[0, 0] = x / 10;
            input[0, 1] = y / 10;
            var teacher = new DenseMatrix(1);
            teacher[0, 0] = x * y / 100;

            var predict = new DenseMatrix(1);

            return _neuralNetModel.Forward(out predict, input, teacher);
        }

        public void Train(double x, double y, double teacher)
        {
            var input = new DenseMatrix(1, 2);
            input[0, 0] = x / 10;
            input[0, 1] = y / 10;

            var t = new DenseMatrix(1);
            t[0, 0] = teacher / 100;

            _neuralNetModel.BackPropagation(input, t);
        }
        #endregion
    }
}
