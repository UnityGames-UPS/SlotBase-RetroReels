using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlotView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Symbol Sprites - Assign by Name")]
    [SerializeField] private Sprite spriteBlank;
    [SerializeField] private Sprite spriteRed7;
    [SerializeField] private Sprite spritePurple7;
    [SerializeField] private Sprite spriteBlue7;
    [SerializeField] private Sprite spriteWhiteBar7;
    [SerializeField] private Sprite spriteTripleBar;
    [SerializeField] private Sprite spriteDoubleBar;
    [SerializeField] private Sprite spriteSingleBar;
    [SerializeField] private Sprite sprite2X;
    [SerializeField] private Sprite sprite3X;
    [SerializeField] private Sprite sprite4X;
    [SerializeField] private Sprite sprite5X;

    private Sprite[] symbolSprites;

    private List<Sprite>[] animationSpriteArrays;

    [Header("Reel Containers")]
    [SerializeField] private Transform[] reelTransforms;

    [Header("Reel Images - 12 images per reel")]
    [SerializeField] private List<ReelImages> reelImagesList;

    private const int ReelImageCount = 12;
    private const int VisibleResultStartIndex = 5;

    [Header("Reel Stop Y Positions")]
    [SerializeField] private float case1StopY = 175f;
    [SerializeField] private float case2StopY = 0f;

    [Header("Spin Settings")]
    [SerializeField] private float symbolHeight = 100f;
    [SerializeField] private float spinSpeed = 4000f;
    [SerializeField] private float fastSpinSpeed = 6000f;
    [SerializeField] private float reelStartStagger = 0.08f;
    [SerializeField] private float reelStopStagger = 0.12f;
    [SerializeField] private float normalReelStopStagger = 0.5f;

    [Header("Stop Animation Settings")]
    [SerializeField] private float stopOvershootDistance = 50f;
    [SerializeField] private float stopOvershootDuration = 0.20f;
    [SerializeField] private float stopSettleDuration = 0.30f;

    [Header("Quick Spin Settings")]
    [SerializeField] private float quickStopStagger = 0.06f;
    [SerializeField] private float quickStopOvershoot = 20f;
    [SerializeField] private float quickStopDuration = 0.2f;
    [SerializeField] private int minSpinCyclesBeforeStop = 1;


    [Header("Win Animation Settings")]
    [SerializeField] private float winSymbolLoopDuration = 1.2f;

    [Header("Phase 1 Total Win Presentation")]
    [SerializeField] private TMPro.TMP_Text phase1TotalWinText;

    [Header("Win Animation Objects — Col 0..4  (each has 2 rows, contains ImageAnimation component)")]
    [SerializeField] private GameObject winAnimationParent;
    [Tooltip("GameObject references for win animations. Each should have an ImageAnimation component attached.")]
    [SerializeField] private ColumnOverlays[] winAnimationColumns = new ColumnOverlays[5];

    [Header("Symbol Info Card")]
    [SerializeField] private SymbolInfoCard symbolInfoCard;

    [Header("Cylindrical Spin Effect Settings")]
    [SerializeField] private bool enableCylindricalEffect = true;
    [Tooltip("Optional parent RectTransform reference (e.g. reel viewport frame) to automatically measure visible half height from parent rect height.")]
    [SerializeField] private RectTransform visibleAreaRectTransform;
    [SerializeField] private float edgeScale = 0.94f;
    [SerializeField] private float outerScale = 0.90f;
    [SerializeField] private float visibleHalfHeight = 145f;
    [SerializeField] private float outerHalfHeight = 220f;

    private float[] reelCurveIntensity = new float[3] { 1f, 1f, 1f };
    private Tween[] reelSettleCurveTweens = new Tween[3];


    private float middlePosition = 0f;
    private float cycleDistance;


    private List<Tween> spinTweens = new List<Tween>();
    private List<Tween> winTweens = new List<Tween>();
    private List<int> reelCycleCount = new List<int>();
    private Coroutine winAnimationCoroutine;


    internal List<List<int>> currentDisplayMatrix;

    private bool isSpinning;

    #region Initialization

    private Dictionary<GameObject, Vector3> originalWinBoxLocalPositions;

    private void CacheOriginalWinBoxPositions()
    {
        if (winAnimationColumns == null) return;
        if (originalWinBoxLocalPositions == null)
            originalWinBoxLocalPositions = new Dictionary<GameObject, Vector3>();
        else
            originalWinBoxLocalPositions.Clear();

        foreach (var colOverlay in winAnimationColumns)
        {
            if (colOverlay != null && colOverlay.rows != null)
            {
                foreach (var go in colOverlay.rows)
                {
                    if (go != null && !originalWinBoxLocalPositions.ContainsKey(go))
                    {
                        originalWinBoxLocalPositions[go] = go.transform.localPosition;
                    }
                }
            }
        }
    }

    private Vector3 GetOriginalWinBoxPosition(GameObject go)
    {
        if (go != null && originalWinBoxLocalPositions != null && originalWinBoxLocalPositions.TryGetValue(go, out Vector3 origPos))
        {
            return origPos;
        }
        return go != null ? go.transform.localPosition : Vector3.zero;
    }

    private void ResetWinBoxPosition(GameObject go)
    {
        if (go != null && originalWinBoxLocalPositions != null && originalWinBoxLocalPositions.TryGetValue(go, out Vector3 origPos))
        {
            go.transform.localPosition = origPos;
        }
    }
    private void Awake()
    {
        BuildSymbolSpriteArray();
        InitializeReels();
    }
    private void Start()
    {
        if (symbolSprites == null || symbolSprites.Length == 0)
        {
            BuildSymbolSpriteArray();
        }

        CacheOriginalWinBoxPositions();
        DisableAllOverlays();
        SetupSymbolButtons();
    }

    internal void DisableAllOverlays()
    {
        DisableColumns(winAnimationColumns);
        if (winAnimationParent) winAnimationParent.SetActive(false);
        HidePhase1TotalWinText(false);
        if (symbolInfoCard) symbolInfoCard.HideCard();
        AudioManager.Instance?.StopTensionBuilder();
        AudioManager.Instance?.StopReelSpinLoop();
    }

    private void SetupSymbolButtons()
    {
        if (reelImagesList == null) return;
        for (int col = 0; col < reelImagesList.Count; col++)
        {
            var reel = reelImagesList[col];
            if (reel == null || reel.images == null) continue;
            int rowCount = 3;
            for (int row = 0; row < rowCount; row++)
            {
                int imageIndex = VisibleResultStartIndex + row;
                if (imageIndex < reel.images.Count && reel.images[imageIndex] != null)
                {
                    Image img = reel.images[imageIndex];
                    SymbolButtonHandler btnHandler = img.GetComponent<SymbolButtonHandler>();
                    if (btnHandler == null)
                    {
                        btnHandler = img.gameObject.AddComponent<SymbolButtonHandler>();
                    }
                    btnHandler.Init(col, row, this);
                }
            }
        }
    }

    internal void OnBetChanged()
    {
        if (symbolInfoCard != null && symbolInfoCard.gameObject.activeSelf)
        {
            symbolInfoCard.RefreshCard(gameManager);
        }
    }

    private Dictionary<Image, int> imageToSymbolIdMap = new Dictionary<Image, int>();

    private void SetImageSymbol(Image img, int symbolId)
    {
        if (img == null) return;
        img.sprite = GetSymbolSprite(symbolId);
        imageToSymbolIdMap[img] = symbolId;
    }

    internal void OnSymbolClicked(int col, int row, RectTransform symbolRect)
    {
        if (isSpinning)
        {
            if (symbolInfoCard != null) symbolInfoCard.HideCard();
            return;
        }

        if (col >= reelImagesList.Count) return;

        var reel = reelImagesList[col];
        if (reel == null || reel.images == null) return;

        float customYOffset = 0f;
        if (reelTransforms != null && col < reelTransforms.Length && reelTransforms[col] != null)
        {
            float reelY = reelTransforms[col].localPosition.y;
            bool isCase1ReelPos = Mathf.Abs(reelY - (-160f)) < 30f;
            bool isCase1Matrix = (currentDisplayMatrix != null && col < currentDisplayMatrix.Count &&
                                  currentDisplayMatrix[col] != null && currentDisplayMatrix[col].Count >= 3 &&
                                  currentDisplayMatrix[col][1] != 0);

            if (isCase1ReelPos || isCase1Matrix)
            {
                if (row == 0) customYOffset = -10f;
                else if (row == 2) customYOffset = 10f;
            }
        }

        int imageIndex = VisibleResultStartIndex + row;
        if (imageIndex < reel.images.Count && reel.images[imageIndex] != null)
        {
            Image clickedImage = reel.images[imageIndex];
            if (imageToSymbolIdMap.TryGetValue(clickedImage, out int symbolId))
            {
                if (symbolInfoCard != null)
                {
                    symbolInfoCard.ShowCard(symbolId, col, row, symbolRect, gameManager, customYOffset);
                }
                return;
            }
        }

        if (currentDisplayMatrix != null && col < currentDisplayMatrix.Count && row < currentDisplayMatrix[col].Count)
        {
            int fallbackId = currentDisplayMatrix[col][row];
            if (symbolInfoCard != null)
            {
                symbolInfoCard.ShowCard(fallbackId, col, row, symbolRect, gameManager, customYOffset);
            }
        }
    }

    private void DisableColumns(ColumnOverlays[] cols)
    {
        if (cols == null) return;
        foreach (var col in cols)
        {
            if (col?.rows != null)
            {
                foreach (var go in col.rows)
                {
                    if (go)
                    {
                        ResetWinBoxPosition(go);
                        go.SetActive(false);
                    }
                }
            }
        }
    }

    private GameObject GetWinBoxObject(int col, int row)
    {
        if (winAnimationColumns == null || col < 0 || col >= winAnimationColumns.Length) return null;
        var overlay = winAnimationColumns[col];
        if (overlay == null || overlay.rows == null || overlay.rows.Length == 0) return null;

        if (winAnimationParent && !winAnimationParent.activeSelf)
        {
            winAnimationParent.SetActive(true);
        }

        GameObject animGO = null;

        if (row == 0)
        {
            animGO = overlay.rows[0];
            ResetWinBoxPosition(animGO);
        }
        else if (row == 2)
        {
            animGO = overlay.rows.Length > 1 ? overlay.rows[1] : overlay.rows[0];
            ResetWinBoxPosition(animGO);
        }
        else if (row == 1)
        {
            animGO = overlay.rows[0];
            if (animGO != null)
            {
                Vector3 basePos = GetOriginalWinBoxPosition(animGO);
                animGO.transform.localPosition = new Vector3(basePos.x, 6.5f, basePos.z);
            }
        }

        return animGO;
    }

    private GameObject WinBox(ColumnOverlays[] cols, int col, int row)
    {
        if (cols == winAnimationColumns)
        {
            return GetWinBoxObject(col, row);
        }
        return (col >= 0 && col < cols?.Length && cols[col]?.rows != null && row >= 0 && row < cols[col].rows.Length)
            ? cols[col].rows[row] : null;
    }

    private void BuildSymbolSpriteArray()
    {
        symbolSprites = new Sprite[12];
        symbolSprites[0] = spriteBlank;
        symbolSprites[1] = spriteRed7;
        symbolSprites[2] = spritePurple7;
        symbolSprites[3] = spriteBlue7;
        symbolSprites[4] = spriteWhiteBar7;
        symbolSprites[5] = spriteTripleBar;
        symbolSprites[6] = spriteDoubleBar;
        symbolSprites[7] = spriteSingleBar;
        symbolSprites[8] = sprite2X;
        symbolSprites[9] = sprite3X;
        symbolSprites[10] = sprite4X;
        symbolSprites[11] = sprite5X;

        Sprite defaultSprite = null;
        for (int i = 0; i < symbolSprites.Length; i++)
        {
            if (symbolSprites[i] != null)
            {
                defaultSprite = symbolSprites[i];
                break;
            }
        }

        for (int i = 0; i < symbolSprites.Length; i++)
        {
            if (symbolSprites[i] == null)
            {
                symbolSprites[i] = defaultSprite;
            }
        }
        animationSpriteArrays = new List<Sprite>[symbolSprites.Length];
        for (int i = 1; i < symbolSprites.Length; i++)
        {
            animationSpriteArrays[i] = new List<Sprite> { symbolSprites[i] };
        }
    }

    private void InitializeReels()
    {
        cycleDistance = symbolHeight;
        middlePosition = 0f;



        int reelCount = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.reelCount : 3;
        int rowCount = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.rowCount : 3;

        currentDisplayMatrix = new List<List<int>>();
        reelCycleCount = new List<int>();
        for (int col = 0; col < reelCount; col++)
        {
            var defaultCol = new List<int>();
            for (int r = 0; r < rowCount; r++)
            {
                defaultCol.Add(0);
            }
            currentDisplayMatrix.Add(defaultCol);
            reelCycleCount.Add(0);
        }
    }

    internal void SetInitialMatrix(List<List<int>> matrix)
    {
        if (matrix == null || matrix.Count == 0) return;

        int reelCount = matrix.Count;
        int rowCount = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.rowCount : 3;

        for (int col = 0; col < reelCount; col++)
        {
            if (matrix[col] != null && matrix[col].Count != rowCount) return;
        }

        currentDisplayMatrix = matrix;

        for (int col = 0; col < reelCount; col++)
        {
            if (col < reelCurveIntensity.Length)
            {
                reelCurveIntensity[col] = 1f;
            }

            if (col < reelImagesList.Count)
            {
                SetReelSymbols(col, matrix[col], true);
            }
        }

        UpdateCylindricalSpinEffect();
    }

    #endregion

    #region Symbol Display

    private float GetTargetYForResult(List<int> columnSymbols)
    {
        bool hasServerResult = columnSymbols != null && columnSymbols.Count > 0 && columnSymbols[0] != 0;
        return middlePosition + (hasServerResult ? case1StopY : case2StopY);
    }

    private void UpdateReelSpacing(int columnIndex, bool hasServerResult)
    {
        if (reelTransforms == null || columnIndex < 0 || columnIndex >= reelTransforms.Length) return;

        VerticalLayoutGroup layoutGroup = reelTransforms[columnIndex].GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = hasServerResult ? -40f : 40f;
        }
    }

    private bool CurrentResultContainsZero()
    {
        if (currentDisplayMatrix == null || currentDisplayMatrix.Count == 0) return true;

        foreach (List<int> reelResult in currentDisplayMatrix)
        {
            if (reelResult == null || reelResult.Count == 0 || reelResult[0] == 0)
            {
                return true;
            }
        }

        return false;
    }

    private void SetReelSymbols(int columnIndex, List<int> visibleSymbolIds, bool isInitial = false)
    {
        if (columnIndex >= reelImagesList.Count) return;

        var reel = reelImagesList[columnIndex];
        if (reel.images == null || reel.images.Count < ReelImageCount) return;

        int resultIconIndex = VisibleResultStartIndex + 1;
        int resultSymbolId = visibleSymbolIds != null && visibleSymbolIds.Count > 0
            ? visibleSymbolIds[0]
            : 0;
        bool hasServerResult = resultSymbolId != 0;
        UpdateReelSpacing(columnIndex, hasServerResult);

        if (!isInitial)
        {
            int symbolCount = symbolSprites != null ? symbolSprites.Length : 0;
            if (hasServerResult)
            {
                SetImageSymbol(reel.images[resultIconIndex], resultSymbolId);

                if (CurrentResultContainsZero())
                {
                    SetImageSymbol(reel.images[VisibleResultStartIndex], 0);
                    SetImageSymbol(reel.images[VisibleResultStartIndex + 2], 0);
                }
                else if (symbolCount > 0)
                {
                    SetImageSymbol(reel.images[VisibleResultStartIndex], Random.Range(0, symbolCount));
                    SetImageSymbol(reel.images[VisibleResultStartIndex + 2], Random.Range(0, symbolCount));
                }
            }
            else
            {
                if (symbolCount > 1)
                {
                    SetImageSymbol(reel.images[VisibleResultStartIndex], Random.Range(1, symbolCount));
                    SetImageSymbol(reel.images[resultIconIndex], Random.Range(1, symbolCount));
                }
            }
        }

        if (isInitial && reelTransforms[columnIndex] != null)
        {
            reelTransforms[columnIndex].localPosition = new Vector3(
                reelTransforms[columnIndex].localPosition.x,
                GetTargetYForResult(visibleSymbolIds),
                reelTransforms[columnIndex].localPosition.z
            );
        }
    }

    private Sprite GetSymbolSprite(int symbolId)
    {
        if (symbolId < 0 || symbolId >= symbolSprites.Length)
        {
            return symbolSprites[0];
        }

        if (symbolSprites[symbolId] == null)
        {
            return symbolSprites[0];
        }

        return symbolSprites[symbolId];
    }

    #endregion

    #region Spin Animation

    internal void StartSpin()
    {
        if (isSpinning) return;

        if (symbolInfoCard != null) symbolInfoCard.HideCard();

        isSpinning = true;
        KillAllTweens(resetReelScales: false);

        for (int i = 0; i < reelCurveIntensity.Length; i++)
        {
            if (reelSettleCurveTweens[i] != null)
            {
                reelSettleCurveTweens[i].Kill();
                reelSettleCurveTweens[i] = null;
            }
        }

        DisableAllOverlays();
        AudioManager.Instance?.PlayReelSpinLoop();

        for (int i = 0; i < reelCycleCount.Count; i++)
        {
            reelCycleCount[i] = 0;
        }

        int reelCount = currentDisplayMatrix != null ? currentDisplayMatrix.Count : (gameManager?.gameConfig != null ? gameManager.gameConfig.reelCount : 3);
        int maxCols = Mathf.Min(reelCount, reelTransforms != null ? reelTransforms.Length : 3);

        for (int col = 0; col < maxCols; col++)
        {
            StartReelCycleWithDelay(col, col * reelStartStagger);
        }
    }

    private void StartReelCycleWithDelay(int columnIndex, float delay)
    {
        if (columnIndex >= reelTransforms.Length) return;

        if (delay > 0)
        {
            Sequence startSequence = DOTween.Sequence();
            startSequence.AppendInterval(delay);
            startSequence.OnComplete(() =>
            {
                if (isSpinning)
                {
                    StartReelCycle(columnIndex);
                }
            });
            startSequence.Play();

            if (spinTweens.Count <= columnIndex)
                spinTweens.Add(startSequence);
            else
                spinTweens[columnIndex] = startSequence;
        }
        else
        {
            StartReelCycle(columnIndex);
        }
    }

    private void StartReelCycle(int columnIndex)
    {
        if (columnIndex >= reelTransforms.Length) return;
        if (!isSpinning) return;

        if (columnIndex < reelCurveIntensity.Length)
        {
            if (reelSettleCurveTweens[columnIndex] != null)
            {
                reelSettleCurveTweens[columnIndex].Kill();
                reelSettleCurveTweens[columnIndex] = null;
            }
        }

        Transform slotTransform = reelTransforms[columnIndex];
        var reel = (columnIndex < reelImagesList.Count) ? reelImagesList[columnIndex] : null;
        int totalImages = (reel != null && reel.images != null && reel.images.Count > 0)
            ? Mathf.Min(reel.images.Count, ReelImageCount)
            : ReelImageCount;

        int bufferCount = totalImages - 3;
        float fullDistance = bufferCount * symbolHeight;
        float halfDistance = fullDistance * 0.5f;
        float spinTopY = middlePosition + halfDistance;
        float spinBottomY = middlePosition - halfDistance;

        slotTransform.localPosition = new Vector3(slotTransform.localPosition.x, spinTopY, 0f);

        float currentSpeed = gameManager != null && gameManager.currentSpinSpeed == SpinSpeed.Turbo
            ? fastSpinSpeed
            : spinSpeed;
        float loopDuration = fullDistance / currentSpeed;

        Tweener loopTweener = slotTransform.DOLocalMoveY(spinBottomY, loopDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .OnStepComplete(() =>
            {
                if (columnIndex < reelCycleCount.Count)
                {
                    reelCycleCount[columnIndex]++;
                }
            });

        if (spinTweens.Count <= columnIndex)
            spinTweens.Add(loopTweener);
        else
            spinTweens[columnIndex] = loopTweener;
    }

    #endregion

    #region Stop Spin

    internal void StopSpin(List<List<int>> resultMatrix, System.Action onComplete, bool isTurbo = false)
    {
        int reelCount = resultMatrix != null ? resultMatrix.Count : (gameManager?.gameConfig != null ? gameManager.gameConfig.reelCount : 3);
        int maxCols = Mathf.Min(reelCount, reelTransforms != null ? reelTransforms.Length : 3);

        if (!isSpinning)
        {
            currentDisplayMatrix = resultMatrix;
            for (int col = 0; col < maxCols; col++)
            {
                SetReelSymbols(col, resultMatrix[col], false);
                reelTransforms[col].localPosition = new Vector3(
                    reelTransforms[col].localPosition.x,
                    GetTargetYForResult(resultMatrix[col]),
                    reelTransforms[col].localPosition.z
                );
            }
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(StopSpinSequence(resultMatrix, onComplete, false, isTurbo));
    }

    private IEnumerator StopSpinSequence(List<List<int>> resultMatrix, System.Action onComplete, bool isQuickStop, bool isTurbo = false)
    {
        currentDisplayMatrix = resultMatrix;
        int reelCount = resultMatrix != null ? resultMatrix.Count : 3;
        int maxCols = Mathf.Min(reelCount, reelTransforms != null ? reelTransforms.Length : 3);

        if (!isQuickStop && !isTurbo)
        {
            while (true)
            {
                bool allReelsReady = true;
                for (int col = 0; col < maxCols; col++)
                {
                    if (col < reelCycleCount.Count && reelCycleCount[col] < minSpinCyclesBeforeStop)
                    {
                        allReelsReady = false;
                        break;
                    }
                }

                if (allReelsReady) break;
                yield return null;
            }
        }

        AudioManager.Instance?.StopReelSpinLoop();

        float stagger = isQuickStop
            ? quickStopStagger
            : (isTurbo ? (reelStopStagger * 0.5f) : normalReelStopStagger);

        for (int col = 0; col < maxCols; col++)
        {
            float delay = col * stagger;
            StartCoroutine(StopSingleReel(col, resultMatrix[col], delay, isQuickStop || isTurbo));
        }

        float longestStopTime;
        if (isQuickStop)
        {
            longestStopTime = ((maxCols - 1) * stagger) + quickStopDuration;
        }
        else if (isTurbo)
        {
            longestStopTime = ((maxCols - 1) * stagger) + (stopOvershootDuration * 0.5f) + (stopSettleDuration * 0.5f);
        }
        else
        {
            longestStopTime = ((maxCols - 1) * stagger) + stopOvershootDuration + stopSettleDuration;
        }

        yield return new WaitForSeconds(longestStopTime);

        AudioManager.Instance?.StopTensionBuilder();

        isSpinning = false;

        foreach (var tween in spinTweens)
        {
            tween?.Kill();
        }
        spinTweens.Clear();

        if (reelSettleCurveTweens != null)
        {
            for (int i = 0; i < reelSettleCurveTweens.Length; i++)
            {
                if (reelSettleCurveTweens[i] != null)
                {
                    reelSettleCurveTweens[i].Kill();
                    reelSettleCurveTweens[i] = null;
                }
            }
        }

        UpdateCylindricalSpinEffect(force: true);

        onComplete?.Invoke();
    }

    private IEnumerator StopSingleReel(int columnIndex, List<int> targetSymbols, float delay, bool isQuickStop)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        if (columnIndex < spinTweens.Count && spinTweens[columnIndex] != null)
        {
            spinTweens[columnIndex].Kill();
        }

        Transform slotTransform = reelTransforms[columnIndex];
        slotTransform.DOKill();

        float targetY = GetTargetYForResult(targetSymbols);

        SetReelSymbols(columnIndex, targetSymbols, false);

        if (columnIndex < reelCurveIntensity.Length)
        {
            if (reelSettleCurveTweens[columnIndex] != null) reelSettleCurveTweens[columnIndex].Kill();
            reelCurveIntensity[columnIndex] = 1f;
        }

        float landingStartTopY = targetY + (2f * symbolHeight);
        slotTransform.localPosition = new Vector3(
            slotTransform.localPosition.x,
            landingStartTopY,
            0
        );

        AudioManager.Instance?.PlayReelStop();

        if (currentDisplayMatrix != null && columnIndex < currentDisplayMatrix.Count)
        {
            bool hasWild = false;
            foreach (int sym in currentDisplayMatrix[columnIndex])
            {
                SymbolInfo symbol = gameManager?.gameConfig?.symbols?.Find(info => info.id == sym);
                if (symbol != null && symbol.isWild) hasWild = true;
            }
            if (hasWild) AudioManager.Instance?.PlayReelStop();
        }

        if (isQuickStop)
        {
            Sequence quickStopSequence = DOTween.Sequence();

            quickStopSequence.Append(
                slotTransform.DOLocalMoveY(targetY - quickStopOvershoot, quickStopDuration * 0.3f)
                    .SetEase(Ease.OutQuad)
            );

            quickStopSequence.Append(
                slotTransform.DOLocalMoveY(targetY, quickStopDuration * 0.7f)
                    .SetEase(Ease.InOutQuad)
            );

            if (spinTweens.Count <= columnIndex)
                spinTweens.Add(quickStopSequence);
            else
                spinTweens[columnIndex] = quickStopSequence;
        }
        else
        {
            Sequence stopSequence = DOTween.Sequence();

            stopSequence.Append(
                slotTransform.DOLocalMoveY(targetY - stopOvershootDistance, stopOvershootDuration)
                    .SetEase(Ease.OutQuad)
            );

            stopSequence.Append(
                slotTransform.DOLocalMoveY(targetY, stopSettleDuration)
                    .SetEase(Ease.InOutQuad)
            );

            if (spinTweens.Count <= columnIndex)
                spinTweens.Add(stopSequence);
            else
                spinTweens[columnIndex] = stopSequence;
        }
    }

    #endregion

    #region Quick Spin

    internal void QuickStop(List<List<int>> resultMatrix, System.Action onComplete = null)
    {
        if (!isSpinning)
        {
            currentDisplayMatrix = resultMatrix;
            int reelCount = resultMatrix != null ? resultMatrix.Count : 3;
            int maxCols = Mathf.Min(reelCount, reelTransforms != null ? reelTransforms.Length : 3);

            for (int col = 0; col < maxCols; col++)
            {
                if (col < reelTransforms.Length)
                {
                    SetReelSymbols(col, resultMatrix[col], false);
                    reelTransforms[col].localPosition = new Vector3(
                        reelTransforms[col].localPosition.x,
                        GetTargetYForResult(resultMatrix[col]),
                        0
                    );
                }
            }

            onComplete?.Invoke();
            return;
        }

        StartCoroutine(StopSpinSequence(resultMatrix, onComplete, true));
    }

    #endregion

    #region Win Line Animation

    internal void ShowWinLineAnimation(List<WinLine> winLines, System.Action onComplete)
    {
        if (winLines == null || winLines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        KillWinTweens();
        winAnimationCoroutine = StartCoroutine(PlaySingleWinLineAnimation(winLines, onComplete));
    }

    private IEnumerator PlaySingleWinLineAnimation(List<WinLine> winLines, System.Action onComplete)
    {
        if (winLines == null || winLines.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        HashSet<int> flatPositions = new HashSet<int>();
        double totalWinAmount = 0;
        foreach (var line in winLines)
        {
            if (line != null)
            {
                totalWinAmount += line.winAmount;
                if (line.positions != null)
                {
                    foreach (int pos in line.positions)
                    {
                        flatPositions.Add(pos);
                    }
                }
            }
        }

        if (flatPositions.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        ShowPhase1TotalWin(totalWinAmount);

        AudioManager.Instance?.PlayWinLinePhase1Start();

        bool isAutoPlaying = (gameManager != null && gameManager.isAutoPlaying);

        if (isAutoPlaying)
        {
            yield return StartCoroutine(AnimateWinPositionsSingleLoop(flatPositions));
            HidePhase1TotalWinText(true);
            yield return new WaitForSeconds(0.15f);
            onComplete?.Invoke();
        }
        else
        {
            StartContinuousWinAnimation(flatPositions);
            onComplete?.Invoke();
        }
    }

    private IEnumerator AnimateWinPositionsSingleLoop(IEnumerable<int> flatPositions)
    {
        if (flatPositions == null) yield break;

        int reelCount = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.reelCount : 3;
        int rowLimit = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.rowCount : 3;

        List<ImageAnimation> activeAnims = new List<ImageAnimation>();
        int completedCount = 0;
        bool isCompleted = false;

        foreach (int flatIndex in flatPositions)
        {
            int row = flatIndex / reelCount;
            int col = flatIndex % reelCount;

            if (col < 0 || col >= 5 || row < 0 || row >= rowLimit) continue;

            if (col >= reelImagesList.Count) continue;
            var reel = reelImagesList[col];
            if (reel.images == null || reel.images.Count < 3) continue;

            int imageIndex = VisibleResultStartIndex + row;
            if (imageIndex >= reel.images.Count) continue;

            Image symbolImage = reel.images[imageIndex];
            if (symbolImage == null) continue;

            var animGO = WinBox(winAnimationColumns, col, row);
            if (animGO == null) continue;

            ImageAnimation imageAnim = animGO.GetComponentInChildren<ImageAnimation>();
            if (imageAnim == null) continue;

            if (currentDisplayMatrix == null || col >= currentDisplayMatrix.Count || row >= currentDisplayMatrix[col].Count) continue;
            int symbolId = currentDisplayMatrix[col][row];
            if (symbolId < 0 || symbolId >= animationSpriteArrays.Length) continue;

            List<Sprite> animSprites = animationSpriteArrays[symbolId];
            if (animSprites == null || animSprites.Count == 0) continue;

            imageAnim.textureArray = animSprites;
            imageAnim.animationMode = ImageAnimation.AnimationMode.SINGLE_PHASE;
            imageAnim.useDynamicFramerate = true;
            imageAnim.dynamicLoopDuration = winSymbolLoopDuration;
            imageAnim.doLoopAnimation = true;
            imageAnim.delayBetweenLoop = 0f;

            animGO.SetActive(true);
            Image animRenderer = imageAnim.rendererDelegate != null ? imageAnim.rendererDelegate : imageAnim.GetComponent<Image>();
            if (animRenderer == null && animGO != null) animRenderer = animGO.GetComponentInChildren<Image>();
            if (animRenderer != null)
            {
                animRenderer.DOKill();
                Color c = animRenderer.color;
                animRenderer.color = new Color(c.r, c.g, c.b, 1f);
                animRenderer.enabled = true;
                animRenderer.gameObject.SetActive(true);
            }

            if (symbolImage != null)
            {
                symbolImage.DOKill();
                Color c = symbolImage.color;
                symbolImage.color = new Color(c.r, c.g, c.b, 0f);
                symbolImage.enabled = false;
                symbolImage.gameObject.SetActive(false);
            }

            activeAnims.Add(imageAnim);

            imageAnim.onLoopComplete = (currentLoop) =>
            {
                if (currentLoop >= 1)
                {
                    imageAnim.onLoopComplete = null;
                    imageAnim.StopAnimation();
                    if (animGO != null)
                    {
                        ResetWinBoxPosition(animGO);
                        animGO.SetActive(false);
                    }

                    if (symbolImage != null)
                    {
                        symbolImage.DOKill();
                        Color c = symbolImage.color;
                        symbolImage.color = new Color(c.r, c.g, c.b, 1f);
                        symbolImage.enabled = true;
                        symbolImage.gameObject.SetActive(true);
                    }

                    completedCount++;
                    if (completedCount >= activeAnims.Count)
                    {
                        isCompleted = true;
                    }
                }
            };
        }

        foreach (var imageAnim in activeAnims)
        {
            imageAnim.StartAnimation();
        }

        if (activeAnims.Count > 0)
        {
            yield return new WaitUntil(() => isCompleted);
        }
        else
        {
            yield return new WaitForSeconds(winSymbolLoopDuration);
        }
    }

    private void StartContinuousWinAnimation(IEnumerable<int> flatPositions)
    {
        if (flatPositions == null) return;

        int reelCount = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.reelCount : 3;
        int rowLimit = (gameManager != null && gameManager.gameConfig != null) ? gameManager.gameConfig.rowCount : 3;

        if (winAnimationParent && !winAnimationParent.activeSelf)
        {
            winAnimationParent.SetActive(true);
        }

        foreach (int flatIndex in flatPositions)
        {
            int row = flatIndex / reelCount;
            int col = flatIndex % reelCount;

            if (col < 0 || col >= 5 || row < 0 || row >= rowLimit) continue;

            if (col >= reelImagesList.Count) continue;
            var reel = reelImagesList[col];
            if (reel.images == null || reel.images.Count < 3) continue;

            int imageIndex = VisibleResultStartIndex + row;
            if (imageIndex >= reel.images.Count) continue;

            Image symbolImage = reel.images[imageIndex];
            if (symbolImage == null) continue;

            var animGO = WinBox(winAnimationColumns, col, row);
            if (animGO == null) continue;

            ImageAnimation imageAnim = animGO.GetComponentInChildren<ImageAnimation>();
            if (imageAnim == null) continue;

            if (currentDisplayMatrix == null || col >= currentDisplayMatrix.Count || row >= currentDisplayMatrix[col].Count) continue;
            int symbolId = currentDisplayMatrix[col][row];
            if (symbolId < 0 || symbolId >= animationSpriteArrays.Length) continue;

            List<Sprite> animSprites = animationSpriteArrays[symbolId];
            if (animSprites == null || animSprites.Count == 0) continue;

            imageAnim.textureArray = animSprites;
            imageAnim.animationMode = ImageAnimation.AnimationMode.SINGLE_PHASE;
            imageAnim.useDynamicFramerate = true;
            imageAnim.dynamicLoopDuration = winSymbolLoopDuration;
            imageAnim.doLoopAnimation = true;
            imageAnim.delayBetweenLoop = 0f;
            imageAnim.onLoopComplete = null;

            animGO.SetActive(true);

            Image animRenderer = imageAnim.rendererDelegate != null ? imageAnim.rendererDelegate : imageAnim.GetComponent<Image>();
            if (animRenderer == null && animGO != null) animRenderer = animGO.GetComponentInChildren<Image>();
            if (animRenderer != null)
            {
                animRenderer.DOKill();
                Color c = animRenderer.color;
                animRenderer.color = new Color(c.r, c.g, c.b, 1f);
                animRenderer.enabled = true;
                animRenderer.gameObject.SetActive(true);
            }

            if (symbolImage != null)
            {
                symbolImage.DOKill();
                Color c = symbolImage.color;
                symbolImage.color = new Color(c.r, c.g, c.b, 0f);
                symbolImage.enabled = false;
                symbolImage.gameObject.SetActive(false);
            }

            imageAnim.StartAnimation();
        }
    }

    private void HideAllWinLineTexts()
    {
        if (reelImagesList == null) return;
        foreach (var reel in reelImagesList)
        {
            if (reel.images != null)
            {
                foreach (var image in reel.images)
                {
                    if (image != null)
                    {
                        Transform textTransform = image.transform.Find("WinLineText");
                        if (textTransform != null)
                        {
                            textTransform.DOKill();
                            textTransform.localScale = Vector3.one;
                            textTransform.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void ShowPhase1TotalWin(double totalWinAmount)
    {
        if (phase1TotalWinText != null)
        {
            phase1TotalWinText.text = FormatSpriteText(totalWinAmount);
            AnimateTextScaleAppear(phase1TotalWinText.transform);
        }
    }

    public static string FormatSpriteText(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in input)
        {
            if (c >= '0' && c <= '9')
            {
                sb.Append("<sprite=").Append(c - '0').Append(">");
            }
            else if (c == '=')
            {
                sb.Append("<sprite=10>");
            }
            else if (c == '.' || c == ',')
            {
                sb.Append("<sprite=11>");
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string FormatSpriteText(double amount)
    {
        return FormatSpriteText(amount.ToString("0.###"));
    }

    private void HidePhase1TotalWinText(bool animate = true)
    {
        if (phase1TotalWinText != null)
        {
            if (animate)
            {
                AnimateTextScaleDisappear(phase1TotalWinText.transform);
            }
            else
            {
                phase1TotalWinText.transform.DOKill();
                phase1TotalWinText.transform.localScale = Vector3.one;
                phase1TotalWinText.gameObject.SetActive(false);
            }
        }
    }

    private void AnimateTextScaleAppear(Transform textTransform, float popScale = 1.2f, float durationUp = 0.15f, float durationDown = 0.10f)
    {
        if (textTransform == null) return;
        textTransform.DOKill();
        textTransform.localScale = Vector3.zero;
        textTransform.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(textTransform.DOScale(popScale, durationUp).SetEase(Ease.OutQuad));
        seq.Append(textTransform.DOScale(1.0f, durationDown).SetEase(Ease.InQuad));
        winTweens.Add(seq);
    }

    private void AnimateTextScaleDisappear(Transform textTransform, float duration = 0.15f, System.Action onComplete = null)
    {
        if (textTransform == null)
        {
            onComplete?.Invoke();
            return;
        }

        textTransform.DOKill();
        if (textTransform.gameObject.activeSelf)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(textTransform.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad));
            seq.OnComplete(() =>
            {
                textTransform.gameObject.SetActive(false);
                textTransform.localScale = Vector3.one;
                onComplete?.Invoke();
            });
            winTweens.Add(seq);
        }
        else
        {
            textTransform.localScale = Vector3.one;
            textTransform.gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }


    private void KillWinTweens(bool stopCoroutine = true, bool resetReelScales = true)
    {
        foreach (var tween in winTweens)
        {
            tween?.Kill();
        }
        winTweens.Clear();

        if (stopCoroutine && winAnimationCoroutine != null)
        {
            StopCoroutine(winAnimationCoroutine);
            winAnimationCoroutine = null;
        }

        if (winAnimationColumns != null)
        {
            foreach (var col in winAnimationColumns)
            {
                if (col?.rows != null)
                {
                    foreach (var animGO in col.rows)
                    {
                        if (animGO != null)
                        {
                            ImageAnimation imageAnim = animGO.GetComponentInChildren<ImageAnimation>();
                            if (imageAnim != null)
                            {
                                imageAnim.onLoopComplete = null;
                                Image animRenderer = imageAnim.rendererDelegate != null ? imageAnim.rendererDelegate : imageAnim.GetComponent<Image>();
                                if (animRenderer == null) animRenderer = animGO.GetComponentInChildren<Image>();
                                if (animRenderer != null)
                                {
                                    animRenderer.DOKill();
                                    Color ac = animRenderer.color;
                                    animRenderer.color = new Color(ac.r, ac.g, ac.b, 1f);
                                }
                                imageAnim.StopAnimation();
                            }
                            if (animGO.activeSelf)
                            {
                                animGO.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        DisableColumns(winAnimationColumns);
        if (winAnimationParent) winAnimationParent.SetActive(false);
        HideAllWinLineTexts();
        HidePhase1TotalWinText(false);

        foreach (var reel in reelImagesList)
        {
            if (reel.images != null)
            {
                foreach (var image in reel.images)
                {
                    if (image != null)
                    {
                        image.DOKill();
                        if (resetReelScales)
                        {
                            image.transform.localScale = Vector3.one;
                        }
                        Color c = image.color;
                        image.color = new Color(c.r, c.g, c.b, 1f);
                        image.enabled = true;
                        if (!image.gameObject.activeSelf)
                        {
                            image.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    #endregion



    internal List<List<int>> GetCurrentDisplayMatrix()
    {
        return currentDisplayMatrix;
    }

    internal bool IsSpinning()
    {
        return isSpinning;
    }


    private void KillAllTweens(bool resetReelScales = true)
    {
        foreach (var tween in spinTweens)
        {
            tween?.Kill();
        }
        spinTweens.Clear();

        if (reelSettleCurveTweens != null)
        {
            for (int i = 0; i < reelSettleCurveTweens.Length; i++)
            {
                if (reelSettleCurveTweens[i] != null)
                {
                    reelSettleCurveTweens[i].Kill();
                    reelSettleCurveTweens[i] = null;
                }
            }
        }

        KillWinTweens(resetReelScales: resetReelScales);
    }

    #region Cylindrical Layout Effect

    private void UpdateCylindricalSpinEffect(bool force = false)
    {
        if (!enableCylindricalEffect || reelTransforms == null || reelImagesList == null) return;

        int maxCols = Mathf.Min(reelTransforms.Length, reelImagesList.Count);

        float effectiveVisibleHalfHeight = visibleHalfHeight;
        if (visibleAreaRectTransform != null && visibleAreaRectTransform.rect.height > 0)
        {
            effectiveVisibleHalfHeight = visibleAreaRectTransform.rect.height * 0.5f;
        }
        float effectiveOuterHalfHeight = Mathf.Max(outerHalfHeight, effectiveVisibleHalfHeight * 1.5f);

        float invVisibleHalfHeight = 1f / Mathf.Max(1f, effectiveVisibleHalfHeight);
        float invOuterRange = 1f / Mathf.Max(1f, effectiveOuterHalfHeight - effectiveVisibleHalfHeight);

        for (int col = 0; col < maxCols; col++)
        {
            Transform slotTransform = reelTransforms[col];
            if (slotTransform == null) continue;

            var reel = reelImagesList[col];
            if (reel == null || reel.images == null) continue;

            float intensity = (col < reelCurveIntensity.Length) ? reelCurveIntensity[col] : 1f;

            int centerImageIndex = VisibleResultStartIndex + 1;
            float centerImageLocalY = (reel.images.Count > centerImageIndex && reel.images[centerImageIndex] != null)
                ? reel.images[centerImageIndex].rectTransform.localPosition.y
                : -305.5f;
            float slotOffsetFromCase1 = slotTransform.localPosition.y - case1StopY;

            int imgCount = Mathf.Min(reel.images.Count, ReelImageCount);
            for (int i = 0; i < imgCount; i++)
            {
                Image img = reel.images[i];
                if (img == null) continue;

                RectTransform rect = img.rectTransform;
                if (rect == null) continue;

                float yRel = (rect.localPosition.y - centerImageLocalY) + slotOffsetFromCase1;
                float absY = Mathf.Abs(yRel);

                float targetScale = 1f;

                if (absY <= effectiveVisibleHalfHeight)
                {
                    float t = absY * invVisibleHalfHeight;
                    float curveFactor = t * t * intensity;

                    targetScale = Mathf.Lerp(1f, edgeScale, curveFactor);
                }
                else
                {
                    float extraT = Mathf.Clamp01((absY - effectiveVisibleHalfHeight) * invOuterRange);
                    targetScale = Mathf.Lerp(1f, Mathf.Lerp(edgeScale, outerScale, extraT), intensity);
                }

                Vector3 localScale = rect.localScale;
                if (force || !Mathf.Approximately(localScale.x, targetScale))
                {
                    rect.localScale = new Vector3(targetScale, targetScale, targetScale);
                }
            }
        }
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        KillAllTweens();
    }

    #endregion
}

[System.Serializable]
public class ReelImages
{
    public List<Image> images = new List<Image>(12);
}


[System.Serializable]
public class ColumnOverlays
{
    [Tooltip("Row 0 = top (Case 2) / middle (Case 1 y=6.5), Row 1 = bottom (Case 2)")]
    public GameObject[] rows = new GameObject[2];
}
