using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARInputManager : MonoBehaviour {

	// debug helpers
	[SerializeField]
	private bool _showEvents;
	private string _lastDebug;

	private float _touchStartTime;

	[SerializeField]
	private float _clickThreshold;

	// previous number of touches
	private int _lastCount;

	// previous location of touch
	private Touch _lastTouch;

	// Update is called once per frame
	void Update () {
		int touchCount = Input.touchCount;	

		if(touchCount == 0)
		{
			if(_lastCount == 1)
			{
				TouchUp(_lastTouch.position);

				if(Time.time - _touchStartTime < _clickThreshold)
				{
					TouchClick(_lastTouch.position);
				}
			}

			_lastCount = 0;
		}

		if(touchCount == 1)
		{
			if(_lastCount == 0)
			{
				_touchStartTime = Time.time;
				TouchDown(_lastTouch.position);
			}

			_lastTouch = Input.touches[0];
			_lastCount = 1;
		}
	}

	public void TouchDown(Vector2 pos)
	{
		DebugWithoutRepeats("TouchDown" + pos);
	}

	public void TouchUp(Vector2 pos)
	{
		DebugWithoutRepeats("TouchUp" + pos);
	}

	public void TouchClick(Vector2 pos)
	{
		DebugWithoutRepeats("TouchClick" + pos);
	}

	public void DebugWithoutRepeats(string logMessage)
    {
        if (_showEvents)
        {
            if (_lastDebug != logMessage)
            {
                Debug.Log(logMessage);
            }
            _lastDebug = logMessage;
        }
    }
}
