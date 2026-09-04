using System;
using System.Collections.Generic;

#region Server Communication Models

[Serializable]
public class InitData
{
    public string id = "initData";
    public ServerGameData gameData;
    public ServerFeatures features;
    public ServerUIData uiData;
    public ServerPlayer player;
    public JackpotData jackpotData;
}

[Serializable]
public class JackpotData
{
    public JackpotValues values;

}

[Serializable]
public class JackpotValues
{
    public string miniJackpot;
    public string minorJackpot;
    public string majorJackpot;
    public string grandJackpot;
}

[Serializable]
public class JackpotSyncData
{
    public string gameId;
    public JackpotValues values;
}

[Serializable]
public class ServerGameData
{
    public List<List<int>> lines;
    public List<double> bets;
    public double creditDivisor = 1;
    public int totalLines = 1;
}

[Serializable]
public class ServerFeatures
{
    public SymbolCombinationFeature anyBars;
    public SymbolCombinationFeature barWhite7;
    public SymbolCombinationFeature mixedSevens;
    public ToggleFeature multiplierWild;
    public MultiplierOnlyWinFeature multiplierOnlyWin;
    public MultiplierJackpotsFeature multiplierJackpots;
}

[Serializable]
public class SymbolCombinationFeature
{
    public bool enabled;
    public double payout;
    public List<string> symbols;
}

[Serializable]
public class ToggleFeature
{
    public bool enabled;
}

[Serializable]
public class MultiplierOnlyWinFeature
{
    public bool enabled;
    public int maxMultiplierSymbols;
    public int minMultiplierSymbols;
}

[Serializable]
public class MultiplierJackpotsFeature
{
    public bool enabled;
    public List<MultiplierJackpotCombination> combinations;
}

[Serializable]
public class MultiplierJackpotCombination
{
    public double payout;
    public List<string> symbols;
}

[Serializable]
public class ServerUIData
{
    public PaylineData paylines;
}

[Serializable]
public class PaylineData
{
    public List<ServerSymbolInfo> symbols;
}

[Serializable]
public class ServerSymbolInfo
{
    public int id;
    public string name;
    public string group;
    public List<double> multiplier;
    public double payout;
    public string description;
    public int minMatch;
}

[Serializable]
public class ServerPlayer
{
    public double balance;
}

[Serializable]
public class ServerSpinResponse
{
    public string id = "spinResult";
    public bool success;
    public List<List<string>> matrix;
    public ServerPlayerBalance player;
    public ServerPayload payload;
}

[Serializable]
public class ServerPlayerBalance
{
    public double? balance;
}

[Serializable]
public class ServerPayload
{
    public List<List<string>> reels;
    public double totalWin;
    public double winAmount;
    public double grandTotalWin;
    public List<ServerWinLine> winningLines;
    public bool isRoundOver;
    public double totalRoundWin;
    public double netReturnRatio;
    public List<ServerWaysWin> waysWins;
}

[Serializable]
public class ServerWinLine
{
    public int lineId = -1;
    public int lineIndex = -1;
    public int symbolId;
    public string symbolName;
    public object positions;
    public List<ServerPosition> matchedPositions;
    public double payout;
    public double winAmount;
    public double multiplier;
    public double wildMultiplier;
}

[Serializable]
public class ServerWaysWin
{
    public int symbolId;
    public int matchCount;
    public int waysCount;
    public List<ServerPosition> matchedPositions;
    public double basePayout;
    public double appliedMultiplier;
    public double winInCredits;
    public double winInCash;
    public string winType;
}

[Serializable]
public class ServerPosition
{
    public int row;
    public int col;
}



[Serializable]
public class SpinRequest
{
    public string type = "SPIN";
    public SpinPayload payload;
}

[Serializable]
public class SpinPayload
{
    public int betIndex;
}

#endregion

#region Game Configuration (Client Side Converted)

[Serializable]
public class GameConfig
{
    public int reelCount = 3;
    public int rowCount = 3;
    public int symbolCount = 12;
    public int paylineCount = 1;
    public List<List<int>> paylines;
    public List<double> availableBets;
    public List<SymbolInfo> symbols;

    public double creditDivisor = 1.0;
    public ServerFeatures features;
}

[Serializable]
public class SymbolInfo
{
    public int id;
    public string name;
    public string description;
    public string group;
    public List<double> multipliers;
    public bool isWild;
    public int wildMultiplier = 1;
    public int minMatch;
}

#endregion

#region Player & Game State (Client Side)

[Serializable]
public class PlayerData
{
    public double balance;
    public int currentBetIndex;
}

