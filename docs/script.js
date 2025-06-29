// script.js
// Lunatisプロジェクトの設計書ウェブサイト用のJavaScriptコードです
// このスクリプトは、ウェブサイトの初期表示やナビゲーション機能を実装します。
// また、GitHubからスクリプトのコードを取得し、表示する機能も含まれています。
// このコードは、HTMLのDOMContentLoadedイベントをリッスンし、
// ページが読み込まれた後に実行されます。
// 主要なHTML要素への参照を取得し、
// 初期表示の更新情報カードを作成します。  
// また、スクリプトカードのサンプルデータを作成し、
// カード表示の関数を定義します。
// 詳細コンテンツ表示の関数も定義し、
// コードのハイライト機能を追加しています。
// また、カードクリックイベントの委譲を実装し、
// スクリプトの詳細情報を表示する機能も含まれています。

document.addEventListener('DOMContentLoaded', () => {

    // ここでは、主要なHTML要素への参照を取得します。
    // これらの要素は、ナビゲーションメニューやコンテンツエリアとして使用されます。▽
	const areaA        = document.getElementById('area-a');
	const areaB        = document.getElementById('area-b');
	const navHome      = document.getElementById('nav-home');
	const navMapSystem = document.getElementById('nav-map-system');
	const navScripts   = document.getElementById('nav-scripts');

    // ここでは、ウェブサイトの初期表示として、更新情報カードのサンプルデータを作成します。
    // 各カードには、タイトル、日付、説明を含めます。▽
    const updateCards = [

        {
            title      : 'ウェブサイト環境セットアップ完了',
            date       : '2025/06/20',
            description: `GitHub Pagesでの設計書ウェブサイトの
                          基本環境を構築しました。
                         `.trim()},
        {
            title      : 'スムーズスクロール機能追加',
            date       : '2025/06/20',
            description: 'ページ内リンクのスムーズスクロールを有効にしました。'},
        {
            title      : '開発ログ: マップ生成の試行',
            date       : '2025/06/19',
            description: 'ランダムマップ生成アルゴリズムの初期プロトタイプを開発中です。'},
        {
            title      : 'プロジェクト開始',
            date       : '2025/06/15',
            description: 'Lunatisプロジェクトの設計書作成を開始しました。'}
    ];

    // ここでは、マップシステムに関連するスクリプトのサンプルデータを作成します。
    // 各スクリプトのタイトル、説明、パス、GitHubの生のURLを含めます。
    // このデータは、実際のプロジェクトに合わせて適宜変更してください。
    // 例として、マップシステムに関連するスクリプトのサンプルデータを以下に示します。▽
    const scriptCards = [
        {
            id         : 'script1',
            title      : 'MapNodeData.cs',
            description: 'マップの各ノードのデータ構造を定義。',
            path       : 'Assets/Scripts/Map/MapNodeData.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapNodeData.cs' },
        {
            id         : 'script2',
            title      : 'MapBoundsClamper.cs',
            description: 'マップの境界内でのオブジェクト位置を制限。',
            path       : 'Assets/Scripts/Map/MapBoundsClamper.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapBoundsClamper.cs' },
        {
            id         : 'script3',
            title      : 'MapInteractionHandler.cs',
            description: 'マップ上のインタラクションを処理。',
            path       : 'Assets/Scripts/Map/MapInteractionHandler.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapInteractionHandler.cs' },
        {
            id         : 'script4',
            title      : 'MapNode.cs',
            description: '個々のマップノードの振る舞いを定義。',
            path       : 'Assets/Scripts/Map/MapNode.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapNode.cs' },
        {
            id         : 'script5',
            title      : 'MapPathManager.cs',
            description: 'マップ内のパス探索と管理。',
            path       : 'Assets/Scripts/Map/MapPathManager.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapPathManager.cs' },
        {
            id         : 'script6',
            title      : 'MapZoomController.cs',
            description: 'マップのズームレベルを制御。',
            path       : 'Assets/Scripts/Map/MapZoomController.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapZoomController.cs' },
        {
            id         : 'script7',
            title      : 'PlayerMapMovementController.cs',
            description: 'プレイヤーのマップ上での移動を制御。',
            path       : 'Assets/Scripts/Map/PlayerMapMovementController.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/PlayerMapMovementController.cs' },
        {
            id         : 'script8',
            title      : 'PlayerLocationSetter.cs',
            description: 'プレイヤーのマップ上の初期位置設定ツール。',
            path       : 'Assets/Scripts/Tools/PlayerLocationSetter.cs',
            rawUrl     : 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Tools/PlayerLocationSetter.cs' }
    ];

    // ここでは、カード表示の関数を用いて指定されたHTMLのターゲット要素にカードをレンダリングします。
    // 各カードは、タイトル、説明、日付（オプション）、詳細リンクを含みます。▽
    function renderCards(targetElement, cardsData) {
 
        // 新しいカードコンテナを作成
        const cardContainer     = document.createElement('div');

        // card-containerクラスは、CSSでスタイルを定義するために使用されます。
        cardContainer.className = 'card-container';

        // 各カードデータをループして、カード要素を作成
        // 各カードは、タイトル、説明、日付（オプション）、詳細、リンクを含みます。
        // cardsDataは、カードのデータを含む配列です。
        cardsData.forEach(card => {

            // 各カードの要素を作成し、必要な情報を設定
            // cardは、個々のカードのデータオブジェクトです。
            const cardElement     = document.createElement('div');

            // classNameは、CSSでスタイルを適用するために使用されます。
            cardElement.className = 'info-card';

            // cardElementは、個々のカードを表すHTML要素です。
            // この要素には、カードのタイトル、説明、日付、詳細リンクが含まれます。
            // 三項演算子：日付が存在する場合は、更新日を表示します。
            // また、IDが存在する場合は、詳細リンクを表示します。
            // また、カードIDが存在する場合は、詳細リンクを表示します。
            // これにより、カードの内容が動的に生成されます。
            cardElement.innerHTML = `

                <h3>${card.title}</h3>
                <p>${card.description}</p>
 
                ${card.date ? `<p class="card-date">更新日: ${card.date}</p>` : ''}
                ${card.id ? `<a href="#" class="card-link" data-card-id="${card.id}">詳細を見る</a>` : ''}
            `;
            cardContainer.appendChild(cardElement);
        });
        targetElement.appendChild(cardContainer);
    }

    // ここでは、選択されたカードの詳細情報を表示する関数を定義します。
    // この関数は、カードデータを受け取り、GitHubからコードを取得して表示します。▽
    async function renderDetailContent(cardData) {

        areaB.innerHTML = `<h2>選択された詳細情報</h2><p>コードを読み込み中...</p>`; // ローディング表示

        // GitHubの生のURLからスクリプトのコードを取得
        // fetch APIを使用して、指定されたURLからコードを取得します。
        try {

            // cardData.rawUrl は、スクリプトの生のコードを取得するためのURLです。
            // このURLは、GitHubのリポジトリから直接スクリプ
            const response    = await fetch(cardData.rawUrl);

            // レスポンスが正常でない場合はエラーをスロー
            // response.ok は、HTTPレスポンスが成功（ステータスコード200-299）であるかどうかを示します。
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // レスポンスのテキストを取得
            // response.text() は、レスポンスの本文をテキストとして取得します
            const codeContent = await response.text();

            // ここでは、以下の要素を含むHTMLを生成しています。
            // - タイトル       : 選択されたスクリプトのタイトル
            // - コードブロック : スクリプトのコードを表示するための<pre><code>要素
            // - ファイル名     : スクリプトのパスを表示するための<div class="code-filename">要素
            // - コピー用ボタン : コードをクリップボードにコピーするためのボタン
            // - コードのハイライト: hljs（Highlight.js）ライブラリを使用して、コードのシンタックスハイライトを適用します。
            const detailHtml         = `
                <h3>${cardData.title} のコード</h3>
                <div class           = "code-block-container">
                    <div class       = "code-filename">${cardData.path}</div>
                    <button class    = "copy-button" data-copy-target="code-display">コピー</button>
                    <pre><code class = "language-csharp" id="code-display">${escapeHtml(codeContent)}</code></pre>
                </div>
            `;

            // areaBの内容を更新
            areaB.innerHTML   = `<h2>選択された詳細情報</h2>${detailHtml}`;

            // 動的に追加されたコードブロックをハイライトする
            const codeElement = document.getElementById('code-display');

            if (codeElement) {

                // hljs.highlightElement() を呼び出すことで、この要素だけがハイライトされる
                hljs.highlightElement(codeElement);
            }

            // areaB.querySelectorAll('.copy-button') は、areaB内のすべての
            // コピーボタンを取得します。これにより、複数のコピーボタンが存在する場合でも、
            // すべてのボタンに対してイベントリスナーを設定できます。
            areaB.querySelectorAll('.copy-button').forEach(button =>{
            

                button.addEventListener('click', () => {

                    // クリックされたボタンの data-copy-target 属性からターゲットIDを取得
                    // data-copy-target 属性は、どのコードブロックをコピーするかを
                    // 指定するために使用されます。
                    // これにより、複数のコードブロックがある場合でも、
                    // どのコードをコピーするかを動的に決定できます。
                    const targetId          = button.dataset.copyTarget;
                    const codeElementToCopy = document.getElementById(targetId);

                    if (codeElementToCopy) {
                        const codeToCopy    = codeElementToCopy.innerText; // <pre><code>の内容を取得

                        navigator.clipboard.writeText(codeToCopy).then(() => {
                            button.textContent     = 'コピーしました！';

                            setTimeout(() =>{
                                button.textContent = 'コピー';

                            }, 2000); // 2秒後に元に戻す

                        }).catch(err =>{
                            console.error('コピーに失敗しました:', err);
                            alert('コードのコピーに失敗しました。ブラウザのセキュリティ設定をご確認ください。');
                        });
                    }
                });
            });

        } catch (error) {

            console.error('スクリプトの読み込みに失敗しました:', error);
            areaB.innerHTML = `<h2>選択された詳細情報</h2><p>コードの読み込み中にエラーが発生しました。</p><p>エラー詳細: ${error.message}</p>`;

        } finally {

            areaB.scrollIntoView({ behavior: 'smooth' });
        }
    }

    // HTMLエンティティをエスケープするヘルパー関数
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    // --- 初期表示: 更新情報カード ---
    function showHomePage() {
        areaA.innerHTML = '<h2>最新の更新情報</h2>';
        renderCards(areaA, updateCards);
        areaB.innerHTML = '<h2>詳細情報表示エリア</h2><p>ここに選択されたコンテンツの詳細が表示されます。</p>';
    }

    // --- イベントリスナー ---
    navHome.addEventListener('click', (e) => {
        e.preventDefault();
        showHomePage();
    });

    navMapSystem.addEventListener('click', (e) => {
        e.preventDefault();
        areaA.innerHTML = '<h2>MAPシステム概要</h2><p>LunatisのMAPシステムに関する概要や、サブ項目がここに表示されます。</p><p>（具体的なMAPシステム関連のカードは今後追加）</p>';
        areaB.innerHTML = '<h2>MAPシステム詳細</h2><p>ここにMAPシステムのメインコンテンツ（詳細設計など）が表示されます。</p>';
        areaB.scrollIntoView({ behavior: 'smooth' });
    });

    navScripts.addEventListener('click', (e) => {
        e.preventDefault();
        areaA.innerHTML = '<h2>スクリプト一覧</h2>';
        renderCards(areaA, scriptCards);
        areaB.innerHTML = '<h2>詳細情報表示エリア</h2><p>ここに選択されたスクリプトのコードや詳細が表示されます。</p>';
    });

    // --- カードクリックイベントの委譲 ---

    areaA.addEventListener('click', (e) => {

        // クリックされた要素が 'card-link' クラスを持つかどうかをチェック
        // e.target は実際にクリックされた要素を指す
        const cardLink = e.target.closest('.card-link');

        // closest() メソッドは、指定されたセレクタに一致する最も近い祖先要素を返します。
        // これにより、カードリンクがクリックされた場合のみ処理が行われます。
        // もし cardLink が存在しない場合、クリックイベントは無視されます。
        if (cardLink) {

            // クリックされたリンクのデフォルトの動作を防止
            // e.preventDefault() は、リンクのデフォルトの動作（ページ遷移など）を防ぎます。
            // これにより、JavaScriptでのカスタム処理が可能になります
            e.preventDefault();

            // data-card-id 属性からカードのIDを取得
            // cardLink.dataset.cardId は、data-card-id 属性の値を取得します。
            const cardId = cardLink.dataset.cardId;
            // クリックされたカードのIDをコンソールに出力（デバッグ用）
            console.log('クリックされたカードのID:', cardId);
            // スクリプトカードの配列から、クリックされたカードのデータを取得。
            // find() メソッドは、配列内の要素を検索し、条件に一致する最初の要素を返します。
            // ここでは、cardCards 配列から、クリックされたカードの ID と一致するカードを検索しています。
            // cardCards は、スクリプトカードのデータを含む配列です。
            // cardId は、クリックされたカードの ID です。 
            const selectedScript = scriptCards.find(card => card.id === cardId);
            // クリックされたカードのデータが存在する場合、詳細コンテンツを表示
            // selectedScript は、クリックされたカードのデータオブジェクトです。
            if (selectedScript) {
                // renderDetailContent 関数を呼び出し、選択されたスクリプトの詳細を表示
                // この関数は、スクリプトのコードを取得し、HTMLにレンダリングします。
                // selectedScript は、クリックされたスクリプトカードのデータオブジェクトです。
                // このオブジェクトには、スクリプトのタイトル、説明、パス、GitHubの生のURLなどが含まれています。
                // renderDetailContent 関数は、選択されたスクリプトの詳細を表示するために使用されます。
                // この関数は、スクリプトのコードをGitHubから取得しHTMLにレンダリングします。
                // また、コードのハイライト機能も追加されています。
                renderDetailContent(selectedScript);
            }
        }
    });

    // 初回ロード時にホームページを表示
    showHomePage();
});