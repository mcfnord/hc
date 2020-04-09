# Hexagonal Chess Online

Hexagonal Chess on ASP.NET Core 3.1 or so. This repository contains server code, a text-based client, and a draft of an AI player.

## Winning

* A player loses when their turn begins with them in checkmate. The player who first caused a checkmate state wins. 
* You can also win by getting your king to the portal.

## Special moves

* If your king and queen touch, your turn can begin by swapping them.
* A pawn that touches two other pawns of the same color can't be attacked.

### Portal

* If you take a piece of a kind you have lost, and the portal (center spot) is empty, your captured piece arrives in the portal.
* If you enter your turn with your piece in the portal, but you don't move it out of that spot, then it's lost again.
* If you capture an opponent piece that occupies the portal, both pieces are lost. If you've lost the kind of piece that you just captured, your own piece appears in the portal.
* A king can land on the portal (while empty or while occupied by a piece of another color) and win the game.
* A castle or queen cannot pass straight through the portal, and a portal is not a passage for pieces that require a path (queen's special move and pawn's attack).

# Join us

This code has been prepared by the Git Blox Bois, an international anarcho-feminist hacker collective. Join us: https://hangouts.google.com/group/wxGfxZPxbj2pOy0M2

Find outdated browser code here: https://github.com/oinke/hexchess-www

https://blogs.scientificamerican.com/beautiful-minds/the-personality-of-political-correctness/
