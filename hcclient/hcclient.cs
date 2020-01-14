using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using HexC;
using Newtonsoft.Json;

namespace HexCClient
{


    class hcclient
    {
        public class Spot
        {
            public int Q { get; set; }
            public int R { get; set; }
            public string Color { get; set; }
            public string Piece { get; set; }

        }

        public class FromString
        {
            public static HexC.PiecesEnum PieceFromString(string piece)
            {
                switch (piece)
                {
                    case "King": return PiecesEnum.King;
                    case "Queen": return PiecesEnum.Queen;
                    case "Castle": return PiecesEnum.Castle;
                    case "Elephant": return PiecesEnum.Elephant;
                    case "Pawn": return PiecesEnum.Pawn;
                }
                System.Diagnostics.Debug.Assert(false);
                return PiecesEnum.King;
            }

            public static HexC.ColorsEnum ColorFromString(string color)
            {
                switch (color)
                {
                    case "Black": return ColorsEnum.Black;
                    case "Tan": return ColorsEnum.Tan;
                    case "White": return ColorsEnum.White;
                }
                System.Diagnostics.Debug.Assert(false);
                return ColorsEnum.Black;
            }
        }

        protected static BoardLocation ShiftedSpot(BoardLocation from, int q, int r)
        {
            if (null == from)
                from = new BoardLocation(0, 0);
            BoardLocation delta = new BoardLocation(q, r);
            BoardLocation newSpot = new BoardLocation(from, delta);
            if (newSpot.IsValidLocation())
                return newSpot;
            return from;
        }



        public class PrettyJsonPiece
        {
            public string Color { get; set; }
            public string Piece { get; set; }
        }

        public class PrettyJsonPlacedPiece : PrettyJsonPiece
        {
            public int Q { get; set; }
            public int R { get; set; }
        }

        public class PrettyJsonBoard : List<object>
        {
            public PrettyJsonBoard(List<PlacedPiece> lpPlaced, PieceList plSidelined)
            {
                foreach (PlacedPiece pp in lpPlaced)
                {
                    PrettyJsonPlacedPiece pjp = new PrettyJsonPlacedPiece();
                    pjp.Q = pp.Location.Q;
                    pjp.R = pp.Location.R;
                    pjp.Color = pp.Color.ToString();
                    pjp.Piece = pp.PieceType.ToString();
                    this.Add(pjp);
                }
                foreach (var p in plSidelined)
                {
                    PrettyJsonPlacedPiece pjp = new PrettyJsonPlacedPiece();
                    pjp.Color = p.Color.ToString();
                    pjp.Piece = p.PieceType.ToString();
                    pjp.Q = 99;
                    pjp.R = 99;

                    this.Add(pjp);
                }
            }
        }


        // We ask the server if this submission is legit.
        async static Task Submit(Board b)
        {
            PrettyJsonBoard pjb = new PrettyJsonBoard(b.PlacedPieces, b.SidelinedPieces);
            // I may as well submit the whole board to the server, which will decide if the change is valid.
            // Which probably means a Post with a JSON attachment.
            using var client = new HttpClient();
            HttpContent content = new StringContent(JsonConvert.SerializeObject(pjb), System.Text.Encoding.UTF8, "application/json");
            var jsonContent = await client.PostAsync("http://hexchess.cloud/Board/Moves", content);

        }


