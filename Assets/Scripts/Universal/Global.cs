using UnityEngine;
public static class Global
{
    public static GameObject _Player;

    #region coins

    //Coins that were collected in another level
    static int _SecuredCoins;

    //Coins that were collected in the current level and will be lost upon death
    public static int _RoundCoins;

    public static void StoreCoins(int amount)
    {
        amount = _RoundCoins < 0 ? _RoundCoins : amount;
        _SecuredCoins += amount;
        _RoundCoins -= amount;
    }
    public static int GetTotalCoins() => _SecuredCoins + _RoundCoins;

    #endregion
}
