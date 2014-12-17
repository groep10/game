using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game.Menu
{
    public class MenuList : MonoBehaviour
    {

        List<GameObject> items = new List<GameObject>();
        bool changed = false;

        void Start() {

        }

        void Update() {
            if (!changed) {
                return;
            }
            float innerHeight = 0;
            float totalHeight = 0;
            RectTransform pos = GetComponent<RectTransform>();
            foreach (GameObject obj in items)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                }
                RectTransform r = obj.GetComponent<RectTransform>();
                r.SetParent(pos, false);
                totalHeight += r.rect.height;
            }

            float width = pos.rect.width;
            foreach(GameObject obj in items) {
                RectTransform r = obj.GetComponent<RectTransform>();
                float height = r.rect.height;
                
                r.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, width);
                r.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, innerHeight, height);

                innerHeight += height;
            }
            pos.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, innerHeight);

            changed = false;
        }

        public void addItem(GameObject obj)
        {
            items.Add(obj);
            changed = true;
        }

        public void setItems(GameObject[] objs)
        {
			foreach(GameObject obj in items) {
				Destroy(obj);
			}
            items.Clear();
            items.AddRange(objs);
            changed = true;
        }
        
    }

}