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
        { id: 'script1', title: 'PlayerController.cs', description: 'プレイヤーの移動と操作を制御するスクリプト。', content: '<h3>PlayerController.cs のコード</h3><pre><code>// PlayerController.cs\npublic class PlayerController : MonoBehaviour\n{\n    public float moveSpeed = 5f;\n    void Update()\n    {\n        // 移動ロジック\n        float horizontal = Input.GetAxis("Horizontal");\n        float vertical = Input.GetAxis("Vertical");\n        transform.position += new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;\n    }\n}</code></pre>' },
        { id: 'script2', title: 'MapGenerator.cs', description: 'ランダムなマップを生成するコアスクリプト。', content: '<h3>MapGenerator.cs のコード</h3><pre><code>// MapGenerator.cs\npublic class MapGenerator : MonoBehaviour\n{\n    public int mapWidth = 100;\n    public int mapHeight = 100;\n    void Start()\n    {\n        GenerateMap();\n    }\n    void GenerateMap()\n    {\n        // マップ生成ロジック\n        Debug.Log("Map Generated!");\n    }\n}</code></pre>' },
        { id: 'script3', title: 'CameraController.cs', description: 'プレイヤーに追従するカメラの動きを制御。', content: '<h3>CameraController.cs のコード</h3><pre><code>// CameraController.cs\npublic class CameraController : MonoBehaviour\n{\n    public Transform target;\n    public Vector3 offset;\n    void LateUpdate()\n    {\n        if (target != null)\n        {\n            transform.position = target.position + offset;\n        }\n    }\n}</code></pre>' },
        { id: 'script4', title: 'ItemManager.cs', description: 'ゲーム内のアイテムの管理とインベントリ処理。', content: '<h3>ItemManager.cs のコード</h3><pre><code>// ItemManager.cs\npublic class ItemManager : MonoBehaviour\n{\n    public List&lt;string&gt; inventory = new List&lt;string&gt;();\n    public void AddItem(string itemName)\n    {\n        inventory.Add(itemName);\n        Debug.Log(itemName + " added to inventory.");\n    }\n}</code></pre>' },
        { id: 'script5', title: 'AudioManager.cs', description: 'ゲーム内のBGMや効果音の再生管理。', content: '<h3>AudioManager.cs のコード</h3><pre><code>// AudioManager.cs\npublic class AudioManager : MonoBehaviour\n{\n    public AudioClip BGM;\n    public AudioClip SFX;\n    void Start()\n    {\n        // BGM再生\n    }\n    public void PlaySFX(AudioClip clip)\n    {\n        // 効果音再生\n    }\n}</code></pre>' }
    ];

    // --- カード表示の関数 ---
    function renderCards(targetElement, cardsData) {
        targetElement.innerHTML = ''; // 古いコンテンツをクリア
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

    // --- 詳細コンテンツ表示の関数 ---
    function renderDetailContent(contentHtml) {
        areaB.innerHTML = `<h2>選択された詳細情報</h2>${contentHtml}`;
        // スムーズスクロールでBエリアに移動
        areaB.scrollIntoView({ behavior: 'smooth' });
    }

    // --- 初期表示: 更新情報カード ---
    function showHomePage() {
        areaA.innerHTML = '<h2>最新の更新情報</h2>';
        renderCards(areaA, updateCards);
        areaB.innerHTML = '<h2>詳細情報表示エリア</h2><p>ここに選択されたコンテンツの詳細が表示されます。</p>'; // Bエリアを初期状態に戻す
    }

    // --- イベントリスナー ---
    navHome.addEventListener('click', (e) => {
        e.preventDefault(); // デフォルトのリンク動作をキャンセル
        showHomePage();
    });

    navMapSystem.addEventListener('click', (e) => {
        e.preventDefault();
        areaA.innerHTML = '<h2>MAPシステム概要</h2><p>LunatisのMAPシステムに関する概要や、サブ項目がここに表示されます。</p><p>（具体的なMAPシステム関連のカードは今後追加）</p>';
        areaB.innerHTML = '<h2>MAPシステム詳細</h2><p>ここにMAPシステムのメインコンテンツ（詳細設計など）が表示されます。</p>';
        areaB.scrollIntoView({ behavior: 'smooth' }); // Bエリアにスクロール
    });

    navScripts.addEventListener('click', (e) => {
        e.preventDefault();
        areaA.innerHTML = '<h2>スクリプト一覧</h2>';
        renderCards(areaA, scriptCards);
        areaB.innerHTML = '<h2>詳細情報表示エリア</h2><p>ここに選択されたスクリプトのコードや詳細が表示されます。</p>'; // Bエリアを初期状態に戻す
    });

    // --- カードクリックイベントの委譲 ---
    areaA.addEventListener('click', (e) => {
        const cardLink = e.target.closest('.card-link'); // クリックされた要素から最も近い.card-linkを探す
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