using System.Collections.Generic;
using GameEngine.GravityDot;
using GameEngine.UnityMock;
using UnityEngine;


//can probly ref this in the game manager as variable then it dont have to be static and could possibly have diffrent engines to assign??
namespace Game.Engine
{
    public static class EngineManager
    {
        public static UnityGameEngine gameEngine;
        static FullState fullstate;

        private static int currentGameID;
        // private static int player1ID;
        // private static int player1ID;


        public static FullState Fullstate
        {
            get => fullstate;
            private set => fullstate = value;
        }


        public static void SetupGame(int gameID, int player1ID, int player2ID)
        {
            currentGameID = gameID;

            gameEngine = new UnityGameEngine();
            gameEngine.CreateGame(gameID, player1ID, player2ID);
            Fullstate = gameEngine.FullState(gameID);
        }

        public static void RefreshFullState()
        {
            Fullstate = gameEngine.FullState(currentGameID);
        }
        
        public static void Tick()
        {
            gameEngine.Tick(currentGameID);
            RefreshFullState();
        }



        #region SelectionPlacement

        public static List<int[]> UpdateSelection(Side side, float row)
        {
            return MockLogic.GetRay(gameEngine.FullState(currentGameID).Board, side, (int) row, gameEngine.FullState(currentGameID).CurrentPlayer);
            
           // return MockLogic.GetRay(Fullstate.Board, side, (int) row, Fullstate.CurrentPlayer);
        }

        public static bool IsValidMove(Side side, float row)
        {
            return gameEngine.ValidateMove(currentGameID, gameEngine.GetPlayerId(Fullstate.CurrentPlayer), side,
                (int) row);
        }

        public static void MakeMove(Side side, float row)
        {
            gameEngine.PlayerMove(currentGameID, gameEngine.GetPlayerId(Fullstate.CurrentPlayer), side, (int) row);
            RefreshFullState();
        }

        #endregion



        #region Bombs

        public static bool IsValidBomb(int player, Vector2 pos)
        {
            var position = new int[]
            {
              (int)pos.x,
              (int)pos.y,
            };
            
            
            return gameEngine.ValidateBomb(currentGameID,gameEngine.GetPlayerId(player), position);
        }
        
        public static void PlaceBomb(int player, int[] pos)
        {
            gameEngine.PlayerBomb(currentGameID,gameEngine.GetPlayerId(player), pos[0], pos[1]);
            RefreshFullState();
        }

        #endregion

    }
}