using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    internal class SceneLoader : MonoBehaviour
    {
        public void Load()
        {
            SceneManager.LoadScene("DungeonScene");
        }
    }
}
