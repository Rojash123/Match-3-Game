using System.Collections.Generic;
using UnityEngine;

public abstract class GridSystem<T> : Singleton<GridSystem<T>>
{
    protected Vector2Int dimensions = new Vector2Int(1, 1);
    private T[,] data;

    public Vector2Int Dimensions
    {
        get { return dimensions; }
    }

    private bool isReady = false;
    public bool IsReady
    {
        get { return isReady; }
    }

    public void Initialize(Vector2Int dimensions)
    {
        if (dimensions.x < 1 || dimensions.y < 1) return;

        this.dimensions = dimensions;
        data = new T[dimensions.x, dimensions.y];
        isReady = true;
    }
    public void Clear()
    {
        data = new T[dimensions.x, dimensions.y];
    }
    public bool CheckBounds(int x, int y)
    {
        if (!isReady)
            Debug.Log("Grid is not initialized");

        return x >= 0 && x < dimensions.x && y >= 0 && y < dimensions.y;
    }
    public bool CheckBounds(Vector2Int dimensions)
    {
        return CheckBounds(dimensions.x, dimensions.y);
    }
    public bool IsEmpty(int x, int y)
    {
        if (!CheckBounds(x, y))
        {
            Debug.Log("value is not in the grid");
        }
        //return data[x, y] == null;
        return EqualityComparer<T>.Default.Equals(data[x, y], default(T));
    }
    public bool IsEmpty(Vector2Int dimensions)
    {
        return IsEmpty(dimensions.x, dimensions.y);
    }

    public bool AddDataToGrid(T item, int x, int y, bool allowOverWrite = false)
    {
        if (!CheckBounds(x, y))
            Debug.Log("not in the grid");

        if (!allowOverWrite && !IsEmpty(x, y))
        {
            return false;
        }
        data[x, y] = item;
        return true;
    }
    public bool AddDataToGrid(T item, Vector2Int dimensions, bool allowOverWrite = false)
    {
        return AddDataToGrid(item, dimensions.x, dimensions.y, allowOverWrite);
    }

    public T GetData(int x, int y)
    {
        if (!CheckBounds(x, y))
            Debug.Log("not in the grid");

        return data[x, y];
    }
    public T GetData(Vector2Int dimensions)
    {
        return GetData(dimensions.x, dimensions.y);
    }

    public T RemoveDataFromGrid(int x, int y)
    {
        if (!CheckBounds(x, y))
            Debug.Log("not in the grid");
        T temp = data[x, y];
        data[x, y] = default(T);
        return temp;
    }
    public T RemoveDataFromGrid(Vector2Int dimensions)
    {
        return RemoveDataFromGrid(dimensions.x, dimensions.y);
    }

    public void SwapItem(int x1, int y1, int x2, int y2)
    {
        if (!CheckBounds(x1, y1) || !CheckBounds(x2, y2))
            Debug.Log("not in the grid");

        T temp= data[x1, y1];
        data[x2, y2] = data[x1, y1];
        data[x1, y1] = temp;
    }
    public void SwapItem(Vector2Int firstPos, Vector2Int secondPos)
    {
        SwapItem(firstPos.x, firstPos.y, secondPos.x, secondPos.y);
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < dimensions.x; i++)
        {
            s += "[";
            for (int j = 0; j < dimensions.y; j++)
            {
                if (IsEmpty(i, j))
                    s += "X";
                else 
                    s += data[i,j].ToString();

                s+= ",";
            }
            s += "]\n";
        }

        return s;
    }
}
