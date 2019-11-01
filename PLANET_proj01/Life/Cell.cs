using System.Collections.Generic;

namespace PLANET_proj01.Life
{
    internal class Cell
    {
        public (int x, int y) pos { get; set; }
        public CellState state { get; private set; }
        public bool born { get; private set; } = true;
        public bool died { get; private set; } = false;
        public bool endangered { get; private set; }

        private int alive = 0;
        private Cell[,] neighbours;

        private CellState futureState;

        public Cell()
        {
            state = CellState.Alive;
        }

        public void ChangeState(IEnumerable<Cell> allCells, List<int> BornCondt, List<int> DeadCondt, int dieWhenAliveFor)
        {
            var aliveNeighbours = SetNeighbours(allCells);
            if (state == CellState.Dead && BornCondt.Contains(aliveNeighbours))
            {
                futureState = CellState.Alive;
            }
            else if (state == CellState.Alive && (DeadCondt.Contains(aliveNeighbours) || dieWhenAliveFor == alive))
            {
                futureState = CellState.Dead;
            }
            else
                futureState = state;
            alive++;

            if (state == CellState.Dead && futureState == CellState.Alive)
                born = true;
            else born = false;
            if (state == CellState.Alive && futureState == CellState.Dead)
                died = true;
            else died = false;
        }

        public void SetEndangeredState(IEnumerable<Cell> allCells, List<int> BornCondt, List<int> DeadCondt, int dieWhenAliveFor)
        {
            var aliveNeighbours = SetNeighbours(allCells);
            if (state == CellState.Alive && (DeadCondt.Contains(aliveNeighbours) || dieWhenAliveFor == alive + 1))
            {
                endangered = true;
            }
            else
                endangered = false;
        }

        public void UpdateState()
        {
            state = futureState;
        }

        public IEnumerable<Cell> GetDeadCells()
        {
            var deadCells = new List<Cell>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0) && neighbours[x + 1, y + 1] == null)
                    {
                        deadCells.Add(new Cell() { pos = (x: pos.x + x, y: pos.y + y), state = CellState.Dead });
                    }
                }
            }
            return deadCells;
        }

        private int SetNeighbours(IEnumerable<Cell> allCells)
        {
            int found = 0;
            neighbours = new Cell[3, 3];
            foreach (var cell in allCells)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (!(x == 0 && y == 0) && pos.x + x == cell.pos.x && pos.y + y == cell.pos.y)
                        {
                            neighbours[x + 1, y + 1] = cell;
                            if (cell.state == CellState.Alive)
                                found++;
                        }
                    }
                }
            }
            return found;
        }
    }
}