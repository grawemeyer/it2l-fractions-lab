using UnityEngine;
using System.Collections;
using SBS.Math;

namespace fractionslab.meshes
{
    public class MeshUtils
    {
        #region Static Fields
        public static int MaxFaces = 64;
        #endregion

        #region Static Members
        public static void CreateCircleSlice(float angle, float radius, Transform tr, Color color, out Mesh sliceMesh)
        {
            sliceMesh = new Mesh();

            int sliceNum = Mathf.FloorToInt(2.0f * Mathf.PI / angle);
            int faceNum = (MaxFaces / sliceNum);
            int totalFaces = faceNum * sliceNum;
            int additionalVertexes = (angle % (2.0f * Mathf.PI) == 0) ? 1 : 2;

            Vector3[] vertices = new Vector3[faceNum + additionalVertexes];
            Vector2[] uv = new Vector2[faceNum + additionalVertexes];
            Vector3[] normals = new Vector3[faceNum + additionalVertexes];
            Color[] colors = new Color[faceNum + additionalVertexes];
            vertices[0] = tr.position;
            uv[0] = new Vector2(0, 0);
            normals[0] = -tr.forward;
            colors[0] = color;

            float angleStep = (Mathf.PI * 2.0f) / (float)totalFaces;

            for (int i = 1; i < vertices.Length; i++)
            {
                float sliceAngle = Mathf.PI * 0.5f - (angleStep * (i - 1));

                vertices[i] = new Vector3(Mathf.Cos(sliceAngle) * radius, Mathf.Sin(sliceAngle) * radius, tr.position.z);
                uv[i] = Vector2.zero;
                normals[i] = -Vector3.forward;
                colors[i] = color;
            }

            sliceMesh.vertices = vertices;
            sliceMesh.uv = uv;
            sliceMesh.normals = normals;
            sliceMesh.colors = colors;

            int[] triangles = new int[faceNum * 3];
            int vIndex = 1;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int nextVIndex = (vIndex + 1 >= vertices.Length) ? (vIndex + 1) % vertices.Length + 1 : vIndex + 1;

                triangles[i] = 0;
                triangles[i + 1] = vIndex;
                triangles[i + 2] = nextVIndex;

                vIndex++;
            }

            sliceMesh.triangles = triangles;
        }

        public static void CreateRectangle(float width, float height, Color color, out Mesh rectMesh)
        {
            rectMesh = new Mesh();
            rectMesh.name = "dynMesh";

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            Vector3[] normals = new Vector3[4];
            Color[] colors = new Color[4];

            vertices[0] = new Vector3(-width * 0.5f, -height * 0.5f, 0.0f);
            vertices[1] = new Vector3(-width * 0.5f, height * 0.5f, 0.0f);
            vertices[2] = new Vector3(width * 0.5f, height * 0.5f, 0.0f);
            vertices[3] = new Vector3(width * 0.5f, -height * 0.5f, 0.0f);

            for (int i = 0; i < 4; i++)
            {
                uv[i] = Vector2.zero;
                normals[i] = -Vector3.forward;
                colors[i] = color;
            }

            rectMesh.vertices = vertices;
            rectMesh.uv = uv;
            rectMesh.normals = normals;
            rectMesh.colors = colors;

            int[] triangles = { 0, 1, 3, 1, 2, 3 };
            rectMesh.triangles = triangles;
        }

        public static void CreateRectangleStroke(GameObject parent, float zDist, float width, Color strokeColor, out GameObject stroke)
        {
            //Color strokeColor = new Color(0.1098f, 0.2431f, 0.0353f);
            stroke = new GameObject("stroke");
            stroke.transform.parent = parent.transform;
            stroke.transform.position = parent.transform.TransformPoint(new Vector3(0.0f, 0.0f, 0.0f));
            stroke.AddComponent<MeshRenderer>();
            stroke.AddComponent<MeshFilter>();

            MeshFilter parentMF = parent.GetComponent<MeshFilter>();
        
            MeshStroke ms = stroke.AddComponent<MeshStroke>();
            ms.SetColors(Color.black, Color.black);
            int vCount = parentMF.mesh.vertexCount + 1;
            ms.SetVertexCount(vCount);
            for (int i = 0; i < vCount; i++)
            {
                Vector3 v = parentMF.mesh.vertices[i % parentMF.mesh.vertexCount];
                v.z -= zDist;
                ms.SetPosition(i, v);
            }

            ms.SetWidth(width, width);
            ms.SetColors(strokeColor, strokeColor);
            stroke.renderer.material = new Material(Shader.Find("Custom/VertexColor"));
        }

