using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour 
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private AndroidCamera _androidCamera;


	private const int MAX_COUNT = 3; // maximum number of tries to load the picture before giving up

	private int _count; // current number of tries 

	private AspectRatioFitter _aspectRatioFitter; // image aspect ratio fitter component, helps to fit or fill the image


	// called when the user presses the take picture button
	public void TakePicture()
	{
		// call the take picture method on the AndroiCamera, set the picture name and register for the callback
		_androidCamera.TakePicture ("picture.png", (bool success, string path) => 
		{
			// if succeded
			if(success)
			{
				// reset the current number of tries
				_count = MAX_COUNT;

				// load the picture using the resulting image path 
				LoadPicture(path);
			}
		});
	}

	private void LoadPicture (string path)
	{
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Debug.Log("Take Picture | " + "path: " + path + ", count: " + _count);
		#endif

		if (_count > 0)
		{
			StartCoroutine (LoadPictureCoroutine (path));
		}
	}

	private IEnumerator LoadPictureCoroutine (string path)
	{
		_count--;

		string picturePath = "file://" + path; // add file:// because its a local file 

		WWW www = new WWW (picturePath);

		yield return www;

		// we've found out that the picture might not be immediately available 
		// so we keep trying to load it until its available
		if (www.size == 0)
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.Log("Load Picture | Picture not available");
			#endif

			yield return new WaitForSeconds (1.0f);

			LoadPicture (path);
		}
		else if (!string.IsNullOrEmpty (www.error))
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.Log("Load Picture Failed | " + "www.error: " + www.error);
			#endif
		}
		else
		{
			// create a new sprite with the loaded texture
			Sprite sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero, 1);

			if(_aspectRatioFitter == null)
			{
				_aspectRatioFitter = _image.GetComponent <AspectRatioFitter>();
			}
				
			// set the picture image ratio
			// change the scale mode on the AspectRatioFitter component
			_aspectRatioFitter.aspectRatio = (float)sprite.texture.width/(float)sprite.texture.height;

			// set the image sprite
			_image.sprite = sprite;

			// unload previous unused images othwerise they are kept in memory
			Resources.UnloadUnusedAssets ();
		}	
	}
}