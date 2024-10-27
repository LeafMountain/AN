using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour {
    public List<Player> players = new();

    public void Add(Player player) {
        players.Add(player);
    }

    public Player Get(int index) {
        return players[index];
    }
}