using DesktopApp.Windows;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;


namespace DesktopApp.ContentViews
{
    public partial class VendingContentView : ContentView
    {
        private List<Vending> _allVendings = new();
        private List<Vending> _filteredVendings = new();
        private int _currentPage = 1;
        private int _pageSize = 50;
        private string _filterText = string.Empty;

        public ObservableCollection<Vending> Vendings { get; } = new ObservableCollection<Vending>();

        public VendingContentView()
        {
            InitializeComponent();
            LoadMockData();
            InitializePicker();

            // Устанавливаем BindingContext
            this.BindingContext = this;

            LoadVendings();
        }

        private void LoadMockData()
        {
            _allVendings = new List<Vending>
            {
                new() { Id = 592053, Name = "ELI «Московский»", Model = "Space Crosails 400",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100025",
                        Address = "Суворова 121 у входа", InServiceSince = new DateTime(2018, 5, 12) },
                new() { Id = 592058, Name = "ITI «Магия»", Model = "Unicorn Reuse",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100029",
                        Address = "Академическая 15 улица, вход", InServiceSince = new DateTime(2018, 5, 13) },
                new() { Id = 592055, Name = "ДОСААФ", Model = "Based BIM 972",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100027",
                        Address = "Беренкац 174 у входа", InServiceSince = new DateTime(2018, 5, 13) },
                new() { Id = 592056, Name = "Завод «Гайфра»", Model = "Necra Kilda Max",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100028",
                        Address = "Грибцевское шоссе 174", InServiceSince = new DateTime(2018, 5, 13) },
                new() { Id = 592054, Name = "Mokka of Tpazaro", Model = "Rheinwebsen Lunt E5",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100026",
                        Address = "Карла Либонента 31 в зале ожидания", InServiceSince = new DateTime(2018, 5, 13) },
                new() { Id = 592057, Name = "Hauvenase", Model = "FAS Perla",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100029",
                        Address = "пер. Воскресенский 28", InServiceSince = new DateTime(2018, 5, 13) },
                new() { Id = 592052, Name = "Power", Model = "Jefremar Cafferreur G250",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100024",
                        Address = "Грибцевское шоссе 45 вход", InServiceSince = new DateTime(2018, 5, 11) },
                new() { Id = 592053, Name = "TV-C-R Beo", Model = "Necra Kilda ES6",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100022",
                        Address = "Кирока 1 3 этаж", InServiceSince = new DateTime(2018, 5, 11) },
                new() { Id = 592053, Name = "TV-C-R Beo", Model = "Necra Kilda ES6",
                        Company = "360641 - '0000 Торговое Автоматы'", Modem = "1924100022",
                        Address = "Кирока 2 3 этаж", InServiceSince = new DateTime(2018, 5, 11) },
                new() { Id = 592053, Name = "TV-C-R Beo", Model = "Necra Kilda ES6",
                        Company = "360642 - '0000 Торговое Автоматы'", Modem = "1924100022",
                        Address = "Кирока 3 3 этаж", InServiceSince = new DateTime(2018, 5, 11) },
                new() { Id = 592051, Name = "TV-C-R Beo (cave)", Model = "Unicorn Field Box",
                        Company = "360640 - '0000 Торговое Автоматы'", Modem = "1924100023",
                        Address = "Кирока 1 3 этаж", InServiceSince = new DateTime(2018, 5, 11) }
            };
        }

        private void InitializePicker()
        {
            // Устанавливаем начальное значение
            PageSizePicker.SelectedIndex = 2; // 50 записей
        }

