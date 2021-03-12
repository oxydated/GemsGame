using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GemBehaviour : MonoBehaviour
{
    public int gemValue;
    public Color gemColor;

    public (int, int) tablePosition;
    private GemBehaviour targetCell;

    public GameObject gem;
    public GameObject Firework;

    private static Object lockSwap = new Object();
    private static bool isAlreadySwapping = false;
    private static bool isSwappingAllowed = true;
    private bool dragStarted = false;
    private bool swapBack = false;

    public enum swapDirection : int { NONE, RIGHT, UP, LEFT, DOWN };
    private swapDirection nextMove = swapDirection.NONE;
    private swapDirection lastMove = swapDirection.NONE;

    public delegate GemBehaviour DoDragFunc( (int, int) sourceCell, swapDirection dir);
    public DoDragFunc DoDrag;

    public bool visited = false;
    public bool isPartOfaLineOrColumn = false;

    public delegate int TestMatchFunc();
    public TestMatchFunc TestMatch;

    private bool willExplode = false;
    private bool willFall = false;
    private bool willBounce = false;

    // here for performance sake
    //private MeshRenderer meshRend = null;
    //private Material gemMaterial = null;

    Vector3 startVec;
    Vector3 endVec;

    private static Color[] gemColors = { Color.white, Color.cyan, Color.green, Color.red, new Color(1f, 0f, 1f), Color.yellow };
    // Start is called before the first frame update
    void Start()
    {
        //gem = GameObject.Find(gameObject.name + "/"
        //meshRend = gem.GetComponent<MeshRenderer>();
        //gemMaterial = gem.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(nextMove != swapDirection.NONE)
        {
            //swapDirection targetMove = swapDirection.NONE;
            switch (nextMove)
            {
                case swapDirection.RIGHT:
                    gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapRight" : "BallSwapRightBack");
                    targetCell.gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapLeft": "BallSwapLeftBack");
                    //targetMove = swapDirection.LEFT;
                    break;
                case swapDirection.UP:
                    gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapUp": "BallSwapUpBack");
                    targetCell.gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapDown": "BallSwapDownBack");
                    //targetMove = swapDirection.DOWN;
                    break;
                case swapDirection.LEFT:
                    gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapLeft": "BallSwapLeftBack");
                    targetCell.gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapRight": "BallSwapRightBack");
                    //targetMove = swapDirection.RIGHT;
                    break;
                case swapDirection.DOWN:
                    gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapDown": "BallSwapDownBack");
                    targetCell.gem.GetComponent<Animator>().Play((!swapBack) ? "BallSwapUp": "BallSwapUpBack");
                    //targetMove = swapDirection.UP;
                    break;
            }
            lastMove = nextMove;
            nextMove = swapDirection.NONE;
            swapBack = false;
        }

        if (willExplode)
        {
            willExplode = false;
            Firework.GetComponent<ParticleSystem>().Play();
        }

        if (willFall)
        {
            //gem.GetComponent<Animator>().Play("BallFalling");
        }

        if (willBounce)
        {
            willBounce = false;
            //gem.GetComponent<Animator>().Play("BallsHitsBottom");
        }
    }

    public void SetGemValue(int value)
    {
        gemValue = value;
        gemColor = gemColors[gemValue];

        if (value == 0)
        {
            gem.GetComponent<MeshRenderer>().enabled = false;
            //Material gemMaterial = gem.GetComponent<Renderer>().material;
            //gemMaterial.color = gemColor;
        } else
        {
            //Material gemMaterial = gem.GetComponent<Renderer>().material;
            //gem.GetComponent<MeshRenderer>().enabled = true;

            Material gemMaterial = gem.GetComponent<Renderer>().material;
            gemMaterial.color = gemColor;

            gem.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void OnMouseDown()
    {
        lock (lockSwap)
        {
            if (isSwappingAllowed && !isAlreadySwapping)
            {
                isAlreadySwapping = true;
                dragStarted = true;
                //SetGemValue(1 + (gemValue + 1) % 5);
                startVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
    void OnMouseUp()
    {
        if (isSwappingAllowed && isAlreadySwapping && dragStarted)
        {
            dragStarted = false;
            //SetGemValue(1 + (gemValue - 1) % 5);
            endVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 DragVec = endVec - startVec;

            float test = Mathf.Abs(DragVec.y / DragVec.x);

            if ((DragVec.x == 0.0f) || (Mathf.Abs(DragVec.y / DragVec.x) > (3.0f / 2.0f) * Mathf.Sqrt(2.0f)))
            {
                nextMove = (DragVec.y > 0) ? swapDirection.UP : swapDirection.DOWN;
                targetCell =  DoDrag( (tablePosition.Item1, tablePosition.Item2), nextMove);
            }
            else
            {
                if (Mathf.Abs(DragVec.y / DragVec.x) < (1.0f / 2.0f))
                {
                    nextMove = (DragVec.x > 0) ? swapDirection.RIGHT : swapDirection.LEFT;
                    targetCell = DoDrag((tablePosition.Item1, tablePosition.Item2), nextMove);
                }
            }
        }
    }
    //public void PlaySwap(swapDirection dir)
    //{
    //    gem.GetComponent<Animator>().SetInteger("SwapGem", (int)dir);
    //    GetComponent<Animator>().Play()
    //}
    public void EndSwap()
    {
        lock (lockSwap)
        {
            isAlreadySwapping = false;

            //int swapVal = cellGems[swapSource.Item1, swapSource.Item2].GetComponent<GemBehaviour>().gemValue;
            //cellGems[swapSource.Item1, swapSource.Item2].GetComponent<GemBehaviour>().SetGemValue(
            //    cellGems[swapTarget.Item1, swapTarget.Item2].GetComponent<GemBehaviour>().gemValue
            //    );
            //cellGems[swapTarget.Item1, swapTarget.Item2].GetComponent<GemBehaviour>().SetGemValue(swapVal);
            if (targetCell != null)
            {
                int sourceVal = gemValue;
                int targetVal = targetCell.gemValue;
                SetGemValue(targetVal);
                targetCell.SetGemValue(sourceVal);
                int numChanged = TestMatch();
                if (numChanged == 0)
                {
                    SetGemValue(sourceVal);
                    targetCell.SetGemValue(targetVal);
                    nextMove = lastMove;
                    swapBack = true;
                }
                else
                {
                    targetCell = null;
                }
            }
        }
    }

    public void WithDrawSwap()
    {
        lock (lockSwap)
        {
            isAlreadySwapping = false;
            targetCell = null;
        }
    }

    public void SetToExplode()
    {
        SetGemValue(0);
        willExplode = true;
    }

    public void SetToFall()
    {
        willFall = true;
    }

    public void StopToFall()
    {
        if (willFall)
        {
            willFall = false;
            willBounce = true;
        }
    }

    public static void allowGemsSwapping(bool isAllowed)
    {
        isSwappingAllowed = isAllowed;
    }

}
