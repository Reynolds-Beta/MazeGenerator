namespace GeneratorLib
{
    public class Cell
    {
        public int X;
        public int Y;
        public long Id;
        public bool Right;
        public bool Left;
        public bool Bottom;
        public bool Top;

        public Cell(int y, int x, int rowWidth)
        {
            this.Id = (y * rowWidth) + x;
            this.X = x;
            this.Y = y;
        }
    }
}
