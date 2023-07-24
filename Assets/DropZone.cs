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

            // A card was dropped on this box
            // Update health as necessary
            if (zoneSide == ZoneSide.Left)
            {
                playedCard.leftAction.Play(gameManager);
                playedCard.gameObject.SetActive(false);
                gameManager.discardPile.Add(playedCard);
                if (gameManager.deck.Count == 0)
                {
                    gameManager.gameState = GameManager.GameState.EnemyTurn;
                }
                else
                {
                    gameManager.DrawCard();
                }
            }
            else if (zoneSide == ZoneSide.Right)
            {
                playedCard.rightAction.Play(gameManager);
                playedCard.gameObject.SetActive(false);
                gameManager.discardPile.Add(playedCard);
                if (gameManager.deck.Count == 0)
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
}