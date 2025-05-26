using System;
using DG.Tweening;
using Presentation.Views.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Sample
{
    public class SampleAreaVisualizer : MonoBehaviour
    {
        [SerializeField] private ButtonRP buttonOn, buttonOff;
        [SerializeField] private Color paintedFull, paintedSafe, paintedFixed;

        private Image _fullGround, _safeGround, _fixedGround;
        private Color _initFull, _initSafe, _initFixed;
        
        private bool _isOn;
        
        private void Start()
        {
            // SampleSceneでは混乱を避けるために可能な限り常駐するスクリプトを減らしたくて以下の処理を書いている！！
            // GameObject.FindとGetComponent<T>は効率が悪く負荷も大きいため、Sample以外ではほぼ許しません。ごめんね。
            _fullGround = GameObject.Find("FullAreaBackground").GetComponent<Image>();
            _safeGround = GameObject.Find("SafeAreaBackground").GetComponent<Image>();
            _fixedGround = GameObject.Find("FixedAreaBackground").GetComponent<Image>();
            
            _initFull = _fullGround.color;
            _initSafe = _safeGround.color;
            _initFixed = _fixedGround.color;
            
            // ToggleButtonっぽくするために、二つの同じ位置にあるボタンの出現状態を切り替えている
            buttonOff.gameObject.SetActive(false);
        }

        private Sequence _seq;
        
        /// <summary>
        /// 背景色を切り替える
        /// </summary>
        /// <param name="visible">着色するかどうか</param>
        public void SetVisible(bool visible)
        {
            const float timeColorChange = 0.5f;
            
            // 重複して呼び出されると面倒なので、これを呼び出すボタンを無効にする。
            if (visible)
            {
                buttonOn.interactable = false;
            }
            else
            {
                buttonOff.interactable = false;
            }
            
            // 前回のアニメーションが終了してなかったら即座に中断する
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            _seq.Append(_fullGround.DOColor(visible ? paintedFull : _initFull, timeColorChange))
                .Join(_safeGround.DOColor(visible ? paintedSafe : _initSafe, timeColorChange))
                .Join(_fixedGround.DOColor(visible ? paintedFixed : _initFixed, timeColorChange))
                .OnComplete(() =>
                {
                    // ボタンが削除されていたら作動させない
                    if (!buttonOn || !buttonOff) return;
                    
                    // アニメーションが最後まで終了したらボタンの無効を解除し、もう一方のボタンと表示を切り替える
                    if (visible)
                    {
                        buttonOn.interactable = true;
                    }
                    else
                    {
                        buttonOff.interactable = true;
                    }
                    buttonOn.gameObject.SetActive(!visible);
                    buttonOff.gameObject.SetActive(visible);
                });
            
            _isOn = visible;
        }

        /// <summary>
        /// OnDisableは、gameObject.SetActive(false)やComponent.enable = false;が行われるなどして
        /// Componentの動作が停止する前に呼び出される。もちろん削除されるときもOnDisableは呼ばれる。
        /// 今回は、ページが遷移して削除されることを想定している。
        /// </summary>
        private void OnDisable()
        {
            if (_isOn) SetVisible(false);
        }

        /// <summary>
        /// 一方こちらは、「アプリが終了する」時に呼ばれる。UnityEditor.PlayModeの終了やタスクキルがこれに当たる。
        /// 他にもOnDestroy()という関数もあるが、特段理由がない限り、このプロジェクトはOnDisable()で十分問題ない。
        /// というのも、"OnDestroy"はアプリが閉じた際にのみ"OnApplicationQuit"よりも後に呼ばれるという特徴がある。
        /// 実はデータベースのクローズ処理がOnApplicationQuitに書かれているので、何かしらのセーブをOnDestroyでされると…
        /// </summary>
        private void OnApplicationQuit()
        {
            // OnDisableでSequenceが動作してしまうので、ApplicationQuit起因のOnDisableだった際に止めておく。
            _seq?.Kill();
        }
    }
}