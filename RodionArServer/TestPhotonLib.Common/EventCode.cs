namespace TestPhotonLib.Common
{
    public enum EventCode:byte
    {
        WorldEnter,
        WorldExit,

        Damage = 8,
        UpdateHealth = 9,
        Death = 20,
        Respawn = 21,
        Restart = 22
    }
}
