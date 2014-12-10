using UnityEngine;
using System.Collections;


namespace Game.Menu
{
    public class Close : MonoBehaviour
    {

        public void close()
        {
            Destroy(this.gameObject);
        }
    }

}