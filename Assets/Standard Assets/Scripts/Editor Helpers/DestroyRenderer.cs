using UnityEngine;

namespace TGAOSG
{
	public class DestroyRenderer : MonoBehaviour
	{
		void Start ()
		{
			if (Application.isPlaying)
			{
				Destroy(GetComponent<Renderer>());
				Destroy(this);
			}
		}
	}
}