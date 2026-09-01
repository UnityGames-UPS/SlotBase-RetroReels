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

    [Header("Anticipation Settings")]
    [SerializeField] private GameObject anticipationObject;
    [SerializeField, Min(1f)] private float anticipationSpeedMultiplier = 1.3f;
    [SerializeField, Min(0f)] private float anticipationDuration = 2f;

    [Header("Win Box")]
    [SerializeField] private GameObject winBoxObject;


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
    private List<int> reelCycleCount = new List<int>();
    private readonly List<Coroutine> stopReelCoroutines = new List<Coroutine>();
    private Coroutine stopSpinCoroutine;

    private const int Symbol2X = 8;
    private const int Symbol3X = 9;
    private const int Symbol4X = 10;
    private const int Symbol5X = 11;


    internal List<List<int>> currentDisplayMatrix;

    private bool isSpinning;

    #region Initialization

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

        DisableAllOverlays();
        RandomizeInitialVisibleSymbols();
        SetupSymbolButtons();
    }

    internal void DisableAllOverlays()
    {
        if (anticipationObject) anticipationObject.SetActive(false);
        if (winBoxObject) winBoxObject.SetActive(false);
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

    }

    private void RandomizeInitialVisibleSymbols()
    {
        if (reelImagesList == null || symbolSprites == null || symbolSprites.Length <= 1) return;

        for (int col = 0; col < reelImagesList.Count; col++)
        {
            ReelImages reel = reelImagesList[col];
            if (reel == null || reel.images == null) continue;

            for (int row = 0; row < 3; row++)
            {
                int imageIndex = VisibleResultStartIndex + row;
                if (imageIndex >= reel.images.Count || reel.images[imageIndex] == null) continue;

                int randomSymbolId = Random.Range(1, symbolSprites.Length);
                SetImageSymbol(reel.images[imageIndex], randomSymbolId);
            }
        }
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
                    SetImageSymbol(reel.images[VisibleResultStartIndex + 2], Random.Range(1, symbolCount));
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
        KillAllTweens();

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

        StartStopSpinSequence(resultMatrix, onComplete, false, isTurbo);
    }

    private void StartStopSpinSequence(List<List<int>> resultMatrix, System.Action onComplete, bool isQuickStop, bool isTurbo = false)
    {
        CancelActiveStopSequence();
        stopSpinCoroutine = StartCoroutine(StopSpinSequence(resultMatrix, onComplete, isQuickStop, isTurbo));
    }

    private void CancelActiveStopSequence()
    {
        if (stopSpinCoroutine != null)
        {
            StopCoroutine(stopSpinCoroutine);
            stopSpinCoroutine = null;
        }

        foreach (Coroutine reelCoroutine in stopReelCoroutines)
        {
            if (reelCoroutine != null)
            {
                StopCoroutine(reelCoroutine);
            }
        }
        stopReelCoroutines.Clear();

        SetAnticipationVisible(false);
        ResetReelSpeedMultipliers();
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

        float stagger = isQuickStop
            ? quickStopStagger
            : (isTurbo ? (reelStopStagger * 0.5f) : normalReelStopStagger);
        float stopAnimationDuration = (isQuickStop || isTurbo)
            ? quickStopDuration
            : stopOvershootDuration + stopSettleDuration;
        bool shouldAnticipate = !isQuickStop && ShouldPlayAnticipation(resultMatrix, maxCols);

        if (shouldAnticipate)
        {
            // Land the first two reels before revealing that the third reel can complete the feature.
            for (int col = 0; col < 2; col++)
            {
                float delay = col * stagger;
                StartTrackedReelStop(col, resultMatrix[col], delay, isTurbo);
            }

            yield return new WaitForSeconds(stagger + stopAnimationDuration);

            SetAnticipationVisible(true);
            SetReelSpeedMultiplier(2, anticipationSpeedMultiplier);
            yield return new WaitForSeconds(anticipationDuration);
            SetReelSpeedMultiplier(2, 1f);
            SetAnticipationVisible(false);

            StartTrackedReelStop(2, resultMatrix[2], 0f, isTurbo);
            yield return new WaitForSeconds(stopAnimationDuration);
        }
        else
        {
            AudioManager.Instance?.StopReelSpinLoop();

            for (int col = 0; col < maxCols; col++)
            {
                float delay = col * stagger;
                StartTrackedReelStop(col, resultMatrix[col], delay, isQuickStop || isTurbo);
            }

            float longestStopTime = ((maxCols - 1) * stagger) + stopAnimationDuration;
            yield return new WaitForSeconds(longestStopTime);
        }

        AudioManager.Instance?.StopReelSpinLoop();
        AudioManager.Instance?.StopTensionBuilder();
        SetAnticipationVisible(false);

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

        stopReelCoroutines.Clear();
        stopSpinCoroutine = null;
        onComplete?.Invoke();
    }

    private void StartTrackedReelStop(int columnIndex, List<int> targetSymbols, float delay, bool useQuickStopAnimation)
    {
        Coroutine reelCoroutine = StartCoroutine(StopSingleReel(columnIndex, targetSymbols, delay, useQuickStopAnimation));
        stopReelCoroutines.Add(reelCoroutine);
    }

    private bool ShouldPlayAnticipation(List<List<int>> resultMatrix, int reelCount)
    {
        if (reelCount < 3 || resultMatrix == null || resultMatrix.Count < 3) return false;
        if (resultMatrix[0] == null || resultMatrix[0].Count == 0) return false;
        if (resultMatrix[1] == null || resultMatrix[1].Count == 0) return false;

        int firstReelSymbol = resultMatrix[0][0];
        int secondReelSymbol = resultMatrix[1][0];

        return firstReelSymbol == Symbol2X &&
               (secondReelSymbol == Symbol3X ||
                secondReelSymbol == Symbol4X ||
                secondReelSymbol == Symbol5X);
    }

    private void SetAnticipationVisible(bool visible)
    {
        if (anticipationObject != null)
        {
            anticipationObject.SetActive(visible);
        }

        if (visible)
        {
            AudioManager.Instance?.PlayTensionBuilder();
        }
        else
        {
            AudioManager.Instance?.StopTensionBuilder();
        }
    }

    private void SetReelSpeedMultiplier(int columnIndex, float multiplier)
    {
        if (columnIndex < 0 || columnIndex >= spinTweens.Count) return;

        Tween reelTween = spinTweens[columnIndex];
        if (reelTween != null && reelTween.IsActive())
        {
            reelTween.timeScale = Mathf.Max(0f, multiplier);
        }
    }

    private void ResetReelSpeedMultipliers()
    {
        foreach (Tween tween in spinTweens)
        {
            if (tween != null && tween.IsActive())
            {
                tween.timeScale = 1f;
            }
        }
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

        StartStopSpinSequence(resultMatrix, onComplete, true);
    }

    #endregion

    #region Win Box

    internal void ShowWinBox()
    {
        if (winBoxObject != null)
        {
            winBoxObject.SetActive(true);
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


    private void KillAllTweens()
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
