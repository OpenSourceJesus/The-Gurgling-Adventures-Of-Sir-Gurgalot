using UnityEngine;

namespace TAoKR
{
	[ExecuteInEditMode]
	public class DestroyAllComponents : MonoBehaviour
	{
		void Start ()
		{
			foreach (Component component in GetComponents<Component>())
				DestroyImmediate(component);
		}
	}
}