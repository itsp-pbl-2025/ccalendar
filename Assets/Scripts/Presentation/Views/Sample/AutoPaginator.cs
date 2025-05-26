using System.Collections.Generic;
using DG.Tweening;
using Presentation.Views.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private FadablePage[] pages;
        [SerializeField] private RectTransform pageParent;

        private readonly Queue<ButtonWithLabel> _pageButtonsPool = new();
        private readonly SortedDictionary<int, ButtonWithLabel> _pageButtons = new();
        private readonly List<RectTransform> _dotsList = new();
        
        private Sequence _seq;
        private FadablePage _currentPage;

        private int _pageAmount, _currentPageIndex;
        
        private void Start()
        {
            // 初期ページの指定と、ページ総数のキャッシュ。
            _currentPageIndex = initPageByIndex;
            _pageAmount = pages.Length;
            
            // 最初はスクロール時間無しでページを召喚する
            _currentPage = PlacePage(initPageByIndex);
            ReorderButtons();
        }
        
        /// <summary>
        /// 現在のページに合わせてボタンとドットを召喚し、適切に配置する。
        /// </summary>
        private void ReorderButtons()
        {
            // ボタンを完全に再配置するので、選択状態をリセット
            EventSystem.current.SetSelectedGameObject(null);
            
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

        /// <summary>
        /// UnityEngine.UI.Buttonが使う、隣のページに移動するメソッド。
        /// 関数の上の小さい文字を見ると、他は "2 usage" とか書いてあるのにこれは "0+ asset usages" と書いてある。
        /// これは、Unityのオブジェクトがこの関数を使っているよ、という意味。
        /// </summary>
        /// <param name="right">スクロール方向は右か？</param>
        public void ScrollPageRight(bool right)
        {
           ScrollPageTo(right ? _currentPageIndex + 1 : _currentPageIndex - 1);
        }

        /// <summary>
        /// 特定のページに移動するメソッド
        /// </summary>
        /// <param name="index">移動先ページ</param>
        public void ScrollPageTo(int index)
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
            pageNew.Rctf.anchoredPosition = new Vector2(right ? scrollWidth : -scrollWidth, 0f);
            pageNew.SetAlpha(0f);
            _seq?.Complete();
            _seq = DOTween.Sequence()
                .Append(pageOld.Rctf.DOAnchorPosX(right ? -scrollWidth : scrollWidth, timeScroll).SetEase(easeScroll))
                .Join(DOVirtual.Float(1f, 0f, timeScroll, pageOld.SetAlpha).SetEase(easeScroll))
                .Join(pageNew.Rctf.DOAnchorPosX(0f, timeScroll).SetEase(easeScroll))
                .Join(DOVirtual.Float(0f, 1f, timeScroll, pageNew.SetAlpha).SetEase(easeScroll))
                .OnComplete(() =>
                {
                    Destroy(pageOld.gameObject);
                });
            
            _currentPage = pageNew;
            _currentPageIndex = index;
            ReorderButtons();
        }

        /// <summary>
        /// 特定のページを生成して、その参照を返す。
        /// </summary>
        /// <param name="index">ページ番号</param>
        /// <returns>ページの参照</returns>
        private FadablePage PlacePage(int index)
        {
            // インデックスが範囲外だったらあきらめる
            if (index < 0 || index >= _pageAmount) return null;
            
            // 特定のページを、 pageParent の子オブジェクトとして生成する。
            return Instantiate(pages[index], pageParent);
        }

        /// <summary>
        /// 可能なら、ページボタンを生成するメソッド。すでに同じページ番号のページがあったらあきらめる。
        /// </summary>
        /// <param name="index">生成するボタンのページ番号</param>
        /// <returns>生成に成功したか？</returns>
        private bool TryPlacePageButton(int index)
        {
            // インデックスが範囲外だったり、既に存在していたらあきらめる
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
        
        /// <summary>
        /// 新しいボタンをもらうメソッド。生成したけど使っていないボタンがあればそれを返して、無ければ新しく生成して返す。
        /// </summary>
        /// <returns></returns>
        private ButtonWithLabel GetPageButton()
        {
            // こういう、「使っていないオブジェクトを削除せず残しておいて後で再利用する」ようなのを「プール処理」と呼ぶ。
            // MonoBehaviourの作成はちょっと重いのでプール処理をしているが、正直小規模な1,2個ならそんなに変わらない…
            // あとそもそもUnityがObjectPool<T>を用意しているので、理解できるならそっちを使うとよい。
            if (_pageButtonsPool.Count <= 0) return Instantiate(pageButtonPrefab, pageButtonParent);
            
            var box = _pageButtonsPool.Dequeue();
            box.gameObject.SetActive(true);
            return box;
        }
        
        /// <summary>
        /// 使い終わったボタンを返してもらうメソッド。GameObject.SetActive(false)を呼ぶことで、見えなくする。
        /// </summary>
        /// <param name="btn">使い終わったボタン</param>
        private void ReleasePageButton(ButtonWithLabel btn)
        {
            btn.gameObject.SetActive(false);            
            _pageButtonsPool.Enqueue(btn);
        }
    }
}