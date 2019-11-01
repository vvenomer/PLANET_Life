using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PLANET_proj01.Life
{
    internal class GameOfLife
    {
        public List<Cell> allCells = new List<Cell>();
        public List<int> BornCondt = new List<int>() { 3 };
        public List<int> DeadCondt = new List<int>() { 0, 1, 4, 5, 6, 7, 8 };
        public int dieWhenAliveFor = -1;

        public GameOfLife()
        {
        }

        public GameOfLife(List<(int x, int y)> startingPositions)
        {
            AddCells(startingPositions);
        }

        public void AddCells(List<(int x, int y)> startingPositions)
        {
            allCells = allCells.Concat(startingPositions.Select(pos => new Cell() { pos = (pos.x, pos.y) })).ToList();
        }

        public void DoTick()
        {
            var deadCells = new Dictionary<(int x, int y), Cell>();
            allCells = allCells.Where(c => !c.died).ToList();
            foreach (var cell in allCells)
            {
                cell.ChangeState(allCells, BornCondt, DeadCondt, dieWhenAliveFor);
                foreach (var deadCell in cell.GetDeadCells())
                {
                    deadCells.TryAdd(deadCell.pos, deadCell);
                }
            }
            foreach (var cell in deadCells)
            {
                cell.Value.ChangeState(allCells, BornCondt, DeadCondt, dieWhenAliveFor);
            }
            var newCells = allCells.Concat(deadCells.Select(d => d.Value));
            foreach (var cell in newCells)
            {
                cell.UpdateState();
            }
            allCells = newCells.Where(c => c.died || c.state == CellState.Alive).ToList();
            foreach (var cell in allCells)
            {
                cell.SetEndangeredState(allCells, BornCondt, DeadCondt, dieWhenAliveFor);
            }
        }

        public void SetEndangeredStates()
        {
            foreach (var cell in allCells)
            {
                cell.SetEndangeredState(allCells, BornCondt, DeadCondt, dieWhenAliveFor);
            }
        }


        public double DisplayAxisMiddleCoordinate(Func<(int x, int y), int> axis)
        {
            try
            {
                return allCells.Average(c => axis(c.pos));
            }
            catch
            {
                return 0;
            }
        }

        public void SaveState()
        {
            var json = JsonConvert.SerializeObject(allCells);
            using var writer = new StreamWriter("save.json");
            writer.WriteLine(json);
        }

        public void LoadState()
        {
            using var reader = new StreamReader("save.json");
            allCells = JsonConvert.DeserializeObject<List<Cell>>(reader.ReadToEnd());
        }
    }
}