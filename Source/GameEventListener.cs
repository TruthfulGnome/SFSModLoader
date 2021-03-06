using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	[Tooltip("Event to register with.")]
	public GameEvent Event;

	[Space, Tooltip("Response to invoke when Event is raised.")]
	public UnityEvent Response;

	private void OnEnable()
	{
		this.Event.RegisterListener(this);
	}

	private void OnDisable()
	{
		this.Event.UnregisterListener(this);
	}

	public void OnEventRaised()
	{
		this.Response.Invoke();
	}
}
