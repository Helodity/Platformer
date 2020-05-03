public static class PlayerStats
{
    //Coins that were collected in another level
    static int _SecuredCoins;

    //Coins that were collected in the current level and will be lost upon death
    public static int _RoundCoins;

    #region getters
    public static int GetTotalCoins() { return _SecuredCoins + _RoundCoins; }
    #endregion

    #region coins

    public static void StoreCoins(int amount)
    {
        amount = _RoundCoins < 0 ? _RoundCoins : amount;
        _SecuredCoins += amount;
        _RoundCoins -= amount;
    }

    #endregion
}
