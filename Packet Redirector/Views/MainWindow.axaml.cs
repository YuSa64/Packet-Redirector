using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using PacketRedirector.ViewModels;

namespace PacketRedirector.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        PacketScrollViewer.SizeChanged += (s, e) =>
        {
            // 오른쪽 패딩 20 정도 여유
            PacketTextBox.MaxWidth = PacketScrollViewer.Bounds.Width - 20;
        };

        // Set background based on current theme
        if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
            Background = new SolidColorBrush(Color.Parse("#1E1E1E"));
        else
            Background = new SolidColorBrush(Color.Parse("#F5F5F5"));
    }
}