        private void LoadVendings()
        {
            // Фильтрация данных
            _filteredVendings = string.IsNullOrEmpty(_filterText)
                ? _allVendings
                : _allVendings.Where(v =>
                    v.Name.Contains(_filterText, StringComparison.OrdinalIgnoreCase) ||
                    v.Model.Contains(_filterText, StringComparison.OrdinalIgnoreCase) ||
                    v.Company.Contains(_filterText, StringComparison.OrdinalIgnoreCase) ||
                    v.Address.Contains(_filterText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            // Расчет пагинации
            var totalPages = Math.Max(1, (int)Math.Ceiling(_filteredVendings.Count / (double)_pageSize));

            // Корректировка текущей страницы
            if (_currentPage > totalPages && totalPages > 0)
                _currentPage = totalPages;
            if (_currentPage < 1)
                _currentPage = 1;

            var startIndex = (_currentPage - 1) * _pageSize;
            var endIndex = Math.Min(startIndex + _pageSize, _filteredVendings.Count);

            // Загрузка данных для текущей страницы
            Vendings.Clear();

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i < _filteredVendings.Count)
                {
                    var item = _filteredVendings[i];
                    // Для Material DataGrid не используем IsEven - он сам управляет стилями
                    // Если нужно чередование цветов, настрой через стили DataGrid
                    Vendings.Add(item);
                }
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            // Обновление информации о записях
            var totalItems = _filteredVendings.Count;
            var startItem = totalItems == 0 ? 0 : (_currentPage - 1) * _pageSize + 1;
            var endItem = Math.Min(_currentPage * _pageSize, totalItems);

            RecordsInfoLabel.Text = $"Всего найдено {totalItems} шт.";

            // Обновление информации о пагинации
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)_pageSize));
            PaginationInfoLabel.Text = $"Записи с {startItem} по {endItem} из {totalItems}";
            CurrentPageNumberButton.Text = _currentPage.ToString();
            // Обновление состояния кнопок пагинации
            PreviousButton.IsEnabled = _currentPage > 1;
            PreviousButton.BackgroundColor = (_currentPage > 1) ? Colors.Transparent : Colors.LightGray;
            NextButton.IsEnabled = _currentPage < totalPages;
            NextButton.BackgroundColor = (_currentPage < totalPages) ? Colors.Transparent : Colors.LightGray;
        }

        // === ОБРАБОТЧИКИ СОБЫТИЙ ===

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            _filterText = e.NewTextValue ?? string.Empty;
            _currentPage = 1; // Сбрасываем на первую страницу при фильтрации
            LoadVendings();
        }

        private void OnPageSizeChanged(object sender, EventArgs e)
        {
            if (PageSizePicker.SelectedItem is int selectedSize)
            {
                _pageSize = selectedSize;
                _currentPage = 1; // Сбрасываем на первую страницу при изменении размера
                LoadVendings();
            }
        }

        private void OnPreviousPageClicked(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadVendings();
            }
        }

        private void OnNextPageClicked(object sender, EventArgs e)
        {
            var totalPages = Math.Max(1, (int)Math.Ceiling(_filteredVendings.Count / (double)_pageSize));
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadVendings();
            }
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            //await Shell.Current.DisplayAlert(
            //    "Добавление",
            //    "Функция добавления нового торгового автомата",
            //    "OK");
            var window = new AddVendingWindow();
            Application.Current.OpenWindow(window);
        }

        private async void OnExportButtonClicked(object sender, EventArgs e)
        {
            var action = await Shell.Current.DisplayActionSheet(
                "Экспорт данных",
                "Отмена",
                null,
                "CSV", "PDF", "HTML");

            if (action != "Отмена")
            {
                await Shell.Current.DisplayAlert(
                    "Экспорт",
                    $"Данные экспортированы в формате {action}",
                    "OK");
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Vending vending)
            {
                await Shell.Current.DisplayAlert(
                    "Редактирование",
                    $"Редактирование автомата: {vending.Name}",
                    "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Vending vending)
            {
                var confirm = await Shell.Current.DisplayAlert(
                    "Подтверждение удаления",
                    $"Вы уверены, что хотите удалить торговый автомат '{vending.Name}'?",
                    "Удалить",
                    "Отмена");

                if (confirm)
                {
                    _allVendings.RemoveAll(v => v.Id == vending.Id);
                    LoadVendings();

                    await Shell.Current.DisplayAlert(
                        "Успешно",
                        "Торговый автомат удален",
                        "OK");
                }
            }
        }
    }

    // Класс модели (можно вынести в отдельный файл Models/Vending.cs)
    public class Vending
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Modem { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime InServiceSince { get; set; }
        public string InServiceSinceFormat { get => InServiceSince.ToString("dd.MM.yyyy"); set => value = InServiceSince.ToString("dd.MM.yyyy"); }
        public bool IsEven { get; set; }
    }
}