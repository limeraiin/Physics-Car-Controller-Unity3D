using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	[SerializeField] private Vector2 _mouseSensivity;
	
	[SerializeField] private float _lookSensivity = 1f;
	
	[SerializeField] private float _rotationAdjustSpeed = 1f;
	
	[SerializeField] private float _minimumFOV = 45f;
	
	[SerializeField] private float _maximumFOV = 75f;
	
	[SerializeField] private float _FOVSensivity = 0.001f;
	
	[SerializeField] private bool _canMoveCamera = true;

	private Camera _mainCamera;
	private Transform _currentTarget;



	private void Start()
	{
		AddListeners();
		_mainCamera = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_mainCamera.fieldOfView = _minimumFOV;

	}

	private void AddListeners()
	{
		GameEvents.Instance.OnCarSwitch += OnCarSwitch;
	}

	private void OnCarSwitch(CarController carController)
	{
		_currentTarget = carController.transform;
	}

	private void Update()
	{
		transform.position = _currentTarget.position;
		if (Time.timeScale > 0f && _canMoveCamera)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	private void LateUpdate()
	{
		if (_canMoveCamera)
		{
			ControlCamera();
		}
	}

	private void FixedUpdate()
	{
		_mainCamera.fieldOfView = Mathf.Lerp(_minimumFOV, _maximumFOV, _currentTarget.GetComponent<Rigidbody>().velocity.magnitude * _FOVSensivity);
	}

	private void ControlCamera()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, _currentTarget.transform.rotation, _rotationAdjustSpeed * Time.deltaTime);
		_mainCamera.gameObject.transform.LookAt(_currentTarget.GetComponent<VisualsManager>().CamTarget);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x + Input.GetAxis("Mouse Y") * _mouseSensivity.y * _lookSensivity * Time.deltaTime, transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSensivity.x * _lookSensivity * Time.deltaTime, 0f);
	}


}
