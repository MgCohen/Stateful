using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scaffold.Stateful.Samples
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private int index;

        public void Change()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }
    }
}
