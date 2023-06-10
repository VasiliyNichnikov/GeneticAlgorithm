using UnityEngine;

namespace MapViewing
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float _panSpeed;
        [SerializeField] private float _panBorderThickness;
        [SerializeField] private Vector2 _panLimit;
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private float _minY;
        [SerializeField] private float _maxY;
        
        private void Update()
        {
            var position = transform.position;
            if (Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - _panBorderThickness)
            {
                position.x -= _panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= _panBorderThickness)
            {
                position.x += _panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - _panBorderThickness)
            {
                position.z += _panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= _panBorderThickness)
            {
                position.z -= _panSpeed * Time.deltaTime;
            }

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            position.y -= scroll * _scrollSpeed * 100f * Time.deltaTime;

            position.x = Mathf.Clamp(position.x, -_panLimit.x, _panLimit.x);
            position.y = Mathf.Clamp(position.y, _minY, _maxY);
            position.z = Mathf.Clamp(position.z, -_panLimit.y, _panLimit.y);

            transform.position = position;
        }
    }
}