[System.Serializable]
public class Turn
{
    public BalootPlayerClass player; public CardClass card;

    public Turn(BalootPlayerClass player, CardClass card)
    {
        this.player = player;
        this.card = card;
    }
    public Turn()
    {

    }
}