using Lab3.Models;

namespace Lab3;

public partial class EditorPage : ContentPage
{
    private Article _article;
    public event Action<Article> OnSave;

    public EditorPage(Article existingArticle)
    {
        InitializeComponent();

        if (existingArticle != null)
        {
            _article = existingArticle;

            TitleEnt.Text = _article.Title;
            AuthorEnt.Text = _article.Author;
            DeptEnt.Text = _article.Department;
            ContentEditor.Text = _article.Annotation;
            CatPicker.SelectedItem = _article.Category;
        }
        else
        {
            _article = new Article();
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEnt.Text))
        {
            await DisplayAlert("Помилка", "Введіть назву!", "OK");
            return;
        }

        _article.Title = TitleEnt.Text;
        _article.Author = AuthorEnt.Text;
        _article.Department = DeptEnt.Text;
        _article.Annotation = ContentEditor.Text;
        _article.Category = CatPicker.SelectedItem?.ToString() ?? "Інше";

        OnSave?.Invoke(_article);
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}