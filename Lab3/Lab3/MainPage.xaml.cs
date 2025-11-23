using CommunityToolkit.Maui.Storage;
using Lab3.Models;
using Lab3.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Lab3
{
    public partial class MainPage : ContentPage
    {
        private List<Article> _allArticles = new List<Article>();
        private JsonFileService _jsonService = new JsonFileService();
        private string _currentFilePath;

        public MainPage()
        {
            InitializeComponent();

            RefreshList();
        }

        private void RefreshList()
        {
            if (SearchTitle == null || SearchAuthor == null || FilterCategory == null) return;

            var filtered = _allArticles.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTitle.Text))
                filtered = filtered.Where(x => x.Title.Contains(SearchTitle.Text, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(SearchAuthor.Text))
                filtered = filtered.Where(x => x.Author.Contains(SearchAuthor.Text, StringComparison.OrdinalIgnoreCase));

            if (FilterCategory.SelectedIndex > 0)
            {
                string selectedCat = FilterCategory.SelectedItem.ToString();
                filtered = filtered.Where(x => x.Category == selectedCat);
            }

            ArticlesList.ItemsSource = new ObservableCollection<Article>(filtered);
        }

        private void OnSearchChanged(object sender, EventArgs e) => RefreshList();

        private async void OnEditClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var article = button.CommandParameter as Article;

            var editor = new EditorPage(article);
            editor.OnSave += (updatedArticle) =>
            {
                var index = _allArticles.FindIndex(a => a.Id == updatedArticle.Id);
                if (index != -1)
                {
                    _allArticles[index] = updatedArticle;
                }
                RefreshList();
            };
            await Navigation.PushModalAsync(editor);
        }
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var article = button.CommandParameter as Article;

            bool answer = await DisplayAlert("Видалення", $"Видалити статтю '{article.Title}'?", "Так", "Ні");
            if (answer)
            {
                _allArticles.Remove(article);
                RefreshList();
            }
        }
        private async void OnAddClicked(object sender, EventArgs e)
        {
            var editor = new EditorPage(null);
            editor.OnSave += (newArticle) =>
            {
                _allArticles.Add(newArticle);
                RefreshList();
            };
            await Navigation.PushModalAsync(editor);
        }

        private async void OnOpenClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Оберіть JSON" });
                if (result != null)
                {
                    _currentFilePath = result.FullPath;
                    _allArticles = await _jsonService.LoadFromFileAsync(_currentFilePath);
                    RefreshList();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async void OnSaveAsClicked(object sender, EventArgs e)
        {
            try
            {
                using var stream = new MemoryStream();
                var options = new JsonSerializerOptions { WriteIndented = true };
                await JsonSerializer.SerializeAsync(stream, _allArticles, options);
                stream.Position = 0;

                var fileSaverResult = await FileSaver.Default.SaveAsync("data.json", stream, CancellationToken.None);
                if (fileSaverResult.IsSuccessful)
                {
                    await DisplayAlert("Успіх", $"Збережено: {fileSaverResult.FilePath}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AboutPage());
        }

        private async void OnArticleSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Article article)
            {
                string fullDetails =
                    $"АВТОР: {article.Author}\n" +
                    $"КАФЕДРА: {article.Department}\n" +
                    $"КАТЕГОРІЯ: {article.Category}\n" +
                    $"ID: {article.Id}\n" +
                    $"\n" +
                    $"--- ТЕКСТ СТАТТІ ---\n" +
                    $"\n" +
                    $"{article.Annotation}";

                await DisplayAlert(article.Title, fullDetails, "Закрити");

                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}
