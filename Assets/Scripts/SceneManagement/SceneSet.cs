using System.Collections.Generic;
using UnityEngine;

namespace WiDiD.SceneManagement
{
	[CreateAssetMenu(fileName = "SceneSet", menuName = "Simulator Configuration/Scene set")]
	public class SceneSet : ScriptableObject
	{
		[SerializeField]
		List<SceneReference> m_Scenes;
		[SerializeField]
		SceneReference m_ActiveScene;

		public List<SceneReference> Scenes => m_Scenes;
		public SceneReference ActiveScene => m_ActiveScene;
	}
}
