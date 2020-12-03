using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Ratings_Calc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly Timer aTimer;
        private readonly Timer bTimer;
        private readonly Dictionary<string, Ratings> boardRatings;
        private readonly Dictionary<string, List<int>> ratingsLists;
        private string currBoard;
        private float flickerOpacity;
        private float _currRating;
        private string _finalRating;
        private Uri _imageUri;

        public MainWindow()
        {
            InitializeComponent();
            aTimer = new();
            bTimer = new();
            aTimer.Elapsed += new(OnTimedEvent);
            bTimer.Elapsed += new(OnTimedEventB);
            aTimer.Interval = 50;
            bTimer.Interval = 5000;
            flickerOpacity = -0.25F;
            _finalRating = "Pending";
            ImageUri = new("pack://application:,,,/Images/ratingheader.png");
            boardRatings = new ();
            ratingsLists = new();
            foreach (var btn in boardGrid.Children.OfType<Button>()) AddBoard(btn.Content.ToString());
            txtCurrRating.DataContext = txtFinalRating.DataContext = boardImage.DataContext = this;

            void AddBoard(string board)
            {
                boardRatings.Add(board, new());
                ratingsLists.Add(board, new());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float CurrRating
        {
            get => _currRating;
            set
            {
                _currRating = value;
                OnPropertyChanged();
            }
        }

        public string FinalRating
        {
            get => _finalRating;
            set
            {
                _finalRating = value;
                OnPropertyChanged();
            }
        }

        public Uri ImageUri
        {
            get => _imageUri;
            set
            {
                _imageUri = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private void Calc(object sender, RoutedEventArgs e)
        {
            if (currBoard is not null)
            {
                ratingsLists[currBoard].Add(Convert.ToInt32((sender as Button).Content.ToString()));
                CalcCurrent(ratingsLists[currBoard]);
            }
        }

        private void CalcCurrent(List<int> ratings)
        {
            int ratingsTotal = 0;
            foreach (int rating in ratings) ratingsTotal += rating;
            float ratingFloat = ((float)ratingsTotal / ratings.Count);
            CurrRating = ratingFloat;
            boardRatings[currBoard].Current = ratingFloat;
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            currBoard = (sender as Button).Content.ToString();
            ImageUri = new("pack://application:,,,/Images/" + currBoard.Replace("/", string.Empty) + ".png");
            if (currBoard is "4/jp/")
            {
                foreach (Button btn in calcGrid.Children) btn.IsEnabled = false;
                btnFinal.IsEnabled = false;
                CurrRating = 0;
                FinalRating = "0";
            }
            else
            {
                foreach (Button btn in calcGrid.Children) btn.IsEnabled = true;
                btnFinal.IsEnabled = true;
                CurrRating = boardRatings[currBoard].Current ?? 0;
                FinalRating = boardRatings[currBoard].Final switch
                {
                    null => "Pending",
                    _ => boardRatings[currBoard].Final
                };
            }
        }

        private void BtnFinal_Click(object sender, RoutedEventArgs e)
        {
            if (currBoard is null) return;
            int finalrating = The_Algorithm.The_Algorithm.TheAlgorithm(boardRatings[currBoard].Current ?? 0);
            boardRatings[currBoard].Final = finalrating switch
            {
                > 10 => "10",
                < 0 => "0",
                _ => finalrating.ToString()
            };
            FinalRating = "CALCULATING";
            foreach (Button btn in boardGrid.Children.OfType<Button>()) btn.IsEnabled = false;
            foreach (Button btn in calcGrid.Children.OfType<Button>()) btn.IsEnabled = false;
            btnFinal.IsEnabled = false;
            aTimer.Start();
            bTimer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (txtFinalRating.Foreground.Opacity is 1) flickerOpacity = -0.25F;
                else if (txtFinalRating.Foreground.Opacity is 0) flickerOpacity = 0.25F;
                txtFinalRating.Foreground.Opacity += flickerOpacity;
                if (!aTimer.Enabled) txtFinalRating.Foreground.Opacity = 1; // in case A is already being invoked when stopped
            });
        }

        private void OnTimedEventB(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                aTimer.Stop();
                txtFinalRating.Foreground.Opacity = 1;
                FinalRating = boardRatings[currBoard].Final;
                foreach (Button btn in boardGrid.Children.OfType<Button>()) btn.IsEnabled = true;
                foreach (Button btn in calcGrid.Children.OfType<Button>()) btn.IsEnabled = true;
                btnFinal.IsEnabled = true;
                bTimer.Stop();
            });
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (currBoard is null) return;
            if (ratingsLists[currBoard].Any())
            {
                ratingsLists[currBoard].RemoveAt(ratingsLists[currBoard].Count - 1);
                if (ratingsLists[currBoard].Any()) CalcCurrent(ratingsLists[currBoard]);
                else CurrRating = 0;
            }
        }

        private void BtnFourjp_Click(object sender, RoutedEventArgs e) => Update(sender, e);

        private void BtnGnfos_Click(object sender, RoutedEventArgs e) => Update(sender, e);

        private void BtnOta_Click(object sender, RoutedEventArgs e) => Update(sender, e);

        private void Zero_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void One_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Two_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Three_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Four_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Five_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Six_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Seven_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Eight_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Nine_Click(object sender, RoutedEventArgs e) => Calc(sender, e);

        private void Ten_Click(object sender, RoutedEventArgs e) => Calc(sender, e);
    }

    record Ratings(float? Current = null, string Final = null)
    {
        public float? Current { get; set; } = Current;
        public string Final { get; set; } = Final;
    }
}