using System.Collections.Generic;
using DG.Tweening;
using Presentation.Views.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Sample
{
    public class AutoPaginator : MonoBehaviour
    {
        [SerializeField] private int initPageByIndex = 0;
        [SerializeField] private float pageButtonInterval = 20f;
        [SerializeField] private Button leftButton, rightButton;
        [SerializeField] private ButtonWithLabel pageButtonPrefab;
        [SerializeField] private RectTransform dotsPrefab, pageButtonContainer, pageButtonParent;
        [SerializeField] private RectTransform[] pages;
        [SerializeField] private RectTransform pageParent;

        private readonly Queue<ButtonWithLabel> _pageButtonsPool = new();
        private readonly SortedDictionary<int, ButtonWithLabel> _pageButtons = new();
        private readonly List<RectTransform> _dotsList = new();
        
        private Sequence _seq;
        private RectTransform _currentPage;

        private int _pageAmount, _currentPageIndex;
        
        private void Start()
        {
            _currentPageIndex = initPageByIndex;
            _pageAmount = pages.Length;
            
            // スクロール時間無しでページを召喚する
            _currentPage = PlacePage(initPageByIndex);
            ReorderButtons();
        }
        
        private void ReorderButtons()
        {
            // 全てのページボタンと3点リーダを消す
            foreach (var (_, button) in _pageButtons)
            {
                ReleasePageButton(button);
            }
            foreach (var dots in _dotsList)
            {
                Destroy(dots.gameObject);
            }
            _pageButtons.Clear();
            _dotsList.Clear();
            
            // 表示したいボタンを設定 (最初、最後、自身の近傍)
            TryPlacePageButton(0);
            TryPlacePageButton(_pageAmount - 1);
            TryPlacePageButton(_currentPageIndex - 1);
            TryPlacePageButton(_currentPageIndex);
            TryPlacePageButton(_currentPageIndex + 1);
            
            // 順に見て、ページ選択ボタンを作成
            var offset = 0f;
            var prevIndex = -1;
            foreach (var (index, button) in _pageButtons)
            {
                // 前回のページから離れていたらドットを表示する
                if (prevIndex + 1 != index)
                {
                    var dots = Instantiate(dotsPrefab, pageButtonParent);
                    dots.anchoredPosition = new Vector2(offset + dots.sizeDelta.x/2, 0f);
                    _dotsList.Add(dots);
                    offset += dots.sizeDelta.x + pageButtonInterval;
                }
                
                button.RectTransform.anchoredPosition = new Vector2(offset + pageButtonPrefab.RectTransform.sizeDelta.x / 2f, 0f);
                offset += button.RectTransform.sizeDelta.x + pageButtonInterval;
                prevIndex = index;
            }
            
            // ページボタンの入れ物のサイズを調整して、右ボタンと左ボタンの位置を整える
            pageButtonContainer.sizeDelta = new Vector2(offset - pageButtonInterval, pageButtonContainer.sizeDelta.y);
            
            // 今がページの端だったら…
            leftButton.interactable = _currentPageIndex > 0;
            rightButton.interactable = _currentPageIndex < _pageAmount - 1;
        }

        public void ScrollPageRight(bool right)
        {
           ScrollPageTo(right ? _currentPageIndex + 1 : _currentPageIndex - 1);
        }

        private void ScrollPageTo(int index)
        {
            // このメソッドでしか使わない定数はローカルで宣言する
            const float timeScroll = 0.2f;
            const Ease easeScroll = Ease.OutQuad;
            
            if (index == _currentPageIndex || index < 0 || index >= _pageAmount) return;
            var right = _currentPageIndex < index;
            var scrollWidth = pageParent.sizeDelta.x;

            // _currentPageをOnCompleteでDestroyするので、参照のために別変数に代入する
            var pageOld = _currentPage;
            var pageNew = PlacePage(index);
            
            // DOTweenでアニメーションをする
            pageNew.anchoredPosition = new Vector2(right ? scrollWidth : -scrollWidth, 0f);
            _seq?.Complete();
            _seq = DOTween.Sequence()
                .Append(pageOld.DOAnchorPosX(right ? -scrollWidth : scrollWidth, timeScroll).SetEase(easeScroll))
                .Join(pageNew.DOAnchorPosX(0f, timeScroll).SetEase(easeScroll))
                .OnComplete(() =>
                {
                    Destroy(pageOld.gameObject);
                });
            
            _currentPage = pageNew;
            _currentPageIndex = index;
            ReorderButtons();
        }

        private RectTransform PlacePage(int index)
        {
            if (index < 0 || index >= _pageAmount) return null;
            return Instantiate(pages[index], pageParent);
        }

        private bool TryPlacePageButton(int index)
        {
            if (index < 0 || index >= _pageAmount) return false;
            if (_pageButtons.ContainsKey(index)) return false;
            
            // ボタンをもらってきてラベルを変え、ボタンについたイベントを削除する
            var button = GetPageButton();
            button.Label.text = (index + 1).ToString();
            button.Button.onClick.RemoveAllListeners();
            
            if (index == _currentPageIndex)
            {
                // 同じページならボタンを押させない
                button.Button.interactable = false;
            }
            else
            {
                // 違うページなら押したときにそのページに移動する
                button.Button.interactable = true;
                button.Button.onClick.AddListener(() =>
                {
                    ScrollPageTo(index);
                });
            }
            _pageButtons.Add(index, button);
            
            return true;
        }
        
        private ButtonWithLabel GetPageButton()
        {
            // MonoBehaviourの作成はちょっと重いのでプール処理をしているが、正直小さい1,2個ならそんなに変わらない
            // あとそもそもUnityがObjectPool<T>を用意しているので、理解できるならそっちを使うとよい
            if (_pageButtonsPool.Count <= 0) return Instantiate(pageButtonPrefab, pageButtonParent);
            
            var box = _pageButtonsPool.Dequeue();
            box.gameObject.SetActive(true);
            return box;
        }
        
        private void ReleasePageButton(ButtonWithLabel btn)
        {
            btn.gameObject.SetActive(false);            
            _pageButtonsPool.Enqueue(btn);
        }
    }
}