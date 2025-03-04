/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blackjack
{
    public class Blackjack : MonoBehaviour
    {
        // UI elements for displaying the game status (assuming you have UI Text objects in your scene)
        public Text playerScoreText;
        public Text dealerScoreText;
        public Text gameStatusText;

        private Deck deck;
        private List<Card> playerHand;
        private List<Card> dealerHand;
        private int playerScore;
        private int dealerScore;

        static void Start()
        {
            // Initialize the deck and hands
            deck = new Deck();
            playerHand = new List<Card>();
            dealerHand = new List<Card>();
            playerScore = 0;
            dealerScore = 0;

            // Start the game
            StartGame();
        }

        public static void ActivateBlackjack()
        {
            Debug.Log("Blackjack active");
        }

        private static void StartGame()
        {
            deck.Shuffle();

            // Deal initial cards to player and dealer
            playerHand.Clear();
            dealerHand.Clear();
            playerScore = 0;
            dealerScore = 0;

            playerHand.Add(deck.DealCard());
            playerHand.Add(deck.DealCard());
            dealerHand.Add(deck.DealCard());
            dealerHand.Add(deck.DealCard());

            // Update UI
            UpdateUI();

            // Start the player's turn
            PlayerTurn();
        }

        private static void UpdateUI()
        {
            playerScore = CalculateScore(playerHand);
            dealerScore = CalculateScore(dealerHand);

            playerScoreText.text = "Player Score: " + playerScore;
            dealerScoreText.text = "Dealer Score: " + dealerScore;

            // Show status message
            if (playerScore > 21)
            {
                gameStatusText.text = "You busted! Dealer wins.";
            }
            else if (dealerScore > 21)
            {
                gameStatusText.text = "Dealer busted! You win!";
            }
            else
            {
                gameStatusText.text = "Player's turn. Choose hit or stand.";
            }
        }

        private static int CalculateScore(List<Card> hand)
        {
            int score = 0;
            int aceCount = 0;

            foreach (Card card in hand)
            {
                score += (int)card.Rank;
                if (card.Rank == Rank.Ace)
                {
                    aceCount++;
                }
            }

            // Adjust for Aces if score is over 21
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }

        // Player's turn: Hit or Stand
        public static void PlayerTurn()
        {
            // Example code to trigger a hit/stand action in Unity (e.g., via buttons)
            // In Unity, you can call `Hit()` or `Stand()` based on player input, e.g., button clicks.

            // For example, if the player chooses "hit":
            // Hit();
            // Or if the player chooses "stand":
            // Stand();
        }

        public static void Hit()
        {
            if (playerScore < 21)
            {
                Card newCard = deck.DealCard();
                playerHand.Add(newCard);
                playerScore = CalculateScore(playerHand);
                UpdateUI();

                if (playerScore > 21)
                {
                    gameStatusText.text = "You busted! Dealer wins.";
                }
            }
        }

        public static void Stand()
        {
            DealerTurn();
        }

        // Dealer's turn: Dealer draws until they have 17 or higher
        private static void DealerTurn()
        {
            while (dealerScore < 17)
            {
                Card newCard = deck.DealCard();
                dealerHand.Add(newCard);
                dealerScore = CalculateScore(dealerHand);
                UpdateUI();
            }

            // Final win/lose check
            DetermineWinner();
        }

        private static void DetermineWinner()
        {
            if (playerScore > 21)
            {
                gameStatusText.text = "You busted! Dealer wins.";
            }
            else if (dealerScore > 21)
            {
                gameStatusText.text = "Dealer busted! You win!";
            }
            else if (playerScore > dealerScore)
            {
                gameStatusText.text = "You win!";
            }
            else if (playerScore < dealerScore)
            {
                gameStatusText.text = "Dealer wins.";
            }
            else
            {
                gameStatusText.text = "It's a tie.";
            }
        }
    }
}
*/