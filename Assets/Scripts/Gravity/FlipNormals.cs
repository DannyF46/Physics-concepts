using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlipNormals : MonoBehaviour
{
    public MeshCollider meshCol;
    // Start is called before the first frame update
    void Awake()
    {
        meshCol = this.GetComponent<MeshCollider>();

        Mesh mesh = meshCol.sharedMesh;

        mesh.triangles = mesh.triangles.Reverse().ToArray(); //reverse triangles
        mesh.normals = mesh.normals.Select(n => -n).ToArray(); //invert normals
    }


}
