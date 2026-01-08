using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopApp
{
    public partial class MainPage : ContentPage
    {
        private bool _isExpanded = true;
        private bool _isUserMenuOpen = false;
        private const double MenuWidth = 200;
        private Button _activeMenuButton;

        public MainPage()
        {
            InitializeComponent();
            MainGrid.ColumnDefinitions[0].Width = MenuWidth;

            // Настраиваем выпадающее меню
            SetupUserDropdownMenu();

            // Настраиваем hover-эффекты
            SetupMenuHoverEffects();
        }

        private void SetupUserDropdownMenu()
        {
            // Закрытие меню при клике вне его
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                if (_isUserMenuOpen)
                {
                    await CloseUserMenu();
                }
            };

            MainGrid.GestureRecognizers.Add(tapGesture);

            // Не закрывать при клике внутри меню
            UserDropdownMenu.GestureRecognizers.Add(new TapGestureRecognizer());
        }

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
            }

            return buttons;
        }

        private void AddHoverEffect(Button button)
        {
            var pointerRecognizer = new PointerGestureRecognizer();

            pointerRecognizer.PointerEntered += (s, e) =>
            {
                // Используем Dispatcher для UI обновлений
                Dispatcher.Dispatch(() =>
                {
                    if (s is Button btn && btn != _activeMenuButton)
                    {
                        btn.BackgroundColor = Color.FromArgb("#34495E");
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

        // ============ БОКОВОЕ МЕНЮ ============

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
                    // Используем Dispatcher вместо Device.BeginInvokeOnMainThread
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
            Dispatcher.Dispatch(() =>
            {
                MainGrid.Animate("ColumnWidth", animation, rate: 16, length: duration);
            });

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
                // Сброс предыдущей активной кнопки
                if (_activeMenuButton != null)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        _activeMenuButton.BackgroundColor = Colors.Transparent;
                        _activeMenuButton.TextColor = Color.FromArgb("#BDC3C7");
                    });
                }

                // Установка новой активной кнопки
                _activeMenuButton = button;

                Dispatcher.Dispatch(() =>
                {
                    _activeMenuButton.BackgroundColor = Color.FromArgb("#3498DB");
                    _activeMenuButton.TextColor = Colors.White;
                    PageLabel.Text = button.Text;
                });

                await DisplayAlert("Навигация", $"Выбрано: {button.Text}", "ОК");
            }
        }

        // ============ ВЫПАДАЮЩЕЕ МЕНЮ ============

        private async void OnUserMenuButtonClicked(object sender, EventArgs e)
        {
            if (_isUserMenuOpen)
            {
                await CloseUserMenu();
            }
            else
            {
                await OpenUserMenu();
            }
        }

        private async Task OpenUserMenu()
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                UserDropdownMenu.IsVisible = true;
                UserDropdownMenu.Opacity = 0;
                UserDropdownMenu.Scale = 0.9;

                await Task.WhenAll(
                    UserDropdownMenu.FadeTo(1, 200),
                    UserDropdownMenu.ScaleTo(1, 200, Easing.SpringOut)
                );

                _isUserMenuOpen = true;
                UserMenuButton.Text = "▲";
            });
        }

        private async Task CloseUserMenu()
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await Task.WhenAll(
                    UserDropdownMenu.FadeTo(0, 150),
                    UserDropdownMenu.ScaleTo(0.9, 150)
                );

                UserDropdownMenu.IsVisible = false;
                _isUserMenuOpen = false;
                UserMenuButton.Text = "▼";
            });
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await CloseUserMenu();
            await DisplayAlert("Профиль", "Открыть страницу профиля", "OK");
        }

        private async void OnSessionsClicked(object sender, EventArgs e)
        {
            await CloseUserMenu();
            await DisplayAlert("Сессии", "Показать активные сессии", "OK");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Выход", "Вы уверены, что хотите выйти?", "Да", "Нет");

            if (confirm)
            {
                await CloseUserMenu();
                await DisplayAlert("Выход", "Вы вышли из системы", "OK");
            }
        }
    }
}