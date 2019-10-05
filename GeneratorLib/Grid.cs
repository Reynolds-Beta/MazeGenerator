namespace GeneratorLib
{
    using System;
    using System.Collections.Generic;

    public class Grid
    {
        // Creates a 2-dimensional grid of cells with connections
        // to each other. These connected cells are called a cellset.

        // Initially, all cells are separate "sets" meaning they
        // have no connections. As connections are made, the sets
        // are unioned together.

        // Eventually, you will only have one set meaning all of
        // the cells in the map are connected in one way or another
        // to every other cell in the map.

        private Random rnd = new Random(1);
        private int width = 0;
        private int height = 0;
        private Dictionary<long, HashSet<long>> Sets;

        public Cell[,] Cells;

        private Grid(int height, int width)
        {
            this.width = width;
            this.height = height;
            this.Cells = new Cell[height, width];
            this.Cells.Initialize();
            this.Sets = new Dictionary<long, HashSet<long>>();
        }

        public static Grid Create(int height, int width)
        {
            var grid = new Grid(height, width);

            for (var y = 0; y < height; y++)
            {
                if (y == 0)
                {
                    grid.AddRow(y);
                }

                grid.MergeAcross(y);
                grid.MergeDown(y);
            }

            grid.Sets.Clear();
            return grid;
        }

        private long AddCell(int y, int x)
        {
            this.Cells[y, x] = new Cell(y, x, this.width);
            return this.Cells[y, x].Id;
        }

        private Cell GetCellById(long id)
        {
            var y = id / this.width;
            var x = id % this.width;
            return this.Cells[y, x];
        }

        private void AddRow(int y)
        {
            for (var x = 0; x < this.width; x++)
            {
                var id = this.AddCell(y, x);
                var set = new HashSet<long>();
                set.Add(id);
                this.Sets.Add(id, set);
            }
        }

        // Creates connections from one cell to another, in the same row
        private void MergeAcross(int y)
        {
            for (var x = 0; x < this.width - 1; x++)
            {
                var curCell = this.Cells[y, x];
                var curCellSet = this.GetSetContains(curCell.Id);
                var rightCell = this.Cells[y, x + 1];
                var rightCellSet = this.GetSetContains(rightCell.Id);

                var doCellSetMerge = (this.rnd.Next(0, 2) == 0 || y == (height - 1));
                if (doCellSetMerge && curCellSet != rightCellSet)
                {
                    if (curCellSet < rightCellSet)
                    {
                        this.Sets[curCellSet].UnionWith(this.Sets[rightCellSet]);
                        this.Sets.Remove(rightCellSet);
                    }
                    else
                    {
                        this.Sets[rightCellSet].UnionWith(this.Sets[curCellSet]);
                        this.Sets.Remove(curCellSet);
                    }

                    curCell.Right = true;
                    rightCell.Left = true;
                }
                else
                {
                    curCell.Right = false;
                    rightCell.Left = false;
                }
            }
        }

        // Creates connections from one row to the next row down
        private void MergeDown(int y)
        {
            if (y == this.height - 1)
            {
                return;
            }

            // Obtain the cellsets for each cell in the cur row
            var curRowSets = GetCellSetsForRow(y);

            // Create the next row, with. Nothing curly connects to this
            this.AddRow(y + 1);

            // Now, go through each cell set and pick 1 or more to
            // make a connection down to the next row
            foreach (var curSet in curRowSets)
            {
                int i = 0;
                foreach (var cell in curSet.Value)
                {
                    // curSet.Value.ElementAt(this.rnd.Next(curSet.Value.Count));
                    var curCell = curSet.Value[this.rnd.Next(curSet.Value.Count)];
                    var bottomCell = this.Cells[curCell.Y + 1, curCell.X];

                    var bottomSet = this.GetSetContains(bottomCell.Id);
                    var doCellSetMerge = (this.rnd.Next(0, 2) == 0 || i == 0) && curSet.Key != bottomSet;
                    if (doCellSetMerge)
                    {
                        this.Sets[curSet.Key].UnionWith(this.Sets[bottomSet]);
                        this.Sets.Remove(bottomSet);
                        curCell.Bottom = true;
                        bottomCell.Top = true;
                    }

                    i++;
                }
            }
        }

        private long GetSetContains(long cellId)
        {
            if (this.Sets.ContainsKey(cellId))
            {
                return cellId;
            }

            foreach(var set in this.Sets)
            {
                if (set.Value.Contains(cellId))
                {
                    return set.Key;
                }
            }

            return -1;
        }

        private Dictionary<long, List<Cell>> GetCellSetsForRow(int y)
        {
            // Obtain the cellsets for each cell in the cur row
            var curRowSets = new Dictionary<long, List<Cell>>();
            long minCellIdInRow = y * this.height;

            for (var x = 0; x < this.width; x++)
            {
                var curSetKey = this.GetSetContains(this.Cells[y, x].Id);
                if (!curRowSets.ContainsKey(curSetKey))
                {
                    curRowSets.Add(curSetKey, new List<Cell>());
                }

                curRowSets[curSetKey].Add(this.Cells[y, x]);
            }

            return curRowSets;
        }
    }
}
