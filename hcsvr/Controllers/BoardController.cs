using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Serialization;
using HexC;


namespace hcsv2020.Controllers
{


    class VisualBoardStore
    {
        protected static HexC.ColorsEnum ColorEnumFromString(string color)
        {
            HexC.ColorsEnum col = HexC.ColorsEnum.White;
            switch (color)
            {
                case "black": col = HexC.ColorsEnum.Black; break;
                case "tan": col = HexC.ColorsEnum.Tan; break;
            }
            return col;
        }

        public static Board GameBoard(string gameId)
        {
            Debug.Assert(ContainsGame(gameId));
            return m_allBoards[gameId];
        }

        public static bool ContainsGame(string gameId) { return m_allBoards.ContainsKey(gameId); } // m_allPieces.ContainsKey(id); }
        // public static Dictionary<string, string> GameBoard(string id) { if (m_allPieces.ContainsKey(id)) return m_allPieces[id]; return null; }
        //        public static Dictionary<string, string> TeamHues(string id, string color) { return m_allHues[id][ColorEnumFromString(color)]; }
        /*
        public static void AddBoard(string id, Dictionary<string, string> board)
        {
            m_allPieces.Add(id, board);
            
        m_allHues.Add(id, new Dictionary<HexC.ColorsEnum, Dictionary<string, string>>());
            m_allHues[id].Add(HexC.ColorsEnum.White, new Dictionary<string, string>());
            // what is this about???   m_allHues[id][HexC.ColorsEnum.White].Add("n0_n0", "9,9,9,1.0");
            m_allHues[id].Add(HexC.ColorsEnum.Black, new Dictionary<string, string>());
            m_allHues[id].Add(HexC.ColorsEnum.Tan, new Dictionary<string, string>());
        }
        */

        // There's just one board per game, but each game has different hue maps for each player.
        //        protected static Dictionary<string, Dictionary<string, string>> m_allPieces = new Dictionary<string, Dictionary<string, string>>();
        protected static Dictionary<string, Board> m_allBoards = new Dictionary<string, Board>();
//        protected static Dictionary<string, Dictionary<HexC.ColorsEnum, Dictionary<string, string>>> m_allHues = new Dictionary<string, Dictionary<HexC.ColorsEnum, Dictionary<string, string>>>();
        // I care who pressed Turn End
        protected static Dictionary<string, HexC.ColorsEnum> m_yourTurn = new Dictionary<string, HexC.ColorsEnum>();

        public static HexC.ColorsEnum WhoseTurn(string gameId)
        {
            Debug.Assert(ContainsGame(gameId));
            return m_yourTurn[gameId];
        }

        public static void LastReportedTurnEnd(string gameId, string color)
        {
            switch (color)
            {
                case "white":
                    m_yourTurn[gameId] = HexC.ColorsEnum.Tan;
                    return;

                case "tan":
                    m_yourTurn[gameId] = HexC.ColorsEnum.Black;
                    return;

                case "black":
                    m_yourTurn[gameId] = HexC.ColorsEnum.White;
                    return;

                default:
                    Debug.Assert(false);
                    return;
            }
        }

        public static void MakeCertainGameExists(string gameId)
        {
            var board = VisualBoardStore.GameBoard(gameId);
            HexC.Board b = new HexC.Board();
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -1, -4));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -4, -1));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.Black, -1, -3));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.Black, -2, -2));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.Black, -3, -1));
            b.Add(new PlacedPiece(PiecesEnum.Queen, ColorsEnum.Black, -3, -2));
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.Black, -2, -3));
            b.Add(new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.Black, -1, -2));
            b.Add(new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.Black, -1, -1));
            b.Add(new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.Black, -2, -1));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Tan, -4, 5));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Tan, -1, 5));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.Tan, -3, 4));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.Tan, -2, 4));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.Tan, -1, 4));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -4));
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.White, 5, -3));
            b.Add(new PlacedPiece(PiecesEnum.Queen, ColorsEnum.White, 5, -2));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -1));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.White, 4, -3));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.White, 4, -2));
            b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.White, 4, -1));
            b.Add(new Piece(PiecesEnum.Castle, ColorsEnum.White)); // I have a castle on the sidelines.
            b.Add(new Piece(PiecesEnum.Castle, ColorsEnum.Tan)); // I have a castle on the sidelines.
        }
    }

    /*
    public class GameHelper
    {
        public static void MakeCertainGameExists(string gameId)
        {
            if (VisualBoardStore.ContainsGame(gameId))
                return;

            // it exists but there aren't any pieces on the board today.
            m_yourTurn[gameId] = HexC.ColorsEnum.Black;

            Debug.Assert(false); // doesn't exist huh.


        }
    }
    */

    public class BoardController : ControllerBase
    {
        public class Spot
        {
            public int Q { get; set; }
            public int R { get; set; }
            public string Color { get; set; }
            public string Piece { get; set; }
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


        [HttpPost]
        public IActionResult Moves([FromQuery] string gameId)
        {
            System.IO.Stream req = Request.Body;
       //     req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new System.IO.StreamReader(req).ReadToEnd();
            var pieces = System.Text.Json.JsonSerializer.Deserialize<List<Spot>>(json);

            // with these pieces, render the board cleanly
            HexC.Board b = new HexC.Board();
            foreach (Spot s in pieces)
            {
                if (s.Q == 99)
                    b.Add(new Piece(FromString.PieceFromString(s.Piece), FromString.ColorFromString(s.Color)));
                else
                    b.Add(new PlacedPiece(FromString.PieceFromString(s.Piece), FromString.ColorFromString(s.Color), s.Q, s.R));
            }

            // We have a board that shows what a player wants to change the board to.
            // What player? We have no idea. I do know the gameId, and that should tell me the turn state.

            return Ok();
        }


        [HttpGet]
        public IActionResult Board([FromQuery] string gameId, [FromQuery] string color)
        {
            // fuck CORS for now: Response.Headers.Add("Access-Control-Allow-Origin", "*");
            System.Threading.Semaphore s = new System.Threading.Semaphore(1, 1, "COPS"); s.WaitOne();
            try
            {
                if (null == gameId || color == null)
                    return BadRequest();

                VisualBoardStore.MakeCertainGameExists(gameId);
                Board b = VisualBoardStore.GameBoard(gameId);

                PrettyJsonBoard pjb = new PrettyJsonBoard(b.PlacedPieces, b.SidelinedPieces);

                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var jsonString = System.Text.Json.JsonSerializer.Serialize(pjb, options);

                Console.WriteLine(jsonString);

                return Ok(jsonString);
            }
            finally { s.Release(); }
        }

        /* revive this when I'm using CORS again

        [HttpOptions]
        public IActionResult Board()
        {
            Response.Headers.Add("Allow", "OPTIONS,GET,PUT,POST,PATCH");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE,OPTIONS");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Authorization,Content-Length,X-Requested-With,Accept");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Content-Length", "0");

            return Ok();
        }
    */

    }
}

