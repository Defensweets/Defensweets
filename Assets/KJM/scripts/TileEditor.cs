using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase groundTile;

    public int mapWidth = 29;
    public int mapHeight = 12;
    public int tileSize = 60;

    private int[,] tileData;

    private int arrayX;
    private int arrayY;

    // Start is called before the first frame update
    void Start()
    {
        tileData = new int[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tileData[x, y] = 1;

                //Vector3Int tilePos = new Vector3Int(x, y, 0);
                //tilemap.SetTile(tilePos, groundTile);
            }
        }
        //tilemap.SetTile(new Vector3Int(30, 0, 0), groundTile);
        //tilemap.SetTile(new Vector3Int(-1, 0, 0), groundTile);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleTile(false);
        }
        else if (Input.GetMouseButton(1))
        {
            HandleTile(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    tileData[x, y] = 1;

                    Vector3Int tilePos = new Vector3Int(x + tilemap.origin.x, y + tilemap.origin.y, 0);
                    tilemap.SetTile(tilePos, groundTile);
                }
            }
        }

        if (CheckComplete())
        {
            Debug.Log("���� �Ϸ�");
        }

    }

    void HandleTile(bool place)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        //���� ī�޶� ���� �ִ� ȭ���� ���콺 ��ġ�� ���� ��ġ�� �ٲ���
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        Vector3Int tilemapOrigin = tilemap.cellBounds.min;

        arrayX = cellPos.x - tilemapOrigin.x;
        arrayY = cellPos.y - tilemapOrigin.y;

        if (arrayX < 0 || arrayX >= mapWidth || arrayY < 0 || arrayY >= mapHeight) return;

        if (place)
        {
            if (CanPlaceTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, groundTile);
                tileData[arrayX, arrayY] = 1;
            }
        }
        else
        {
            if(CanRemoveTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, null);
                tileData[arrayX, arrayY] = 0;
            }
            
        }
        
    }

    bool CanRemoveTile(int x, int y)
    {
        if ((x == 0 && y == 4) && tileData[x, y] == 1) return true;

        if (tileData[x, y] == 0)
        {
            return false;
        }

        if (x > 0 && tileData[x - 1, y] == 0) return true;
        if (x < mapWidth - 1 && tileData[x + 1, y] == 0) return true;
        if (y > 0 && tileData[x, y-1] == 0) return true;
        if (y < mapHeight - 1 && tileData[x, y + 1] == 0) return true;

        return false;
    }

    bool CanPlaceTile(int x, int y)
    {
        if (x == 0 && y == 4)
        {
            if (tileData[x, y + 1] == 1 && tileData[x, y - 1] == 1 && tileData[x + 1, y] == 1) return true;
            else return false;
        }
        if (tileData[x, y] == 1) return false;

        //��or�Ʒ�&&����or�� ����or��&&��or�Ʒ� �� �� 0�̰�, �� ���⿡ ���� �������밢��, ���޴밢��... �밢�� ĭ�� 0���� üũ
        //�� ���� ��� ����ó��
        if (y + 1 < mapHeight && x > 0) // ��, ��
        {
            if (tileData[x, y + 1] == 0 && tileData[x - 1, y] == 0)
            {
                if ((x + 1 < mapWidth && tileData[x + 1, y] == 0) || (y > 0 && tileData[x, y - 1] == 0))
                {

                }
                else if (y + 1 < mapHeight && x > 0 && tileData[x - 1, y + 1] == 0) return true;
            }
        }

        if (y + 1 < mapHeight && x + 1 < mapWidth) // ��, ����
        {
            if (tileData[x, y + 1] == 0 && tileData[x + 1, y] == 0)
            {
                if ((x > 0 && tileData[x - 1, y] == 0) || (y > 0 && tileData[x, y - 1] == 0))
                {

                }
                else if (y + 1 < mapHeight && x + 1 < mapWidth && tileData[x + 1, y + 1] == 0) return true;
            }
        }

        if (y > 0 && x + 1 < mapWidth) // �Ʒ�, ����
        {
            if (tileData[x, y - 1] == 0 && tileData[x + 1, y] == 0)
            {
                if ((x > 0 && tileData[x - 1, y] == 0) || (y + 1 < mapHeight && tileData[x, y + 1] == 0))
                {

                }
                else if (y > 0 && x + 1 < mapWidth && tileData[x + 1, y - 1] == 0) return true;
            }
        }

        if (y > 0 && x > 0) // �Ʒ�, ��
        {
            if (tileData[x, y - 1] == 0 && tileData[x - 1, y] == 0)
            {
                if ((x + 1 < mapWidth && tileData[x + 1, y] == 0) || (y + 1 < mapHeight && tileData[x, y + 1] == 0))
                {

                }
                else if (y > 0 && x > 0 && tileData[x - 1, y - 1] == 0) return true;
            }
        }

        //if (x > 0 && x < mapWidth - 1 && y > 0 && y < mapHeight - 1)
        //{
        //    if (tileData[x, y + 1] == 0 && tileData[x, y - 1] == 0 &&
        //        tileData[x - 1, y] == 0 && tileData[x + 1, y] == 0)
        //    {
        //        if (tileData[x - 1, y + 1] == 0 && tileData[x + 1, y + 1] == 0 &&
        //            tileData[x - 1, y - 1] == 0 && tileData[x + 1, y - 1] == 0)
        //        {
        //            return true;
        //        }
        //    }
        //}


        int emptyCount = 0;

        if (x == 0 || tileData[x - 1, y] == 1) emptyCount++;
        if (x == mapWidth - 1 || tileData[x + 1, y] == 1) emptyCount++;
        if (y == 0 || tileData[x, y-1] == 1) emptyCount++;
        if (y == mapHeight - 1 || tileData[x, y+1] == 1) emptyCount++;

        return emptyCount >= 3;
    }

    bool CheckComplete()
    {
        if (tileData[mapWidth - 1, 4] == 0) return true;
        else return false;
    }
}