        async static Task Main(string[] args)
        {
            using var client = new HttpClient();
            var jsonContent = await client.GetStringAsync($"http://hexchess.cloud/Board/Board?gameId={args[0]}&color={args[1]}");

            var pieces = System.Text.Json.JsonSerializer.Deserialize<List<Spot>>(jsonContent);

            // with these pieces, render the board cleanly
            HexC.Board b = new HexC.Board();
            foreach (Spot s in pieces)
            {
                if (s.Q == 99)
                    b.Add(new Piece(FromString.PieceFromString(s.Piece), FromString.ColorFromString(s.Color)));
                else
                    b.Add(new PlacedPiece(FromString.PieceFromString(s.Piece), FromString.ColorFromString(s.Color), s.Q, s.R));
            }

            Board turnStartBoard = new Board(b); // clone this

            // loop as a user interface by showing the board, and commands for the newbies:
            BoardLocation cursor = new BoardLocation(0, 0);
            BoardLocation selected = null;
            int iSlotOfSidelinedPiece = -1;

            while (true)
            {
                Console.Clear();

                // Captured pieces across the top please.
                var sidelined = b.SidelinedPieces;
                for (int iPieceSlot = 0; iPieceSlot < sidelined.Count; iPieceSlot++)
                {
                    SetPieceColor(sidelined[iPieceSlot].Color);
                    if (iSlotOfSidelinedPiece == iPieceSlot)
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(sidelined[iPieceSlot].ToChar());
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
                Console.WriteLine();

                ShowTextBoard(b, cursor); // cursor could be null

                Console.WriteLine();
                Console.WriteLine("134679:Move Cursor\r\n5:Select\r\n-:Sidelined pieces\r\nR:Reset to turn start\r\nF:Finish turn");

                ConsoleKeyInfo cki = Console.ReadKey();
                switch (cki.KeyChar.ToString().ToLower()[0])
                {
                    case 'f': await Submit(b); break;
                    case 'r': iSlotOfSidelinedPiece = -1; b = turnStartBoard; break;
                    case '1': iSlotOfSidelinedPiece = -1; cursor = ShiftedSpot(cursor, -1, 1); break;
                    case '3': iSlotOfSidelinedPiece = -1; cursor = ShiftedSpot(cursor, 0, 1); break;
                    case '4': iSlotOfSidelinedPiece = -1; cursor = ShiftedSpot(cursor, -1, 0); break;
                    case '6': iSlotOfSidelinedPiece = -1; cursor = ShiftedSpot(cursor, 1, 0); break;
                    case '7': iSlotOfSidelinedPiece = -1; cursor = ShiftedSpot(cursor, 0, -1); break;
                    case '9': iSlotOfSidelinedPiece = -1; cursor = ShiftedSpot(cursor, 1, -1); break;
                    case '*': // move piece to sideline
                        {
                            iSlotOfSidelinedPiece = -1;
                            var piece = b.AnyoneThere(cursor);
                            b.Remove(piece);
                            b.SidelinedPieces.Add(piece);
                            break;
                        }

                    case '-':
                        {
                            if (-1 == iSlotOfSidelinedPiece)
                            {
                                if (b.SidelinedPieces.Count == 0)
                                    break; // nobody in limbo!
                                cursor = null;
                                iSlotOfSidelinedPiece = 0;
                            }
                            else
                            {
                                iSlotOfSidelinedPiece++;
                                if (b.SidelinedPieces.Count < iSlotOfSidelinedPiece)
                                    iSlotOfSidelinedPiece = -1;
                            }
                        }
                        break;

                    case '5':
                        if (-1 != iSlotOfSidelinedPiece)
                        {
                            // if nobody in center, put this piece there.
                            BoardLocation bl = new BoardLocation(0, 0);

                            if (null == b.AnyoneThere(bl))
                            {
                                var piece = sidelined[iSlotOfSidelinedPiece];
                                b.SidelinedPieces.Remove(piece);
                                PlacedPiece pp = new PlacedPiece(piece.PieceType, piece.Color, 0, 0);
                                b.Add(pp);
                                iSlotOfSidelinedPiece = -1;
                                break;
                            }
                        }
                        else // did we previously select a spot?
                        if (null == selected)
                        {
                            // none previously selected. just remember this spot as the selection.
                            if (null != b.AnyoneThere(cursor))
                                selected = cursor;
                        }
                        else
                        {
                            PlacedPiece pp = b.AnyoneThere(selected); // we selected this one earlier

                            PlacedPiece pp_dest = b.AnyoneThere(cursor);
                            if (null != pp_dest)
                            {
                                // I can drop my piece on opponent, or Queen-King if valid. But postponing Queen-King magic for now.
                                if (pp_dest.Color == pp.Color)
                                    continue;

                                b.Remove(pp_dest);
                                b.SidelinedPieces.Add(pp_dest);
                            }
                            b.Remove(pp);
                            PlacedPiece ppNew = new PlacedPiece(pp, cursor);
                            b.Add(ppNew);

                            selected = null;
                        }
                        break;
                }

                // ok, let user cycle through my pieces.
                // um i don't have a color... why does Board/Board specify a color? color=white?

                // The user might hit a key to cycle through their pieces of a type.
                // Or they might hit the Select (enter?).

                // since an any-spot selector needs to run for the drop-here phase,
                // start iwth an any-spot selector for the select phase.

                // a board has these UI states:
                // is one piece selected? it can be carried anywhere!
                // so a white selector background, then when a piece is chosen,
                // a gray pad under it, and a new white selector for the destination...
                // and then select a destination. and the move occurs.
            }
        }


        protected static void SetPieceColor(ColorsEnum color)
        {
            switch (color)
            {
                case ColorsEnum.Black: Console.ForegroundColor = ConsoleColor.Cyan; break;
                case ColorsEnum.White: Console.ForegroundColor = ConsoleColor.White; break;
                case ColorsEnum.Tan: Console.ForegroundColor = ConsoleColor.Red; break;
            }
        }


        public static void ShowTextBoard(Board b, BoardLocation singleSpot = null, BoardLocationList highlights = null)
        {
            // Spit sequentially
            // the lines grow, then shrink
            // 6, then 7, then 8, then 9, up to 11, then back down.
            // FIRST:  how many in this line.
            // SECOND: first of the two coordinates, that increment across the line.
            // THIRD:  the other coordinate, that does not change across this line.
            int[,] Lines =  { { 6,  0, -5 },
                              { 7, -1, -4 },
                              { 8, -2, -3 },
                              { 9, -3, -2 },
                              {10, -4, -1 },
                              {11, -5,  0 },
                              {10, -5,  1 },
                              { 9, -5,  2 },
                              { 8, -5,  3 },
                              { 7, -5,  4 },
                              { 6, -5,  5 } };

            for (int iLine = 0; iLine < Lines.GetLength(0); iLine++)
            {
                Console.ResetColor();
                // First ident this many spaces: 11 minus how-many-this-line
                Console.Write("             ".Substring(0, 11 - Lines[iLine, 0]));

                // now increment through the line
                for (int iPos = 0; iPos < Lines[iLine, 0]; iPos++)
                {
                    BoardLocation spot = new BoardLocation(Lines[iLine, 1] + iPos, Lines[iLine, 2]);

                    if (highlights != null)
                    {
                        if (highlights.ContainsTheLocation(spot))
                        {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                        }
                    }

                    if (singleSpot != null)
                    {
                        if (BoardLocation.IsSameLocation(singleSpot, spot))
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                    }

                    PlacedPiece p = b.AnyoneThere(spot);
                    if (null == p)
                    {
                        if (spot.IsPortal)
                            Console.BackgroundColor = ConsoleColor.DarkGray;

                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(spot.IsPortal ? "X" : "·");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ResetColor();
                        Console.Write(" ");
                    }
                    else
                    {
                        SetPieceColor(p.Color);
                        Console.Write(p.ToChar());
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }

        }

    }
}
