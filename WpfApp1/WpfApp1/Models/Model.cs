using Livet;
using MathNet.Numerics.LinearAlgebra.Double;

namespace WpfApp1.Models
{
    public class Model : NotificationObject
    {
        #region メンバ変数
        private NNModel nnModel;
        #endregion

        #region コンストラクタ
        public Model()
        {
            nnModel = new NNModel();
        }
        #endregion

        #region メソッド
        public double Forward(double x, double y)
        {
            var input = new DenseMatrix(1, 2);
            input[0, 0] = x;
            input[0, 1] = y;

            return nnModel.Forward(input);
        }

        public void Train(double x, double y, double teacher)
        {
            var input = new DenseMatrix(1, 2);
            input[0, 0] = x;
            input[0, 1] = y;

            nnModel.Train(input, teacher);
        }
        #endregion
    }
}
