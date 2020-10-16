using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public static class ObjParser
{
    /// <summary>
    /// Parses *.obj files containing multiple objects
    /// </summary>
    /// <param name="objFile"></param>
    /// <param name="mesh"></param>
    /// <param name="meshList"></param>
    public static void ParseObj(string objFile, out Mesh mesh, out List<Mesh> meshList)
    {
        mesh = null;
        meshList = new List<Mesh>();

        string[] lines = objFile.Split('\n');

        List<Vector3> vertices = null;
        List<int> indices = null;

        int totalVertices = 0;
        foreach (var line in lines)
        {
            if (line == "")
            {
                mesh.vertices = vertices.ToArray();
                mesh.triangles = indices.ToArray();
                mesh.RecalculateNormals();
                meshList.Add(mesh);             
            }
            else if (line[0] == 'o')
            {
                if (mesh != null)
                {
                    mesh.vertices = vertices.ToArray();                   
                    mesh.triangles = indices.ToArray();
                    mesh.RecalculateNormals();
                    meshList.Add(mesh);

                    totalVertices += vertices.Count;
                }

                mesh = new Mesh();
                vertices = new List<Vector3>();
                indices = new List<int>();
            }
            else if (line[0] == 'v')
            {
                if (line[1] != 'n')
                {
                    string[] vertexLine = line.Split(' ');
                    float x = -float.Parse(vertexLine[1], CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(vertexLine[2], CultureInfo.InvariantCulture.NumberFormat);
                    float z = float.Parse(vertexLine[3], CultureInfo.InvariantCulture.NumberFormat);
                    vertices.Add(new Vector3(x, y, z));
                }
            }
            else if(line[0] == 'f')
            {
                string[] faceLine = line.Split(' ');
                string[] face1 = faceLine[1].Split('/');
                string[] face2 = faceLine[2].Split('/');
                string[] face3 = faceLine[3].Split('/');

                indices.Add(int.Parse(face1[0]) - 1 - totalVertices);
                indices.Add(int.Parse(face2[0]) - 1 - totalVertices);
                indices.Add(int.Parse(face3[0]) - 1 - totalVertices);
            }
        }

        if(meshList.Count > 1)
            mesh = null;
        else
            meshList = null;
    }

    /// <summary>
    /// Parses *.obj files, containing only one object
    /// </summary>
    /// <param name="objFile"></param>
    /// <param name="vertices"></param>
    /// <param name="indices"></param>
    public static void ParseSimpleObj(string objFile, out List<Vector3> vertices, out List<int> indices)
    {
        string[] lines = objFile.Split('\n');

        vertices = null;
        indices = null;

        foreach (var line in lines)
        {
            if (line == "") continue;

            if(line[0] == 'o')
            {
                vertices = new List<Vector3>();
                indices = new List<int>();
            }
            else if (line[0] == 'v')
            {
                if (line[1] != 'n')
                {
                    string[] vertexLine = line.Split(' ');
                    float x = -float.Parse(vertexLine[1], CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(vertexLine[2], CultureInfo.InvariantCulture.NumberFormat);
                    float z = float.Parse(vertexLine[3], CultureInfo.InvariantCulture.NumberFormat);
                    vertices.Add(new Vector3(x, y, z));
                }
            }
            else if (line[0] == 'f')
            {
                string[] faceLine = line.Split(' ');
                string[] face1 = faceLine[1].Split('/');
                string[] face2 = faceLine[2].Split('/');
                string[] face3 = faceLine[3].Split('/');

                indices.Add(int.Parse(face1[0]) - 1);
                indices.Add(int.Parse(face2[0]) - 1);
                indices.Add(int.Parse(face3[0]) - 1);
            }
        }
    }
}
