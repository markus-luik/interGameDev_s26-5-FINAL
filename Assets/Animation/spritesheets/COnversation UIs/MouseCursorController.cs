using UnityEngine;

public class MouseCursorController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotspot = Vector2.zero; // 点击点
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }
}