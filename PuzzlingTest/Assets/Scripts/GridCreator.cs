/// <summary>
/// This code will be similar if we have more than 2 sprites
/// As we have only 2 sprites as option the extra check on
/// Line 98 has to be done.
/// If we havee more than 3 elements we can remove the extra xheck on line 98
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    #region Variables
    [Header("GridOptions")]
    [SerializeField] int height;//height of the grid(total rows)
    [SerializeField] int width;//width of the grid(total (columns)

    [Header("Single Cell")]
    [SerializeField] Cell _borderPrefab;//Cell prefab
    [SerializeField] Tile _tile;//Tile prefab

    [Header("Lists")]
    public Sprite[] imageOptions;//List of all sprite options
    public Cell[,] cells;//2D array to store the generated cells
    public Dictionary<string, int> rowDict;
    public Dictionary<string, int> columnDict;

    [Header("CellProperties")]
    int numberOfItemsPerColoumn;
    int numberOfItemsPerRow;
    #endregion

    #region Initialize
    private void Start()
    {
        Initialize();
        GenerateGrid();
    }

    //Shuffle the whole grid
    public void Shuffle()
    {
        SpawnTile();
    }

    public void Initialize()
    {
        //2D Aray to store all the cells
        cells = new Cell[width, height];

        //Number of Items that can be in a row or coloumn
        numberOfItemsPerColoumn = width / imageOptions.Length;
        numberOfItemsPerRow = height / imageOptions.Length;

        rowDict = new Dictionary<string, int>();
        columnDict = new Dictionary<string, int>();
    }
    #endregion

    #region GridGeneration
    private void GenerateGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell tileBorder = Instantiate
                    (
                    _borderPrefab,
                    new Vector2(i, j),
                    Quaternion.identity
                    );

                tileBorder.transform.parent = transform;
                tileBorder.name = $"Tile( { i } ,{ j } )";
                cells[i, j] = tileBorder;
            }
        }
        SpawnTile();
        SetGridPosition();
    }
    #endregion

    #region Spawn the inner tile
    //Spawn a new Tile
    public void SpawnTile()
    {
        foreach (var cell in cells)
        {
            //Get the cell position to put a tile on the same position
            int x = (int)cell.transform.position.x;
            int y = (int)cell.transform.position.y;

            Sprite newSprite;

            //Get a random sprite
            newSprite = GetrandomSprite();

            //Check count for the current Index
            if(CheckCount(x,y,newSprite))
            {
                newSprite = GettheotherSprite(newSprite);
            }

            //Check if match found(3 together)
            while (MatchFound(x, y, newSprite.name))
            {
                //To check for a certain condition which only occurs when we have only 2 elements
                //Situation is given below
                //******
                //*oo@**
                //***x**
                //***x**
                //******
                //Place where @ is, cannot be a 'o' or a 'x' because
                //in the x axis and the y axis both we have matches of 3
                //It goes to an infinite loop

                //To solve this we go back 1 block in each direction and check 
                //if we can put the alternate block without any matches forming
                //We repeat the process until we do not find a match.
                if (x > 1 && y > 1)
                {
                    if (
                           cells[x, y - 1].tag == newSprite.name
                        && cells[x, y - 2].tag == newSprite.name
                        && cells[x - 1, y].tag != newSprite.name
                        && cells[x - 2, y].tag != newSprite.name
                        
                        || cells[x, y - 1].tag != newSprite.name
                        && cells[x, y - 2].tag != newSprite.name
                        && cells[x - 1, y].tag == newSprite.name
                        && cells[x - 2, y].tag == newSprite.name)
                    {
                        int newY = y - 1;
                        int newX = x - 1;
                        Sprite newSprite1 = GettheotherSprite(newSprite);
                        while (MatchFound(x, newY, newSprite1.name))
                        {
                            newSprite1 = GettheotherSprite(newSprite);
                            newY--;
                        }
                        while (MatchFound(newX, y, newSprite1.name))
                        {
                            newSprite1 = GettheotherSprite(newSprite);
                            newX--;
                        }
                        SetNewTile(x,newY,newSprite1);
                        SetNewTile(newX, y, newSprite1);
                        break;
                    }
                }
                newSprite = GetrandomSprite();
            }
            SetNewTile(x, y, newSprite);
        }
    }

    //Check the count from a given position
    public bool CheckCount(int x, int y, Sprite newSprite)
    {
        //Count the number of Items in previous row blocks
        CountNumberofItemsinSinglecolumn(x, y);

        //Count the number of Items in previous column blocks
        CountNumberofItemsinSinglerow(y, x);

        if (rowDict.ContainsKey(newSprite.name)
            && columnDict.ContainsKey(newSprite.name))
        {
            if (rowDict[newSprite.name] >= numberOfItemsPerColoumn
            || columnDict[newSprite.name] >= numberOfItemsPerRow)
            {
                return true;
            }
        }
        return false;
    }

    //Get the alternate sprite
    public Sprite GettheotherSprite(Sprite newSprite)
    {
        int index = System.Array.IndexOf(imageOptions, newSprite);
        return imageOptions[index == 1 ? 0 : 1];
    }

    //Get a random sprite from image list
    public Sprite GetrandomSprite()
    {
        return imageOptions[Random.Range(
                    0, imageOptions.Length)];
    }

    //Put a new sprite on the tile
    public void SetNewTile(int x,int y,Sprite newSprite)
    {
        Tile newTile;

        //If a tile gameobject already exist 
        //just change the sprite
        //else instantiate a new gameObject
        if(cells[x, y].GetComponentInChildren<Tile>() == null)
        {
            newTile = Instantiate(
            _tile,
            new Vector2(x, y),
            Quaternion.identity
            );
        }
        else
        {
            newTile = cells[x, y].GetComponentInChildren<Tile>();
        }


        newTile._sprite.sprite = newSprite;
        newTile.transform.parent = cells[x, y].transform;
        if (newSprite != null) { cells[x, y].tag = newSprite.name; }
        cells[x, y].SetTile(newTile);
    }

    //Count the number od elements in the row
    public void CountNumberofItemsinSinglerow(int y, int x)
    {
        rowDict["Tree"] = 0;
        rowDict["Grass"] = 0;
        for (int i = 0; i < x; i++)
        { 
            if (cells[i, y].tag == "Tree")
            {
                rowDict["Tree"]++;
            }
            else
            {
                rowDict["Grass"]++;
            }
        }
        /*Debug.Log($"Counting Row : While on {x}, {y} : " +
    $"Grass {rowDict["Tree"]}" +
    $"Tree {rowDict["Grass"]}");*/
    }

    //Count the number od elements in the column
    public void CountNumberofItemsinSinglecolumn(int x, int y)
    {
        columnDict["Tree"] = 0;
        columnDict["Grass"] = 0;
        for (int i = 0; i < y; i++)
        {
            if (cells[x, i].tag == "Tree")
            {
                columnDict["Tree"]++;
            }
            else
            {
                columnDict["Grass"]++;
            }
        }
        /*Debug.Log($"Counting Column : While on {x}, {y} : " +
    $"Grass {columnDict["Grass"]}" +
    $"Tree {columnDict["Tree"]}");*/
    }

    #endregion

    #region Check if match is more than 3
    //Check if there are matches of 3
    bool MatchFound(int x, int y, string name)
    {
        if (x > 1 && y > 1)
        {
            if (cells[x - 1, y].tag == name
                && cells[x - 2, y].tag == name)
            {
                return true;
            }
            if (cells[x, y - 1].tag == name
                && cells[x, y - 2].tag == name)
            {
                return true;
            }
        }
        else if (x <= 1 || y <= 1)
        {
            if (y > 1)
            {
                if (cells[x, y - 1].tag == name
                && cells[x, y - 2].tag == name)
                {
                    return true;
                }
            }
            if (x > 1)
            {
                if (cells[x - 1, y].tag == name
                && cells[x - 2, y].tag == name)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Put the grid in right position
    //Putting the grid in center by adjusting the camera
    //position and size
    private void SetGridPosition()
    {
        var mid = new Vector2(width / 2 - 0.5f, height / 2 - 0.5f);

        Camera.main.transform.position = new Vector3(
            mid.x, mid.x, -10);

        Camera.main.orthographicSize = width;
    }
    #endregion
}