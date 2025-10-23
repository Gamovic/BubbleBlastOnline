using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBorders : MonoBehaviour
{
    [Header("Border Settings")]
    public float levelWidth = 20f;  // Width of the level (not screen width)
    public float levelHeight = 10f; // Height of the level
    public float borderThickness = 1f;
    
    [Header("Border Materials")]
    public PhysicsMaterial2D bounceMaterial;
    
    private GameObject floorBorder;
    private GameObject topBorder;
    private GameObject leftBorder;
    private GameObject rightBorder;

    void Start()
    {
        CreateBorders();
    }

    private void CreateBorders()
    {
        // Create floor border
        floorBorder = CreateBorder("FloorBorder", new Vector3(0, -levelHeight/2 - borderThickness/2, 0), 
            new Vector3(levelWidth + borderThickness, borderThickness, 1f));
        
        // Create top border
        topBorder = CreateBorder("TopBorder", new Vector3(0, levelHeight/2 + borderThickness/2, 0), 
            new Vector3(levelWidth + borderThickness, borderThickness, 1f));
        
        // Create left border
        leftBorder = CreateBorder("LeftBorder", new Vector3(-levelWidth/2 - borderThickness/2, 0, 0), 
            new Vector3(borderThickness, levelHeight + borderThickness, 1f));
        
        // Create right border
        rightBorder = CreateBorder("RightBorder", new Vector3(levelWidth/2 + borderThickness/2, 0, 0), 
            new Vector3(borderThickness, levelHeight + borderThickness, 1f));
    }

    private GameObject CreateBorder(string name, Vector3 position, Vector3 scale)
    {
        GameObject border = new GameObject(name);
        border.transform.position = position;
        border.transform.localScale = scale;
        
        // Add BoxCollider2D
        BoxCollider2D collider = border.AddComponent<BoxCollider2D>();
        collider.sharedMaterial = bounceMaterial;
        
        // Add SpriteRenderer for visibility
        SpriteRenderer renderer = border.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateBorderSprite();
        renderer.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // Semi-transparent gray
        
        // Make it a child of this object
        border.transform.SetParent(transform);
        
        return border;
    }

    private Sprite CreateBorderSprite()
    {
        // Create a simple white square sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    // Method to check if a position is within level bounds
    public bool IsWithinLevelBounds(Vector3 position)
    {
        return position.x >= -levelWidth/2 && position.x <= levelWidth/2 &&
               position.y >= -levelHeight/2 && position.y <= levelHeight/2;
    }

    // Method to clamp a position to level bounds
    public Vector3 ClampToLevelBounds(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, -levelWidth/2, levelWidth/2);
        float clampedY = Mathf.Clamp(position.y, -levelHeight/2, levelHeight/2);
        return new Vector3(clampedX, clampedY, position.z);
    }

    void OnDrawGizmos()
    {
        // Draw level bounds in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(levelWidth, levelHeight, 0));
    }
}
