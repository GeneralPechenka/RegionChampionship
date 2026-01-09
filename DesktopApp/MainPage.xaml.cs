using CommunityToolkit.Maui.Views;
using DesktopApp.ContentViews;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DesktopApp
{
    public partial class MainPage : ContentPage
    {
        private bool _isExpanded = true;
        private bool _isDropdownOpened = false;
        private const double MenuWidth = 200;
        private Button _activeMenuButton;

        private Dictionary<string, ContentView> _contentViews;
        public MainPage()
        {
            
            InitializeComponent();
            MainGrid.ColumnDefinitions[0].Width = MenuWidth;
            _contentViews = new()
            {
                ["Компании"] = new CompanyContentView(),
                ["Торговые автоматы"] = new VendingContentView(),
                ["Дополнительные"] = new NewContent1()
            };
            // Настраиваем выпадающее меню
            //SetupUserDropdownMenu();

            // Настраиваем hover-эффекты
            SetupMenuHoverEffects();
        }

        //private void SetupUserDropdownMenu()
        //{
        //    // Закрытие меню при клике вне его
        //    var tapGesture = new TapGestureRecognizer();
        //    tapGesture.Tapped += async (s, e) =>
        //    {
        //        if (_isUserMenuOpen)
        //        {
        //            await CloseUserMenu();
        //        }
        //    };
        //    MainGrid.GestureRecognizers.Add(tapGesture);

        //    // Не закрывать при клике внутри меню
        //    UserDropdownMenu.GestureRecognizers.Add(new TapGestureRecognizer());
        //}

        private void SetupMenuHoverEffects()
        {
            var menuButtons = FindMenuButtons();
            foreach (var button in menuButtons)
            {
                AddHoverEffect(button);
            }
        }

        private List<Button> FindMenuButtons()
        {
            
            var buttons = new List<Button>();
            if (MenuContainer != null)
            {
                foreach (var child in MenuContainer.Children)
                {
                    if (child is Button button)
                    {
                        buttons.Add(button);
                    }
                    
                }
                foreach (var item in AdminExpanderBody.Children)
                {
                    if (item is Button button)
                    {
                        buttons.Add(button);
                    }
                }
            }
            return buttons;
        }

        private void AddHoverEffect(Button button)
        {
            var pointerRecognizer = new PointerGestureRecognizer();

            pointerRecognizer.PointerEntered += (s, e) =>
            {
                Dispatcher.Dispatch(() =>
                {
                    if (s is Button btn && btn != _activeMenuButton)
                    {
                        btn.BackgroundColor = Color.FromArgb("#181A1D");
                        btn.TextColor = Colors.White;
                    }
                });
            };

            pointerRecognizer.PointerExited += (s, e) =>
            {
                Dispatcher.Dispatch(() =>
                {
                    if (s is Button btn && btn != _activeMenuButton)
                    {
                        btn.BackgroundColor = Colors.Transparent;
                        btn.TextColor = Color.FromArgb("#BDC3C7");
                    }
                });
            };

            button.GestureRecognizers.Add(pointerRecognizer);
        }

        // ===== БОКОВОЕ МЕНЮ =====
        private void ToggleMenu(object sender, EventArgs e)
        {
            var column = MainGrid.ColumnDefinitions[0];
            var currentWidth = column.Width.Value;
            var targetWidth = _isExpanded ? 0 : MenuWidth;

            Task.Run(() => AnimateMenu(currentWidth, targetWidth));
        }

        private async Task AnimateMenu(double start, double end, uint duration = 300)
        {
            var column = MainGrid.ColumnDefinitions[0];

            var animation = new Animation(
                callback: v =>
                {
                    Dispatcher.Dispatch(() =>
                    {
                        column.Width = v;
                        MainGrid.InvalidateMeasure();
                    });
                },
                start: start,
                end: end,
                easing: Easing.CubicInOut);

            // Запускаем анимацию
            MainGrid.Animate("ColumnWidth", animation, rate: 16, length: duration);

            await Task.Delay((int)duration);

            // Обновляем состояние
            Dispatcher.Dispatch(() =>
            {
                _isExpanded = !_isExpanded;
            });
        }

        private async void BtnClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                //Сброс предыдущей активной кнопки
                if (_activeMenuButton != null)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        _activeMenuButton.BackgroundColor = Colors.Transparent;
                        _activeMenuButton.TextColor = Color.FromArgb("#BDC3C7");
                    });

                }
                //Активная кнопка
                _activeMenuButton = button;

                Dispatcher.Dispatch(() =>
                {

                    _activeMenuButton.BackgroundColor = Color.FromArgb("#181A1D");
                    _activeMenuButton.TextColor = Colors.White;
                    PageLabel.Text = button.Text;
                });
                //await DisplayAlert("Навигация", $"Выбрано: {button.Text}", "ОК");
                var contentView = _contentViews[button.Text.Trim()];
                contentContainer.Content = contentView;
            }
        }

        // ===== ВЫПАДАЮЩЕЕ МЕНЮ =====

        private void DropdownButton_Clicked(object sender, EventArgs e)
        {
            Task.Delay(150);
            DropdownMenu.IsVisible = !_isDropdownOpened;
            DropdownMenuButton.Text = _isDropdownOpened ? "▼" : "▲";
            _isDropdownOpened = !_isDropdownOpened;


        }
        //private async Task AnimateDropdownMenu(int duration)
        //{
        //    await Task.Delay(300);
        //    DropdownMenu.IsVisible = !_isDropdownOpened;
        //    DropdownMenuButton.Text = _isDropdownOpened ? "▼" : "▲";
        //    _isDropdownOpened = !_isDropdownOpened;
        //}

        private void AdminExpander_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
        {
            AdminExpanderIcon.Text = AdminExpander.IsExpanded ? "∧" : "∨";
        }

        private void TmcExpander_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
        {
            TmcExpanderIcon.Text = TmcExpander.IsExpanded ? "∧" : "∨";
        }

        private void ReportsExpander_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
        {
            ReportsExpanderIcon.Text = ReportsExpander.IsExpanded ? "∧" : "∨";
        }
    }
}