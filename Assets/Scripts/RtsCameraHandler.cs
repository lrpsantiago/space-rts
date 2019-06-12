using PushingBoxStudios.Controls;
using UnityEngine;

namespace PushingBoxStudios.SteampunkTd.Cameras
{
    [AddComponentMenu("Cameras/Real Time Strategy")]
    public class RtsCameraHandler : MonoBehaviour
    {
        [Header("Camera Control")]
        [SerializeField]
        private Transform _target;

        #region Translation Fields

        [Header("Translation")]
        [SerializeField]
        private bool _enableTranslation = false;

        [SerializeField]
        private float _translationSpeed = 10f;

        [SerializeField]
        private float _translationSmoothTime = 0.2f;

        [SerializeField]
        private Vector3 _minBounds = new Vector3(-16, 0, -16);

        [SerializeField]
        private Vector3 _maxBounds = new Vector3(16, 0, 16);

        private Vector3 _destTranslation;
        private Vector3 _translationVelocity;

        #endregion

        #region Rotation Fields

        [Header("Rotation")]
        [SerializeField]
        private bool _enableRotation = false;

        [SerializeField]
        private float _rotationSmoothTime = 0.2f;

        [SerializeField]
        private EMouseButton _rotationMouseButton = EMouseButton.MiddleButton;

        private float _destRotation;
        private float _rotationVelocity;
        private const float MAX_ROTATION_SPEED = 90f;

        #endregion

        #region Zoom Fields

        [Header("Zoom")]
        [SerializeField]
        private bool _enableZoom = false;

        [SerializeField]
        private float _zoomSmoothTime = 0.2f;

        [SerializeField]
        private float _minZoomDistance = 5f;

        [SerializeField]
        private float _maxZoomDistance = 10f;

        private Vector3 _destZoom;
        private Vector3 _zoomVelocity;

        #endregion

        private Camera _camera;
        private readonly Quaternion _defaultRotation = Quaternion.Euler(60, 0, 0);
        private readonly Vector3 _defaultZoomDistance = new Vector3(0, 10, -10);

        public Vector3 MinBounds
        {
            get { return _minBounds; }
            set { _minBounds = value; }
        }

        public Vector3 MaxBounds
        {
            get { return _maxBounds; }
            set { _maxBounds = value; }
        }

        // Constants
        private const float TRANSLATE_SCREEN_DEADZONE = 20f;

        public void Awake()
        {
            if (_target == null)
            {
                throw new UnityException("A target is required.");
            }

            _camera = Camera.main;
            _camera.transform.LookAt(_target);

            _destTranslation = _target.transform.position;
            _destRotation = _target.transform.rotation.y;
            _destZoom = _camera.transform.localPosition;
        }

        private void Start()
        {
            Reset();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Reset();
            }

            if (_enableRotation)
            {
                HandleRotation();
            }

            if (_enableTranslation)
            {
                HandleTranslation();
            }

            if (_enableZoom)
            {
                HandleZooming();
            }

            HandleMovement();
        }

        private void HandleRotation()
        {
            if (Input.GetMouseButton((int)_rotationMouseButton))
            {
                float rotationFactor = Input.GetAxis("Mouse X");
                const float sensitivity = 90;

                _destRotation += rotationFactor * sensitivity * Time.deltaTime;
            }
        }

        private void HandleTranslation()
        {
            if (Input.GetMouseButton((int)_rotationMouseButton))
            {
                _translationVelocity = Vector3.zero;
                _destTranslation = _target.transform.position;
                return;
            }

            Vector3 mousePosition = Input.mousePosition;
            Vector3 direction = Vector3.zero;

            bool goingLeft = mousePosition.x < TRANSLATE_SCREEN_DEADZONE || Input.GetKey(KeyCode.LeftArrow);

            if (goingLeft)
            {
                direction -= _target.transform.right;
            }

            bool goingRight = mousePosition.x > Screen.width - TRANSLATE_SCREEN_DEADZONE ||
                Input.GetKey(KeyCode.RightArrow);

            if (goingRight)
            {
                direction += _target.transform.right;
            }

            bool goingBack = mousePosition.y < TRANSLATE_SCREEN_DEADZONE || Input.GetKey(KeyCode.DownArrow);

            if (goingBack)
            {
                direction -= _target.transform.forward;
            }

            bool goingForward = mousePosition.y > Screen.height - TRANSLATE_SCREEN_DEADZONE ||
                Input.GetKey(KeyCode.UpArrow);

            if (goingForward)
            {
                direction += _target.transform.forward;
            }

            Vector3 movement = direction.normalized * _translationSpeed * Time.deltaTime;
            movement = Vector3.ClampMagnitude(movement, _translationSpeed);

            if ((_target.transform.position.x <= _minBounds.x && movement.x < 0) ||
                (_target.transform.position.x >= _maxBounds.x && movement.x > 0))
            {
                movement.x = 0;
            }

            if ((_target.transform.position.z <= _minBounds.z && movement.z < 0) ||
                (_target.transform.position.z >= _maxBounds.z && movement.z > 0))
            {
                movement.z = 0;
            }

            _destTranslation += movement;
        }

        private void HandleZooming()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            var sensivity = 25f;
            var factor = scroll * sensivity;

            if (Mathf.Abs(factor) > 0)
            {
                var camPos = _camera.transform.localPosition;
                var targetPos = Vector3.zero;
                var forward = Vector3.Normalize(targetPos - camPos);
                var destination = camPos + forward * factor;

                _destZoom = ClampZoom(destination, _minZoomDistance, _maxZoomDistance);
            }
        }

        private void HandleMovement()
        {
            DoRotation();
            DoTranslation();
            DoZoomming();
        }

        private void DoRotation()
        {
            float rotY = Mathf.SmoothDampAngle(_target.transform.eulerAngles.y, _destRotation, ref _rotationVelocity,
                _rotationSmoothTime, float.MaxValue, Time.deltaTime);

            _target.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }

        private void DoTranslation()
        {
            Vector3 targetPos = _target.transform.position;

            targetPos = Vector3.SmoothDamp(targetPos, _destTranslation, ref _translationVelocity,
                _translationSmoothTime);

            targetPos.x = Mathf.Clamp(targetPos.x, _minBounds.x, _maxBounds.x);
            targetPos.z = Mathf.Clamp(targetPos.z, _minBounds.z, _maxBounds.z);

            _target.transform.position = targetPos;
        }

        private void DoZoomming()
        {
            Vector3 camPos = _camera.transform.localPosition;

            camPos = Vector3.SmoothDamp(camPos, _destZoom, ref _zoomVelocity, _zoomSmoothTime);

            _camera.transform.localPosition = camPos;
        }

        private void Reset()
        {
            _destRotation = _defaultRotation.eulerAngles.y;
            _destZoom = _defaultZoomDistance;
        }

        private Vector3 ClampZoom(Vector3 destination, float min, float max)
        {
            destination = Vector3.ClampMagnitude(destination, max);

            float minSide = Mathf.Sqrt(Mathf.Pow(min, 2) / 2);
            Vector3 minDist = Vector3.zero;
            minDist.y = minSide;
            minDist.z = minSide;

            Vector3 distance = Vector3.zero - destination;

            if (distance.magnitude < min || destination.y < 0 || destination.z > 0)
            {
                destination.x = 0;
                destination.y = minDist.y;
                destination.z = -minDist.z;
            }

            return destination;
        }
    }
}
