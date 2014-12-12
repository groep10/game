using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game.Menu
{
    public class MenuList : MonoBehaviour
    {

        List<GameObject> items = new List<GameObject>();
        bool changed = true;

        public GameObject testFab;

        void Start()
        {
            for (int i = 0; i < 5; i += 1)
            {
                items.Add(Instantiate(testFab) as GameObject);
            }
        }

        void Update()
        {
            if (!changed) {
                return;
            }
            float innerHeight = 0;

            RectTransform pos = GetComponent<RectTransform>();
            float width = pos.rect.width;
            foreach(GameObject obj in items) {
                RectTransform r = obj.GetComponent<RectTransform>();
                r.SetParent(pos, false);
                //float ratio = r.rect.width / width;
                //float height = r.rect.height / ratio;
                float height = r.rect.height;
                r.offsetMin = new Vector2(0, - innerHeight - height);
                r.offsetMax = new Vector2(0, - innerHeight);

                innerHeight += height;
            }
            // Strech
            pos.sizeDelta = new Vector2(0, innerHeight);

            changed = false;
        }

        
    }

}