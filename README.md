# Hexagonal Chess Online

Hexagonal Chess on ASP.NET Core 3.1 or so.

## Game Rules

If your king and queen touch, your turn can begin by swapping them. You can do this to exit check, but can't enter check.

If you take a piece of a kind you have lost, and the portal (center spot) is empty, you can place your piece there.

If you enter your turn with your piece in the portal, but you don't move it out of that spot, then it's lost again.

If you attack an opponent piece that occcupies the portal, both pieces are lost, but if you've lost the kind of piece that you just attacked, your own piece appears in the portal.

A player loses when their turn begins with them in checkmate. The player who first caused a checkmate state wins. 

You can also win by getting your king down the portal.

## Quirks you must deal with until I fix them

To swap king and queen, move one piece to a neutral spot first.

## Differences from meatspace rules

A pawn that touches two other pawns can't be attacked.

Reaching the opposite side of the board with a pawn has no special outcome.

You can't add pieces (such as two-queen in the Olympia variant, or extra pawns in limbo as a handicap).

The ability to attack a portal occupant probably differs.

# Join us

This code has been prepared by the Git Blox Bois. We welcome new players and contributors. 
This repository contains the server code. You can also see the browser code here:
https://github.com/oinke/hexchess-www

The Git Blox Bois are an international anarchist hacker collective that opposes the patriarchy
and encourages gender and sexual minorities to contribute and play. You might find us here:
https://hangouts.google.com/group/wxGfxZPxbj2pOy0M2

https://blogs.scientificamerican.com/beautiful-minds/the-personality-of-political-correctness/
