[System.Serializable]
public class Turn
{
    public PlayerClass player; public CardClass card;

    public Turn(PlayerClass player, CardClass card)
    {
        this.player = player;
        this.card = card;
    }
    public Turn()
    {

    }
}