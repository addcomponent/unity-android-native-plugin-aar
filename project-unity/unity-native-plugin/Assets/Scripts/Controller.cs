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


	private const int MAX_COUNT = 3;

	private int _count;

	private AspectRatioFitter _aspectRatioFitter;


	public void TakePicture()
	{
		_androidCamera.TakePicture ("picture.png", (bool success, string path) => 
		{
			if(success)
			{
				_count = MAX_COUNT;

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

		string picturePath = "file://" + path;

		WWW www = new WWW (picturePath);

		yield return www;

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
			Sprite sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero, 1);

			if(_aspectRatioFitter == null)
			{
				_aspectRatioFitter = _image.GetComponent <AspectRatioFitter>();
			}

			_aspectRatioFitter.aspectRatio = (float)sprite.texture.width/(float)sprite.texture.height;

			_image.sprite = sprite;

			Resources.UnloadUnusedAssets ();
		}	
	}
}