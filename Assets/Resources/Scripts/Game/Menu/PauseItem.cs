using UnityEngine;
using System.Collections;


namespace Game.Menu
{
    public class PauseItem : MonoBehaviour
    {
        private Animator _animator2;
        private CanvasGroup _CanvasGroup2;

        public bool IsOpened
        {
            get { return _animator2.GetBool("IsOpened"); }
			set { _animator2.SetBool ("IsOpened", value);}
//            set { _animator2.SetBool("IsOpened", value); }
        }

        public void Awake()
        {
            _animator2 = GetComponent<Animator>();
            _CanvasGroup2 = GetComponent<CanvasGroup>();

            var rect = GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = new Vector2(0, 0);

        }

        public void Update()
        {
            if (!_animator2.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                _CanvasGroup2.blocksRaycasts = _CanvasGroup2.interactable = false;
            }
            else
            {
                _CanvasGroup2.blocksRaycasts = _CanvasGroup2.interactable = true;
            }
        }
    }
}