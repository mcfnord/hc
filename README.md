# Hexagonal Chess Online

Hexagonal Chess on ASP.NET Core 3.1 or so.

## Winning

* A player loses when their turn begins with them in checkmate. The player who first caused a checkmate state wins. 
* You can also win by getting your king down the portal.

## Special moves

* If your king and queen touch, your turn can begin by swapping them. You can do this to exit check, but can't enter check.
* A pawn that touches two other pawns of the same color can't be attacked.

### Portal

* If you take a piece of a kind you have lost, and the portal (center spot) is empty, your captured piece arrives in the portal.
* If you enter your turn with your piece in the portal, but you don't move it out of that spot, then it's lost again.
* If you attack an opponent piece that occcupies the portal, both pieces are lost, but if you've lost the kind of piece that you just attacked, your own piece appears in the portal.

## UI Quirks

To swap king and queen, move one piece to a neutral spot first.

# Join us

This code has been prepared by the Git Blox Bois. We welcome new players and contributors. 
This repository contains the server code. You can also see the browser code here:
https://github.com/oinke/hexchess-www

The Git Blox Bois are an international anarchist hacker collective that opposes the patriarchy
and encourages gender and sexual minorities to contribute and play. You might find us here:
https://hangouts.google.com/group/wxGfxZPxbj2pOy0M2

https://blogs.scientificamerican.com/beautiful-minds/the-personality-of-political-correctness/
