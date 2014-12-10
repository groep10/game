using UnityEngine;
using System.Collections;


namespace Game.Menu
{
    public class Item : MonoBehaviour
    {
        private Animator _animator;
        private CanvasGroup _CanvasGroup;

        public bool IsOpen
        {
            get { return _animator.GetBool("IsOpen"); }
            set { _animator.SetBool("IsOpen", value); }
        }

        public void Awake()
        {
            _animator = GetComponent<Animator>();
            _CanvasGroup = GetComponent<CanvasGroup>();

            var rect = GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = new Vector2(0, 0);

        }

        public void Update()
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                _CanvasGroup.blocksRaycasts = _CanvasGroup.interactable = false;
            }
            else
            {
                _CanvasGroup.blocksRaycasts = _CanvasGroup.interactable = true;
            }
        }
    }
}