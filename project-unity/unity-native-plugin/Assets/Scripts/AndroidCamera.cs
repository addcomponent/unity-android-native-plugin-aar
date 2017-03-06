using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidCamera : MonoBehaviour 
{
	private const string CAMERA_PACKAGE_NAME = "com.addcomponent.camera";
	private const string CAMERA_CLASS_NAME = ".CameraFragment";
	private const string CAMERA_METHOD_TAKE = "takePicture";
	private const string CAMERA_METHOD_TAKE_CALLBACK = "TakePictureCallback";


	public delegate void OnTakePictureCallbackHandler (bool success, string path);

	private OnTakePictureCallbackHandler _callback;


	public void TakePicture (string filename, OnTakePictureCallbackHandler callback)
	{
		using (AndroidJavaObject camera = new AndroidJavaObject (CAMERA_PACKAGE_NAME + CAMERA_CLASS_NAME))
		{ 
			_callback = callback;

			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.Log("Take Picture | " + "filename: " + filename);
			#endif

			camera.Call (CAMERA_METHOD_TAKE, gameObject.name, filename, CAMERA_METHOD_TAKE_CALLBACK);
		} 
	}

	public void TakePictureCallback (string result)
	{
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Debug.Log("Take Picture Callback | " + "result: " + result);
		#endif

		if(_callback != null)
		{
			_callback.Invoke (!string.IsNullOrEmpty (result), result);
			_callback = null;
		}
		else 
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.Log("No callback defined");
			#endif
		}
	}
}