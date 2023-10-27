using UnityEngine;

public class VisualsManager : MonoBehaviour
{
	[SerializeField] private GameObject _smokeParticles;
	public Transform CamTarget;

	public void ToggleVisuals(bool toggle)
	{
		if (_smokeParticles == null)
		{
			return;
		}
		_smokeParticles.SetActive(toggle);
	}

}
