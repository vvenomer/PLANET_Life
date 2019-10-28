using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PLANET_proj01.Life
{
    class GameOfLife
    {
        public List<Cell> allCells = new List<Cell>();

        public GameOfLife(List<(int x, int y)> startingPositions)
        {
            foreach (var pos in startingPositions)
            {
                allCells.Add(new Cell() { pos = (pos.x, pos.y) });
            }
        }

        public void DoTick()
        {
            var deadCells = new Dictionary<(int x, int y), Cell>();
            foreach (var cell in allCells)
            {
                cell.ChangeState(allCells);
                foreach (var deadCell in cell.GetDeadCells())
                {
                    deadCells.TryAdd(deadCell.pos, deadCell);
                }
            }
            foreach (var cell in deadCells)
            {
                cell.Value.ChangeState(allCells);
            }
            var newCells = allCells.Concat(deadCells.Select(d => d.Value));
            foreach (var cell in newCells)
            {
                cell.UpdateState();
            }
            allCells = newCells.Where(c => c.state == CellState.Alive).ToList();
            foreach (var cell in allCells)
            {
                cell.SetEndangeredState(allCells);
            }
        }

        public double DisplayAxisMiddleCoordinate(Func<(int x, int y), int> axis)
        {
            return allCells.Average(c => axis(c.pos));
        }

        public void SaveState()
        {
            var json = JsonConvert.SerializeObject(allCells);
            using (var writer = new StreamWriter("save.json"))
            {
                writer.WriteLine(json);
            }
        }

        public void LoadState()
        {
            using (var reader = new StreamReader("save.json"))
            {
                allCells = JsonConvert.DeserializeObject<List<Cell>>(reader.ReadToEnd());
            }
        }
    }
}
