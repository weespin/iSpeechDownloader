using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using NAudio.Wave;

namespace iSpeech_downloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new VoiceData();
            InitializeComponent();
            listboxFolder1.SelectionChanged += ListboxFolder1_SelectionChanged;
        }

        public static bool isMale = false;
        public static int speed = -2;

        private void ListboxFolder1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxFolder1.SelectedItems.Count > 0)
            {
                var selected = (Voice) listboxFolder1.SelectedItems[0];
                if (!selected.Female && selected.Male)
                {
                    Storyboard sb = (this.FindResource("ToRight") as Storyboard);
                    sb.Begin();
                    Man.IsEnabled = false;
                    Woman.IsEnabled = false;
                    backbtn.Background = Brushes.Gray;
                    isMale = true;
                }

                else if (!selected.Male && selected.Female)
                {
                    Storyboard sb = (this.FindResource("ToLeft") as Storyboard);
                    sb.Begin();
                    Woman.IsEnabled = false;
                    Man.IsEnabled = false;
                    backbtn.Background = Brushes.Gray;
                    isMale = false;
                }
                else
                {
                    Woman.IsEnabled = true;
                    Man.IsEnabled = true;
                    backbtn.Background = (Brush) new BrushConverter().ConvertFromString("#FF5A9AE0");
                }

            }
        }



        private void Woman_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("ToLeft") as Storyboard);
            sb.Begin();
            isMale = false;
        }

        private void Man_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("ToRight") as Storyboard);
            sb.Begin();
            isMale = true;
        }

        private void ToSlow(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("SecoundToLeft") as Storyboard);
            sb.Begin();
            speed = -2;
        }

        private void ToRegular(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("SecoundToCenter") as Storyboard);
            sb.Begin();
            speed = 0;
        }

        private void ToFast(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("SecoundToRight") as Storyboard);
            sb.Begin();
            speed = 2;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox.Text))
            {
                MessageBox.Show("No text");
                return;
            }

            if (listboxFolder1.SelectedItems.Count > 0)
            {
           

                var b = new Task(() =>
                {
                    var selected = Dispatcher.Invoke(()=>(Voice)listboxFolder1.SelectedItems[0]);
                    using (var mf = new MediaFoundationReader(Downloader.Parse(selected, isMale, Dispatcher.Invoke(() => TextBox.Text))))
                    using (var wo = new WaveOutEvent())
                    {

                        wo.Init(mf);
                        wo.Play();
                        while (wo.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(500);
                        }
                    }
                });
                b.Start();
            }
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox.Text))
            {
                MessageBox.Show("No text");
                return;
            }

            if (listboxFolder1.SelectedItems.Count > 0)
            {
                var selected = (Voice) listboxFolder1.SelectedItems[0];
                var link = Downloader.Parse(selected, isMale, TextBox.Text);
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "MP3 File (*.mp3)|*.mp3";
                dialog.FileName = "tts.mp3";
                var s = dialog.ShowDialog();
                if (s==true)
                {
                    if (link == "")
                    {
                        MessageBox.Show("Can't download. Maybe this voice is paid.");
                        return;
                    }

                    using (var web = new WebClient())
                    {
                        web.DownloadFile(link, dialog.FileName);
                        MessageBox.Show("Downloaded!");
                    }

                }
            }
        }

        public class Voice
        {
            public string Name { get; set; }
            public bool Male { get; set; }
            public bool Female { get; set; }
            public string FullName { get; set; }
            public string _countrycode { get; set; }

            public string ImageLink
            {
                get => GetImageLink();
            }

            string GetImageLink()
            {
                return $"https://github.com/hjnilsson/country-flags/blob/master/png250px/{_countrycode}.png?raw=true";
            }
        }

        public static class Downloader
        {
            public static string Parse(Voice voice, bool male, string text)
            {
                var f = male ? "male" : "female";

                  var a =  $"http://cache.ispeech.org/api/rest/?apikey=e3a4477c01b482ea5acc6ed03b1f419f&action=convert&format=mp3&voice={voice.Name + f}&speed={MainWindow.speed}&text={text}&version=0.2.99";
                return a;

            }
        }

        public class VoiceData
        {
            public List<Voice> VocieDataList { get; } = new List<Voice>
            {
                new Voice
                {
                    Name = "usenglish",
                    FullName = "US English",
                    _countrycode = "us",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "ukenglish",
                    FullName = "UK English",
                    _countrycode = "gb-nir",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "auenglish",
                    FullName = "Australian English",
                    _countrycode = "au",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "usspanish",
                    FullName = "US Spanish",
                    Female = true,
                    _countrycode = "es",
                    Male = true
                },
                new Voice {Name = "chchinese", FullName = "Chinese", Female = true, _countrycode = "cn", Male = false},
                new Voice
                {
                    Name = "hkchinese",
                    FullName = "Hong Kong Chinese",
                    _countrycode = "hk",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "twchinese",
                    FullName = "Taiwan Chinese",
                    _countrycode = "tw",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "jpjapanese",
                    FullName = "Japanese Chinese",
                    _countrycode = "jp",
                    Female = true,
                    Male = false
                },
                new Voice {Name = "krkorean", FullName = "Korean", _countrycode = "kr", Female = true, Male = false},
                new Voice
                {
                    Name = "caenglish",
                    FullName = "Canadian English",
                    _countrycode = "ca",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "huhungarian",
                    FullName = "Hungarian",
                    _countrycode = "hu",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "brportuguese",
                    FullName = "Brazilian Portuguese",
                    _countrycode = "br",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurportuguese",
                    FullName = "European Portuguese",
                    _countrycode = "pt",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "eurspanish",
                    FullName = "European Spanish",
                    _countrycode = "es",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "eurczech",
                    FullName = "European Czech",
                    _countrycode = "cz",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurdanish",
                    FullName = "European Danish",
                    _countrycode = "dk",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurfinnish",
                    FullName = "European Finnish",
                    _countrycode = "fi",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurfrench",
                    FullName = "European French",
                    _countrycode = "fr",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "eurnorwegian",
                    FullName = "European Norwegian",
                    _countrycode = "no",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurdutch",
                    FullName = "European Dutch",
                    _countrycode = "nl",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurpolish",
                    FullName = "European Polish",
                    _countrycode = "pl",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "euritalian",
                    FullName = "European Italian",
                    _countrycode = "it",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "eurturkish",
                    FullName = "European Turkish",
                    _countrycode = "tr",
                    Female = true,
                    Male = true
                },
                new Voice
                {
                    Name = "eurgreek",
                    FullName = "European Greek",
                    _countrycode = "gr",
                    Female = true,
                    Male = false
                },
                new Voice
                {
                    Name = "eurgerman",
                    FullName = "European German",
                    _countrycode = "de",
                    Female = true,
                    Male = true
                },
                new Voice {Name = "rurussian", FullName = "Russian", _countrycode = "ru", Female = true, Male = true},
                new Voice {Name = "swswedish", FullName = "Swedish", _countrycode = "se", Female = true, Male = false},
                new Voice
                {
                    Name = "cafrench",
                    FullName = "Canadian French",
                    _countrycode = "ca",
                    Female = true,
                    Male = true
                },
                new Voice {Name = "arabic", FullName = "Arabic", _countrycode = "eg", Female = false, Male = true}
            };
        }
    }
}
