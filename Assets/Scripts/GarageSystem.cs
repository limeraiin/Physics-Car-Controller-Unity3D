using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageSystem : MonoBehaviour
{
	
    [SerializeField] private CarController[] _vehicles;

    private CarController _currentVehicle;
    private int _vehicleIndex = -1;

    private void Start()
    {
	    SwitchVehicle();
    }

    private void Update()
    {
	    if (Input.GetKeyDown(KeyCode.Tab))
	    {
		    SwitchVehicle();
	    }
    }

    private void SwitchVehicle()
    {
	    if (_vehicleIndex>-1)
	    {
		    _currentVehicle.Deactivate();
	    }
	    
	    if (_vehicleIndex == _vehicles.Length-1)
	    {
		    _vehicleIndex = 0;
	    }
	    else
	    {
		    _vehicleIndex++;
	    }
	    _currentVehicle = _vehicles[_vehicleIndex];
	    GameEvents.Instance.OnCarSwitch.Invoke(_currentVehicle);
	    
	    _currentVehicle.Activate();
    }

}
