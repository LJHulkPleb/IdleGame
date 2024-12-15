[System.Serializable]
public class PetStats
{
    public string PetName;
    public int Strength;
    public int Defense;
    public int Speed;
    public int Health;
    public string BonusStat;

    public PetStats(string petName, int strength, int defense, int health, int speed, string bonusStat)
    {
        PetName = petName;
        Strength = strength;
        Defense = defense;
        Health = health;
        Speed = speed;
        BonusStat = bonusStat;
    }
}