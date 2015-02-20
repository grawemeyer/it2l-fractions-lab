using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;
using System.Collections.Generic;


namespace fractionslab.behaviours
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RectangleMeshElement : WSElement, IWSElement
    {
    public float TileWidth = 2;
    public float TileHeight = 1;
    public int TileGridWidth = 5; // righe
    public int TileGridHeight = 3; //colonne
    public float wHorLine = 0.02f;
    public float wVerLine = 0.02f;
    public float wBorderLine = 0.02f;
    public float wPartLine = 0.04f;
    public float wSepLine = 0.1f;
    public Color colorLine = new Color(0.1098f, 0.2431f, 0.0353f, 1.0f);
    public Color colorEmptyTile = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public SBSBounds internalBounds;
    public bool isVRect = false;
 
    public float DefaultTileX;
    public float DefaultTileY;
    public Texture2D Texture;

    protected float NumTilesX = 1;
    protected float NumTilesY = 1;
    protected float actualGridWidth = 0;
    protected float actualGridHeight = 0;
    protected List<Color> colorMesh;
    protected List<bool> sliceIndex = new List<bool>();
    protected List<SliceVertex> sliceFace = new List<SliceVertex>();


    void OnClicked(Vector3 position)
    {
        //  Debug.Log("CLICKED " + ((tmpCol * TileGridWidth) + (TileGridHeight - tmpRig)));
        int tmpCol, tmpRig;
        SBSBounds meshBounds = GetBounds();
        position.z = meshBounds.max.z;
        Vector3 tmp =  transform.InverseTransformPoint(position);
       // Debug.Log("x " + tmp.x + "y " + tmp.y);

        if (meshBounds.ContainsPointXY(position))
        {
            tmpCol = (int)((tmp.x + (Width / 2) - wBorderLine) / (TileHeight + wVerLine));
            tmpRig = (int)((tmp.y + (Height / 2) - wBorderLine) / (TileWidth + wHorLine));
            if (type == ElementsType.HRect && partDenominator != 0) 
            {
                tmpRig = TileGridWidth - tmpRig - 1;
                int index = Mathf.Clamp(((tmpRig * TileGridHeight) + tmpCol), 0 , gameObject.transform.parent.GetComponent<RectangleElement>().slices.Count-1);
                gameObject.transform.parent.GetComponent<RectangleElement>().ClickSlice(index);
            }
            if (type == ElementsType.VRect && partDenominator != 0) 
            {
                int index = Mathf.Clamp((((tmpCol * TileGridWidth) + (TileGridWidth - tmpRig)) -1),0 , gameObject.transform.parent.GetComponent<RectangleElement>().slices.Count-1);
                gameObject.transform.parent.GetComponent<RectangleElement>().ClickSlice(index);
             }
          }
        Draw();
    }

    public override SBSBounds GetBounds()
    {
        if (state == ElementsState.Cut) 
        {
            return internalBounds;
        }
        bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
        return bounds;
    }

    void Awake() 
    {
       gameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        internalBounds = new SBSBounds();
    }

    void Start() 
    {   
        Draw();
    }

    /* void Update()
     {
         if (Input.GetKeyDown(KeyCode.C)) 
         {
             TileGridHeight++;
             Draw();
         }
         if (Input.GetKeyDown(KeyCode.V))
         {
             if (TileGridHeight > 1)
             {
                 TileGridHeight--;
                 Draw();
             }

         }
         if (Input.GetKeyDown(KeyCode.R))
         {
             TileGridWidth++;
             Draw();
         }
         if (Input.GetKeyDown(KeyCode.T))
         {
             if (TileGridWidth > 1) 
             {
                 TileGridWidth--;
                 Draw();
             }
         }
     }*/

    void UpdateSliceIndex() 
    {
        sliceIndex.Clear();
        if (type == ElementsType.HRect && partDenominator != 0) 
        {   
            for (int i = TileGridHeight-1; i >=0 ; i--) 
            {
                for (int j = 0; j < TileGridWidth; j++)
                {
                    sliceIndex.Add(gameObject.transform.parent.GetComponent<RectangleElement>().slices[ (i + (TileGridHeight * j))]);
                }
            }   
        }
        if (type == ElementsType.VRect && partDenominator != 0)
        {
              for (int i = 1; i <= TileGridHeight ; i++)
              {
                  for (int j = 1; j <= TileGridWidth; j++)
                  {
                    // Debug.Log("index processed i " + i + " j " + j + " TileGridWidth  " + TileGridWidth +" " + (i * (TileGridWidth - j)));
                      sliceIndex.Add(gameObject.transform.parent.GetComponent<RectangleElement>().slices[(i * TileGridWidth) - j]);
                  }
              } 

        }
    }
    public void UpdateGrid(Vector2 gridIndex, Vector2 tileIndex, float tileWidth, float tileHeight, float gridWidth)
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var uvs = mesh.uv;
 
        var tileSizeX = 1.0f / NumTilesX;
        var tileSizeY = 1.0f / NumTilesY;
 
        mesh.uv = uvs;
 
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 0] = new Vector2(tileIndex.x * tileSizeX, tileIndex.y * tileSizeY);
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 1] = new Vector2((tileIndex.x + 1) * tileSizeX, tileIndex.y * tileSizeY);
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 2] = new Vector2((tileIndex.x + 1) * tileSizeX, (tileIndex.y + 1) * tileSizeY);
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 3] = new Vector2(tileIndex.x * tileSizeX, (tileIndex.y + 1) * tileSizeY);
 
        mesh.uv = uvs;
    }

    protected void Initialize() 
    {
        TileGridHeight = 1;
        TileGridWidth = 1;
    }
    public void Draw()
    {
        GetComponent<MeshFilter>().mesh.Clear();
        if (partDenominator != 0)
        {
            wBorderLine = wSepLine;
            if (type == ElementsType.VRect)
            {
                wVerLine = wSepLine;
                wHorLine = wPartLine;
                TileGridWidth = partitions;
                TileGridHeight = partDenominator/partitions;
                //Debug.Log("TileGridHeight " + TileGridHeight + " partDenominator " + partDenominator);
                TileHeight = (Width - ((wVerLine * (TileGridHeight - 1)) + (wBorderLine * 2))) / TileGridHeight;
                //if (partitions != 1)
                TileWidth = (Height - ((wHorLine * (TileGridWidth - 1)) + (wBorderLine * 2))) / TileGridWidth;
                //TileWidth = Height;

            }
            else if (type == ElementsType.HRect)
            {
                wVerLine = wPartLine;
                wHorLine = wSepLine;
                TileGridHeight = partitions;
                TileGridWidth = partDenominator/partitions;
                TileWidth = (Height - ((wHorLine * (TileGridWidth - 1)) + (wBorderLine * 2))) / TileGridWidth;
                //if(partitions != 1)
                TileHeight = (Width - ((wVerLine * (TileGridHeight - 1)) + (wBorderLine * 2))) / TileGridHeight;
               // TileHeight = Width;
            }
        }
        else 
        {
            TileHeight = (Width - (wBorderLine * 2)) / TileGridHeight;
            TileWidth = (Height - (wBorderLine * 2)) / TileGridHeight;
        }
        CreateGrid(TileWidth, TileHeight, TileGridWidth, TileGridHeight);
    }

    int coloredSlice = 0;
    void CreateGrid(float tileHeight, float tileWidth, float gridHeight, float gridWidth)
    {
        UpdateSliceIndex();
        internalBounds.Reset();
        coloredSlice = 0;
        actualGridWidth = -(Width / 2.0f);
        actualGridHeight = -(Height / 2.0f);
        Mesh mesh = new Mesh();
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.renderer.material.SetTexture("_MainTex", Texture);
        mf.mesh = mesh;
 
        float tileSizeX = 1.0f / NumTilesX;
        float tileSizeY = 1.0f / NumTilesY;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        colorMesh = new List<Color>();
        float lineWidth = wBorderLine;
        int  index = 0;

        for (int x = 0; x < (gridWidth*2)+1; x++) { 
            for (int y = 0; y < (gridHeight*2)+1; y++) {   
                if (x % 2 == 0)
                {
                    if (x == 0 || x == (gridWidth * 2))
                    {
                        lineWidth = wBorderLine;
                    }
                    else 
                    {
                        lineWidth = wVerLine;
                    }
                    if (x > 0 && y == 0 ) 
                    {
                        actualGridHeight = -(Height / 2.0f);
                        actualGridWidth += tileWidth;
                    }
                    if (y % 2 == 0) // quadratino piccolo
                    {
                        //Debug.Log("Quadratino x " + x + " y " + y);
                        if (((y == 0 || y == (gridHeight * 2)) && (x == 0 || x == (gridWidth * 2))) || (type == ElementsType.VRect && (y == 0 || y == (gridHeight * 2)) || (type == ElementsType.HRect && (x == 0 || x == (gridWidth * 2)))))
                        {
                            AddVertices(wBorderLine, wBorderLine, y, x, vertices, false);
                            index = AddTriangles(index, triangles);
                            AddNormals(normals);
                            AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
                            actualGridHeight += wBorderLine;
                        }
                        else
                        {
                            AddVertices(wHorLine, wVerLine, y, x, vertices, false);
                            index = AddTriangles(index, triangles);
                            AddNormals(normals);
                            AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
                            actualGridHeight += wHorLine;
                        }
                        
                    }
                    else   // linea orizzontale spessa
                    {
                        //Debug.Log("linea orizzontale spessa x " + x + " y " + y);
                        AddVertices(tileHeight, lineWidth, y, x, vertices, false);
                        index = AddTriangles(index, triangles);
                        AddNormals(normals);
                        AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
                        actualGridHeight += tileHeight;
                    }
                    
                }
                else
                {
                    if (x >= 0 && y == 0)
                    {
                        actualGridHeight = -(Height / 2.0f);
                        actualGridWidth += lineWidth;
                    }
                    if (y % 2 == 0)//linea verticale spessa
                    {
                        if (y == 0 || y == (gridHeight * 2))
                        {
                            lineWidth = wBorderLine;
                        }
                        else
                        {
                            lineWidth = wHorLine;
                        }
                        //Debug.Log("linea verticale spessa x " + x + " y " + y);
                        AddVertices(lineWidth, tileWidth, y, x, vertices, false);
                        index = AddTriangles(index, triangles);
                        AddNormals(normals);
                        AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
                        actualGridHeight += lineWidth;
                       
                    }
                    else // tile
                    {
                        lineWidth = wVerLine;
                        //Debug.Log("tile x " + x + " y " + y);
                        AddVertices(tileHeight, tileWidth, y, x, vertices, true);
                        index = AddTriangles(index, triangles);
                        AddNormals(normals);
                        AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
                        actualGridHeight += tileHeight;
                    }

                }
               
            }
        }
 
        mesh.vertices = vertices.ToArray();
        mesh.colors = colorMesh.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
      
    }


    protected void AddVertices(float tileHeight, float tileWidth, int y, int x, ICollection<Vector3> vertices,  bool isSlice)
    {
        Color colorTile = colorEmptyTile;
        if (state == ElementsState.Cut)
            colorTile = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Color additiveColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
       // bool isColored = false;
        if (isSlice)
        {
            if (partDenominator > 0)
            {
                if (type==ElementsType.HRect && sliceIndex[sliceIndex.Count - coloredSlice - 1])
                {
                    if (mode == InteractionMode.Freeze)
                    {
                        //color.a = 0.5f;
                        additiveColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    }
                    else
                    {
                        //color.a = 1.0f;
                        internalBounds.Encapsulate(new SBSBounds(transform.TransformPoint(actualGridWidth + (tileWidth / 2), actualGridHeight + (tileHeight / 2), transform.position.z), new SBSVector3(tileWidth + wHorLine, tileHeight + wVerLine, 0.0f)));
                    }
                    colorTile = color+additiveColor;
                }
                else if (type == ElementsType.VRect && sliceIndex[coloredSlice])
                {
                    if (mode == InteractionMode.Freeze)
                    {
                        //color.a = 0.5f;
                        additiveColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    }
                    else
                    {
                        //color.a = 1.0f;
                        internalBounds.Encapsulate(new SBSBounds(transform.TransformPoint(actualGridWidth + (tileWidth / 2), actualGridHeight + (tileHeight / 2), transform.position.z), new SBSVector3(tileWidth + wHorLine, tileHeight + wVerLine, 0.0f)));
                    }
                    colorTile = color+additiveColor;    
                }
            }
            //Debug.Log("colortile " + colorTile);
            coloredSlice++;
        }
        else 
        {
            colorTile = colorLine;
        }
        vertices.Add(new Vector3(actualGridWidth , actualGridHeight, 0));
        colorMesh.Add(colorTile);
        vertices.Add(new Vector3(actualGridWidth + tileWidth, actualGridHeight, 0));
        colorMesh.Add(colorTile);
        vertices.Add(new Vector3(actualGridWidth + tileWidth, actualGridHeight + tileHeight, 0));
        colorMesh.Add(colorTile);
        vertices.Add(new Vector3(actualGridWidth, actualGridHeight + tileHeight, 0));
        colorMesh.Add(colorTile);
       /* if (isSlice)
        {
            SliceVertex tmp = new SliceVertex();
            tmp.vertex = new int[4];
            tmp.vertex[0] = vertices.Count - 4;
            tmp.vertex[1] = vertices.Count - 3;
            tmp.vertex[2] = vertices.Count - 2;
            tmp.vertex[3] = vertices.Count - 1;
            tmp.isColored = isColored;
            sliceFace.Add(tmp);
        }*/
    }
 
    private int AddTriangles(int index, ICollection<int> triangles)
    {
        triangles.Add(index + 2);
        triangles.Add(index + 1);
        triangles.Add(index);
        triangles.Add(index);
        triangles.Add(index + 3);
        triangles.Add(index + 2);
        index += 4;
        return index;
    }
 
    private void AddNormals(ICollection<Vector3> normals)
    {
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
    }

    private void AddUvs(float tileRow, float tileSizeY, float tileSizeX, ICollection<Vector2> uvs, float tileColumn)
    {
        uvs.Add(new Vector2(tileColumn * tileSizeX, tileRow * tileSizeY));
        uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, tileRow * tileSizeY));
        uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, (tileRow + 1) * tileSizeY));
        uvs.Add(new Vector2(tileColumn * tileSizeX, (tileRow + 1) * tileSizeY));
    }

    #region Setup
    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
        bounds = new SBSBounds(transform.position, new SBSVector3(Width, Height, 0.0f));
       // Debug.Log("getbound " + GetBounds());
    }

    void SetStrokeWidth(float wStroke) 
    {
        if (type == ElementsType.VRect)
        {
            wBorderLine = wStroke;
            wVerLine = wStroke;
        }
        else 
        {
            wBorderLine = wStroke;
            wHorLine = wStroke;
        }    
    }

        void SetEmpyColor(Color cl)
        {
            colorEmptyTile = cl;
        }

        void SetStrokeColor(Color cl) 
        {
            colorLine = cl;
        }
    #endregion

    }
}
