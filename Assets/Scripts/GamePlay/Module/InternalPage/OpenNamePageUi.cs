using System;
using System.Collections.Generic;
using System.Text;
using Common.GameRoot;
using Data;
using GamePlay.Main;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GamePlay.Module.InternalPage
{
    public class OpenNamePageUi : InternalPageScript
    {
        private TextMeshProUGUI _txtName;
        private TextMeshProUGUI _txtDes;

        private TMP_InputField _inputFieldName;

        private string[] nameRandom =
        {
            "Alex", "Bella", "Charlie", "Danny", "Ellie", "Finn", "Grace", "Harry", "Ivy", "Jack", "Kate", "Leo", "Mia",
            "Noah", "Olivia", "Peter", "Quinn", "Ruby", "Sam", "Tara", "Uma", "Vince", "Wendy", "Xavier", "Yara",
            "Zack", "Amy", "Ben", "Chloe", "Dora", "Ethan", "Fiona", "George", "Holly", "Ian", "Julia", "Kyle", "Lucy",
            "Max", "Nina", "Oscar", "Priya", "Ray", "Sophie", "Tom", "Una", "Victor", "Willow", "Xena", "Zoe", "Adam",
            "Brooke", "Carson", "Dana", "Eric", "Freya", "Grant", "Harper", "Isaac", "Jasmine", "Kent", "Lila", "Mason",
            "Nora", "Owen", "Peyton", "Quinn", "Riley", "Sierra", "Tyler", "Uma", "Vincent", "Whitney", "Xavier",
            "Yasmin", "Zach", "Avery", "Blake", "Cameron", "Dakota", "Elliot", "Faith", "Gavin", "Hayden", "Iris",
            "Jordan", "Kennedy", "Logan", "Madison", "Nathan", "Paige", "Quentin", "Reese", "Shane", "Taylor", "Uriah",
            "Vanessa", "Wyatt", "Xavier", "Yvonne", "SunnyBunny123", "JellyBeanQueen", "MrHappyPants", "DreamyDaisy",
            "BubbleGumLover", "SweetiePie", "CuddleBug", "SnuggleNugget", "CandyKiss", "StarLightMagic", "PopcornPal",
            "RainbowRider", "CookieMonster", "HappyGoLucky", "SparkleStar", "MoonlightMuse", "FluffyCloud",
            "GigglyGuru", "HugMachine", "SmileSponge", "JoyJumper", "LoveLlama", "PuddingPop", "WaffleWanderer",
            "DonutDreamer", "CupcakeCrusher", "BouncyBall", "SockPuppet", "MarshmallowMuncher", "PineapplePrince",
            "BananaBandit", "LemonLover", "OrangeOwl", "AppleAce", "KiwiKicker", "GrapeGlide", "MangoMania",
            "StrawberrySmash", "CherryChaser", "MelonMaster", "PeachPunch", "BlueberryBoss", "WatermelonWhiz",
            "CocoCutie", "NuttyNinja", "ToastTamer", "TeaTimeTrudy", "CakeCrusader", "PancakePaladin", "PopTartPirate",
            "GameOn", "PixelPunch", "CouchCommander", "LazyLegend", "CouchKing", "TapMaster", "MatchMaker",
            "MergeMonarch", "SwapSultan", "SwipeStar", "PuzzlePro", "IdleInventor", "CouchCrusher", "TapperTitan",
            "MergeMagician", "SwapSage", "ClickConqueror", "TapTiger", "SwipeSorcerer", "MergeMystic",
            "IdleIllusionist", "TapTemplar", "CouchCrusader", "SwapSamurai", "MergeMercenary", "PuzzlePirate",
            "TapTrooper", "SwipeShogun", "MergeMentor", "IdleIcon", "CouchCaptain", "TapTycoon", "MergeMaven",
            "SwapSensei", "PuzzlePhantom", "TapTitaness", "SwipeShaman", "MergeMythos", "IdleInnovator",
            "CouchComedian", "TapTactician", "MergeMuse", "SwapStrategist", "PuzzleProdigy", "TapTrendsetter",
            "SwipeSavant", "MergeMystic", "IdleInfluencer", "CouchChampion", "TapTriumphant", "JollyJester",
            "WhimsyWhale", "GlimmerGnome", "TwinkleToes", "GiggleGarden", "FrolicFox", "BounceBunny", "SproutSprinkle",
            "FizzFairy", "FlutterFrog", "DrizzleDuck", "WobbleWalrus", "SnickerSnail", "WiggleWombat", "HoppyHippo",
            "DoodleDragon", "JingleJelly", "SproingSquirrel", "NibbleNugget", "CrinklyCat", "MellowMoose",
            "PuddlePenguin", "SnoreSeal", "NapNestor", "SnoozeSloth", "YawnYak", "DozeDodo", "SlumberSheep",
            "SnoozerSnake", "DreamyDeer", "NapNanny", "CozyCamel", "SnoozeSparrow", "RestyRabbit", "SleepySwan",
            "ChillChester", "NapNest", "SnoozySeagull", "DozeDolphin", "ZzzZebra", "LuckyLuke", "HappyHarley",
            "SmileySam", "GoofyGina", "BoomerBill", "FlashFred", "ZoomyZoe", "SpeedySteve", "DashDexter", "ZoomZoom",
            "BreezyBrittany", "SunnySasha", "CheeryChris", "JazzyJill", "FunkyFrank", "GroovyGary", "CoolCalvin",
            "WildWillow", "BraveBella", "FearlessFinn", "HeroHugo", "SuperSue", "CaptainCarla", "WonderWendy",
            "MagicMike", "WizardWill", "FairyFaith", "PixiePam", "ElfEvan", "DragonDan", "KnightKyle", "QueenQuinn",
            "KingKarl", "PrincessPatty", "PrincePaul", "NobleNick", "RoyalRachel", "BaronBarry", "DukeDerek",
            "EmpressEmma", "EmperorEric", "LordLucas", "LadyLuna", "SirSimon", "MissMaddie", "MadamMaya", "GeneralGreg",
            "AdmiralAllie", "ColonelCaleb", "SergeantSophie", "PrivateParker", "CommanderCassie", "MajorMiles",
            "CaptainCole", "LieutenantLiam", "CorporalClara", "OfficerOlivia", "CadetCody", "RecruitReese",
            "TroopTammy", "HappyHoppy", "BouncyBabe", "JumpingJack", "SwirlySwan", "TopsyTurvy", "WobblyWarrior",
            "TrippyTurtle", "FlipFlopFreddy", "ZigZagZane", "LoopyLarry", "SpunkySpencer", "WigglyWanda", "DizzyDiva",
            "JitteryJerry", "TwitchyTanya", "ShakySharon", "QuakyQuincy", "TrembleTim", "RumbleRay", "JumpyJen",
            "FloppyFlorence", "BouncyBoris", "SpringySally", "BumpkinBob", "TumblerTom", "RollinRon", "SlideySuzie",
            "SkidMarkMike", "GlideGreg", "SlipperySam", "WobbleWalter", "StumbleStan", "TumbleTrevor", "TripTara",
            "StaggerSue", "ShuffleShawn", "ScootScout", "CreepyCory", "GhostlyGwen", "PhantomPhil", "ShadowSam",
            "SpookySarah", "HauntedHarry", "ScaryScott", "BooBianca", "GhoulishGeorge", "WitchyWinnie",
            "VampireVincent", "ZombieZack", "FrankFang", "MummyMaggie", "WerewolfWalt", "GoblinGretchen", "DemonDave",
            "GhoulGina", "BansheeBeth", "PolterPeter", "CrypticCarl", "PhantomPhoebe", "HauntingHannah"
        };


        public override void Initial()
        {
            _inputFieldName = transform.Find("Set/_inputFieldName").GetComponent<TMP_InputField>();
            _txtDes = transform.Find("Set/txtDes").GetComponent<TextMeshProUGUI>();
            transform.Find("Set/btnConfirm").GetComponent<Button>().onClick.AddListener(OnConfirmClick);
            transform.Find("Set/btnRandomName").GetComponent<Button>().onClick.AddListener(OnRandomNameClick);

            base.Initial();
        }

        private void OnRandomNameClick()
        {
            var sbRandomName = new StringBuilder();
            sbRandomName.Append(nameRandom[Random.Range(0, nameRandom.Length)]);
            sbRandomName.Append(Random.Range(0, 10000));
            sbRandomName.Append(nameRandom[Random.Range(0, nameRandom.Length)]);
            _inputFieldName.text = sbRandomName.ToString();
        }

        private void OnConfirmClick()
        {
            if (_inputFieldName.text != "")
            {
                DataHelper.CurUserInfoData.userName = _inputFieldName.text;
                DataHelper.ModifyLocalData(new List<string>(1) { "userName" }, () => { _txtDes.text = "Name Set Success";});
                GameRootLoad.Instance.StartLoad(DataHelper.nextSceneName);
                CloseInternalPage();
            }

            else
            {
                _txtDes.text = "Name is empty!";
            }
                
        }
    }
}