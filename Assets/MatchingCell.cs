using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MatchingCellExtensions
{
    public class MatchingCell
    {
        public GemBehaviour gemInCell = null;

        private MatchingCell[] childrenArr = { null, null, null, null };
        private int numChildren = 0;
        public MatchingCell(GemBehaviour gem)
        {
            gemInCell = gem;
            gemInCell.visited = true;
        }
        public void AddChild(MatchingCell cell)
        {
            if (numChildren < 4)
            {
                childrenArr[numChildren] = cell;
                numChildren++;
            }
        }

        public IEnumerable<MatchingCell> Children()
        {
            int pos = 0;
            while (childrenArr[pos] != null)
            {
                yield return childrenArr[pos];
                pos++;
            }
        }
    }

    public static class MatchingCellsSearch
    {
        private static List<List<MatchingCell>> MatchingCellsFound = new List<List<MatchingCell>>();

        public static int VisitCell(this StartBehaviour table, (int, int) cell)
        {
            var gem = table.GetCell(cell);
            int count = 0;
            List<MatchingCell> sequence = new List<MatchingCell>();
            if (!gem.visited && gem.isPartOfaLineOrColumn)
            {
                Stack<MatchingCell> cellStack = new Stack<MatchingCell>();
                MatchingCell root = new MatchingCell(gem);
                count++;
                cellStack.Push(root);
                while (cellStack.Count != 0)
                {
                    var top = cellStack.Pop();
                    (int i, int j) = top.gemInCell.tablePosition;
                    (int, int)[] cross = { (i - 1, j), (i, j + 1), (i + 1, j), (i, j - 1) };
                    foreach ((int ic, int jc) in cross)
                    {
                        if ((0 <= ic) && (ic < 8) && (0 <= jc) && (jc < 8))
                        {
                            var gemChild = table.GetCell((ic, jc));
                            if (!gemChild.visited && gemChild.isPartOfaLineOrColumn && (gemChild.gemValue == gem.gemValue))
                            {
                                MatchingCell childCell = new MatchingCell(gemChild);
                                top.AddChild(childCell);
                                count++;
                                cellStack.Push(childCell);
                            }
                        }
                    }
                    sequence.Add(top);
                }
                if (sequence.Count != 0)
                {
                    MatchingCellsFound.Add(sequence);
                }
            }

            return count;
        }
        public static void UpdateMatchingGems()
        {
            //Stack<MatchingCell> cellStack = new Stack<MatchingCell>();
            foreach (var sequence in MatchingCellsFound)
            {
                foreach( var matchCell in sequence)
                {
                    matchCell.gemInCell.SetToExplode();
                }
                //cellStack.Push(root);
                //while(cellStack.Count != 0)
                //{
                //    var top = cellStack.Pop();
                //    //top
                //    foreach(var child in root.Children())
                //    {
                //        cellStack.Push(child);
                //    }
                //}
            }
            MatchingCellsFound.Clear();
        }
    }
}
