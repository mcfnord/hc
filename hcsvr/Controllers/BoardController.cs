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


    public class VisualBoardStore
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

        public static bool ContainsGame(string id) { return m_allPieces.ContainsKey(id); }
        public static Dictionary<string, string> GameBoard(string id) { if (m_allPieces.ContainsKey(id)) return m_allPieces[id]; return null; }
        //        public static Dictionary<string, string> TeamHues(string id, string color) { return m_allHues[id][ColorEnumFromString(color)]; }
        public static void AddBoard(string id, Dictionary<string, string> board)
        {
            m_allPieces.Add(id, board);
            m_allHues.Add(id, new Dictionary<HexC.ColorsEnum, Dictionary<string, string>>());
            m_allHues[id].Add(HexC.ColorsEnum.White, new Dictionary<string, string>());
            // what is this about???   m_allHues[id][HexC.ColorsEnum.White].Add("n0_n0", "9,9,9,1.0");
            m_allHues[id].Add(HexC.ColorsEnum.Black, new Dictionary<string, string>());
            m_allHues[id].Add(HexC.ColorsEnum.Tan, new Dictionary<string, string>());
        }

        // There's just one board per game, but each game has different hue maps for each player.
        protected static Dictionary<string, Dictionary<string, string>> m_allPieces = new Dictionary<string, Dictionary<string, string>>();
        protected static Dictionary<string, Dictionary<HexC.ColorsEnum, Dictionary<string, string>>> m_allHues = new Dictionary<string, Dictionary<HexC.ColorsEnum, Dictionary<string, string>>>();
        // I care who pressed Turn End
        protected static Dictionary<string, HexC.ColorsEnum> m_yourTurn = new Dictionary<string, HexC.ColorsEnum>();

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
    }

    public class GameHelper
    {
        public static void MakeCertainGameExists(string id)
        {
            if (VisualBoardStore.ContainsGame(id))
                return;

            Dictionary<string, string> board = new Dictionary<string, string>();

            /* no more  s to hold space for blanks 
            board.Add("n0_n0", "XX");
            board.Add("n0_n1", "XX");
            board.Add("n0_n2", "XX");
            board.Add("n0_n3", "XX");
            board.Add("n0_n4", "XX");
            board.Add("n0_n5", "XX");
            board.Add("n0_p1", "XX");
            board.Add("n0_p2", "XX");
            board.Add("n0_p3", "XX");
            board.Add("n0_p4", "XX");
            board.Add("n0_p5", "XX");
            board.Add("n1_n0", "XX");
            board.Add("n1_p1", "XX");
            board.Add("n2_n0", "XX");
            board.Add("n2_p1", "XX");
            board.Add("n2_p2", "XX");
            board.Add("n3_n0", "XX");
            board.Add("n3_p1", "XX");
            board.Add("n3_p2", "XX");
            board.Add("n3_p3", "XX");
            board.Add("n4_n2", "XX");
            board.Add("n4_p1", "XX");
            board.Add("n4_p2", "XX");
            board.Add("n4_p3", "XX");
            board.Add("n4_p4", "XX");
            board.Add("n5_n0", "XX");
            board.Add("n5_n1", "XX");
            board.Add("n5_n2", "XX");
            board.Add("n5_p1", "XX");
            board.Add("n5_p2", "XX");
            board.Add("n5_p3", "XX");
            board.Add("n5_p4", "XX");
            board.Add("n5_p5", "XX");
            board.Add("p1_n0", "XX");
            board.Add("p1_n1", "XX");
            board.Add("p1_n2", "XX");
            board.Add("p1_n3", "XX");
            board.Add("p1_n4", "XX");
            board.Add("p1_n5", "XX");
            board.Add("p1_p1", "XX");
            board.Add("p1_p2", "XX");
            board.Add("p1_p3", "XX");
            board.Add("p1_p4", "XX");
            board.Add("p1_p5", "XX");
            board.Add("p2_n0", "XX");
            board.Add("p2_n2", "XX");
            board.Add("p2_n3", "XX");
            board.Add("p2_n4", "XX");
            board.Add("p2_n5", "XX");
            board.Add("p2_p1", "XX");
            board.Add("p2_p2", "XX");
            board.Add("p2_p3", "XX");
            board.Add("p2_p4", "XX");
            board.Add("p3_n0", "XX");
            board.Add("n4_n0", "XX");
            board.Add("p3_n3", "XX");
            board.Add("p3_n4", "XX");
            board.Add("p3_n5", "XX");
            board.Add("p3_p1", "XX");
            board.Add("p3_p2", "XX");
            board.Add("p3_p3", "XX");
            board.Add("p3_p4", "XX");
            board.Add("p4_n0", "XX");
            board.Add("p4_n4", "XX");
            board.Add("p4_n5", "XX");
            board.Add("p4_p1", "XX");
            board.Add("p4_p2", "XX");
            board.Add("p5_n0", "XX");
            board.Add("p5_n5", "XX");
            board.Add("p5_p3", "XX");
            board.Add("WV_01", "XX");
            board.Add("WV_02", "XX");
            board.Add("WV_03", "XX");
            board.Add("WV_04", "XX");
            board.Add("WV_05", "XX");
            board.Add("WV_06", "XX");
            board.Add("WV_07", "XX");
            board.Add("WV_08", "XX");
            board.Add("WV_09", "XX");
            board.Add("WV_10", "XX");
            board.Add("WV_11", "XX");
            board.Add("WV_12", "XX");
            board.Add("WV_13", "XX");
            board.Add("TV_01", "XX");
            board.Add("TV_02", "XX");
            board.Add("TV_03", "XX");
            board.Add("TV_04", "XX");
            board.Add("TV_05", "XX");
            board.Add("TV_06", "XX");
            board.Add("TV_07", "XX");
            board.Add("TV_08", "XX");
            board.Add("TV_09", "XX");
            board.Add("TV_10", "XX");
            board.Add("TV_11", "XX");
            board.Add("TV_12", "XX");
            board.Add("TV_13", "XX");
            board.Add("BV_01", "XX");
            board.Add("BV_02", "XX");
            board.Add("BV_03", "XX");
            board.Add("BV_04", "XX");
            board.Add("BV_05", "XX");
            board.Add("BV_06", "XX");
            board.Add("BV_07", "XX");
            board.Add("BV_08", "XX");
            board.Add("BV_09", "XX");
            board.Add("BV_10", "XX");
            board.Add("BV_11", "XX");
            board.Add("BV_12", "XX");
            board.Add("BV_13", "XX");

            */

            board.Add("n3_n2", "TK");
            board.Add("n3_p5", "WK");
            board.Add("p5_n2", "BK");



            //    board.Add("n1_p2", "WC");
            //   board.Add("n1_n1", "WP");

            board.Add("n1_p2", "WP");
            board.Add("n1_n1", "TP");
            board.Add("n1_n2", "TP");
            board.Add("n1_n3", "TE");
            board.Add("n1_n4", "TC");
            board.Add("n1_p3", "WP");
            board.Add("n1_p4", "WE");
            board.Add("n1_p5", "WC");
            board.Add("n2_n1", "TP");
            board.Add("n2_n2", "TE");
            board.Add("n2_n3", "TQ");
            board.Add("n2_p3", "WP");
            board.Add("n2_p4", "WE");
            board.Add("n2_p5", "WQ");
            board.Add("n3_n1", "TE");
            board.Add("n3_p4", "WE");
            board.Add("n4_n1", "TC");
            board.Add("n4_p5", "WC");
            board.Add("p2_n1", "BP");
            board.Add("p3_n1", "BP");
            board.Add("p3_n2", "BP");
            board.Add("p4_n1", "BE");
            board.Add("p4_n2", "BE");
            board.Add("p4_n3", "BE");
            board.Add("p5_n1", "BC");
            board.Add("p5_n3", "BQ");
            board.Add("p5_n4", "BC");

            VisualBoardStore.AddBoard(id, board);
            VisualBoardStore.AddBoard(id + "SNAPSHOT", board);

            // allHues.Add(id, new Dictionary<string, string>());
        }

    }



    public class BoardController : ControllerBase
    {
        public class Spot
        {
            public int Q { get; set; }
            public int R { get; set; }
            public string color { get; set; }
            public string piece { get; set; }
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

        [HttpGet]
        public IActionResult Board([FromQuery] string gameId, [FromQuery] string color)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            System.Threading.Semaphore s = new System.Threading.Semaphore(1, 1, "COPS"); s.WaitOne();
            try
            {
                if (null == gameId || color == null)
                    return BadRequest();

                GameHelper.MakeCertainGameExists(gameId);
                var board = VisualBoardStore.GameBoard(gameId);
                //              var hues = VisualBoardStore.TeamHues(gameId, color);
                //            var hues = VisualBoardStore.ReplaceHues(gameid, color);

                /*
                List<Spot> pieces = new List<Spot>();
                foreach( var item in board )
                {
                    var piece = new Spot();
                    piece.Q = item.Key[1];
                    if (item.Key[0] == 'n')
                        piece.Q = -piece.Q;
                    piece.R = 1;
                    piece.color = "white";
                    piece.piece = "horsie";
                    pieces.Add(piece);
                }
                */




                /*
                board.Add("n3_p5", "WK");
                board.Add("p5_n2", "BK");
                //    board.Add("n1_p2", "WC");
                //   board.Add("n1_n1", "WP");
                board.Add("n1_p2", "WP");
                board.Add("n1_n1", "TP");
                board.Add("n1_n2", "TP");
                board.Add("n1_n3", "TE");
                board.Add("n1_n4", "TC");
                board.Add("n1_p3", "WP");
                board.Add("n1_p4", "WE");
                board.Add("n1_p5", "WC");
                board.Add("n2_n1", "TP");
                board.Add("n2_n2", "TE");
                board.Add("n2_n3", "TQ");
                board.Add("n2_p3", "WP");
                board.Add("n2_p4", "WE");
                board.Add("n2_p5", "WQ");
                board.Add("n3_n1", "TE");
                board.Add("n3_p4", "WE");
                board.Add("n4_n1", "TC");
                board.Add("n4_p5", "WC");
                board.Add("p2_n1", "BP");
                board.Add("p3_n1", "BP");
                board.Add("p3_n2", "BP");
                board.Add("p4_n1", "BE");
                board.Add("p4_n2", "BE");
                board.Add("p4_n3", "BE");
                board.Add("p5_n1", "BC");
                board.Add("p5_n3", "BQ");
                board.Add("p5_n4", "BC");
                */


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
                //                b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.Tan, -3, -2));

                b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -4));
                b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.White, 5, -3));
                b.Add(new PlacedPiece(PiecesEnum.Queen, ColorsEnum.White, 5, -2));
                b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -1));
                b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.White, 4, -3));
                b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.White, 4, -2));
                b.Add(new PlacedPiece(PiecesEnum.Elephant, ColorsEnum.White, 4, -1));

                b.Add(new Piece(PiecesEnum.Castle, ColorsEnum.White)); // I have a castle on the sidelines.
                b.Add(new Piece(PiecesEnum.Castle, ColorsEnum.Tan)); // I have a castle on the sidelines.


                PrettyJsonBoard pjb = new PrettyJsonBoard(b.PlacedPieces, b.SidelinedPieces);

                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var jsonString = System.Text.Json.JsonSerializer.Serialize(pjb, options);

                Console.WriteLine(jsonString);

                return Ok(jsonString);

                /*

                string json = "[";

                foreach (var spot in board)
                {
                    json += "{\"loc\":\"" + spot.Key;
                    json += "\",\"tok\":\"" + spot.Value;

                    string hue = "128,128,128,0.9";
                    if (spot.Key == "n0_n0")
                        hue = "0,0,0,1.0";

                    if (hues.ContainsKey(spot.Key))
                        hue = hues[spot.Key];



                    json += "\",\"hue\":\"" + hue;

                    json += "\"},";
                }

                json = json.Substring(0, json.Length - 1);
                json += "]";

    */

                //return Ok(json);
            }
            finally { s.Release(); }
        }


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
    }
}

