using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T mInstance;
	
	public static T Instance
	{
		get
		{
			if ( !mInstance )
			{
				mInstance = ( T ) FindObjectOfType( typeof(T) );
				
				if ( !mInstance )
				{
					Debug.LogError("Singleton object '" + typeof(T).ToString() + "' not found in scene!");
				}
			}
			
			return mInstance;
		}
	}
	
	public virtual void Destroy()
	{
		mInstance = null;
	}
}