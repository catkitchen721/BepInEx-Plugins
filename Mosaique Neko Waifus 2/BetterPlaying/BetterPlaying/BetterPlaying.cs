using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace BetterPlaying
{
    [BepInPlugin("com.xjpjeass.mnw2.solve_puzzle", "Puzzle Solver", "1.0")]
    [BepInProcess("Mosaique Neko Waifus 2.exe")]
    public class PuzzleSolver : BaseUnityPlugin
    {
        ConfigEntry<KeyboardShortcut> keyConfig;
        private bool isOpenGUI;
        private List<MosaiqueBoard> mbs;
        private List<PuzzlePiece> pps;
        private MosaiqueManager mm;

        void Start()
        {
            new Harmony("com.xjpjeass.mnw2.solve_puzzle").PatchAll();

            isOpenGUI = false;

            Logger.LogInfo("key config");
            keyConfig = Config.Bind<KeyboardShortcut>("KeyCodes", "solve_puzzle_key", new KeyboardShortcut(KeyCode.S), "解決拼圖 熱鍵");
        }

        void Update()
        {
            if (keyConfig.Value.IsDown())
            {
                if(isOpenGUI)
                    isOpenGUI = false;
                else
                    isOpenGUI = true;
            }
        }

        private void SolvePuzzle()
        {
            try
            {
                mbs = MosaiqueSpawner.GetCurrentBoards();
                mm = MosaiqueSpawner.GetCurrentMosaiqueManager();
                Logger.LogInfo(mbs.Count + " boards.");
                foreach (MosaiqueBoard mb in mbs)
                {
                    
                    this.pps = mb.ReturnUnsolvedPieces();
                    foreach (PuzzlePiece pp in this.pps)
                    {
                        pp.SolvePiece();
                    }
                    Logger.LogInfo("Successed solve puzzle!");
                }
                for(int i=0; i<mm.dataPacket.submosaiqueRotations.Length; i++)
                {
                    mm.dataPacket.submosaiqueRotations[i] = RotationState.up;
                    mm.ChangeInThePuzzleTriggered();
                    mm.CheckIfSolved();
                }
            }
            catch(ArgumentOutOfRangeException e)
            {
                Logger.LogInfo("No Puzzles here.");
            }
        }

        void OnGUI()
        {
            if(isOpenGUI)
            {
                if(GUI.Button(new Rect((float)(Screen.width/2 - 100), (float)(Screen.height - 100), 200, 50), "Solve Puzzle!"))
                {
                    SolvePuzzle();
                }
            }
        }
    }

    [HarmonyPatch(typeof(StartGameMenu), "QuitGame")]
    class StartGame_QuitGame_Patch
    {
        public static bool Prefix()
        {
            return true;
        }
    }
}
