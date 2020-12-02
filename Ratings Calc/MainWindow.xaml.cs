using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Ratings_Calc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly Timer aTimer;
        private readonly Timer bTimer;
        private readonly Dictionary<string, float?[]> boardRatings;
        private readonly Dictionary<string, List<int>> ratingsLists;
        private string currBoard;
        private float flickerOpacity;
        private float currRating;
        private string finalRating;
        private Uri imageUri;

        public MainWindow()
        {
            InitializeComponent();
            aTimer = new Timer();
            bTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            bTimer.Elapsed += new ElapsedEventHandler(OnTimedEventB);
            aTimer.Interval = 50;
            bTimer.Interval = 5000;
            flickerOpacity = -0.25F;
            finalRating = "Pending";
            ImageUri = new Uri("pack://application:,,,/Images/ratingheader.png");
            boardRatings = new Dictionary<string, float?[]>();
            ratingsLists = new Dictionary<string, List<int>>();
            foreach (var p in boardGrid.Children) if (p is Button) AddBoard((p as Button).Content.ToString());
            txtCurrRating.DataContext = this;
            txtFinalRating.DataContext = this;
            boardImage.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float CurrRating
        {
            get { return currRating; }
            set
            {
                currRating = value;
                OnPropertyChanged();
            }
        }

        public string FinalRating
        {
            get { return finalRating; }
            set
            {
                finalRating = value;
                OnPropertyChanged();
            }
        }

        public Uri ImageUri
        {
            get { return imageUri; }
            set
            {
                imageUri = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddBoard(string board)
        {
            boardRatings.Add(board, new float?[] { null, null });
            ratingsLists.Add(board, new List<int>());
        }

        private void Calc(object sender, RoutedEventArgs e)
        {
            if (currBoard == null || currBoard == "4/jp") return;
            ratingsLists[currBoard].Add(Convert.ToInt32((sender as Button).Content.ToString()));
            CalcCurrent(ratingsLists[currBoard]);
        }

        private void CalcCurrent(List<int> ratings)
        {
            int ratingsTotal = 0;
            foreach (int rating in ratings) ratingsTotal += rating;
            float ratingFloat = ((float)ratingsTotal / ratings.Count);
            CurrRating = ratingFloat;
            boardRatings[currBoard][0] = ratingFloat;
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            currBoard = (sender as Button).Content.ToString();
            ImageUri = new Uri("pack://application:,,,/Images/" + currBoard.Replace("/", string.Empty) + "icon.png");
            if (currBoard == "4/jp/")
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
                CurrRating = boardRatings[currBoard][0] ?? 0;
                FinalRating = boardRatings[currBoard][1] switch
                {
                    null => "Pending",
                    _ => boardRatings[currBoard][1].ToString()
                };
            }
        }

        private void BtnFinal_Click(object sender, RoutedEventArgs e)
        {
            if (currBoard == null || currBoard == "4/jp") return;
            int finalrating = The_Algorithm.The_Algorithm.TheAlgorithm(boardRatings[currBoard][0] ?? 0);
            boardRatings[currBoard][1] = finalrating switch
            {
                > 10 => 10,
                < 0 => 0,
                _ => finalrating
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
                if (txtFinalRating.Foreground.Opacity == 1) flickerOpacity = -0.25F;
                else if (txtFinalRating.Foreground.Opacity == 0) flickerOpacity = 0.25F;
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
                FinalRating = boardRatings[currBoard][1].ToString();
                foreach (Button btn in boardGrid.Children.OfType<Button>()) btn.IsEnabled = true;
                foreach (Button btn in calcGrid.Children.OfType<Button>()) btn.IsEnabled = true;
                btnFinal.IsEnabled = true;
                bTimer.Stop();
            });
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (currBoard == null || currBoard == "4/jp") return;
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
}