[Serializable]
public class SpinResult
{
    public List<List<int>> resultMatrix;
    public double winAmount;
    public double grandTotalWin;
    public List<WinLine> winLines;
    public PlayerData playerData;
    public bool isRoundOver;
}

[Serializable]
public class WinLine
{
    public int lineId;
    public int symbolId;
    public List<int> positions;
    public double winAmount;
}

#endregion

#region Platform Communication

[Serializable]
public class AuthData
{
    public string token;
    public string socketURL;
    public string nameSpace;
}

#endregion

#region Enums

public enum GameState
{
    Initializing,
    Idle,
    Spinning,
    Stopping,
    ShowingWin
}

public enum SpinSpeed
{
    Normal,
    Turbo,
    QuickSpin
}

#endregion

#region Helper Classes for Conversion

public static class InitDataConverter
{
    internal static GameConfig ConvertToGameConfig(InitData serverData)
    {
        var config = new GameConfig
        {
            reelCount = 3,
            rowCount = 3,
            symbolCount = (serverData?.uiData?.paylines?.symbols != null) ? serverData.uiData.paylines.symbols.Count : 12,
            paylineCount = (serverData?.gameData != null) ? serverData.gameData.totalLines : 1,
            paylines = serverData?.gameData?.lines,
            availableBets = serverData?.gameData?.bets,
            creditDivisor = (serverData?.gameData != null && serverData.gameData.creditDivisor > 0) ? serverData.gameData.creditDivisor : 1.0,
            features = serverData?.features,
            symbols = new List<SymbolInfo>()
        };

        if (serverData?.uiData?.paylines?.symbols != null)
        {
            foreach (var serverSymbol in serverData.uiData.paylines.symbols)
            {
                var symbolInfo = new SymbolInfo
                {
                    id = serverSymbol.id,
                    name = serverSymbol.name,
                    description = serverSymbol.description,
                    group = serverSymbol.group,
                    multipliers = new List<double>(),
                    isWild = serverSymbol.multiplier != null && serverSymbol.multiplier.Count > 0,
                    wildMultiplier = (serverSymbol.multiplier != null && serverSymbol.multiplier.Count > 0)
                        ? (int)serverSymbol.multiplier[0]
                        : 1,
                    minMatch = serverSymbol.minMatch > 0 ? serverSymbol.minMatch : 3
                };

                symbolInfo.multipliers.Add(serverSymbol.payout);
                config.symbols.Add(symbolInfo);
            }
        }

        return config;
    }

    internal static PlayerData ConvertToPlayerData(ServerPlayer serverPlayer, int defaultBetIndex = 0)
    {
        return new PlayerData
        {
            balance = serverPlayer != null ? serverPlayer.balance : 0,
            currentBetIndex = defaultBetIndex
        };
    }

    internal static SpinResult ConvertServerResponseToSpinResult(ServerSpinResponse serverResponse, double currentBalance, double betAmount, GameConfig gameConfig)
    {
        double winAmountVal = 0;
        if (serverResponse.payload != null)
        {
            winAmountVal = serverResponse.payload.winAmount > 0 ? serverResponse.payload.winAmount : serverResponse.payload.totalWin;
        }

        double totalPay = (gameConfig != null && gameConfig.creditDivisor > 0) ? betAmount * gameConfig.creditDivisor : betAmount;
        double newBalance = serverResponse.player?.balance ?? CalculateNewBalance(currentBalance, totalPay, winAmountVal);

        double grandTotalWinVal = (serverResponse.payload != null && serverResponse.payload.grandTotalWin > 0)
            ? serverResponse.payload.grandTotalWin 
            : winAmountVal;

        var result = new SpinResult
        {
            resultMatrix = ConvertReelsToMatrix(serverResponse.payload?.reels, serverResponse.matrix, serverResponse.payload?.waysWins, gameConfig),
            winAmount = winAmountVal,
            grandTotalWin = grandTotalWinVal,
            winLines = ConvertWinningLines(serverResponse.payload?.winningLines, serverResponse.payload?.waysWins, gameConfig),

            playerData = new PlayerData
            {
                balance = newBalance,
                currentBetIndex = 0
            },
            isRoundOver = serverResponse.payload != null && serverResponse.payload.isRoundOver
        };

        return result;
    }

