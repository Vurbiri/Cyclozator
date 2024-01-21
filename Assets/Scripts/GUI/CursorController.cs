using UnityEngine;

public class CursorController : Singleton<CursorController>
{
    [SerializeField] private float _timeToHideCursor = 5f;

    private float _timer = 0f;


    private void Update()
    {
        float mouseRun = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
        _timer += Time.unscaledDeltaTime;
        if (mouseRun > 0f)
        {
            _timer = 0f;
            Cursor.visible = true;
        }
        else if (_timer > _timeToHideCursor)
        {
            Cursor.visible = false;
        }
    }
}
