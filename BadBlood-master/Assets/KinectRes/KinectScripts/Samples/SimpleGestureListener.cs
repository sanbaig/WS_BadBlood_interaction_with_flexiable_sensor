using UnityEngine;
//using Windows.Kinect;
using System.Collections;
using System;
using UnityEngine.UI;

public class SimpleGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	[Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
	public Text GestureInfo;
	
	// private bool to track if progress message has been displayed
	private bool progressDisplayed;
	private float progressGestureTime;

	
	public void UserDetected(long userId, int userIndex)
	{
		// as an example - detect these user specific gestures
		KinectManager manager = KinectManager.Instance;
		manager.DetectGesture(userId, KinectGestures.Gestures.Jump);
		manager.DetectGesture(userId, KinectGestures.Gestures.Squat);
		manager.DetectGesture(userId, KinectGestures.Gestures.LeanLeft);
		manager.DetectGesture(userId, KinectGestures.Gestures.LeanRight);

		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<Text>().text = "Swipe, Jump, Squat or Lean.";
		}
	}
	
	public void UserLost(long userId, int userIndex)
	{
		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<Text>().text = string.Empty;
		}
	}

	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		if((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{
			string sGestureText = string.Format ("{0} - {1:F0}%", gesture, screenPos.z * 100f);
			if(GestureInfo != null)
			{
				GestureInfo.GetComponent<Text>().text = sGestureText;
			}

			progressDisplayed = true;
			progressGestureTime = Time.realtimeSinceStartup;
		}
		else if((gesture == KinectGestures.Gestures.Wheel || gesture == KinectGestures.Gestures.LeanLeft || 
		         gesture == KinectGestures.Gestures.LeanRight) && progress > 0.5f)
		{
			string sGestureText = string.Format ("{0} - {1:F0} degrees", gesture, screenPos.z);
			if(GestureInfo != null)
			{
				GestureInfo.GetComponent<Text>().text = sGestureText;
			}

			//Debug.Log(sGestureText);
			progressDisplayed = true;
			progressGestureTime = Time.realtimeSinceStartup;
		}
	}

	public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		if(progressDisplayed)
			return true;

		string sGestureText = gesture + " detected";
		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<Text>().text = sGestureText;
		}
		
		return true;
	}

	public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		if(progressDisplayed)
		{
			progressDisplayed = false;

			if(GestureInfo != null)
			{
				GestureInfo.GetComponent<Text>().text = String.Empty;
			}
		}
		
		return true;
	}

	public void Update()
	{
		if(progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
		{
			progressDisplayed = false;
			
			if(GestureInfo != null)
			{
				GestureInfo.GetComponent<Text>().text = String.Empty;
			}

			Debug.Log("Forced progress to end.");
		}
	}
	
}
