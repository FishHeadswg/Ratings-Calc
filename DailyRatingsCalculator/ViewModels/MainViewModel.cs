using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using The_Algorithm;

namespace DailyRatingsCalculator.ViewModels
{
    public class MainViewModel : BindableBase, INavigationAware
    {
        private readonly Dictionary<string, Ratings> boardRatings;
        private readonly Dictionary<string, Stack<int>> ratingsStack;
        private float _currRating;
        private string _finalRating;
        private Uri _imageUri;
        private float _finalOpacity;
        private bool _calcGridEnabled;
        private bool _boardGridEnabled;
        private readonly Timer aTimer;
        private readonly Timer bTimer;
        private float flickerOpacity;
        private ICommand _setAddRatingCommand;
        private ICommand _setAddBoardCommand;
        private ICommand _setUpdateUICommand;
        private ICommand _setDisplayFinalScoreCommand;
        private ICommand _setUndoCommand;

        public MainViewModel()
        {
            boardRatings = new();
            ratingsStack = new();
            _finalRating = "Pending";
            ImageUri = new("pack://application:,,,/Images/ratingheader.png");
            aTimer = new();
            bTimer = new();
            aTimer.Elapsed += new(OnTimedEvent);
            bTimer.Elapsed += new(OnTimedEventB);
            aTimer.Interval = 50;
            bTimer.Interval = 5000;
            flickerOpacity = -0.25F;
            List<string> boards = new();
            BoardGridEnabled = CalcGridEnabled = true;
            FinalOpacity = 1f;
        }

        public ICommand SetAddRatingCommand => _setAddRatingCommand ??= new DelegateCommand<string>(AddRating);

        public ICommand SetAddBoardCommand => _setAddBoardCommand ??= new DelegateCommand<string>(AddBoard);

        public ICommand SetUpdateUICommand => _setUpdateUICommand ??= new DelegateCommand<string>(UpdateUI);

        public ICommand SetDisplayFinalScoreCommand => _setDisplayFinalScoreCommand ??= new DelegateCommand(DisplayFinalScore);

        public ICommand SetUndoCommand => _setUndoCommand ??= new DelegateCommand(Undo);

        private void AddBoard(string board)
        {
            try {
                boardRatings.Add(board, new());
                ratingsStack.Add(board, new());
            }
            catch { return; }
        }

        private void AddRating(string rating) =>
            Calc(Convert.ToInt32(rating));

        private void UpdateUI(string board)
        {
            CurrBoard = board;
            ImageUri = new("pack://application:,,,/Images/" + board.Replace("/", string.Empty) + ".png");
            if (board is "4/jp/") Setup4jp();
            else
            {
                Setup(board);
            }
        }

        private void Setup(string currBoard)
        {
            CalcGridEnabled = true;
            CurrRating = GetBoardRatings(currBoard).Current ?? 0;
            FinalRating = GetBoardRatings(currBoard).Final switch
            {
                null => "Pending",
                _ => GetBoardRatings(currBoard).Final
            };
        }

        private void Setup4jp()
        {
            CalcGridEnabled = false;
            CurrRating = 0;
            FinalRating = "0";
        }

        private void DisplayFinalScore()
        {
            if (CurrBoard is null) return;
            GetFinalRating();
            FinalRating = "CALCULATING";
            BoardGridEnabled = false;
            CalcGridEnabled = false;
            aTimer.Start();
            bTimer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (FinalOpacity is 1) flickerOpacity = -0.25F;
            else if (FinalOpacity is 0) flickerOpacity = 0.25F;
            FinalOpacity += flickerOpacity;
            if (!aTimer.Enabled) FinalOpacity = 1; // in case A is already being invoked when stopped
        }

        private void OnTimedEventB(object source, ElapsedEventArgs e)
        {
            aTimer.Stop();
            FinalOpacity = 1;
            FinalRating = GetBoardRatings(CurrBoard).Final;
            BoardGridEnabled = true;
            CalcGridEnabled = true;
            bTimer.Stop();
        }

        public string CurrBoard { get; set; }

        public float CurrRating
        {
            get => _currRating;
            set => SetProperty(ref _currRating, value);
        }

        public string FinalRating
        {
            get => _finalRating;
            set => SetProperty(ref _finalRating, value);
        }

        public float FinalOpacity
        {
            get => _finalOpacity;
            set => SetProperty(ref _finalOpacity, value);
        }

        public Uri ImageUri
        {
            get => _imageUri;
            set => SetProperty(ref _imageUri, value);
        }

        public bool CalcGridEnabled
        {
            get => _calcGridEnabled;
            set => SetProperty(ref _calcGridEnabled, value);
        }

        public bool BoardGridEnabled
        {
            get => _boardGridEnabled;
            set => SetProperty(ref _boardGridEnabled, value);
        }

        public Ratings GetBoardRatings(string board)
        {
            return boardRatings[board];
        }

        public void GetFinalRating()
        {
            int finalrating = TheAlgorithm.TheAlgorithmGeneric(boardRatings[CurrBoard].Current ?? 0);
            boardRatings[CurrBoard].Final = finalrating switch
            {
                > 10 => "10",
                < 0 => "0",
                _ => finalrating.ToString()
            };
        }

        public void Undo()
        {
            if (CurrBoard is null) return;
            if (ratingsStack[CurrBoard].Any())
            {
                ratingsStack[CurrBoard].Pop();
                if (ratingsStack[CurrBoard].Any()) CalcCurrent(ratingsStack[CurrBoard]);
                else CurrRating = 0;
            }
        }

        public void Calc(int score)
        {
            if (CurrBoard is not null)
            {
                ratingsStack[CurrBoard].Push(score);
                CalcCurrent(ratingsStack[CurrBoard]);
            }
        }

        private void CalcCurrent(Stack<int> ratings) =>
            boardRatings[CurrBoard].Current = CurrRating = (float)ratings.Average();

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        public record Ratings(float? Current = null, string Final = null)
        {
            public float? Current { get; set; } = Current;
            public string Final { get; set; } = Final;
        }
    }
}