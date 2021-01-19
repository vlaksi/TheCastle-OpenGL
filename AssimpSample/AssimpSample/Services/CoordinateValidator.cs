namespace AssimpSample.Services
{
    public class CoordinateValidator
    {
        public int BoundaryUp { get; set; }
        public int BoundaryDown { get; set; }

        public int BoundaryLeft { get; set; }
        public int BoundaryRight { get; set; }

        public CoordinateValidator(int boundaryUp = 145, int boundaryDown = 20, int boundaryLeft=90, int boundaryRight=90)
        {
            // 80 zato sto sam to primetio debagovanjem, nisam siguran da li je ovo najpametniji nacin, def treba proveriti
            // TODO: Proveriti jel okej ovako
            BoundaryUp = boundaryUp;
            BoundaryDown = boundaryDown;
            BoundaryLeft = boundaryLeft;
            BoundaryRight = boundaryRight;
        }

        public bool ValidDownRotate(float currentRotationX)
        {
            return currentRotationX > -BoundaryDown;
        }

        public bool ValidUpRotate(float currentRotationX)
        {
            return currentRotationX < BoundaryUp;
        }

        public bool ValidLeftRotate(float currentRotationY)
        {
            return currentRotationY > -BoundaryLeft;
        }

        public bool ValidRightRotate(float currentRotationY)
        {
            return currentRotationY < BoundaryRight;
        }
    }
}