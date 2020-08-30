using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using HexC;
using System.Diagnostics;
using Newtonsoft.Json;


namespace BestMove
{

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
        public PrettyJsonBoard(List<PlacedPiece> lpPlaced)
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
        }
    }

    /*

namespace HttpClientSample
{

    public class Spot
    {
        public string loc { get; set; }
        public string tok { get; set; }
        public string hue { get; set; }
    }


    public class Move
    {
        public string gameId { get; set; }
        public string color { get; set; }
        public string moveFrom { get; set; }
        public string moveTo { get; set; }
    }

        */

    class BestMoveBaby
    {
        static HttpClient client = new HttpClient();

        /*
        static async Task<List<Spot>> GetProductAsync(string path)
        {

            List<Spot> board = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                board = await response.Content.ReadAsAsync<List<Spot>>();
            }
            return board;

        }
        */

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
                switch (color.ToLower())
                {
                    case "black": return ColorsEnum.Black;
                    case "tan": return ColorsEnum.Tan;
                    case "white": return ColorsEnum.White;
                }
                System.Diagnostics.Debug.Assert(false);
                return ColorsEnum.Black;
            }
        }


        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            while (true)
            {
                //            var jsonContent = await client.GetStringAsync("https://34.211.78.118/Board/Board?gameId=abc&color=white");
                var whoseTurn = await client.GetStringAsync($"http://{args[0]}/Board/WhoseTurn?gameId={args[1]}");
                whoseTurn = whoseTurn.Replace("\"", "");
                HexC.ColorsEnum whose = FromString.ColorFromString(whoseTurn);

                var jsonContent = await client.GetStringAsync($"http://{args[0]}/Board/Board?gameId={args[1]}&color={whose}");

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

                // now i got me the board. what i do now? why not ask what the best move for each color is on this board?
                // which is the shallowest analysis first.
                // i believe um this entails examining what each piece can cause,
                // and doing some numeric on me-gains, them-losses.
                // shallow as possible.

                Dictionary<List<PieceEvent>, int> allEmOptions = OptionsExamined(b, whose);

                // yeah sure maybe later i look derper into what each option set can cause, but right now i wanna numeric of the outcome.
                // and this is fuckin broken cuz i don't think it results in a piece into the portal on successful capture.
                // because i don't care what's off the board...
                // wait, isn't the plan to calculate the captured based on the fielded?
                // yeah, that's the plan.

                var items = from options in allEmOptions
                            orderby options.Value descending
                            select options;

                // just take the top item from items... and exact moves that carry it out.
                // this could get tricky.
                var mahmoove = items.ElementAt(0);

                // we know the event results... but translate that into Moves...
                // but you know i wish i could just um shove this into a special endpoint please.
                // adds and removes in one body.
                var themEvents = mahmoove.Key;

                // i have to feed the new board to the API. So gotta pass a whole board, and then make sure the turn was accepted.


                // ok, i can check that the turn works
                {
                    var whoseTurnBefore = await client.GetStringAsync($"http://{args[0]}/Board/WhoseTurn?gameId={args[1]}");

                    // Listen up: If your turn STARTED with a piece of your color in the portal,
                    // and it's still there, then i will take it out for you.
                    /* BESTMOVE SHOULDN'T NEED THIS BUT I'LL LEAVE IT HERE BECAUSE OPTIONSEXAMINED SHOULD RESULT IN PERFECT BOARD STATE.
                    HexC.ColorsEnum whose = FromString.ColorFromString(whoseTurnBefore.Replace("\"", ""));
                    var centerPiece = turnStartBoard.AnyoneThere(new BoardLocation(0, 0));
                    if (null != centerPiece)
                        if (centerPiece.Color == whose)
                            b.Remove(centerPiece);
                    */

                    // ok hacker, all removes, then all adds.
                    foreach (var doe in themEvents)
                    {
                        if (doe.EventType == EventTypeEnum.Remove)
                            b.Remove(doe.Regarding);
                    }

                    foreach (var doe in themEvents)
                    {
                        if (doe.EventType == EventTypeEnum.Add)
                            b.Add(doe.Regarding);
                    }

                    PrettyJsonBoard pjb = new PrettyJsonBoard(b.PlacedPieces);
                    //                PrettyJsonBoard pjb = new PrettyJsonBoard(b.PlacedPieces, b.SidelinedPieces);
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(pjb), System.Text.Encoding.UTF8, "application/json");
                    await client.PostAsync($"http://{args[0]}/Board/Moves?gameId={args[1]}", content);
                    // ok, the event occurred... now ask whose turn it is! That's how we know if the turn was accepted! Clever, no?
                    var whoseTurnNow = await client.GetStringAsync($"http://{args[0]}/Board/WhoseTurn?gameId={args[1]}");
                    if (whoseTurnBefore == whoseTurnNow)
                    {
                        Debug.Assert(false);
                        Console.WriteLine("FAILED");
                        return;
                    }
                }
            }
        }

        static Dictionary<List<PieceEvent>, int> OptionsExamined(HexC.Board b, HexC.ColorsEnum col)
        {
            Dictionary<List<PieceEvent>, int> themDeltas = new Dictionary<List<PieceEvent>, int>();
            foreach (var piece in b.PlacedPiecesThisColor(col))
            {
                var options = b.WhatCanICauseWithDoo(piece);
                foreach (var option in options)
                {
                    int delta = 0;
                    foreach (var anEvent in option)
                    {
                        int swing = 0;
                        switch (anEvent.Regarding.PieceType)
                        {
                            case PiecesEnum.Castle: swing = 5; break;
                            case PiecesEnum.Elephant: swing = 4; break;
                            case PiecesEnum.King: swing = 99; break;
                            case PiecesEnum.Pawn: swing = 1; break;
                            case PiecesEnum.Queen: swing = 20; break;
                        }

                        // if it's a remove, fflip it.
                        if (anEvent.EventType == EventTypeEnum.Remove)
                            swing = -swing;

                        // if it's not me, flip it
                        if (anEvent.Regarding.Color != col)
                            swing = -swing;

                        delta += swing;
                    }
                    themDeltas.Add(option, delta);
                }
            }
            return themDeltas;
        }
    }
}