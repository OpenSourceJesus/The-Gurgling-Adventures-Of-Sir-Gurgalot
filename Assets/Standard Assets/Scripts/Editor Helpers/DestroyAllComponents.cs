using UnityEngine;

namespace TGAOSG
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