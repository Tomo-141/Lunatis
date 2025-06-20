document.addEventListener('DOMContentLoaded', () => {
    const areaA = document.getElementById('area-a');
    const areaB = document.getElementById('area-b');
    const navHome = document.getElementById('nav-home');
    const navMapSystem = document.getElementById('nav-map-system');
    const navScripts = document.getElementById('nav-scripts');

    // --- テスト用のデータ ---
    const updateCards = [
        { title: 'ウェブサイト環境セットアップ完了', date: '2025/06/20', description: 'GitHub Pagesでの設計書ウェブサイトの基本環境を構築しました。' },
        { title: 'スムーズスクロール機能追加', date: '2025/06/20', description: 'ページ内リンクのスムーズスクロールを有効にしました。' },
        { title: '開発ログ: マップ生成の試行', date: '2025/06/19', description: 'ランダムマップ生成アルゴリズムの初期プロトタイプを開発中です。' },
        { title: 'プロジェクト開始', date: '2025/06/15', description: 'Lunatisプロジェクトの設計書作成を開始しました。' }
    ];

    const scriptCards = [
        { id: 'script1', title: 'PlayerController.cs', description: 'プレイヤーの移動と操作を制御するスクリプト。', path: 'Assets/Scripts/Player/PlayerController.cs', content: '<h3>PlayerController.cs のコード</h3><div class="code-block-container"><div class="code-filename">Assets/Scripts/Player/PlayerController.cs</div><button class="copy-button" data-copy-target="code-script1">コピー</button><pre><code id="code-script1">// PlayerController.cs\npublic class PlayerController : MonoBehaviour\n{\n    public float moveSpeed = 5f;\n    void Update()\n    {\n        // 移動ロジック\n        float horizontal = Input.GetAxis("Horizontal");\n        float vertical = Input.GetAxis("Vertical");\n        transform.position += new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;\n    }\n}</code></pre></div>' },
        { id: 'script2', title: 'MapGenerator.cs', description: 'ランダムなマップを生成するコアスクリプト。', path: 'Assets/Scripts/Map/MapGenerator.cs', content: '<h3>MapGenerator.cs のコード</h3><div class="code-block-container"><div class="code-filename">Assets/Scripts/Map/MapGenerator.cs</div><button class="copy-button" data-copy-target="code-script2">コピー</button><pre><code id="code-script2">// MapGenerator.cs\npublic class MapGenerator : MonoBehaviour\n{\n    public int mapWidth = 100;\n    public int mapHeight = 100;\n    void Start()\n    {\n        GenerateMap();\n    }\n    void GenerateMap()\n    {\n        // マップ生成ロジック\n        Debug.Log("Map Generated!");\n    }\n}</code></pre></div>' },
        { id: 'script3', title: 'CameraController.cs', description: 'プレイヤーに追従するカメラの動きを制御。', path: 'Assets/Scripts/Camera/CameraController.cs', content: '<h3>CameraController.cs のコード</h3><div class="code-block-container"><div class="code-filename">Assets/Scripts/Camera/CameraController.cs</div><button class="copy-button" data-copy-target="code-script3">コピー</button><pre><code id="code-script3">// CameraController.cs\npublic class CameraController : MonoBehaviour\n{\n    public Transform target;\n    public Vector3 offset;\n    void LateUpdate()\n    {\n        if (target != null)\n        {\n            transform.position = target.position + offset;\n        }\n    }\n}</code></pre></div>' },
        { id: 'script4', title: 'ItemManager.cs', description: 'ゲーム内のアイテムの管理とインベントリ処理。', path: 'Assets/Scripts/GameSystem/ItemManager.cs', content: '<h3>ItemManager.cs のコード</h3><div class="code-block-container"><div class="code-filename">Assets/Scripts/GameSystem/ItemManager.cs</div><button class="copy-button" data-copy-target="code-script4">コピー</button><pre><code id="code-script4">// ItemManager.cs\npublic class ItemManager : MonoBehaviour\n{\n    public List&lt;string&gt; inventory = new List&lt;string&gt;();\n    public void AddItem(string itemName)\n    {\n        inventory.Add(itemName);\n        Debug.Log(itemName + " added to inventory.");\n    }\n}</code></pre></div>' },
        { id: 'script5', title: 'AudioManager.cs', description: 'ゲーム内のBGMや効果音の再生管理。', path: 'Assets/Scripts/Audio/AudioManager.cs', content: '<h3>AudioManager.cs のコード</h3><div class="code-block-container"><div class="code-filename">Assets/Scripts/Audio/AudioManager.cs</div><button class="copy-button" data-copy-target="code-script5">コピー</button><pre><code id="code-script5">// AudioManager.cs\npublic class AudioManager : MonoBehaviour\n{\n    public AudioClip BGM;\n    public AudioClip SFX;\n    void Start()\n    {\n        // BGM再生\n    }\n    public void PlaySFX(AudioClip clip)\n    {\n        // 効果音再生\n    }\n}</code></pre></div>' }
    ];

    // --- カード表示の関数 (変更なし) ---
    function renderCards(targetElement, cardsData) {
        targetElement.innerHTML = '';
        const cardContainer = document.createElement('div');
        cardContainer.className = 'card-container';

        cardsData.forEach(card => {
            const cardElement = document.createElement('div');
            cardElement.className = 'info-card';
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

    // --- 詳細コンテンツ表示の関数 (変更あり) ---
    function renderDetailContent(contentHtml) {
        areaB.innerHTML = `<h2>選択された詳細情報</h2>${contentHtml}`;

        // コピーボタンのイベントリスナーを設定
        areaB.querySelectorAll('.copy-button').forEach(button => {
            button.addEventListener('click', () => {
                const targetId = button.dataset.copyTarget;
                const codeElement = document.getElementById(targetId);
                if (codeElement) {
                    const codeToCopy = codeElement.innerText; // <pre><code>の内容を取得
                    navigator.clipboard.writeText(codeToCopy).then(() => {
                        button.textContent = 'コピーしました！';
                        setTimeout(() => {
                            button.textContent = 'コピー';
                        }, 2000); // 2秒後に元に戻す
                    }).catch(err => {
                        console.error('コピーに失敗しました:', err);
                        alert('コードのコピーに失敗しました。ブラウザのセキュリティ設定をご確認ください。');
                    });
                }
            });
        });

        areaB.scrollIntoView({ behavior: 'smooth' });
    }

    // --- 初期表示: 更新情報カード (変更なし) ---
    function showHomePage() {
        areaA.innerHTML = '<h2>最新の更新情報</h2>';
        renderCards(areaA, updateCards);
        areaB.innerHTML = '<h2>詳細情報表示エリア</h2><p>ここに選択されたコンテンツの詳細が表示されます。</p>';
    }

    // --- イベントリスナー (変更なし) ---
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

    // --- カードクリックイベントの委譲 (変更なし) ---
    areaA.addEventListener('click', (e) => {
        const cardLink = e.target.closest('.card-link');
        if (cardLink) {
            e.preventDefault();
            const cardId = cardLink.dataset.cardId;
            const selectedScript = scriptCards.find(card => card.id === cardId);
            if (selectedScript) {
                renderDetailContent(selectedScript.content);
            }
        }
    });

    // 初回ロード時にホームページを表示
    showHomePage();
});