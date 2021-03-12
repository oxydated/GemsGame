using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using MatchingCellExtensions;
using ColumnGapsControl;
public class StartBehaviour : MonoBehaviour
{
    public GameObject cellPrefab;
    private GameObject[,] cellGems = new GameObject[8, 8];

    private bool areGemsFalling = false;

    //private int[,] visitedTable = new int[8, 8];
    //private int[,] countMatchsTable = new int[8, 8];

    //private readonly Object lockSwap = new Object();
    //private bool isAlreadySwaping = false;

    //private (int, int) swapSource = (-1, -1);
    //private (int, int) swapTarget = (-1, -1);

    // Start is called before the first frame update
    public GemBehaviour GetCell((int, int) cellPos)
    {
        return cellGems[cellPos.Item1, cellPos.Item2].GetComponent<GemBehaviour>();
    }
    void Start()
    {
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 8; i++)
            {
                int[] forbidenValues = { 0, 0, 0, 0, 0, 0 };
                int[] allowedValues = { 0, 0, 0, 0, 0, 0 };
                var NewCell = GameObject.Instantiate(cellPrefab, this.gameObject.transform);
                NewCell.transform.position = NewCell.transform.TransformVector(new Vector3((float)i - 3.5f, (float)j - 3.5f, 0.0f));
                NewCell.name = "Cell_" + i + "_" + j;
                cellGems[i, j] = NewCell;
                //cellGems[i, j] = Random.Range(0, 5);
                if ((i > 1) && (cellGems[i - 1, j].GetComponent<GemBehaviour>().gemValue == cellGems[i - 2, j].GetComponent<GemBehaviour>().gemValue))
                {
                    forbidenValues[cellGems[i - 2, j].GetComponent<GemBehaviour>().gemValue] = 1;
                }
                if ((j > 1) && (cellGems[i, j - 1].GetComponent<GemBehaviour>().gemValue == cellGems[i, j - 2].GetComponent<GemBehaviour>().gemValue))
                {
                    forbidenValues[cellGems[i, j - 2].GetComponent<GemBehaviour>().gemValue] = 1;
                }
                int totalAllowedValues = 0;
                for (int k = 0; k < 6; k++)
                {
                    if (forbidenValues[k] == 0)
                    {
                        allowedValues[totalAllowedValues++] = k;
                    }
                }
                NewCell.GetComponent<GemBehaviour>().tablePosition = (i, j);
                NewCell.GetComponent<GemBehaviour>().SetGemValue(allowedValues[Random.Range(1, totalAllowedValues - 1)]);
                NewCell.GetComponent<GemBehaviour>().DoDrag += DragCells;
                NewCell.GetComponent<GemBehaviour>().TestMatch += SweepTable;
                //NewCell.GetComponent<GemBehaviour>().FinishDrag += SwapMotionFinished;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateTable()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                cellGems[i, j].GetComponent<GemBehaviour>().SetGemValue(1);
            }
        }
    }

    GemBehaviour DragCells((int, int) swapSource, GemBehaviour.swapDirection dir)
    {
        GemBehaviour swapTarget = null;
        int i = swapSource.Item1;
        int j = swapSource.Item2;

        switch (dir)
        {
            case GemBehaviour.swapDirection.RIGHT:
                if (i < 7)
                {
                    //cellGems[i, j].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.RIGHT);
                    //cellGems[i + 1, j].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.LEFT);
                    swapTarget = cellGems[i + 1, j].GetComponent<GemBehaviour>();
                }
                break;
            case GemBehaviour.swapDirection.UP:
                if (j < 7)
                {
                    //cellGems[i, j].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.UP);
                    //cellGems[i, j + 1].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.DOWN);
                    swapTarget = cellGems[i, j + 1].GetComponent<GemBehaviour>();
                }
                break;
            case GemBehaviour.swapDirection.LEFT:
                if (i > 0)
                {
                    //cellGems[i, j].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.LEFT);
                    //cellGems[i - 1, j].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.RIGHT);
                    swapTarget = cellGems[i - 1, j].GetComponent<GemBehaviour>();
                }
                break;
            case GemBehaviour.swapDirection.DOWN:
                if (j > 0)
                {
                    //cellGems[i, j].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.DOWN);
                    //cellGems[i, j - 1].GetComponent<GemBehaviour>().PlaySwap(GemBehaviour.swapDirection.UP);
                    swapTarget = cellGems[i, j - 1].GetComponent<GemBehaviour>();
                }
                break;
        }
        return swapTarget;
    }

    //void SwapMotionFinished()
    //{
    //    lock (lockSwap)
    //    {
    //        if (isAlreadySwaping)
    //        {
    //            int swapVal = cellGems[swapSource.Item1, swapSource.Item2].GetComponent<GemBehaviour>().gemValue;
    //            cellGems[swapSource.Item1, swapSource.Item2].GetComponent<GemBehaviour>().SetGemValue(
    //                cellGems[swapTarget.Item1, swapTarget.Item2].GetComponent<GemBehaviour>().gemValue
    //                );
    //            cellGems[swapTarget.Item1, swapTarget.Item2].GetComponent<GemBehaviour>().SetGemValue(swapVal);

    //            cellGems[swapTarget.Item1, swapTarget.Item2].GetComponent<GemBehaviour>().EndSwap();
    //            cellGems[swapSource.Item1, swapSource.Item2].GetComponent<GemBehaviour>().EndSwap();

    //        }
    //        isAlreadySwaping = false;
    //    }
    //}
    int SweepTable()
    {
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 8; i++)
            {
                //visitedTable[i, j] = 0;
                //countMatchsTable[i, j] = 0;
                GetCell((i, j)).isPartOfaLineOrColumn = false;
                GetCell((i, j)).visited = false;
            }
        }

        for (int j = 0; j < 8; j++)
        {
            int countGemsMatching = 1;
            for (int i = 1; i < 8; i++)
            {
                var currentGem = GetCell((i, j));
                var previousGem = GetCell((i-1, j));
                int previousCell = i - 1;
                if (currentGem.gemValue != 0 && currentGem.gemValue == previousGem.gemValue)
                {
                    countGemsMatching++;
                }
                else
                {
                    if(countGemsMatching >= 3)
                    {
                        for(int k = 0; k < countGemsMatching; k++)
                        {
                            GetCell((previousCell - k, j)).isPartOfaLineOrColumn = true;
                        }
                    }
                    countGemsMatching = 1;
                }
            }
            if (countGemsMatching >= 3)
            {
                for (int k = 0; k < countGemsMatching; k++)
                {
                    GetCell((7 - k, j)).isPartOfaLineOrColumn = true;
                }
            }
        }

        for (int i = 0; i < 8; i++)
        {
            int countGemsMatching = 1;
            for (int j = 1; j < 8; j++)
            {
                var currentGem = GetCell((i, j));
                var previousGem = GetCell((i, j - 1));
                int previousCell = j - 1;
                if (currentGem.gemValue != 0 && currentGem.gemValue == previousGem.gemValue)
                {
                    countGemsMatching++;
                }
                else
                {
                    if (countGemsMatching >= 3)
                    {
                        for (int k = 0; k < countGemsMatching; k++)
                        {
                            GetCell((i, previousCell - k)).isPartOfaLineOrColumn = true;
                        }
                    }
                    countGemsMatching = 1;
                }
            }
            if (countGemsMatching >= 3)
            {
                for (int k = 0; k < countGemsMatching; k++)
                {
                    GetCell((i, 7 - k)).isPartOfaLineOrColumn = true;
                }
            }
        }

        int cellsMatching = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                cellsMatching += this.VisitCell((i, j));
            }
        }
        MatchingCellsSearch.UpdateMatchingGems();

        this.CheckForGapsToFill();

        GemBehaviour.allowGemsSwapping(!ColumnGaps.isThereGaps());

        return cellsMatching;
    }

    public void UpdateFallingColumns()
    {
        if (ColumnGaps.isThereGaps())
        {
            areGemsFalling = true;
            GemBehaviour.allowGemsSwapping(false);

            foreach ((int i, int topOfColumn, int heightOfGap) in ColumnGapsControl.ColumnGaps.GetFallingColumns())
            {
                for (int j = topOfColumn + 1; j < 8; j++)
                {
                    if (heightOfGap > 0)
                    {
                        int nextGemValue = 0;
                        if (j < 7)
                        {
                            nextGemValue = GetCell((i, j + 1)).gemValue;
                        }
                        else
                        {
                            nextGemValue = Random.Range(1, 5);
                        }
                        GetCell((i, j)).SetGemValue(nextGemValue);
                        GetCell((i, j)).SetToFall();
                    }
                    else
                    {

                    }
                }
            }

            this.CheckForGapsToFill();
        }
        else
        {
            if (areGemsFalling)
            {
                areGemsFalling = false;
                GemBehaviour.allowGemsSwapping(true);
                SweepTable();
            }
        }
    }
}
