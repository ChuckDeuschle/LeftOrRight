using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    private GameManager gameManager;
    public enum ZoneSide { Left, Right }
    public ZoneSide zoneSide;

    private void Start()
    {
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
        else
        {
            Debug.LogError("Cannot find GameManager object");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableCard draggableCard = eventData.pointerDrag.GetComponent<DraggableCard>();

        if (draggableCard != null)
        {
            Card playedCard = eventData.pointerDrag.GetComponent<Card>();
            if (playedCard == null) { return; }

            if (zoneSide == ZoneSide.Left)
            {
                playedCard.leftAction.Play(gameManager);
            }
            else if (zoneSide == ZoneSide.Right)
            {
                playedCard.rightAction.Play(gameManager);
            }

            playedCard.gameObject.SetActive(false);
            gameManager.discardPile.Add(playedCard);

            gameManager.OnCardPlayed(playedCard, zoneSide);

            if (gameManager.deck.Count == 0 || gameManager.activeRules.ShouldInterruptPlayerTurn(gameManager))
            {
                gameManager.gameState = GameManager.GameState.EnemyTurn;
            }
            else
            {
                gameManager.DrawCard();
            }
        }
    }
}
