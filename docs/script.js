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
        { id: 'script1', title: 'MapNodeData.cs', description: 'マップの各ノードのデータ構造を定義。', path: 'Assets/Scripts/Data/MapNodeData.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Data/MapNodeData.cs' },
        { id: 'script2', title: 'MapBoundsClamper.cs', description: 'マップの境界内でのオブジェクト位置を制限。', path: 'Assets/Scripts/Map/MapBoundsClamper.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapBoundsClamper.cs' },
        { id: 'script3', title: 'MapInteractionHandler.cs', description: 'マップ上のインタラクションを処理。', path: 'Assets/Scripts/Map/MapInteractionHandler.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapInteractionHandler.cs' },
        { id: 'script4', title: 'MapNode.cs', description: '個々のマップノードの振る舞いを定義。', path: 'Assets/Scripts/Map/MapNode.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapNode.cs' },
        { id: 'script5', title: 'MapPathManager.cs', description: 'マップ内のパス探索と管理。', path: 'Assets/Scripts/Map/MapPathManager.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapPathManager.cs' },
        { id: 'script6', title: 'MapZoomController.cs', description: 'マップのズームレベルを制御。', path: 'Assets/Scripts/Map/MapZoomController.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Map/MapZoomController.cs' },
        { id: 'script7', title: 'PlayerMapMovementController.cs', description: 'プレイヤーのマップ上での移動を制御。', path: 'Assets/Scripts/PlayerMap/PlayerMapMovementController.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/PlayerMap/PlayerMapMovementController.cs' },
        { id: 'script8', title: 'PlayerLocationSetter.cs', description: 'プレイヤーのマップ上の初期位置設定ツール。', path: 'Assets/Scripts/Tools/PlayerLocationSetter.cs', rawUrl: 'https://raw.githubusercontent.com/Tomo-141/Lunatis/main/Assets/Scripts/Tools/PlayerLocationSetter.cs' }
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
    async function renderDetailContent(cardData) {
        areaB.innerHTML = `<h2>選択された詳細情報</h2><p>コードを読み込み中...</p>`; // ローディング表示

        try {
            const response = await fetch(cardData.rawUrl);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const codeContent = await response.text();

            const detailHtml = `
                <h3>${cardData.title} のコード</h3>
                <div class="code-block-container">
                    <div class="code-filename">${cardData.path}</div>
                    <button class="copy-button" data-copy-target="code-display">コピー</button>
                    <pre><code id="code-display" class="language-csharp">${escapeHtml(codeContent)}</code></pre>
                </div>
            `;
            areaB.innerHTML = `<h2>選択された詳細情報</h2>${detailHtml}`;

            // highlight.jsを適用
            // `code-display`は動的に生成されるため、要素を取得してハイライトを適用
            const codeElement = document.getElementById('code-display');
            if (codeElement) {
                hljs.highlightElement(codeElement); // 変更点：highlightAll()ではなくhighlightElement()を使う
            }

            // コピーボタンのイベントリスナーを設定
            areaB.querySelectorAll('.copy-button').forEach(button => {
                button.addEventListener('click', () => {
                    const targetId = button.dataset.copyTarget;
                    const codeElementForCopy = document.getElementById(targetId); // コピー用の要素を取得
                    if (codeElementForCopy) {
                        const codeToCopy = codeElementForCopy.innerText; // <pre><code>の内容を取得
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
                renderDetailContent(selectedScript); // ここでcardDataオブジェクト全体を渡す
            }
        }
    });

    // 初回ロード時にホームページを表示
    showHomePage();
});