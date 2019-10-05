namespace SampleApp
{
    using System;
    using GeneratorLib;

    class Program
    {
        static void Main(string[] args)
        {
            DoGrid1(args);
        }

        static void DoGrid1(string[] args)
        {
            ////int height = 15;
            ////int width = 90;

            int height = 1000;
            int width = 1000;

            var grid = Grid.Create(height, width);

            for (var y = 0; y < height && y < 15; y++)
            {
                var row = string.Empty;
                var next = string.Empty;
                for (var x = 0; x < width && x < 90; x++)
                {
                    row += $"O";
                    if (grid.Cells[y, x].Right)
                    {
                        row += "--";
                    }
                    else
                    {
                        row += "  ";
                    }


                    if (grid.Cells[y, x].Bottom)
                    {
                        next += "|  ";
                    }
                    else
                    {
                        next += "   ";
                    }
                }

                Console.WriteLine(row);
                Console.WriteLine(next);
            }
        }
    }
}
