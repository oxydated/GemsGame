using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnGapsControl
{
    public static class ColumnGaps
    {
        private static (int, int)[] columnGapsArray = { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) };

        private static bool thereIsNoGaps = true;

        //private static bool[] columnIsFalling = { false, false, false, false, false, false, false, false };
        public static void CheckForGapsToFill(this StartBehaviour table)
        {
            thereIsNoGaps = true;

            for (int i = 0; i < 8; i++)
            {
                bool startNotFound = true;
                bool ceilingNotFound = false;

                (int topOfColumn, int heightOfGap) = (-1, 0);

                for (int j = 0; j < 8; j++)
                {
                    if (startNotFound)
                    {
                        if (table.GetCell((i, j)).gemValue != 0)
                        {
                            topOfColumn = j;
                        }
                        else
                        {
                            startNotFound = false;
                            ceilingNotFound = true;
                            heightOfGap = 1;
                            thereIsNoGaps = false;
                        }
                    }
                    else
                    {
                        if (ceilingNotFound)
                        {
                            if (table.GetCell((i, j)).gemValue == 0)
                            {
                                heightOfGap++;
                            }
                            else
                            {
                                ceilingNotFound = false;
                            }
                        }
                    }
                }

                //if(heightOfGap > 0)
                //{
                //    columnIsFalling[i] = true;
                //}

                columnGapsArray[i] = (topOfColumn, heightOfGap);
            }
        }

        public static bool isThereGaps()
        {
            return !thereIsNoGaps;
        }

        public static IEnumerable<(int, int, int)> GetFallingColumns()
        {

            for (int i = 0; i < 8; i++)
            {
                (int topOfColumn, int heightOfGap) = columnGapsArray[i];
                (int, int, int) retColumn = (i, topOfColumn, heightOfGap);
                if(heightOfGap > 0)
                {
                    columnGapsArray[i] = (topOfColumn, heightOfGap - 1);
                }
                yield return retColumn;
            }


        }
    }
}
