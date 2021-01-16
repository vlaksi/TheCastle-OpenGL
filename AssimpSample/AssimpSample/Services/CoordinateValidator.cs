namespace AssimpSample.Services
{
    public class CoordinateValidator
    {
        public int Boundary { get; set; }

        public CoordinateValidator(int boundary = 80)
        {
            // 80 zato sto sam to primetio debagovanjem, nisam siguran da li je ovo najpametniji nacin, def treba proveriti
            // TODO: Proveriti jel okej ovako
            Boundary = boundary;

        }

        public bool ValidDownRotate(float currentRotationX)
        {
            return currentRotationX >= -Boundary;
        }

        public bool ValidUpRotate(float currentRotationX)
        {
            return currentRotationX <= Boundary;
        }

        public bool ValidLeftRotate(float currentRotationY)
        {
            return currentRotationY >= -Boundary;
        }

        public bool ValidRightRotate(float currentRotationY)
        {
            return currentRotationY <= Boundary;
        }
    }
}