        public static void CreateLine(Vector3[] pointList, float width, Color color, bool isClosed, out Mesh lineMesh)
        {
            /* 
             * A -------- C
             * B -------- D
            */

            lineMesh = new Mesh();
            lineMesh.name = "lineMesh";

            if (pointList.Length < 2)
                return;

            int vNum = pointList.Length * 2;
            int tNum = (pointList.Length - 1) * 2 * 3;
            int vIndex = 0;
            int tIndex = 0;
            Vector3[] vertices = new Vector3[vNum];
            Vector2[] uv = new Vector2[vNum];
            Vector3[] normals = new Vector3[vNum];
            Color[] colors = new Color[vNum];
            int[] triangles = new int[tNum];
            if (isClosed)
                triangles = new int[tNum + 2 * 3];

            for (int i = 0; i < pointList.Length - 1; i++)
            {
                Vector3 direction = (pointList[i + 1] - pointList[i]).normalized;
                Vector3 ortho = Vector3.Cross(direction, Vector3.forward);
                SBSVector3 A = pointList[i] - ortho * width * 0.5f;
                SBSVector3 B = pointList[i] + ortho * width * 0.5f;

                direction = (pointList[i] - pointList[i + 1]).normalized;
                ortho = Vector3.Cross(direction, Vector3.forward);
                SBSVector3 C = pointList[i + 1] + ortho * width * 0.5f;
                SBSVector3 D = pointList[i + 1] - ortho * width * 0.5f;
                
                if (i == 0)
                {
                    vertices[vIndex] = A;
                    vertices[vIndex + 1] = B;
                }
                else
                {
                    SBSLine2D line1 = new SBSLine2D(vertices[vIndex - 2], vertices[vIndex]);
                    SBSLine2D line2 = new SBSLine2D(A, C);
                    SBSVector3 intersection;
                    bool parallel = !line1.Intersect2D(line2, out intersection);
                    if (parallel)
                        vertices[vIndex] = A;
                    else
                        vertices[vIndex] = intersection;

                    line1 = new SBSLine2D(vertices[vIndex - 1], vertices[vIndex + 1]);
                    line2 = new SBSLine2D(B, D);
                    parallel = !line1.Intersect2D(line2, out intersection);
                    if (parallel)
                        vertices[vIndex + 1] = B;
                    else
                        vertices[vIndex + 1] = intersection;
                }

                vertices[vIndex + 2] = C;
                vertices[vIndex + 3] = D;

                triangles[tIndex] = vIndex;
                triangles[tIndex + 1] = vIndex + 2;
                triangles[tIndex + 2] = vIndex + 1;
                triangles[tIndex + 3] = vIndex + 1;
                triangles[tIndex + 4] = vIndex + 2;
                triangles[tIndex + 5] = vIndex + 3;
                
                tIndex += 6;
                vIndex += 2;
            }

            if (isClosed)
            {
                Vector3 lastPoint = pointList[pointList.Length - 1];
                Vector3 firstPoint = pointList[0];

                SBSVector3 A1 = vertices[0];
                SBSVector3 B1 = vertices[1];
                SBSVector3 C1 = vertices[2];
                SBSVector3 D1 = vertices[3];

                SBSVector3 A2 = vertices[vNum - 1 - 3];
                SBSVector3 B2 = vertices[vNum - 1 - 2];
                SBSVector3 C2 = vertices[vNum - 1 - 1];
                SBSVector3 D2 = vertices[vNum - 1];
                
                Vector3 direction = (firstPoint - lastPoint).normalized;
                Vector3 ortho = Vector3.Cross(direction, Vector3.forward);
                SBSVector3 A = lastPoint - ortho * width * 0.5f;
                SBSVector3 B = lastPoint + ortho * width * 0.5f;

                direction = (lastPoint - firstPoint).normalized;
                ortho = Vector3.Cross(direction, Vector3.forward);
                SBSVector3 C = firstPoint + ortho * width * 0.5f;
                SBSVector3 D = firstPoint - ortho * width * 0.5f;

                SBSLine2D line1 = new SBSLine2D(A2, C2);
                SBSLine2D line2 = new SBSLine2D(A, C);
                SBSVector3 intersection;
                bool parallel = !line1.Intersect2D(line2, out intersection);
                if (parallel)
                    vertices[vIndex] = A;
                else
                    vertices[vIndex] = intersection;

                line1 = new SBSLine2D(B2, D2);
                line2 = new SBSLine2D(B, D);
                parallel = !line1.Intersect2D(line2, out intersection);
                if (parallel)
                    vertices[vIndex + 1] = B;
                else
                    vertices[vIndex + 1] = intersection;

                line1 = new SBSLine2D(A, C);
                line2 = new SBSLine2D(A1, C1);
                parallel = !line1.Intersect2D(line2, out intersection);
                if (parallel)
                    vertices[0] = A1;
                else
                    vertices[0] = intersection;

                line1 = new SBSLine2D(B, D);
                line2 = new SBSLine2D(B1, D1);
                parallel = !line1.Intersect2D(line2, out intersection);
                if (parallel)
                    vertices[1] = B1;
                else
                    vertices[1] = intersection;

                triangles[tIndex] = vIndex;
                triangles[tIndex + 1] = 0;
                triangles[tIndex + 2] = vIndex + 1;
                triangles[tIndex + 3] = vIndex + 1;
                triangles[tIndex + 4] = 0;
                triangles[tIndex + 5] = 1;
            }

            for (int i = 0; i < vNum; i++)
            {
                uv[i] = Vector2.zero;
                normals[i] = -Vector3.forward;
                colors[i] = color;
            }

            lineMesh.vertices = vertices;
            lineMesh.uv = uv;
            lineMesh.normals = normals;
            lineMesh.colors = colors;

            string log = string.Empty;
            for (int i = 0; i < triangles.Length; i++)
                log += triangles[i] + ", ";
            //Debug.Log(" triangles: " + log);

            lineMesh.triangles = triangles;
        }
        #endregion
    }
}