    private static List<List<int>> ConvertReelsToMatrix(List<List<string>> serverReels, List<List<string>> serverMatrix, List<ServerWaysWin> waysWins, GameConfig gameConfig)
    {
        var sourceReels = serverMatrix ?? serverReels;
        int rowCount = gameConfig != null ? gameConfig.rowCount : 3;
        int reelCount = gameConfig != null ? gameConfig.reelCount : 3;

        if (sourceReels == null || sourceReels.Count == 0)
        {
            UnityEngine.Debug.LogError("Invalid server reels/matrix: sourceReels is null or empty");
            return GenerateDefaultMatrix(rowCount, reelCount);
        }

        int totalRows = sourceReels.Count;
        int totalCols = sourceReels[0].Count;

        var matrix = new List<List<int>>();

        for (int col = 0; col < totalCols; col++)
        {
            var column = new List<int>();
            for (int row = 0; row < totalRows; row++)
            {
                if (col < sourceReels[row].Count)
                {
                    string symbolStr = sourceReels[row][col];
                    if (int.TryParse(symbolStr, out int symbolId))
                    {
                        column.Add(symbolId);
                    }
                    else
                    {
                        column.Add(0);
                    }
                }
                else
                {
                    column.Add(0);
                }
            }
            matrix.Add(column);
        }

        return matrix;
    }

    private static List<List<int>> GenerateDefaultMatrix(int rowCount = 3, int reelCount = 3)
    {
        var matrix = new List<List<int>>();
        for (int col = 0; col < reelCount; col++)
        {
            var column = new List<int>();
            for (int row = 0; row < rowCount; row++)
            {
                column.Add(0);
            }
            matrix.Add(column);
        }
        return matrix;
    }

    private static List<WinLine> ConvertWinningLines(List<ServerWinLine> serverWinLines, List<ServerWaysWin> serverWaysWins, GameConfig gameConfig)
    {
        var winLines = new List<WinLine>();
        int reelCount = gameConfig != null ? gameConfig.reelCount : 3;

        if (serverWinLines != null && serverWinLines.Count > 0)
        {
            int index = 0;
            foreach (var line in serverWinLines)
            {
                var flatPositions = new List<int>();
                if (line.matchedPositions != null && line.matchedPositions.Count > 0)
                {
                    foreach (var pos in line.matchedPositions)
                    {
                        int flatIndex = pos.row * reelCount + pos.col;
                        flatPositions.Add(flatIndex);
                    }
                }
                else if (line.positions != null)
                {
                    if (line.positions is Newtonsoft.Json.Linq.JArray jArr)
                    {
                        foreach (var item in jArr)
                        {
                            if (item is Newtonsoft.Json.Linq.JArray subArr && subArr.Count >= 2)
                            {
                                int row = (int)subArr[0];
                                int col = (int)subArr[1];
                                flatPositions.Add(row * reelCount + col);
                            }
                            else if (item is Newtonsoft.Json.Linq.JArray subArr1 && subArr1.Count == 1)
                            {
                                flatPositions.Add((int)subArr1[0]);
                            }
                            else if (int.TryParse(item.ToString(), out int pVal))
                            {
                                flatPositions.Add(pVal);
                            }
                        }
                    }
                    else if (line.positions is List<List<int>> list2D)
                    {
                        foreach (var pair in list2D)
                        {
                            if (pair.Count >= 2)
                            {
                                int row = pair[0];
                                int col = pair[1];
                                flatPositions.Add(row * reelCount + col);
                            }
                            else if (pair.Count == 1)
                            {
                                flatPositions.Add(pair[0]);
                            }
                        }
                    }
                    else if (line.positions is List<int> list1D)
                    {
                        flatPositions.AddRange(list1D);
                    }
                }

                int effectiveLineId = line.lineIndex >= 0 ? line.lineIndex : (line.lineId >= 0 ? line.lineId : index++);
                double effectiveWinAmount = line.payout > 0 ? line.payout : line.winAmount;

                winLines.Add(new WinLine
                {
                    lineId = effectiveLineId,
                    symbolId = line.symbolId,
                    positions = flatPositions,
                    winAmount = effectiveWinAmount
                });
            }
            return winLines;
        }

        if (serverWaysWins != null && serverWaysWins.Count > 0)
        {
            int index = 0;
            foreach (var waysWin in serverWaysWins)
            {
                var flatPositions = new List<int>();
                if (waysWin.matchedPositions != null)
                {
                    foreach (var pos in waysWin.matchedPositions)
                    {
                        int flatIndex = pos.row * reelCount + pos.col;
                        flatPositions.Add(flatIndex);
                    }
                }

                winLines.Add(new WinLine
                {
                    lineId = index++,
                    symbolId = waysWin.symbolId,
                    positions = flatPositions,
                    winAmount = waysWin.winInCash
                });
            }
        }

        return winLines;
    }

    private static double CalculateNewBalance(double currentBalance, double totalPay, double winAmount)
    {
        return currentBalance + winAmount;
    }
}

#endregion
