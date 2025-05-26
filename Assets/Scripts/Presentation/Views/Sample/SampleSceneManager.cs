using UnityEngine;

namespace Presentation.Views.Sample
{
    /// <summary>
    /// MonoBehaviourを継承したクラス。
    /// クラス名がファイル名と同じでかつ、MonoBehaviourを継承しているとUnityEditorでオブジェクトにAttach(取り付け)をできる。
    /// </summary>
    public class SampleSceneManager : MonoBehaviour
    {
        // SerializeField属性を利用すると、UnityEngine側から開始時に保有する変数やクラスを指定できます。
        // なんでも入れられるわけではなく、SerializeField属性の付与には制限があります。属性がRiderで赤紫になってればok!
        // Headerという属性もついているけど、これはUnityEngine側で文字を表示できる機能。ロジックには関係ない。
        [Header("ここに登録したGameObjectは、開始時に削除される")]
        [SerializeField] private GameObject[] deleteInitial;

        /// <summary>
        /// Awake関数は、このスクリプトがついているGameObjectが生成された瞬間に実行される、呼び出しの必要がない関数。
        /// Unity側から呼ばれる、呼び出す必要のない関数は総じて"Event Function"と言われる。
        /// 類似した昨日のStart関数は、生成された次のフレームで実行される。実行順は公式のスクリプトリファレンスを閲覧しよう。
        /// ちなみにこの "summary" に囲まれた部分は特殊なコメントで、他の場所からでも閲覧しやすくなる。
        /// 試しに、下の Awake の文字にマウスオーバーをしてみよう…
        /// </summary>
        private void Awake()
        {
            // deleteInitialに登録されたGameObjectについて順番に以下を実行…
            foreach (var obj in deleteInitial)
            {
                // GameObject自身を破壊！
                Destroy(obj);
            }
        }
    }
}