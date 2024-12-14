using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.ObjectModel;



namespace LorDudProjekt
{

    public partial class MainPage : ContentPage
    {
        const string baseURI = "https://www.biblia.info.pl";
        const string infoPostfix = "/api/info/bt";
        const string basePostfix = "/api/biblia/bt";

        private ObservableCollection<string> booksList = new ObservableCollection<string>();
        private ObservableCollection<int> chaptersList = new ObservableCollection<int>();
        private ObservableCollection<int> allVersesList = new ObservableCollection<int>();
        private ObservableCollection<int> selectableVersesList = new ObservableCollection<int>();

        bool error = false;
       public MainPage()
        {
            InitializeComponent();
            pickerBook.ItemsSource = booksList;
            pickerChapter.ItemsSource = chaptersList;
            pickerStart.ItemsSource = allVersesList;
            pickerEnd.ItemsSource = selectableVersesList;

        }
        private async Task<string> GetJSON(string url)
        {
            // Tworzymy więź emocjonalną z naszym API 
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseURI);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return jsonResponse;
        }
        private async Task<string> GetBook(bool random)
        {

            booksList.Clear();
            var json = await GetJSON(infoPostfix);
            Books books = JsonSerializer.Deserialize<Books>(json) ?? new Books();
            foreach (Book book in books.books)
            {
                booksList.Add(book.abbreviation);
            }
            if (random)
            {
                Random rnd = new Random();
                int bookNum = rnd.Next(books.books.Length);
                string abbrev = books.books[bookNum].abbreviation;
                pickerBook.SelectedItem = abbrev;
                return abbrev;
            }
            else
            {
                string abbrev = books.books[0].abbreviation; 
                pickerBook.SelectedItem = abbrev;
                return abbrev;
            }
                


        }
        private async Task<string> GetChapter(string bookAbbrev, bool random, string chosenChapter = "1")
        {
            chaptersList.Clear();
            var json = await GetJSON(infoPostfix + '/' + bookAbbrev);
            Book book = JsonSerializer.Deserialize<Book>(json);
            for (int i = 0; i <= book.chapters; i++)
            {
                chaptersList.Add(i + 1);
            }
            int chaptersNum = (int)book.chapters;

            if (random)
            {
                Random rnd = new Random();
                int randomChapter = rnd.Next(chaptersNum) + 1;
                pickerChapter.SelectedItem = randomChapter;
                return randomChapter.ToString();
            }
            else
            {
                return chosenChapter; // Pierwszy rozdział
            }


        }
        private async Task<string> GetVerses(string bookAbbrev, string chapterNum, bool random, string verseStart = "1", string verseEnd = "2")
        {
            allVersesList.Clear();
            var json = await GetJSON(basePostfix + '/' + bookAbbrev + '/' + chapterNum);
            Chapter chapter = JsonSerializer.Deserialize<Chapter>(json);
            int len = chapter.verses.Length;
            int secondVerse = 2;
            for (int i = 0; i < len; i++)
            {
                allVersesList.Add(i + 1);
            }
            if (random)
            {
                Random rnd = new Random();
                int randomVerse = rnd.Next(len) + 1;
                pickerStart.SelectedItem = randomVerse;
                secondVerse = randomVerse;


                if (secondVerse > len)
                {
                    secondVerse = len;
                }
            }
            else
            {
                secondVerse = int.Parse(verseEnd);
            }

            selectableVersesList.Clear();
            for (int i = secondVerse; i <= len; i++)
            {
                selectableVersesList.Add(i);
            }

            pickerEnd.SelectedItem = secondVerse;

            string result = "";
            foreach (var verse in chapter.verses)
            {
                result += verse.text;
            }

            return result;
        }
        private async void GetRandomBibleVerse()
        {
            string book = await GetBook(true);

            string chapter = await GetChapter(book, true);

            string verses = await GetVerses(book, chapter, true);

            blck_cite.Text = verses;
        }
        private async void GetBibleVerse(string bookChoice, string chapterChoice, string verseStart, string verseEnd)
        {
            string book = await GetBook(false);
            string chapter = await GetChapter(bookChoice, false, chapterChoice);
            string verses = await GetVerses(bookChoice, chapter, false, verseStart, verseEnd);
            blck_cite.Text = verses;
        }

        private void Button_Click(object sender,  EventArgs e)
        {
            GetRandomBibleVerse();
        }

        private async void btnSelect_ClickedAsync(object sender, EventArgs e)
        {

            string book = await GetBook(false);
            string bookChoice = pickerBook.SelectedItem.ToString();
            string chapter = pickerChapter.SelectedItem.ToString();
            string verseStart = pickerStart.SelectedItem.ToString() ?? "1";
            string verseEnd = pickerEnd.SelectedItem.ToString() ?? "2";

            GetBibleVerse(bookChoice, chapter, verseStart, verseEnd);
        }
    }

}

