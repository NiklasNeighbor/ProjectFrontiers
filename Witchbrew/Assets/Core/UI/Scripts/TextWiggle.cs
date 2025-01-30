using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWiggle : MonoBehaviour
{
    public float wiggleIntensity = 1f; // How much the text wiggles
    public float wiggleSpeed = 5f;     // How fast the text wiggles

    private TextMeshProUGUI textMesh;
    private Vector3[] originalVertices;
    private TMP_TextInfo textInfo;

    void Start()
    {
        // Get the TextMeshProUGUI component
        textMesh = GetComponent<TextMeshProUGUI>();

        // Ensure the text updates its geometry
        textMesh.ForceMeshUpdate();

        // Store the original vertices of the text
        textInfo = textMesh.textInfo;
        originalVertices = textInfo.meshInfo[0].vertices.Clone() as Vector3[];
    }

    void Update()
    {
        WiggleText();
    }

    void WiggleText()
    {
        // Force the text to update its geometry
        textMesh.ForceMeshUpdate();

        // Get the mesh info for the text
        TMP_MeshInfo[] meshInfo = textInfo.meshInfo;

        // Loop through each character and modify its vertices
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Skip invisible characters (like spaces)
            if (!charInfo.isVisible)
                continue;

            // Get the index of the first vertex of the character
            int vertexIndex = charInfo.vertexIndex;

            // Apply a wiggle effect to each vertex of the character
            for (int j = 0; j < 4; j++) // Each character has 4 vertices
            {
                Vector3 offset = new Vector3(
                    Mathf.Sin(Time.time * wiggleSpeed + vertexIndex + j) * wiggleIntensity,
                    Mathf.Cos(Time.time * wiggleSpeed + vertexIndex + j) * wiggleIntensity,
                    0
                );

                // Apply the offset to the vertex
                meshInfo[0].vertices[vertexIndex + j] = originalVertices[vertexIndex + j] + offset;
            }
        }

        // Update the mesh with the modified vertices
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
