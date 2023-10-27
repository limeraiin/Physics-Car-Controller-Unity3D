using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CarController : MonoBehaviour
{
	[SerializeField] private bool _active;

	[SerializeField] private bool _canAccelerate;
	
	[SerializeField] private Vector3 _centerOfMass;

	[SerializeField] private float _forwardTorque;

	[SerializeField] private float _reverseTorque;

	[SerializeField] private float _brakeForce;

	[SerializeField] private float _handBrakeForce = 500f;

	[SerializeField] private float _steerAngle;

	[SerializeField] private WheelCollider[] _colliders;

	[SerializeField] private WheelCollider[] _turningColliders;

	[SerializeField] private Transform[] _meshes;

	[SerializeField] private GameObject[] _brakeLights;

	[SerializeField] private AudioSource _engineSound;

	[SerializeField] private float _engineSoundVolume;

	[SerializeField] private float _enginePitchMinimum;

	[SerializeField] private float _enginePitchMaximum;

	[Space(20)] 
	[SerializeField] private VisualsManager _visualsManager;

	private bool _hasStopped;

	private void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = _centerOfMass;
	}

	private void Update()
	{
		UpdateWheelsPositions();
		AudioManagement();
		if (_active)
		{
			ControlTurning();
		}
		else
		{
			ResetSteering();
		}
		if (_active && _canAccelerate)
		{
			ControlAcceleration();
			_hasStopped = false;
		}
		else if (!_hasStopped)
		{
			StopAcceleration();
			_hasStopped = true;
		}
	}

	private void ControlAcceleration()
	{
		for (int i = 0; i < _colliders.Length; i++)
		{
			if (CrossPlatformInputManager.GetAxis("Brake/Reverse") > 0f)
			{
				if (MediumRPM() <= 0f)
				{
					_colliders[i].motorTorque = (0f - _reverseTorque) * CrossPlatformInputManager.GetAxis("Brake/Reverse");
					_colliders[i].brakeTorque = 0f;
				}
				else
				{
					_colliders[i].motorTorque = 0f;
					_colliders[i].brakeTorque = _brakeForce * CrossPlatformInputManager.GetAxis("Brake/Reverse");
				}
			}
			else if (MediumRPM() >= 0f)
			{
				_colliders[i].motorTorque = _forwardTorque * CrossPlatformInputManager.GetAxis("Accelerate");
				_colliders[i].brakeTorque = 0f;
			}
			else
			{
				_colliders[i].motorTorque = 0f;
				_colliders[i].brakeTorque = _brakeForce * CrossPlatformInputManager.GetAxis("Accelerate");
			}
			if (CrossPlatformInputManager.GetAxis("HandBrake") > 0.9f)
			{
				_colliders[i].brakeTorque = _handBrakeForce;
			}
		}
	}

	private float MediumRPM()
	{
		float num = 0f;
		for (int i = 0; i < _colliders.Length; i++)
		{
			num = _colliders[i].rpm;
		}
		return num / _colliders.Length;
	}

	private void ControlTurning()
	{
		for (int i = 0; i < _turningColliders.Length; i++)
		{
			_turningColliders[i].steerAngle = _steerAngle * CrossPlatformInputManager.GetAxis("Horizontal");
		}
	}

	public void StopAcceleration()
	{
		for (int i = 0; i < _colliders.Length; i++)
		{
			_colliders[i].motorTorque = 0f;
		}
	}

	private void ResetSteering()
	{
		for (int i = 0; i < _turningColliders.Length; i++)
		{
			_turningColliders[i].steerAngle = 0f;
		}
	}


	private void UpdateWheelsPositions()
	{
		for (int i = 0; i < _meshes.Length; i++)
		{
			Vector3 pos;
			Quaternion rot;
			_colliders[i].GetWorldPose(out pos, out rot);
			_meshes[i].position = pos;
			_meshes[i].rotation = rot;
		}
	}

	private void AudioManagement()
	{
		if (_active && _canAccelerate)
		{
			if (_colliders[0].rpm >= 0f)
			{
				_engineSound.pitch = _enginePitchMinimum + (_colliders[0].rpm + _colliders[1].rpm + _colliders[2].rpm + _colliders[3].rpm) / 4f * 0.0006f;
			}
			if (_colliders[0].rpm < 0f)
			{
				_engineSound.pitch = _enginePitchMinimum + (_colliders[0].rpm + _colliders[1].rpm + _colliders[2].rpm + _colliders[3].rpm) / 4f * (-0.0006f);
			}
			if (_engineSound.pitch > _enginePitchMaximum)
			{
				_engineSound.pitch = _enginePitchMaximum;
			}
			_engineSound.volume = _engineSoundVolume;
		}
		else
		{
			_engineSound.volume = 0f;
		}
	}

	


	public void Activate()
	{
		_active = true;
		_canAccelerate = true;
		
		_visualsManager.ToggleVisuals(true);
	}
	
	public void Deactivate()
	{
		StopAcceleration();
		_active = false;
		_canAccelerate = false;
		_brakeLights[0].SetActive(false);
		
		_visualsManager.ToggleVisuals(false);
	}

}
